using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class KeyboardInput : AbstractInput
{
    //键鼠输入
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

        //判断组件是否可用
        ComponentIsActivate();

        //角色移动输入转输出
        InputToOutput();

        //额外按钮控制方法
        ButtonControl();
    }
}
