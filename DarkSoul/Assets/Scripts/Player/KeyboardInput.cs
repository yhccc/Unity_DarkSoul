using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class KeyboardInput : AbstractInput
{
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


    [Header("----- Input Mouse Settings -----")]
    public bool mouseEnable = false;
    public float mouseSensitivityX = 0.8f;
    public float mouseSensitivityY = 1.5f;

    private void Update()
    {
        //将wasd的按下转为（-1，1）间的一个信号
        targetDup = (Input.GetKey(keyUp) ? 1.0f : 0.0f) - (Input.GetKey(keyDown) ? 1.0f : 0.0f);
        targetDright = (Input.GetKey(keyRight) ? 1.0f : 0.0f) - (Input.GetKey(keyLeft) ? 1.0f : 0.0f);
        if (!mouseEnable)
        {
            Jup = (Input.GetKey(keyJUp) ? 1.0f : 0.0f) - (Input.GetKey(keyJDown) ? 1.0f : 0.0f);
            Jright = (Input.GetKey(keyJRight) ? 1.0f : 0.0f) - (Input.GetKey(keyJLeft) ? 1.0f : 0.0f);
        }
        else
        {
            Jup = Input.GetAxis("Mouse Y") * mouseSensitivityY;
            Jright = Input.GetAxis("Mouse X") * mouseSensitivityX;
        }

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

        ButtonControl();
    }
}
