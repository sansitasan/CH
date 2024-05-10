using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveData
{
    public float audio_Master;
    public float audio_BGM;
    public float audio_SFX;
    public bool[] stageCleared;

    public static event EventHandler<SaveData> OnSaveDataValueChanged;

    public SaveData()
    {
        audio_Master = .5f;
        audio_BGM = .5f;
        audio_SFX = .5f;
        stageCleared = new bool[5];
    }
}
