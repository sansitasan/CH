using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour, IInteractable
{
    private Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }
    public bool Interact(Vector3 dir)
    {
        //Destroy!
        _anim.SetBool("Destroy", true);
        GameScene.Instance.GetEvent(EventTypes.Middle);
        return false;
    }
}
