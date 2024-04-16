using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Level5Coin : MonoBehaviour
{


    public enum CoinType { Test, Silver, Gold, Event }

    [SerializeField] Color[] coinColors;

    [SerializeField] private CoinType m_Type;
    [SerializeField] SpriteRenderer m_SpriteRenderer;
    PlayerEventTrigger m_trigger;

    public void SetCoinType(CoinType type) => m_Type = type;


    private void OnEnable()
    {
        m_trigger = GetComponent<PlayerEventTrigger>();
        SetDefaultState();
        PlayerEventTrigger.OnPlayerEntered += PlayerEventTrigger_OnPlayerEntered;
    }

    private void PlayerEventTrigger_OnPlayerEntered(object sender, System.EventArgs e)
    {
        var playerEventTrigger = (PlayerEventTrigger)sender;
        if (playerEventTrigger == m_trigger)
        {
            m_trigger.SetTriggerActivation(false);
            DisableAnimation().Forget();
        }
    }

    void SetDefaultState()
    {
        m_SpriteRenderer.color = coinColors[(int)m_Type];
        m_SpriteRenderer.transform.localPosition = Vector2.zero;

        string typeSelf = m_Type.ToString().ToLower();

        m_trigger.Init($"level5_coin_{typeSelf}");
        m_trigger.SetTriggerActivation(true);
    }


    async UniTaskVoid DisableAnimation()
    {
        float DISABLE_ANIMATION_DURATION = .3f;
        float DISABLE_ANIMATION_DISTANCE = 1.0f;

        var curColor = coinColors[(int)m_Type];
        var endColor = new Color(curColor.r, curColor.g, curColor.b, 0.3f);
        var token = new CancellationToken(false);

        await UniTask.WhenAll(
            m_SpriteRenderer.DOColor(endColor, DISABLE_ANIMATION_DURATION).WithCancellation(token),
            m_SpriteRenderer.transform.DOLocalMoveY(DISABLE_ANIMATION_DISTANCE, DISABLE_ANIMATION_DURATION).WithCancellation(token)
            );

        gameObject.SetActive(false);
    }
}
