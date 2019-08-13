using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class CameraController : MonoBehaviour
{

    public float horizontalSpeed = 100.0f;
    public float verticalSpeed = 80.0f;

    private GameObject playerHandle;
    private GameObject cameraHandle;
    private AbstractInput playerInput;
    private GameObject model;
    new private Camera camera;

    private float tempEulerX;
    private Vector3 tempModelEuler;
    private Vector3 cameraDampVelocity;

    private LockTarget lockTarget = null;
    public Image lockDot;
    public bool lockState;

    private void Awake()
    {
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
        AbstractInput[] inputs = playerHandle.GetComponents<AbstractInput>();
        foreach (var input in inputs)
        {
            if (input.enabled)
            {
                playerInput = input;
                break;
            }
        }
        model = playerInput.GetComponent<ActorController>().model;
        camera = Camera.main;
        tempEulerX = camera.transform.localEulerAngles.x;

        Cursor.lockState = CursorLockMode.Locked;//运行时鼠标隐藏

        lockDot.enabled = false;
        lockState = false;
    }

    private void Update()
    {
        if (lockTarget != null)
        {
            lockDot.rectTransform.position = Camera.main.WorldToScreenPoint
                (lockTarget.obj.transform.position + new Vector3(0, lockTarget.halfHeight, 0));
            if (Vector3.Distance(model.transform.position, lockTarget.obj.transform.position) > 10)
            {
                Unlock();
            }
        }
    }

    private void FixedUpdate()
    {
        if (lockTarget == null)
        {
            tempModelEuler = model.transform.eulerAngles;

            //相机左右旋转
            playerHandle.transform.Rotate(Vector3.up, playerInput.Jright * horizontalSpeed * Time.fixedDeltaTime);

            //如果直接限制X，则存在Bug
            //cameraHandle.transform.Rotate(Vector3.right, - playerInput.Jup * verticalSpeed * Time.deltaTime);

            //相机上下旋转
            tempEulerX -= playerInput.Jup * verticalSpeed * Time.deltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, -30, 40);
            cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);

            //移除模型随相机左右旋转而旋转
            model.transform.eulerAngles = tempModelEuler;
        }
        else//lockOn模式
        {
            Vector3 tempForward = lockTarget.obj.transform.position - model.transform.position;
            tempForward.y = 0;
            playerHandle.transform.forward = tempForward;
            cameraHandle.transform.LookAt(lockTarget.obj.transform.position);
        }

        //相机Lerp跟随
        //camera.transform.position = Vector3.Lerp(camera.transform.position,transform.position,0.2f);
        //相机SmoothDamp跟随
        camera.transform.position = Vector3.SmoothDamp(camera.transform.position, transform.position, ref cameraDampVelocity, 0.2f);

        //camera.transform.eulerAngles = transform.eulerAngles;
        camera.transform.LookAt(cameraHandle.transform);
    }

    public void LockUnlock()
    {
        Vector3 modelOrigin = model.transform.position + new Vector3(0, 1.0f, 0);
        Vector3 boxCenter = modelOrigin + model.transform.forward * 5.0f;
        Collider[] cols = Physics.OverlapBox(boxCenter, new Vector3(0.5f, 0.5f, 5.0f),
            model.transform.rotation, LayerMask.GetMask("Enemy"));

        if (cols.Length == 0)
        {
            Unlock();
        }
        else
        {
            if (lockTarget != null && lockTarget.obj == cols[cols.Length - 1].gameObject)
            {
                Unlock();
            }
            else
            {
                lockTarget = new LockTarget(cols[cols.Length - 1].gameObject, 
                    cols[cols.Length - 1].bounds.extents.y);
                lockDot.enabled = true;
                lockState = true;
            }
        }
    }

    private void Unlock()
    {
        lockTarget = null;
        lockDot.enabled = false;
        lockState = false;
    }


    private class LockTarget
    {
        public GameObject obj;
        public float halfHeight;

        public LockTarget(GameObject obj,float halfHeight)
        {
            this.obj = obj;
            this.halfHeight = halfHeight;
        }
    }
}
