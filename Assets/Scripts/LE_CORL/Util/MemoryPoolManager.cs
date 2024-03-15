using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MemoryPoolManager : MonoBehaviour, ICore
{
    [SerializeField] int GenerationDefaultCount = 15;

    private Dictionary<string, List<GameObject>> pools;
    private Dictionary<string, GameObject> originals;

    public bool IsIntitalized => isInited;
    bool isInited;

    public void Init()
    {
        pools = new Dictionary<string, List<GameObject>>();
        originals = new Dictionary<string, GameObject>();
        isInited = true;
    }

    
    
    void GeneratePoolObjects(string objName, GameObject origianl = null)
    {
        if (!pools.ContainsKey(objName))
        {
            Debug.LogError("등록되지 않은 오브젝트입니다");
            return;
        }

        var pool = pools[objName];
        var item = origianl != null ? origianl : originals[objName];

        float increaseRatio = (11 / 9f);
        int generationCount = pool.Count == 0 ? GenerationDefaultCount 
            : Mathf.RoundToInt((float)pool.Count * increaseRatio);

        for (int i = 0; i < generationCount; i++)
        {
            var go = Instantiate(item, transform);
            pool.Add(go);
            go.gameObject.SetActive(false);
        }
    }



    public void RegisterMemorypoolObj(string objName, GameObject original)
    {
        if (pools.ContainsKey(objName))
        {
            Debug.LogError("이미 등록된 오브젝트 입니다");
            return;
        }
        originals.Add(objName, original);
        pools.Add(objName, new List<GameObject>());
        GeneratePoolObjects(objName, original); 
    }

    public GameObject GetGameObject(string objName)
    {
        if(!pools.ContainsKey(objName))
        {
            Debug.LogError("등록되지 않은 오브젝트입니다");
            return null;
        }
        var pool = pools[objName];
        var go = pool.Find((item) => !item.activeSelf);

        if (go == null)
        {
            GeneratePoolObjects(objName);
            return GetGameObject(objName);
        }
        else
        {
            return go;
        }
    }

    public async void UnregisterMemoryPool(string objName)
    {
        if (!pools.ContainsKey(objName))
        {
            Debug.LogError("등록되지 않은 오브젝트입니다");
            return;
        }

        await Task.Run(() => DeleteMemoryPool(objName));
        await Task.Run(() => 
        {
            Destroy(originals[objName]);
            originals.Remove(objName);
        });
    }

    void DeleteMemoryPool(string objectName)
    {
        var pool = pools[objectName];
        foreach (var item in pool)
        {
            Destroy(item.gameObject);
        }
        pool.Clear();
        pools.Remove(objectName);
    }

    public void Disable()
    {
        var list = pools.Keys;
        foreach (var item in list)
            DeleteMemoryPool(item);

        pools.Clear();
        originals.Clear();
    }
}
