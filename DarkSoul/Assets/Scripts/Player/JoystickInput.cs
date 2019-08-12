using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class JoystickInput : AbstractInput {

    [Header("----- Joystick Setting -----")]
    public string axisDright = "axisX";
    public string axisDup = "axisY";
    public string axisJright = "axis3";
    public string axisJup = "axis5";


    private void Update()
    {
        //将wasd的按下转为（-1，1）间的一个信号
        targetDup = Input.GetAxis(axisDup);
        targetDright = Input.GetAxis(axisDright);
        Jup = Input.GetAxis(axisJup);
        Jright = Input.GetAxis(axisJright);

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
