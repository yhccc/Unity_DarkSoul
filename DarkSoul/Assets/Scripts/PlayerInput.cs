using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class PlayerInput : MonoBehaviour {

    //Variable
    [Header("----- Input Key Settings -----")]
    //控制人物移动
    public KeyCode keyUp = KeyCode.W;
    public KeyCode keyDown = KeyCode.S;
    public KeyCode keyLeft = KeyCode.A;
    public KeyCode keyRight = KeyCode.D;
           
    //控制视野旋转
    public KeyCode keyJUp = KeyCode.UpArrow;
    public KeyCode keyJDown = KeyCode.DownArrow;
    public KeyCode keyJLeft = KeyCode.LeftArrow;
    public KeyCode keyJRight = KeyCode.RightArrow;

    public KeyCode keyA = KeyCode.LeftShift;//对应跑步
    public KeyCode keyB = KeyCode.Space;//对应跳跃
    public KeyCode keyC;
    public KeyCode keyD;


    [Header("----- Output Signals -----")]
    public float Dup;
    public float Dright;
    public float Jup;
    public float Jright;
    public float Dmagnitude;
    public Vector3 Ddirection;

    //1.pressing signal
    public bool run;//是否跑步
    //2.trigger once signal
    public bool jump;
    //3.double trigger

    [Header("----- Component Activate -----")]
    public bool inputEnable = true;

    private float targetDup;
    private float targetDright;

    private float velocityDup;
    private float velocityDright;

    private Vector2 tempDAxis;
    private float tempDup;
    private float tempDright;
    private void Update()
    {
        //将wasd的按下转为（-1，1）间的一个信号
        targetDup = (Input.GetKey(keyUp) ? 1.0f : 0.0f) - (Input.GetKey(keyDown) ? 1.0f : 0.0f);
        targetDright = (Input.GetKey(keyRight) ? 1.0f : 0.0f) - (Input.GetKey(keyLeft) ? 1.0f : 0.0f);
        Jup= (Input.GetKey(keyJUp) ? 1.0f : 0.0f) - (Input.GetKey(keyJDown) ? 1.0f : 0.0f);
        Jright = (Input.GetKey(keyJRight) ? 1.0f : 0.0f) - (Input.GetKey(keyJLeft) ? 1.0f : 0.0f);

        //该脚本是否开启
        if (!inputEnable)
        {
            targetDup = 0;
            targetDright = 0;
        }

        //根据wasd按下的程度设置值（存在过渡过程）
        Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, 0.1f);
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, 0.1f);
        //解决Bug：斜向移动时playerInput.Dmagnitude最大为根号2,直行时playerInput.Dmagnitude最大为1,造成移动速度不一致
        tempDAxis = SquareToCircle(new Vector2(Dright, Dup));
        tempDup = tempDAxis.y;
        tempDright = tempDAxis.x;

        //计算到原点之间的距离（适用于摇杆）
        //Bug：斜向移动时playerInput.Dmagnitude最大为根号2,直行时playerInput.Dmagnitude最大为1,造成移动速度不一致
        //Dmagnitude = Mathf.Sqrt((Dup * Dup) + (Dright * Dright));
        Dmagnitude = Mathf.Sqrt((tempDup * tempDup) + (tempDright * tempDright));
        //计算前进的方向
        Ddirection = tempDright * transform.right + tempDup * transform.forward;

        run = Input.GetKey(keyA);
        jump = Input.GetKeyDown(keyB);
    }

    /// <summary>
    /// 将输入的方形坐标转成球形坐标
    /// 解决Bug：斜向移动时playerInput.Dmagnitude最大为根号2,直行时playerInput.Dmagnitude最大为1,造成移动速度不一致
    /// </summary>
    /// <param name="input">输入坐标</param>
    /// <returns>输出坐标</returns>
    private Vector2 SquareToCircle(Vector2 input)
    {
        Vector2 output = new Vector2(input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f),
            input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f));
        return output;
    }
}
