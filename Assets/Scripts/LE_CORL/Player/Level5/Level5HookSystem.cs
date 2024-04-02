using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level5HookSystem : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform hookTrigger;
    [SerializeField] DistanceJoint2D hook;
    [SerializeField] LineRenderer line;

    [SerializeField] float throwingDuration;
    [SerializeField] float throwingMaxDistance;
    [SerializeField] float hookDuration;

    [SerializeField] LayerMask groundMask;

    bool onThrowingHook;
    bool onHook;
    bool hookCancel;

    private void Update()
    {
        if (!onThrowingHook && !onHook)
            return;

        line.SetPosition(0, hookTrigger.position);
        line.SetPosition(1, player.position);
    }

    public void ThrowHook()
    {
        onThrowingHook = true;

        //CheckByRayCast();
        // MoveHookTrigger().Forget();
        MoveHookTrigger().Forget();
    }



    async UniTaskVoid MoveHookTrigger()
    {
        float amount = 0;

        while (amount < 1)
        {
            Vector2 added = Vector2.Lerp(Vector2.zero, Vector2.one * throwingMaxDistance, amount);

            Vector2 targetPoint = (Vector2)player.position + added;
            hookTrigger.position = targetPoint;

            var hit = Physics2D.OverlapCircle(targetPoint, .1f, groundMask);

            if(hit != null)
            {
                hookTrigger.transform.position = hit.ClosestPoint(targetPoint);
                onHook = true;
                break;
            }

            await UniTask.Yield();

            amount += (Time.deltaTime / throwingDuration);
        }

        if (!onHook)
            DisableHook();
        else
        {
            OnHookSuccesed().Forget();
        }
    }

    void CheckByRayCast()
    {
        RaycastHit2D hit = Physics2D.Raycast(player.position, Vector2.one, 10, groundMask);
        
        print(hit.rigidbody.gameObject.name);

        if (hit)
        {
            hookTrigger.position = hit.point;
            hookTrigger.parent = hit.transform;
            OnHookSuccesed().Forget();
        }
        else
        {
            DisableHook();
        }

    }


    async UniTaskVoid OnHookSuccesed()
    {
        hook.transform.position = hookTrigger.position;
        hook.gameObject.SetActive(true);

        var playerRb = player.GetComponent<Rigidbody2D>();
        
        playerRb.velocity = Vector2.zero;

        playerRb.AddForce(Vector2.one * 10, ForceMode2D.Impulse);


        await UniTask.WhenAny(
            UniTask.WaitUntil(() => player.transform.position.x > hookTrigger.transform.position.x),
            // UniTask.WaitUntil(() => !onHook)
            UniTask.WaitUntil(() => hookCancel)
            );

        DisableHook();
    }


    void DisableHook()
    {
        onThrowingHook = false;
        onHook = false;
        hookCancel = false;
        hook.gameObject.SetActive(false);
        hookTrigger.gameObject.SetActive(false);
        hookTrigger.parent = transform;
        line.SetPosition(0, Vector2.zero);
        line.SetPosition(1, Vector2.zero);
    }

    public void CancelHook()
    {
        if (onHook)
        {
            print("hook.Cancel");
            hookCancel = true;
        }
    }


}
