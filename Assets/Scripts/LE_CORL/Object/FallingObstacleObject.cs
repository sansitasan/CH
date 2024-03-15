using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObstacleObject : MonoBehaviour
{
    [SerializeField] SpriteRenderer marker_SpriteRenderer;
    [SerializeField] Transform fallingObstacleTransform;
    [SerializeField] Transform falled_ObjectTransform;
    [SerializeField] PlayerEventTrigger trigger;
    [Space]
    [SerializeField] float marker_Duration;
    [SerializeField] float falling_StartingHeight;
    [SerializeField] AnimationCurve falling_Speed;
    [SerializeField] float falling_Duration;
    [SerializeField] float falling_TriggerCheckingAmount;
    [SerializeField] float falling_TriggerCheckingRadius;
    [SerializeField] float falled_ObjectDuration;
    [SerializeField] float falled_DisableAnimDuration;



    void Initialize()
    {
        marker_SpriteRenderer.gameObject.SetActive(false);

        fallingObstacleTransform.gameObject.SetActive(false);
        fallingObstacleTransform.localPosition = Vector2.up * falling_StartingHeight;

        falled_ObjectTransform.gameObject.SetActive(false);
        var falled_ObjectSpriteRenderer = falled_ObjectTransform.GetComponent<SpriteRenderer>();
        Color currentColor = falled_ObjectSpriteRenderer.color;
        falled_ObjectSpriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1);

        trigger.GetComponent<CircleCollider2D>().radius = falling_TriggerCheckingRadius;
        trigger.SetTriggerActivation(false);
    }

    private void OnEnable()
    {
        Initialize();
        StartCoroutine (FallingObstacleAnimation());
    }

    IEnumerator FallingObstacleAnimation()
    {
        var frame = new WaitForFixedUpdate();

        // mark enable
        marker_SpriteRenderer.gameObject.SetActive(true);
        yield return new WaitForSeconds(marker_Duration);

        // marker disable, 
        marker_SpriteRenderer.gameObject.SetActive(false);

        // falling enable, falling duration
        fallingObstacleTransform.gameObject.SetActive(true);
        float amount = 0;
        while(amount <= 1)
        {
            float currentHeight = falling_StartingHeight * (1 - falling_Speed.Evaluate(amount));
            fallingObstacleTransform.localPosition = new Vector2(0, currentHeight);

            // player trigger checking
            if(amount >= falling_TriggerCheckingAmount)
            {
                trigger.SetTriggerActivation(true);
            }

            amount += (Time.deltaTime / falling_Duration);
            yield return frame;
        }

        // trigger disable
        trigger.SetTriggerActivation(false);

        // falling disable
        fallingObstacleTransform.gameObject.SetActive(false);

        // falled enable, 
        falled_ObjectTransform.gameObject.SetActive(true);
        yield return new WaitForSeconds(falled_ObjectDuration);

        amount = 0;
        SpriteRenderer falled_ObjectSpriteRenderer = falled_ObjectTransform.GetComponent<SpriteRenderer>(); 
        Color currentColor = falled_ObjectSpriteRenderer.color;
        Color targetColor = new Color(currentColor.r, currentColor.g, currentColor.b, 0);
        while(amount <= 1)
        {
            falled_ObjectSpriteRenderer.color = Color.Lerp(currentColor, targetColor, amount);

            amount += (Time.deltaTime / falled_DisableAnimDuration);
            yield return frame;
        }

        // runtime out, gameObject disable, callback, 

        gameObject.SetActive(false);
        yield break;
    }

    [ContextMenu("Initialize")]
    public void Init_Editor() => Initialize();
}
