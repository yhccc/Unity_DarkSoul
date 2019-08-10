using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class ActorController : MonoBehaviour
{

    public GameObject model;
    public float walkSpeed = 1.4f;//角色移动速度
    public float runSpeed = 2.8f;

    private PlayerInput playerInput;
    private Animator anim;
    private Rigidbody rigid;
    private Vector3 movingVec;//角色移动向量

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        anim = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //控制动画播放和角色转向

        //动画播放
        anim.SetFloat("forward", playerInput.Dmagnitude *
            Mathf.Lerp(anim.GetFloat("forward"), playerInput.run ? 2.0f : 1.0f, 0.5f));//动画过渡效果
        if (playerInput.jump)
            anim.SetTrigger("jump");

        //防止停止按键后，Dup和Dright归0，forward被设置为0，转回原来方向
        //角色转向
        if (playerInput.Dmagnitude > 0.1f)
        {
            model.transform.forward =
                Vector3.Slerp(model.transform.forward, playerInput.Ddirection, 0.3f);//转身过渡效果
        }

        //角色移动向量
        movingVec = (model.transform.forward * playerInput.Dmagnitude * walkSpeed)
            * (playerInput.run ? runSpeed : 1.0f);//跑步为2，行走为1
    }

    private void FixedUpdate()
    {
        //两种让角色移动的方式：
        //rigid.position += movingVec * Time.fixedDeltaTime;//设置位置=速度*时间

        //rigid.velocity = movingVec;//设置速度，存在Bug：由于y分量为0，没有算入重力，当从高处下坡时会飞起来
        rigid.velocity = new Vector3(movingVec.x, rigid.velocity.y, movingVec.z);
    }
}
