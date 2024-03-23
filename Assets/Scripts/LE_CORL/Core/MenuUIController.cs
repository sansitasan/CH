using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] Selectable first;

    private void OnEnable()
    {
        first.Select();
    }
}