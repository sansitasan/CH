using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class TResourceManager
{
    [Header("로드된 스크립트")]
    [SerializeField]
    private Util.SerializableDictionary<string, TextAsset> _scripts = new Util.SerializableDictionary<string, TextAsset>();

    [Header("로드된 스프라이트")]
    [SerializeField]
    private Util.SerializableDictionary<string, Sprite> _sprites = new Util.SerializableDictionary<string, Sprite>();

    private static TResourceManager d_instance;

    public static TResourceManager Instance
    {
        get
        {
            if (d_instance == null)
            {
                d_instance = new TResourceManager();
            }

            return d_instance;
        }
    }

    public async UniTask<bool> LoadAsyncAssets()
    {
        if (_scripts.Count != 0)
            return true;

        List<UniTask<bool>> tasks = new List<UniTask<bool>>();
        tasks.Add(LoadAsyncAll<TextAsset>("Scripts"));
        tasks.Add(LoadAsyncAll<Sprite>("Image"));

        await UniTask.WhenAll(tasks);
        return true;
    }

    public List<Script> TryGetScript(string path)
    {
        if (_scripts.ContainsKey($"{path}"))
        {
            return JsonUtility.FromJson<ScriptLoad>(_scripts[path].text).GetScript();
        }

        else
        {
            return null;
        }
    }

    public Sprite GetSprite(Characters c, Emotions e)
    {
        string key = $"{c}_{e}";
        if (_sprites.ContainsKey(key))
        {
            return _sprites[key];
        }
        else
        {
            return null;
        }
    }

    public List<string> GetScriptName()
    {
        return _scripts.Keys.ToList();
    }

    private async UniTask LoadAsync<T>(string path)
    {
        if (_scripts.TryGetValue(path, out var obj) || _sprites.TryGetValue(path, out var obj1))
        {
            return;
        }

        var asyncOperation = Addressables.LoadAssetAsync<T>(path);
        asyncOperation.Completed += (op) =>
        {
            if (typeof(T) == typeof(TextAsset))
            {
                _scripts.TryAdd(GetObjectName(path), op.Result as TextAsset);
            }
            else if (typeof(T) == typeof(Sprite))
            {
                _sprites.TryAdd(GetObjectName(path), op.Result as Sprite);
            }
            Addressables.Release(asyncOperation);
        };

        await UniTask.WaitUntil(() => asyncOperation.IsDone);
    }

    private async UniTask<bool> LoadAsyncAll<T>(string path)
    {
        List<UniTask> tasks = new List<UniTask>();
        var asyncOperation = Addressables.LoadResourceLocationsAsync(path, typeof(T));
        tasks.Add(UniTask.WaitUntil(() => asyncOperation.IsDone));
        asyncOperation.Completed += (op) =>
        {
            int total = op.Result.Count;
            for (int i = 0; i < total; i++)
            {
                tasks.Add(LoadAsync<T>(op.Result[i].PrimaryKey));
            }
            Addressables.Release(asyncOperation);
        };

        await UniTask.WhenAll(tasks);

        return true;
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
}
