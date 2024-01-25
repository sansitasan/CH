using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum Emotions
{
    None,
    Smile = 1,
    Curious,
    Disgusting,
    Shy,
    Embarrassment,
    Sad,
    Smile_Big,
    Angry,
    Quip,
    Idle
}

public enum Characters
{
    None,
    Tabi,
    BD,
    HBD,
    BBD,
    Kanna,
    Yuni
}

public class ResourceManager
{
    private Dictionary<string, TextAsset> _scripts = new Dictionary<string, TextAsset>();
    private Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();

    private static ResourceManager d_instance;
    public static ResourceManager Instance { 
        get
        {
            if (d_instance == null)
            {
                d_instance = new ResourceManager();
            }

            return d_instance;
        } 
    }

    public void Init() { }

    private ResourceManager()
    {
        LoadAsyncAll<TextAsset>("Scripts");
        LoadAsyncAll<Sprite>("Image");
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

    private void LoadAsync<T>(string path)
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
        };
    }

    private void LoadAsyncAll<T>(string path)
    {
        var asyncOperation = Addressables.LoadResourceLocationsAsync(path, typeof(T));

        asyncOperation.Completed += (op) =>
        {
            int total = op.Result.Count;
            for (int i = 0; i < total; i++)
            {
                LoadAsync<T>(op.Result[i].PrimaryKey);
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
}
