using System.Collections;
using UnityEngine;

//[CreateAssetMenu()]
public class Level4FallingObjSetting : ScriptableObject
{
    [Header("Mark")]
    public float mark_Duration;
    public Color mark_Color;

    [Header("Falling")]
    [SerializeField] Sprite[] falling;
    [SerializeField] float falling_StartHeigth;
    [SerializeField] AnimationCurve falling_Speed;
    [SerializeField, Range(0, 1)] float falling_TriggerCheckingAmount;
    public float falling_Duration;

    [Header("Falled")]
    [SerializeField] Sprite[] falled;
    public float falled_DisableAnimDuration;

    /// <summary>
    /// 진행도를 입력받아 스프라이트, 높이, 판정여부를 반환
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public (float, bool) GetValuesWithAmount(float amount)
    {
        float currentHeight = falling_StartHeigth * (1 - falling_Speed.Evaluate(amount));
        bool onChecking = (1 - amount <= falling_TriggerCheckingAmount);

        return (currentHeight, onChecking);
    }

    public Sprite GetFirstAnimFrame()
    {
        return falling[0];
    }


    public Sprite GetSpriteByState(bool isFalled, float amount)
    {
        int lastIndex = isFalled ? falling.Length - 1 : falling.Length - 1;
        int targetIDX = Mathf.FloorToInt(Mathf.Lerp(0, lastIndex, amount));
        Sprite sprite = isFalled ? falled[targetIDX] : falling[targetIDX];

        return sprite;
    }

    public GameObject GeneratePolyTrigger(Transform parent)
    {
        GameObject triggerGO = new GameObject("trigger");
        triggerGO.SetActive(false);

        SpriteRenderer rnd = triggerGO.AddComponent<SpriteRenderer>();
        rnd.sprite = falling[0];
        rnd.color = Color.clear;
        rnd.enabled = false;

        var collider = triggerGO.AddComponent<PolygonCollider2D>();
        collider.isTrigger = true;

        var playerEventTrigger = triggerGO.AddComponent<PlayerEventTrigger>();
        playerEventTrigger.Init("level4_falllingObject");


        triggerGO.transform.parent = parent;
        triggerGO.transform.localPosition = Vector3.zero;
        return triggerGO;
    }
}