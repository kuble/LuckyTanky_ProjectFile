using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, 
    IPointerDownHandler, 
    IPointerUpHandler, 
    IDragHandler
{ 
    private Vector3 startPosition;
    private Vector3 pullPosition;
    
    private Camera MainCamera;
 
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float maxPullDistance;

    private void Awake()
    {
        MainCamera = Camera.main;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        pullPosition = startPosition = MainCamera.ScreenToWorldPoint(
            new Vector3(eventData.position.x, 
                eventData.position.y , 
                MainCamera.WorldToScreenPoint(transform.position).z));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Camera.main != null)
        {
            Vector3 mouseWorldPos = MainCamera.ScreenToWorldPoint(
                new Vector3(eventData.position.x, 
                eventData.position.y , 
                MainCamera.WorldToScreenPoint(transform.position).z));
            
            //mouseWorldPos.z = transform.position.z;
            Debug.Log(mouseWorldPos);
            
            Vector3 pullDirection = startPosition - mouseWorldPos;

            Vector3 LinePosition1 = Vector3.zero;
            
            if (pullDirection.magnitude > maxPullDistance)
            {
                pullDirection = pullDirection.normalized * maxPullDistance;
                LinePosition1 = startPosition - pullDirection;
            }
            else
            {
                LinePosition1 = mouseWorldPos;
            }
            
            transform.position = mouseWorldPos;
            
            lineRenderer.SetPosition(1, LinePosition1);
        }
    }
}
