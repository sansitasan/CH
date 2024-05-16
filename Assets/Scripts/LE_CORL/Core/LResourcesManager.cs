using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Unity.VisualScripting;

public class LResourcesManager : MonoBehaviour, ICore
{
    public bool IsIntitalized => isInited;

    bool isInited = false;

    Dictionary<string, TextAsset> _scripts;
    Dictionary<string, Sprite> _sprites;
    Dictionary<string, StageData> _stageDatas;
    Dictionary<string, AudioClip> _audioClips;


    public void Init()
    {
        _scripts = new Dictionary<string, TextAsset>();
        _sprites = new Dictionary<string, Sprite>();
        _stageDatas = new Dictionary<string, StageData>();
        _audioClips = new Dictionary<string, AudioClip>();

        LoadAllAssets().Forget();
    }

    public void Disable()
    {
        _scripts.Clear();
        _sprites.Clear();
        _stageDatas.Clear();
        _audioClips.Clear();
    }

    async UniTaskVoid LoadAllAssets()
    {
        bool isScriptLoaded = false;
        bool isSpriteLoaded = false;
        bool isStageDataLoaded = false;

        LoadAsyncAll("Scripts", () => { isScriptLoaded = true; Debug.Log("Script - loaded"); }, _scripts);
        LoadAsyncAll("Image", () => { isSpriteLoaded = true; Debug.Log("Sprites - loaded"); }, _sprites);
        LoadAsyncAll("SO", () => { isStageDataLoaded = true; Debug.Log("Data - loaded"); }, _stageDatas);
        LoadAsyncAll("Sound", () => { isStageDataLoaded = true; Debug.Log("Data - loaded");}, _audioClips);

        await UniTask.WaitUntil(() => isScriptLoaded && isSpriteLoaded && isStageDataLoaded);

        isInited = true;
    }


    private void LoadAsync<T>(string addressableID, int total, Action callback, Dictionary<string, T> dict)
    {
        if (_scripts.ContainsKey(addressableID) || _sprites.ContainsKey(addressableID))
            return;

        var asyncOperation = Addressables.LoadAssetAsync<T>(addressableID);
        asyncOperation.Completed += (op) =>
        {
            dict.TryAdd(GetObjectName(addressableID), op.Result);
            if (total == dict.Count)
                callback();
        };
    }

    private void LoadAsyncAll<T>(string path, Action callback, Dictionary<string, T> dict)
    {
        var asyncOperation = Addressables.LoadResourceLocationsAsync(path, typeof(T));

        asyncOperation.Completed += (op) =>
        {
            int total = op.Result.Count;
            for (int i = 0; i < total; i++)
            {
                LoadAsync<T>(op.Result[i].PrimaryKey, total, callback, dict);
            }
        };
    }


    private string GetObjectName(string path)
    {
        int temp = path.IndexOf('/');
        while (temp != -1)
        {
            ++temp;
            path = path.Substring(temp, path.Length - temp);
            temp = path.IndexOf('/');
        }
        return path.Substring(0, path.IndexOf('.'));
    }

    #region get assets

    public static bool TryGetSprite(Characters character, Emotions emotion, out Sprite sprite)
    {
        var self = GameMainContoller.GetCore<LResourcesManager>();
        string key = $"{character}_{emotion}";
        bool has = self._sprites.ContainsKey(key);

        sprite = has ? self._sprites[key] : null;
        return has;
        
    }

    public static bool TryGetScriptData(string key, out List<Script> scriptList)
    {
        var self = GameMainContoller.GetCore<LResourcesManager>();
        bool has = self._scripts.ContainsKey(key);
        scriptList = has ? JsonUtility.FromJson<ScriptLoad>(self._scripts[key].text).GetScript() : null;
        return has;
    }

    public static bool TryGetStageData(int stage, out StageData stageData)
    {
        var self = GameMainContoller.GetCore<LResourcesManager>();
        string key = $"Stage {stage} Data";
        bool has = self._stageDatas.ContainsKey(key);
        stageData = has ? self._stageDatas[key] : null;
        return has;
    }

    public static bool TryGetSoundClip(ESoundType type, out AudioClip clip)
    {
        var self = GameMainContoller.GetCore<LResourcesManager>();
        string key = $"{type}";
        bool has = self._audioClips.ContainsKey(key);
        clip = has ? self._audioClips[key] : null;
        return has;
    }

    #endregion

    public static void CheckInit()
    {
        var self = GameMainContoller.GetCore<LResourcesManager>();
        Debug.Log(self._stageDatas.Count);
    }
}