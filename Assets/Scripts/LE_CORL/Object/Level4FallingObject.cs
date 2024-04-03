using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level4FallingObject : MonoBehaviour
{
    [SerializeField] PlayerEventTrigger trigger;
    [SerializeField] Animator anim;

    [SerializeField] Color markColor;
    [SerializeField] float markDuration;
    [SerializeField] float startHeigth;
    [SerializeField] float fallingDuration;
    [SerializeField] float collisionCheckDuration;
    [SerializeField] float disableDuration;



    private void OnEnable()
    {
        trigger.Init("level4_falllingObject");
        trigger.SetTriggerActivation(false);
        Action().Forget();
    }

    async UniTaskVoid Action()
    {
        var markSpriteRenderer = trigger.GetComponent<SpriteRenderer>();
        var objSpriteRenderer = anim.GetComponent<SpriteRenderer>();

        // 낙하지점 표시
        markSpriteRenderer.color = markColor;
        
        // 대기
        await UniTask.Delay(TimeSpan.FromSeconds(markDuration));

        // 낙하 애니메이션 전처리
        var token = new System.Threading.CancellationToken(false);
        anim.transform.localPosition = Vector2.up * startHeigth;
        objSpriteRenderer.color = Color.white;
        anim.gameObject.SetActive(true);
        anim.SetTrigger("hit");

        // 낙하 애니메이션
        var falling = anim.transform.DOLocalMove(Vector3.zero, fallingDuration);
        await falling.WithCancellation(token);

        // 낙하 애니메이션 후처리
        markSpriteRenderer.color = Color.clear;

        // 충돌 감지
        trigger.SetTriggerActivation(true);
        await UniTask.Delay(TimeSpan.FromSeconds(collisionCheckDuration));

        // 오브젝트 후처리
        Color endColor = Color.black;
        endColor.a = 0;
        var disable = objSpriteRenderer.DOBlendableColor(endColor, disableDuration);

        // 대기
        await disable.WithCancellation(token);

        // 비활성화
        anim.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
