using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine.SceneManagement;

public class LoadingSceneController : MonoBehaviour
{
    public static bool OnAnimation { get; private set; }

    public RawImage fadePanel;
    public Color fadePanelColor = Color.black;
    public float fadeDuration = .5f;


    public async UniTaskVoid Fade(bool isFadeIn)
    {
        OnAnimation= true;

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 fromSize = isFadeIn ? Vector2.zero : screenSize;
        Vector2 toSize = isFadeIn ? screenSize : Vector2.zero;

        var panelRectT = fadePanel.GetComponent<RectTransform>();
        panelRectT.sizeDelta = fromSize;

        var cancellationToken = this.GetCancellationTokenOnDestroy();

        await UniTask.WhenAll(
            panelRectT.DOSizeDelta(toSize, fadeDuration).WithCancellation(cancellationToken),
            UniTask.Delay(TimeSpan.FromSeconds(fadeDuration))
            );

        OnAnimation = false;
    }

    private void OnEnable()
    {
        fadePanel.color = fadePanelColor;
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        Fade(true).Forget();
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        Fade(false).Forget();
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }
}