using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class PlayerInput : MonoBehaviour {

    //Variable
    public string keyUp = "w";
    public string keyDown = "s";
    public string keyLeft = "a";
    public string keyRight = "d";

    public float Dup;
    public float Dright;
    public float Dmagnitude;
    public Vector3 Ddirection;

    public bool inputEnable = true;

    private float targetDup;
    private float targetDright;

    private float velocityDup;
    private float velocityDright;
    private void Update()
    {
        //将wasd的按下转为（-1，1）间的一个信号
        targetDup = (Input.GetKey(keyUp) ? 1.0f : 0.0f) - (Input.GetKey(keyDown) ? 1.0f : 0.0f);
        targetDright = (Input.GetKey(keyRight) ? 1.0f : 0.0f) - (Input.GetKey(keyLeft) ? 1.0f : 0.0f);
        
        //该脚本是否开启
        if(!inputEnable)
        {
            targetDup = 0;
            targetDright = 0;
        }

        //根据wasd按下的程度设置值（存在过渡过程）
        Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, 0.1f);
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, 0.1f);

        //计算到原点之间的距离（适用于摇杆）
        Dmagnitude = Mathf.Sqrt((Dup * Dup) + (Dright * Dright));
        //计算前进的方向
        Ddirection = Dright * transform.right + Dup * transform.forward;
    }
}
