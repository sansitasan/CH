using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SoundVolume
{
    [Range(0, 1f)]
    public float TotalVolume;
    [Range(0, 1f)]
    public float BGMVolume;
    [Range(0, 1f)]
    public float EffectVolume;

    public SoundVolume(float total = 1f, float bgm = 0.5f, float effect = 0.5f)
    {
        TotalVolume = total;
        BGMVolume = bgm;
        EffectVolume = effect;
    }
}
