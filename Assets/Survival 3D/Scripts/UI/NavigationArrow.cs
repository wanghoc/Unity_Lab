using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationArrow : MonoBehaviour
{
    [Header("Settings")]
    public Transform player; // Player transform
    public float distanceFromPlayer = 2f; // Khoảng cách từ player
    public float heightOffset = -0.5f; // Độ cao (dưới chân player)
    public float rotationSpeed = 5f; // Tốc độ xoay mũi tên
    
    [Header("Arrow Visual")]
    public GameObject arrowObject; // 3D model mũi tên hoặc sprite
    public bool rotateToTarget = true; // Xoay mũi tên về phía target
    
    [Header("Animation")]
    public bool enablePulse = true; // Hiệu ứng nhấp nháy
    public float pulseSpeed = 2f;
    public float pulseScale = 0.2f;
    
    [Header("Distance Display")]
    public TMPro.TextMeshProUGUI distanceText; // Hiển thị khoảng cách đến zombie
    public bool showDistance = true;
    public string distanceFormat = "{0:F1}m"; // Format: "15.5m"
    
    private Transform currentTarget;
    private Vector3 originalScale;

    private void Start()
    {
        if (player == null)
        {
            player = PlayerController.instance.transform;
        }

        if (arrowObject != null)
        {
            originalScale = arrowObject.transform.localScale;
        }
    }

    private void Update()
    {
        // Get current zombie position from WaveManager
        if (WaveManager.instance != null)
        {
            currentTarget = WaveManager.instance.GetCurrentZombiePosition();
        }

        if (currentTarget == null || player == null)
        {
            // Hide arrow if no target
            if (arrowObject != null)
            {
                arrowObject.SetActive(false);
            }
            return;
        }

        // Show arrow
        if (arrowObject != null && !arrowObject.activeSelf)
        {
            arrowObject.SetActive(true);
        }

        // Position arrow at player's feet
        Vector3 targetPosition = player.position + Vector3.up * heightOffset;
        transform.position = targetPosition;

        // Rotate arrow to point at zombie
        if (rotateToTarget)
        {
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            direction.y = 0; // Keep arrow horizontal
            
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, 
                    targetRotation, 
                    rotationSpeed * Time.deltaTime
                );
            }
        }

        // Pulse animation
        if (enablePulse && arrowObject != null)
        {
            float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseScale;
            arrowObject.transform.localScale = originalScale * scale;
        }
        
        // Update distance text
        if (showDistance && distanceText != null)
        {
            float distance = Vector3.Distance(player.position, currentTarget.position);
            distanceText.text = string.Format(distanceFormat, distance);
        }
    }

    private void OnDrawGizmos()
    {
        // Debug visualization
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 arrowPos = player.position + Vector3.up * heightOffset;
            Gizmos.DrawWireSphere(arrowPos, 0.5f);
            
            if (currentTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(arrowPos, currentTarget.position);
            }
        }
    }
}
