using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class AnimTriggerController : MonoBehaviour {

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    //动画事件
    public void ResetTrigger(string triggerName)
    {
        anim.ResetTrigger("attack");
    }
}
