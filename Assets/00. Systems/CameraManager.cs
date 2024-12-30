using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    private Camera _mainCamera;
    public int validCameraID;
    private UnityEngine.InputSystem.PlayerInput _playerInput;
    private InputAction _cameraInput;

    private void Start()
    {
        _playerInput = GameObject.Find("Tank").GetComponent<UnityEngine.InputSystem.PlayerInput>();
        _cameraInput = _playerInput.actions["CameraChange"];
        _mainCamera = this.GetComponent<Camera>();
        validCameraID = 1;
        _cameraInput.performed += SetObservationCamera;
        _cameraInput.canceled += context => SetValidCameraID(1);   
    }

    void LateUpdate()
    {
    }

    private void Update()
    {
         
    }
    public void SetValidCameraID(int cameraID)
    {
        validCameraID = cameraID;
    }

    public void MoveCameraWithTank(Transform target, float xAngle, float yAngle)
    {
        transform.position = target.position + target.rotation * new Vector3(1, 2, -3);
        transform.rotation = Quaternion.Euler(xAngle + 15, yAngle, 0);   
        //Math.Clamp(xAngle + 15, -105, 15)
    }

    public void MoveCameraWithMissile(Transform target)
    {
        transform.position = target.position + target.rotation * new Vector3(0, 0, -3);
        transform.rotation = Quaternion.Euler(target.rotation.eulerAngles.x, target.rotation.eulerAngles.y, 0);   
    }

    public void SetObservationCamera(InputAction.CallbackContext context)
    {
        Debug.Log(context.control.name);
        // 현재 눌린 키 값 가져오기
        var pressedKey = context.control.name;
        switch (pressedKey)
        {
            case "1":
                SetValidCameraID(3);
                transform.position = new Vector3(-25, 10, -25);
                transform.rotation = Quaternion.Euler(15, 50, 0);
                break;

            case "2":
                SetValidCameraID(4);
                transform.position = new Vector3(25, 10, -25);
                transform.rotation = Quaternion.Euler(15, 310, 0);
                break;
            case "3":
                SetValidCameraID(5);
                transform.position = new Vector3(25, 10, 25);
                transform.rotation = Quaternion.Euler(15, 230, 0);
                break;
            case "4":
                SetValidCameraID(6);
                transform.position = new Vector3(-25, 10, 25);
                transform.rotation = Quaternion.Euler(15, 130, 0);
                break;

        }
    }
}
