using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class JoystickInput : AbstractInput {

    //摇杆输入
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

        //判断组件是否可用
        ComponentIsActivate();

        //角色移动输入转输出
        InputToOutput();

        //额外按钮控制方法
        ButtonControl();
    }
}
