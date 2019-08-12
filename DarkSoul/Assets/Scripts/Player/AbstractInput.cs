using UnityEngine;

/// <summary>
/// 用户输入抽象类
/// </summary>
public abstract class AbstractInput : MonoBehaviour
{

    [Header("----- Additional Button Setting -----")]//额外按键设置（可自定义）
    public KeyCode keyA = KeyCode.LeftShift;
    public KeyCode keyB = KeyCode.Space;//对应跑步、跳跃、翻滚、后跳
    public KeyCode keyC = KeyCode.J;//对应攻击
    public KeyCode keyD = KeyCode.K;//对应举盾

    //用于监听对应按键的状态
    protected MyButton buttonA = new MyButton();
    protected MyButton buttonB = new MyButton();
    protected MyButton buttonC = new MyButton();
    protected MyButton buttonD = new MyButton();

    [Header("----- Output Signals -----")]//输出信号
    public float Dup;
    public float Dright;//用于控制角色移动的信号
    public float Jup;
    public float Jright;//用于控制相机视野的信号
    public float Dmagnitude;//角色移动程度
    public Vector3 Ddirection;//角色移动方向

    //1.pressing signal
    public bool run;//是否跑步
    public bool defense;//是否举盾
    //2.trigger once signal
    public bool attack;//攻击信号
    public bool roll;//翻滚信号
    //3.double trigger
    public bool jump;//跳跃信号

    [Header("----- Component Activate -----")]
    public bool inputEnable = true;//脚本是否可用

    //根据不同输入有不同的设置目标Dup、Dright的方法，写在子类中
    protected float targetDup;
    protected float targetDright;

    //Dup->targetDup、Dright->targetDright缓冲过渡使用
    protected float velocityDup;
    protected float velocityDright;

    //SquareToCircle方法中间变量
    protected Vector2 tempDAxis;
    protected float tempDup;
    protected float tempDright;
    //上述7个变量是角色移动输入转输出的中间变量

    /// <summary>
    /// 将输入的方形坐标转成球形坐标
    /// 解决Bug：斜向移动时playerInput.Dmagnitude最大为根号2,直行时 
    /// playerInput.Dmagnitude最大为1,造成移动速度不一致
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


    /// <summary>
    /// 是否开启该脚本
    /// </summary>
    protected void ComponentIsActivate()
    {
        if (!inputEnable)
        {
            targetDup = 0;
            targetDright = 0;
        }
    }

    /// <summary>
    /// 角色移动输入转输出
    /// </summary>
    protected void InputToOutput()
    {
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
    }
}