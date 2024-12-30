using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cursor = UnityEngine.UIElements.Cursor;

public class TankController : MonoBehaviour
{
    //조작 관련 프로퍼티
    [Header("[Default Reference]")] 
    [SerializeField] private GameObject _tankTower;
    [SerializeField] private GameObject _tankCannon;
    [SerializeField] private Camera _camera;
    private UnityEngine.InputSystem.PlayerInput _playerInput;
    private InputAction _moveInput;
    private InputAction _fireInput;
    private InputAction _mouseLookInput;
    private InputAction _zoomInput;
    private Rigidbody _rigidbody;
    private Tank _tank;
    private float zoomSeneitivity = 0.05f;
    
    [Header("[Mouse Property]")] 
    public float minXLook = -75.0f;
    public float maxXLook = 0.0f;
    private float mouseSensitivity = 10.0f;
    private Vector2 mouseDelta;
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    
    [Header("[Keyboard Property]")]
    [SerializeField] float speed = 2.0f;
    [SerializeField] private float rotationSpeed = 50.0f;
    private Vector3 moveDirection;
    private float rotationAmount;
   
    //가하는 힘 슬라이더 관련 프로퍼티
    [Header("[Projection Power]")]
    [SerializeField] private float maxPower;
    [SerializeField] private float currentPower;
    [SerializeField] private float fillSpeed;
    public Slider slider;
    private bool isPowerOver = false;

    private bool bIsControllValid = false;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _tank = GetComponent<Tank>();
        _playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        _moveInput = _playerInput.actions["Movement"];
        _fireInput = _playerInput.actions["Fire"];
        _mouseLookInput = _playerInput.actions["MouseLook"];
        _zoomInput = _playerInput.actions["Zoom"];
    }
    
    private void LateUpdate()
    {
        transform.position += moveDirection.normalized * speed * Time.deltaTime;
        transform.Rotate(0, rotationAmount, 0);
        if (_camera.GetComponent<CameraManager>().validCameraID == 1)
        {
            _camera.GetComponent<CameraManager>().MoveCameraWithTank(_tankTower.transform,
                _tankCannon.transform.rotation.eulerAngles.x, 
                _tankTower.transform.rotation.eulerAngles.y);   
        }
        
    }

    private void OnMovement(InputValue value)
    {
        if (!bIsControllValid) return;
        Vector2 moveValue = value.Get<Vector2>();
        rotationAmount = moveValue.x * rotationSpeed * Time.deltaTime;
        moveDirection = transform.forward * moveValue.y *  Time.deltaTime + transform.right * moveValue.x *  Time.deltaTime;
        if (bIsControllValid)
        {
            if (_fireInput.IsPressed())
            {
                _tank.PredictTrajectory(currentPower);
            }   
        }
    }

    private void Update()
    {
        if (bIsControllValid)
        {
            if (_fireInput.IsPressed())
            {
                SetPower();
            }
            _fireInput.canceled += context => Fire();   
        }
    }
    
    
    private void OnMouseLook(InputValue value)
    {
        if (!bIsControllValid) return;
        Vector2 mouseValue = value.Get<Vector2>();
        float mouseX = mouseValue.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseValue.y * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minXLook, maxXLook);
        yRotation += mouseX;
        _tankCannon.transform.localRotation = Quaternion.Euler(xRotation, 0, 0.0f);
        _tankTower.transform.localRotation = Quaternion.Euler(0, yRotation, 0.0f);
    }

    
    private void SetPower()
    {
        if (currentPower >= maxPower)
        {
            isPowerOver = true;
        }
        if (currentPower == 0)
        {
            isPowerOver = false;
        }
        if(!isPowerOver)
        {
            currentPower += fillSpeed * Time.deltaTime;
            currentPower = Mathf.Clamp(currentPower, 0, maxPower);
            slider.value = currentPower / maxPower;   
        }
        if(isPowerOver)
        {
            currentPower -= fillSpeed * Time.deltaTime;
            currentPower = Mathf.Clamp(currentPower, 0, maxPower);
            slider.value = currentPower / maxPower;
        }
        _tank.PredictTrajectory(currentPower);
    }

    private void Fire()
    {
        if (bIsControllValid)
        {
            _tank.Fire(currentPower);
            slider.value = 0.0f;
            currentPower = 0.0f;
        }
    }
    

    private void OnZoom(InputValue value)
    {
        if (!bIsControllValid) return;
        float zoomValue = value.Get<float>();
        if (_camera.fieldOfView - zoomValue * zoomSeneitivity > 30 &&
            _camera.fieldOfView - zoomValue * zoomSeneitivity < 100)
        {
            _camera.fieldOfView -= zoomValue * zoomSeneitivity;   
        }
    }

    public void SetControllable(bool isControllable)
    {
        bIsControllValid = isControllable;
    }
    
}
