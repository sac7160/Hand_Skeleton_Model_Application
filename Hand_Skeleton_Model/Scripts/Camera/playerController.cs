using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class PlayerController : MonoBehaviour
{
    private RotateToMouse rotateToMouse; // 마우스 이동으로 카메라 회전
    private Zoom zoom;
    
    void Awake()
    {
        rotateToMouse = GetComponent<RotateToMouse>();
        zoom = GetComponentInChildren<Zoom>();
    }
 
    void Update()
    {
        if(Input.GetMouseButton(0)) UpdateRotate();
        UpdateZoom();
        UpdatePosition();
    }
 
    void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        rotateToMouse.CalculateRotation(mouseX, mouseY);
    }

    void UpdatePosition()
    {
        float keyH = Input.GetAxis("Horizontal");
        float keyV = Input.GetAxis("Vertical");
        rotateToMouse.moveObject(keyH, keyV);
    }
 
    void UpdateZoom()
    {
        float t_zoomDirection = Input.GetAxis("Mouse ScrollWheel");
        zoom.ZoomInOut(t_zoomDirection);
    }
}