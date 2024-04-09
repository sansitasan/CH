using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Util
{
    [System.Diagnostics.Conditional("ENABLE_LOG")]
    public static void Log(object message)
    {
        Debug.Log(message);
    }

    public static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T component = null;
        go.TryGetComponent(out component);

        if (component == null)
        {
            Log($"{nameof(T)} is not exist {go.name}");
            component = go.AddComponent<T>();
        }
        return component;
    }

    public static T GetOrAddComponent<T>(Transform t) where T : Component
    {
        T component = null;
        t.gameObject.TryGetComponent(out component);

        if (component == null)
        {
            Log($"{nameof(T)} is not exist {t.name}");
            component = t.gameObject.AddComponent<T>();
        }
        return component;
    }

    public static T GetComponentInChild<T>(Transform t) where T : Component
    {
        T component = null;
        for (int i = 0; i < t.childCount; ++i)
        {
            t.GetChild(i).TryGetComponent(out component);

            if (component != null)
            {
                return component;
            }
        }
        Log($"{nameof(T)} is not exist {t.name}");
        return null;
    }

    public static T AddComponentInChild<T>(Transform t, int idx) where T : Component
    {
        if (t.childCount < idx)
        {
            Log($"{nameof(T)} is not exist {idx}th child");
            return null;
        }

        else
        {
            Transform child = t.GetChild(idx);
            return t.GetComponent<T>();
        }
    }

    public static List<T> ShuffleList<T>(List<T> list, int seed)
    {
        System.Random r = new System.Random(seed);
        for (int i = 0; i < list.Count - 1; i++)
        {
            int targetIDX = r.Next(i, list.Count);
            T tmp = list[i];
            list[i] = list[targetIDX];
            list[targetIDX] = tmp;
        }
        return list;
    }


    public static int TryGetUnityObjectsOfTypeFromPath<T>(string path, out List<T> assetsFound) where T : UnityEngine.Object
    {
        string[] filePaths = System.IO.Directory.GetFiles(path);

        int countFound = 0;
        assetsFound = new List<T>();

        Debug.Log(filePaths.Length);
        if (filePaths == null || filePaths.Length <= 0)
            return countFound;

        for (int i = 0; i < filePaths.Length; i++)
        {
            UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(filePaths[i]);
            if (obj is T asset)
            {
                countFound++;
                if (!assetsFound.Contains(asset))
                {
                    assetsFound.Add(asset);
                }
            }
        }

        return countFound;
    }



    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys = new List<TKey>();

        [SerializeField]
        private List<TValue> values = new List<TValue>();

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
            for (int i = 0; i < keys.Count; i++)
                Add(keys[i], values[i]);
        }

    }

}
