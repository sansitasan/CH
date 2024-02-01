using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GameSceneInEditor : MonoBehaviour
{
    [SerializeField]
    private StageData _data;
    private PlayerModel _playerModel;

    private void Awake()
    {
        if (!Application.isPlaying)
        {

        }
    }

    private void Init()
    {
        _playerModel = FindObjectOfType<PlayerModel>();
        _playerModel.EditInit(_data);
    }
}
