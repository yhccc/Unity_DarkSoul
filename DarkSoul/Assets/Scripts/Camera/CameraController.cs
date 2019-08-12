using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class CameraController : MonoBehaviour {

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
    }

    private void FixedUpdate()
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

        //相机Lerp跟随
        //camera.transform.position = Vector3.Lerp(camera.transform.position,transform.position,0.2f);
        //相机SmoothDamp跟随
        camera.transform.position = Vector3.SmoothDamp(camera.transform.position, transform.position, ref cameraDampVelocity, 0.2f);

        //camera.transform.eulerAngles = transform.eulerAngles;
        camera.transform.LookAt(cameraHandle.transform);
    }

}
