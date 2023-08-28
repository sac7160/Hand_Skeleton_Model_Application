using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Zoom : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 0f;
    [SerializeField] private float zoomMax = 5f;
    [SerializeField] private float zoomMin = 0.8f;
 
    PlayerController player;
 
    private void Awake()
    {
        player = GetComponentInParent<PlayerController>();
    }
 
    public void ZoomInOut(float zoomDirection)
    {
        float zoom = Vector3.Distance(transform.position, player.transform.position);
        zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
 
        if (zoom >= zoomMax && zoomDirection > 0) return;
        if (zoom <= zoomMin && zoomDirection < 0) return;
        
        transform.position += transform.forward * zoomDirection * zoomSpeed;
    }
}