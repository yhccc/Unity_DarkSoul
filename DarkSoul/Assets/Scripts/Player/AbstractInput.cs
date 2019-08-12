using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public abstract class AbstractInput : MonoBehaviour {

    [Header("----- Additional Button Setting -----")]
    public KeyCode keyA = KeyCode.LeftShift;//对应跑步
    public KeyCode keyB = KeyCode.Space;//对应跑步、跳跃、翻滚、后跳
    public KeyCode keyC = KeyCode.J;//对应攻击
    public KeyCode keyD = KeyCode.K;//对应举盾

    protected MyButton buttonA = new MyButton();
    protected MyButton buttonB = new MyButton();
    protected MyButton buttonC = new MyButton();
    protected MyButton buttonD = new MyButton();

    [Header("----- Output Signals -----")]
    public float Dup;
    public float Dright;
    public float Jup;
    public float Jright;
    public float Dmagnitude;
    public Vector3 Ddirection;

    //1.pressing signal
    public bool run;//是否跑步
    public bool defense;//是否举盾
    //2.trigger once signal
    public bool attack;//攻击信号
    public bool roll;//翻滚信号
    //3.double trigger
    public bool jump;//跳跃信号

    [Header("----- Component Activate -----")]
    public bool inputEnable = true;

    protected float targetDup;
    protected float targetDright;

    protected float velocityDup;
    protected float velocityDright;

    protected Vector2 tempDAxis;
    protected float tempDup;
    protected float tempDright;

    /// <summary>
    /// 将输入的方形坐标转成球形坐标
    /// 解决Bug：斜向移动时playerInput.Dmagnitude最大为根号2,直行时playerInput.Dmagnitude最大为1,造成移动速度不一致
    /// </summary>
    /// <param name="input">输入坐标</param>
    /// <returns>输出坐标</returns>
    protected Vector2 SquareToCircle(Vector2 input)
    {
        Vector2 output = new Vector2(input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f),
            input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f));
        return output;
    }

    /// <summary>
    /// 额外输入按钮控制
    /// </summary>
    protected void ButtonControl()
    {
        #region Button Controll
        buttonB.Tick(Input.GetKey(keyB));

        run = (buttonB.isPressing && !buttonB.isDelaying) || buttonB.isExtending;
        jump = buttonB.onPressed && buttonB.isExtending;
        roll = buttonB.isDelaying && buttonB.onReleased;

        attack = Input.GetKeyDown(keyC);
        defense = Input.GetKey(keyD);
        #endregion
    }
}
