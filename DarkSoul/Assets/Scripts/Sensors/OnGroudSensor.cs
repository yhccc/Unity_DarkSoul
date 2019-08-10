using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 监听角色是否在地面
/// </summary>
public class OnGroudSensor : MonoBehaviour {

    public CapsuleCollider capsuleCollider;
    public float offset = 0.1f;

    private float radius;
    private Vector3 point1;
    private Vector3 point2;
    private Collider[] outputCols;

    private void Awake()
    {
        radius = capsuleCollider.radius-0.05f;
    }

    private void FixedUpdate()
    {
        point1 = transform.position + transform.up * (radius - offset);
        point2 = transform.position + 
            transform.up * (capsuleCollider.height - offset) - transform.up * radius;

        outputCols = Physics.OverlapCapsule(point1, point2, radius,LayerMask.GetMask("Ground"));
        if (outputCols.Length > 0)
        {
            SendMessageUpwards("IsOnGround");
        }
        else
        {
            SendMessageUpwards("IsNotOnGround");
        }
    }
}
