using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class ActorController : MonoBehaviour
{

    public GameObject model;
    public float walkSpeed = 1.4f;//角色行走速度
    public float runSpeed = 2.8f;//角色奔跑速度=walkSpeed*runspeed
    public float jumpHeight = 3.0f;//角色跳跃高度
    public float rollHeight = 3.0f;//角色翻滚高度

    [Space(10)]
    [Header("----- Friction Setting -----")]
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;


    private AbstractInput playerInput;
    private Rigidbody rigid;
    private CapsuleCollider capsuleCol;

    private Animator anim;//角色上的动画组件

    private Vector3 movingVec;//角色移动向量
    private Vector3 thrustVec;//角色动画移动冲量向量
    private bool lockMovingVec;//是否锁死角色移动向量
    private bool canAttack = false;//角色是否可以攻击
    private float lerpTarget;//lerp切换动画权重
    private Vector3 deltaPos;//动画Root Motion的移动量

    private void Awake()
    {
        AbstractInput[] inputs = GetComponents<AbstractInput>();
        foreach (var input in inputs)
        {
            if(input.enabled)
            {
                playerInput = input;
                break;
            }
        }
        capsuleCol = GetComponent<CapsuleCollider>();
        rigid = GetComponent<Rigidbody>();

        anim = model.GetComponent<Animator>();
    }

    private void Update()
    {
        //控制动画播放和角色转向

        //动画播放
        anim.SetFloat("forward", playerInput.Dmagnitude *
            Mathf.Lerp(anim.GetFloat("forward"), playerInput.run ? 2.0f : 1.0f, 0.3f));//动画过渡效果
        anim.SetBool("defense", playerInput.defense);
        if (playerInput.jump)
            anim.SetTrigger("jump");//跳跃动画
        //避免滚动、跳跃等动画播放时可以攻击
        if (playerInput.attack && CheckState("ground") && canAttack)
            anim.SetTrigger("attack");//攻击动画
        if (playerInput.roll || rigid.velocity.magnitude > 5.0f)//下落速度高时
            anim.SetTrigger("roll");//翻滚动画

        //防止停止按键后，Dup和Dright归0，forward被设置为0，转回原来方向
        //角色转向
        if (playerInput.Dmagnitude > 0.1f)
        {
            model.transform.forward =
                Vector3.Slerp(model.transform.forward, playerInput.Ddirection, 0.3f);//转身过渡效果
        }

        if (!lockMovingVec)
        {
            //角色移动向量
            movingVec = (model.transform.forward * playerInput.Dmagnitude * walkSpeed)
                * (playerInput.run ? runSpeed : 1.0f);//跑步为2，行走为1
        }
    }

    private void FixedUpdate()
    {
        //两种让角色移动的方式：
        //rigid.position += movingVec * Time.fixedDeltaTime;//设置位置=速度*时间

        //rigid.velocity = movingVec;//设置速度，存在Bug：由于y分量为0，没有算入重力，当从高处下坡时会飞起来
        rigid.position += deltaPos;//增加动画移动量
        rigid.velocity = new Vector3(movingVec.x, rigid.velocity.y, movingVec.z) + thrustVec;
        thrustVec = Vector3.zero;
        deltaPos = Vector3.zero;
    }

    /// <summary>
    /// 检查动画播放状态
    /// </summary>
    /// <param name="stateName">要检查的动画名称</param>
    /// <param name="layerName">要检查的动画图层</param>
    /// <returns>是否正在播放</returns>
    private bool CheckState(string stateName, string layerName="Base Layer")
    {
        //int layerIndex = anim.GetLayerIndex(layerName);
        //bool result = anim.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
        //return result;
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(layerName)).IsName(stateName);
    }



    #region Message Processing Block

    //Animator Message
    public void OnJumpEnter()
    {
        playerInput.inputEnable = false;
        lockMovingVec = true;
        canAttack = false;
        thrustVec = new Vector3(0, jumpHeight, 0);
    }
    //public void OnJumpExit()
    //{
    //    playerInput.inputEnable = true;
    //    lockMovingVec = false;
    //}
    public void OnGroundEnter()
    {
        playerInput.inputEnable = true;
        lockMovingVec = false;
        canAttack = true;
        capsuleCol.material = frictionOne;
    }
    public void OnGroundExit()
    {
        capsuleCol.material = frictionZero;
    }
    public void OnFallEnter()
    {
        playerInput.inputEnable = false;
        lockMovingVec = true;
    }
    public void OnRollEnter()
    {
        playerInput.inputEnable = false;
        lockMovingVec = true;
        canAttack = false;
        thrustVec = new Vector3(0, rollHeight, 0);
    }
    public void OnJadEnter()
    {
        playerInput.inputEnable = false;
        lockMovingVec = true;
        canAttack = false;
    }
    public void OnJadUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("jabDistance") ;
    }
    public void OnAttack1hAEnter()
    {
        playerInput.inputEnable = false;
        lerpTarget = 1.0f;
    }
    public void OnAttack1hAUpdate()
    {
        //动画图层权重lerp调整
        //float currentWeight = anim.GetLayerWeight(anim.GetLayerIndex("Attack"));
        //currentWeight = Mathf.Lerp(currentWeight, lerpTarget, 0.1f);
        //anim.SetLayerWeight(anim.GetLayerIndex("Attack"), currentWeight);
        anim.SetLayerWeight(anim.GetLayerIndex("Attack"), 
            Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("Attack")), lerpTarget, 0.3f));

        thrustVec = model.transform.forward * anim.GetFloat("attack1hADistance");
    }
    public void OnAttackIdleEnter()
    {
        playerInput.inputEnable = true;
        lerpTarget = 0.0f;
    }
    public void OnAttackIdleUpdate()
    {
        anim.SetLayerWeight(anim.GetLayerIndex("Attack"),
            Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("Attack")), lerpTarget, 0.3f));
    }


    //OnGroudSensor Message
    public void IsOnGround()
    {
        anim.SetBool("onGround", true);
    }
    public void IsNotOnGround()
    {
        anim.SetBool("onGround", false);
    }


    public void OnUpdateRootMotion(Vector3 deltaPos)
    {
        if (CheckState("attack1hC", "Attack"))
            this.deltaPos += (0.6f * this.deltaPos + 0.4f * deltaPos);
    }


    #endregion
}
