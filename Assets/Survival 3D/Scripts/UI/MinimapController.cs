using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    [Header("Minimap Camera")]
    public Camera minimapCamera; // Camera nhìn từ trên xuống
    public float cameraHeight = 50f; // Độ cao của camera
    public float zoomLevel = 20f; // Zoom level (orthographicSize)
    
    [Header("Player")]
    public Transform player; // Player transform
    public bool rotateWithPlayer = true; // Xoay minimap theo hướng player
    
    [Header("Settings")]
    public float smoothSpeed = 5f; // Tốc độ smooth camera movement
    public LayerMask minimapLayer; // Layer cho minimap objects
    
    [Header("Zoom Controls")]
    public bool enableZoomControl = true;
    public float minZoom = 10f;
    public float maxZoom = 50f;
    public float zoomSpeed = 5f;
    public KeyCode zoomInKey = KeyCode.KeypadPlus;
    public KeyCode zoomOutKey = KeyCode.KeypadMinus;
    
    [Header("Toggle")]
    public KeyCode toggleMinimapKey = KeyCode.M;
    public GameObject minimapUI; // UI Panel chứa minimap
    
    private void Start()
    {
        if (minimapCamera == null)
        {
            Debug.LogError("⚠️ MinimapController: Minimap camera not assigned!");
            enabled = false;
            return;
        }
        
        if (player == null)
        {
            player = PlayerController.instance.transform;
        }
        
        // Setup camera
        minimapCamera.orthographic = true;
        minimapCamera.orthographicSize = zoomLevel;
        minimapCamera.cullingMask = minimapLayer;
        
        if (minimapUI != null)
        {
            minimapUI.SetActive(true);
        }
    }
    
    private void LateUpdate()
    {
        if (player == null || minimapCamera == null)
            return;
        
        // Toggle minimap
        if (Input.GetKeyDown(toggleMinimapKey))
        {
            ToggleMinimap();
        }
        
        // Update camera position
        Vector3 targetPosition = player.position + Vector3.up * cameraHeight;
        minimapCamera.transform.position = Vector3.Lerp(
            minimapCamera.transform.position, 
            targetPosition, 
            smoothSpeed * Time.deltaTime
        );
        
        // Rotate with player
        if (rotateWithPlayer)
        {
            Quaternion targetRotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
            minimapCamera.transform.rotation = Quaternion.Lerp(
                minimapCamera.transform.rotation,
                targetRotation,
                smoothSpeed * Time.deltaTime
            );
        }
        
        // Zoom controls
        if (enableZoomControl)
        {
            if (Input.GetKey(zoomInKey))
            {
                zoomLevel -= zoomSpeed * Time.deltaTime;
            }
            if (Input.GetKey(zoomOutKey))
            {
                zoomLevel += zoomSpeed * Time.deltaTime;
            }
            
            // Clamp zoom
            zoomLevel = Mathf.Clamp(zoomLevel, minZoom, maxZoom);
            minimapCamera.orthographicSize = Mathf.Lerp(
                minimapCamera.orthographicSize,
                zoomLevel,
                smoothSpeed * Time.deltaTime
            );
        }
    }
    
    public void ToggleMinimap()
    {
        if (minimapUI != null)
        {
            minimapUI.SetActive(!minimapUI.activeSelf);
            Debug.Log($"🗺️ Minimap: {(minimapUI.activeSelf ? "ON" : "OFF")}");
        }
    }
    
    public void SetZoom(float zoom)
    {
        zoomLevel = Mathf.Clamp(zoom, minZoom, maxZoom);
    }
}
