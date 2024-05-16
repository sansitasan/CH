using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum ESoundType
{
    Stage1_Jump,
    Stage1_Grass_Move,
    Stage1_Grass_Land,
    Stage1_Rock_Move,
    Stage1_Rock_Land,
    Stage2_Move,
    Stage2_Skill,
    Stage2_PressButton
}

public class SoundManager : MonoBehaviour, ICore
{
    public static SoundVolume Volume { 
        get 
        {
            var self = GameMainContoller.GetCore<SoundManager>();
            return self._volume;
        } 
    }

    public bool IsIntitalized => _isInited;
    private bool _isInited = false;

    private SoundVolume _volume;

    public static event Action<float> BGMAction;
    public static event Action<float> EffectAction;

    public void Play2DSound(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, Vector3.zero);
    }

    public static void SaveSound(SoundVolume vol)
    {
        var self = GameMainContoller.GetCore<SoundManager>();

        string voldata = JsonUtility.ToJson(vol);
        string path = Path.Combine(Application.persistentDataPath, "volume");
        File.WriteAllText(path, voldata);
        self._volume = vol;
        BGMAction?.Invoke(self._volume.BGMVolume * self._volume.TotalVolume);
        EffectAction?.Invoke(self._volume.EffectVolume * self._volume.TotalVolume);
    }

    public static void SetVolumeTemporary(float vol, ESound type)
    {
        var self = GameMainContoller.GetCore<SoundManager>();
        if (type == ESound.Bgm)
            BGMAction?.Invoke(vol * self._volume.TotalVolume);
        else if (type == ESound.Effect)
            EffectAction?.Invoke(vol * self._volume.TotalVolume);
        else
        {
            BGMAction?.Invoke(vol * self._volume.BGMVolume);
            EffectAction?.Invoke(vol * self._volume.EffectVolume);
        }
    }

    private SoundVolume LoadVolume(string path)
    {
        if (File.Exists(path))
        {
            string voldata = File.ReadAllText(path);
            return JsonUtility.FromJson<SoundVolume>(voldata);
        }
        else
        {
            return new SoundVolume(1, 0.3f, 0.3f);
        }
    }

    public void Init()
    {
        _volume = LoadVolume(Path.Combine(Application.persistentDataPath, "volume"));
        BGMAction?.Invoke(_volume.BGMVolume);
        EffectAction?.Invoke(_volume.EffectVolume);

        _isInited = true;
    }

    public void Disable()
    {

        _isInited = false;
    }
}