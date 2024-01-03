using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    List<int> a = new List<int>();
    List<int> b;

    void Start()
    {
        b = a;
        a.Add(10);
        Debug.Log(a[0]);
    }


    void Update()
    {
        
    }
}
