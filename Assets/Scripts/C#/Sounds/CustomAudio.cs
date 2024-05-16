using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ESound
{
    Total,
    Effect,
    Bgm
}

public class CustomAudio : IDisposable
{
    private AudioSource _audioSource;
    private ESound _type;
    public float OwnVolume { get; set; }

    public CustomAudio(AudioSource a, ESound type, float ownVolume = 1)
    {
        _audioSource = a;
        _type = type;
        SoundVolume volume = SoundManager.Volume;
        switch (_type)
        {
            case ESound.Effect:
                SoundManager.EffectAction -= SetVolume;
                SoundManager.EffectAction += SetVolume;
                SetVolume(volume.EffectVolume * volume.EffectVolume);
                break;

            case ESound.Bgm:
                SoundManager.BGMAction -= SetVolume;
                SoundManager.BGMAction += SetVolume;
                SetVolume(volume.BGMVolume * volume.EffectVolume);
                break;
        }

        SceneManager.activeSceneChanged -= InitVolume;
        SceneManager.activeSceneChanged += InitVolume;
        OwnVolume = ownVolume;
    }

    public void SetPitch(float value)
    {
        _audioSource.pitch = value;
    }

    public void PlaySound(AudioClip clip, float pitch = 1f)
    {
        if (clip != null)
        {
            PlaySound(clip, _type, pitch);
        }

        else
        {
            SetPitch(pitch);
        }
    }

    public void PlaySound(AudioClip clip, ESound type, float pitch = 1f)
    {

        if (_audioSource.isPlaying)
        {
            if (_audioSource.clip != clip)
                _audioSource.Stop();
            else
                return;
        }

        _audioSource.pitch = pitch;

        if (type == ESound.Bgm)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }

        else
            _audioSource.PlayOneShot(clip);
    }

    public void StopSoundFade(float time = 1.5f)
    {
        _audioSource.DOFade(0, time);
    }

    public void StopSound()
    {
        if (_audioSource.isPlaying)
            _audioSource.Stop();
    }

    public void Dispose()
    {
        SceneManager.activeSceneChanged -= InitVolume;
        SoundManager.EffectAction -= SetVolume;
        SoundManager.BGMAction -= SetVolume;
        _audioSource = null;
    }

    private void InitVolume(Scene prev, Scene next)
    {
        if (_audioSource != null)
        {
            SoundVolume volume = SoundManager.Volume;
            switch (_type)
            {
                case ESound.Effect:
                    SetVolume(volume.EffectVolume * volume.EffectVolume);
                    break;

                case ESound.Bgm:
                    SetVolume(volume.BGMVolume * volume.BGMVolume);
                    break;
            }
        }
        else
            Dispose();
    }

    private void SetVolume(float value)
    {
        _audioSource.volume = value;
        _audioSource.pitch = 1;
    }
}