using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class UIManager : MonoBehaviour
{
    TankController _tankController;
    private Tank _tank;
    [SerializeField] private GameObject ui_HUD;
    [SerializeField] private GameObject ui_Guide;
    [SerializeField] private GameObject ui_Pause;
    [SerializeField] private GameObject ui_Finish;
    [SerializeField] private GameObject _missileImage01;
    [SerializeField] private GameObject _missileImage02;
    private UnityEngine.InputSystem.PlayerInput _playerInput;
    private InputAction _escapeInput;
    
    
    private void Start()
    {
        _tank = GameObject.Find("Tank").GetComponent<Tank>();
        _tankController = GameObject.Find("Tank").GetComponent<TankController>();
        UnityEngine.Cursor.visible = true;
        _playerInput = GameObject.Find("Tank").GetComponent<UnityEngine.InputSystem.PlayerInput>();
        _escapeInput =_playerInput.actions["Escape"];
        
        ui_HUD = this.transform.GetChild(0).gameObject;
        ui_Guide = this.transform.GetChild(1).gameObject;
        ui_Pause = this.transform.GetChild(2).gameObject;
        ui_Finish = this.transform.GetChild(3).gameObject;
        
        ui_HUD.SetActive(true);
        ui_Guide.SetActive(true);
        ui_Pause.SetActive(false);
        ui_Finish.SetActive(false);
        ui_Guide.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        ui_Guide.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        
        _tankController.SetControllable(false);
        UnityEngine.Cursor.visible = true;
        
        _escapeInput.started += OnEscapeButtonClicked;
        SetMissileImage();
    }
    public void OnExitGuideButtonClicked()
    {
        if (ui_Guide.gameObject.transform.GetChild(0).gameObject.activeSelf)
        {
            ui_Guide.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            ui_Guide.gameObject.transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (ui_Guide.gameObject.transform.GetChild(1).gameObject.activeSelf)
        {
            ui_Guide.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            ui_Guide.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            ui_Guide.gameObject.SetActive(false);
            UnityEngine.Cursor.visible = false;
            _tankController.SetControllable(true);
        }
        
    }

    public void OnEscapeButtonClicked(InputAction.CallbackContext context)
    {
        if (!ui_Guide.gameObject.activeSelf && !ui_Pause.gameObject.activeSelf)
        {
            ui_HUD.SetActive(false);
            ui_Pause.SetActive(true);
            UnityEngine.Cursor.visible = true;
            _tankController.SetControllable(false);
        }
        else
        {
            if (ui_Guide.gameObject.activeSelf)
            {
                OnExitGuideButtonClicked();
            }
            if (ui_Pause.gameObject.activeSelf)
            {
                ui_HUD.SetActive(true);
                ui_Pause.gameObject.SetActive(false);
                UnityEngine.Cursor.visible = false;
                _tankController.SetControllable(true);
            }   
        }
    }

    public void OnClickContinueButton()
    {
        ui_HUD.SetActive(true);
        ui_Pause.SetActive(false);
        UnityEngine.Cursor.visible = false;
        _tankController.SetControllable(true);
    }

    public void OnClickGuideButton()
    {
        ui_Pause.SetActive(false);
        ui_HUD.SetActive(true);
        ui_Guide.gameObject.SetActive(true);
        UnityEngine.Cursor.visible = true;
        _tankController.SetControllable(false);
    }

    public void OnClickQuitButton()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void SetMissileImage()
    {
        if (_tank.missiles.Count <= 0)
        {
            _missileImage01.GetComponent<UnityEngine.UI.Image>().enabled = false;
        }
        else
        {
            _missileImage01.GetComponent<UnityEngine.UI.Image>().sprite = _tank.missiles[0].missileIcon;   
        }
        if (_tank.missiles.Count <= 1)
        {
            _missileImage02.GetComponent<UnityEngine.UI.Image>().enabled = false;
        }
        else
        {
            _missileImage02.GetComponent<UnityEngine.UI.Image>().sprite = _tank.missiles[1].missileIcon;   
        }
    }

    public void OnGameFinished(int Score, bool bIsAllClear)
    {
        //활성 세팅
        ui_HUD.SetActive(false);
        ui_Guide.SetActive(false);
        ui_Pause.SetActive(false);
        ui_Finish.SetActive(true);
        UnityEngine.Cursor.visible = true;
        _tankController.SetControllable(false);
        
        ui_Finish.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "당신의 점수는! : " + Score.ToString() + "점";
        if (bIsAllClear)
        {
            ui_Finish.gameObject.transform.GetChild(3).gameObject.SetActive(true);
        }
        else
        {
            ui_Finish.gameObject.transform.GetChild(3).gameObject.SetActive(false);
        }
        StartCoroutine(WaitForFinish(4));
    }
    
    private IEnumerator WaitForFinish(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Application.Quit();
    }
}
