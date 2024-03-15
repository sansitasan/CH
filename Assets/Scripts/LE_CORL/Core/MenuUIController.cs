using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] Button btn;

    private void OnEnable()
    {
        btn.onClick.AddListener(() => print("menu btn click"));
    }
}