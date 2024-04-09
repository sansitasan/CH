using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Level4FallingObject : MonoBehaviour
{
    [SerializeField] Level4FallingObjSetting objSetting;

    [SerializeField] SpriteRenderer mark_renderer;
    [SerializeField] SpriteRenderer anim_renderer;
    [SerializeField] PlayerEventTrigger trigger;

    CancellationTokenSource cancellationToken;

    public void SetFallinObject(Level4FallingObjSetting objSetting)
    {
        this.objSetting = objSetting;
        objSetting.GeneratePolyTrigger(transform).GetComponent<PlayerEventTrigger>();
    }

    private void OnEnable()
    {
        if(cancellationToken != null)
        {
            cancellationToken.Dispose();
        }
        cancellationToken = new CancellationTokenSource();

        FallingAction(cancellationToken).Forget();
    }

    private void OnDisable()
    {
        cancellationToken.Cancel();
    }

    async UniTaskVoid FallingAction(CancellationTokenSource cancellation)
    {
        while(!cancellation.IsCancellationRequested)
        {
            // 초기화
            anim_renderer.color = Color.white;
            // 낙하지점 표시
            mark_renderer.color = objSetting.mark_Color;
            mark_renderer.gameObject.SetActive(true);

            // 대기
            await UniTask.Delay(TimeSpan.FromSeconds(objSetting.mark_Duration));

            // 낙하 애니메이션 전처리
            anim_renderer.transform.localPosition = Vector2.up * objSetting.GetValuesWithAmount(0).Item1;
            anim_renderer.color = Color.white;
            anim_renderer.gameObject.SetActive(true);

            // 애니메이션
            float amount = 0;
            bool isChecking = false;
            bool isFalled = false;

            while (amount < 1)
            {
                var tmp = objSetting.GetValuesWithAmount(amount);
                anim_renderer.sprite = objSetting.GetSpriteByState(isFalled, amount);
                anim_renderer.transform.localPosition = Vector2.up * tmp.Item1;

                if (tmp.Item2 && !isChecking)
                {
                    isChecking = true;
                    trigger.SetTriggerActivation(true);
                    trigger.gameObject.SetActive(true);
                }

                await UniTask.Yield();
                amount += Time.deltaTime / objSetting.falling_Duration;
            }

            // 판정 및 애니메이션 후처리
            mark_renderer.gameObject.SetActive(false);
            trigger.SetTriggerActivation(false);
            trigger.gameObject.SetActive(false);

            // 낙하 애니메이션 후처리
            amount = 0;
            isFalled = true;
            while (amount < 1)
            {
                anim_renderer.sprite = objSetting.GetSpriteByState(isFalled, amount);

                Color endColor = Color.black;
                endColor.a = 0;
                anim_renderer.color = Color.Lerp(Color.white, endColor, amount);

                await UniTask.Yield();
                amount += Time.deltaTime / objSetting.falled_DisableAnimDuration;
            }

            // 비활성화
            anim_renderer.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// 런타임용 x, 에디터
    /// </summary>
    [ContextMenu("Editor: Apply Object Setting")]
    void ApplyObjectSettings_Editor()
    {
        if(objSetting == null)
        {
            Debug.LogError("오브젝트 세팅이 없습니다.");
            return;
        }
        if(trigger != null)
            DestroyImmediate(trigger.gameObject);

        mark_renderer.sprite = objSetting.GetFirstAnimFrame();
        mark_renderer.gameObject.SetActive(false);
        anim_renderer.sprite = objSetting.GetFirstAnimFrame();
        anim_renderer.gameObject.SetActive(false);


        GameObject triggerGO = objSetting.GeneratePolyTrigger(transform);
        trigger = triggerGO.GetComponent<PlayerEventTrigger>();

        triggerGO.SetActive(false);
        gameObject.SetActive(false);
    }
}
