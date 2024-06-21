using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum EventTypes
{
    None,
    Start,
    Middle,
    End,
    Dead
}

public class EventTrigger : MonoBehaviour
{
    public EventTypes Type;

    private GameScene _scene;

    private AudioClip _pressClip;
    private CustomAudio _ca;

    private void Awake()
    {
        _scene = transform.parent.GetComponent<GameScene>();
        _ca = new CustomAudio(GetComponent<AudioSource>(), ESound.Effect);
    }

    private void Start()
    {
        LResourcesManager.TryGetSoundClip(ESoundType.Stage2_PressButton, out _pressClip);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _scene.GetEvent(Type);
            _ca.PlaySound(_pressClip);
            ActiveFalseAsync().Forget();
            enabled = false;
        }
    }

    private async UniTask ActiveFalseAsync()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_pressClip.length));
        gameObject.SetActive(false);
    }
}
