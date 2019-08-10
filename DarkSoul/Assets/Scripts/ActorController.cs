using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class ActorController : MonoBehaviour {

    public GameObject model;
    public float walkSpeed = 2.0f;//角色移动速度

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
        anim.SetFloat("forward", playerInput.Dmagnitude);
        //防止停止按键后，Dup和Dright归0，forward被设置为0，转回原来方向
        if (playerInput.Dmagnitude > 0.1f)
        {
            model.transform.forward = playerInput.Ddirection;
        }
        movingVec = model.transform.forward * playerInput.Dmagnitude * walkSpeed;
        //Bug：斜向移动时playerInput.Dmagnitude最大为根号2,直行时playerInput.Dmagnitude最大为1,造成移动速度不一致
    }

    private void FixedUpdate()
    {
        //两种让角色移动的方式：
        //rigid.position += movingVec * Time.fixedDeltaTime;//设置位置=速度*时间
        
        //rigid.velocity = movingVec;//设置速度，存在Bug：由于y分量为0，没有算入重力，当从高处下坡时会飞起来
        rigid.velocity = new Vector3(movingVec.x, rigid.velocity.y, movingVec.z);
    }
}
