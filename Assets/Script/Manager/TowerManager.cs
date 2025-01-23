using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerManager : MonoBehaviour
{
    public float detectrange = 100000f;
    public LayerMask Enemy;
    public Transform rayOrigin;

    private void Update()
    {
        DetectTarget();
    }
    void DetectTarget()
    {
        RaycastHit hit;

        if(Physics.Raycast(rayOrigin.position,rayOrigin.forward,out hit, detectrange, Enemy))
        {
            Debug.Log($"Detected: {hit.collider.gameObject.name}");

            HandleDetection(hit.collider.gameObject);
        }
        else
        {
            Debug.Log("No target detected");
        }
    }
    void HandleDetection(GameObject detectedObject)
    {
        Debug.Log($"Chasing: {detectedObject.name}");
    }
    void OnDrawGizmos()
    {
        if(rayOrigin != null) 
        { 
            Gizmos.color = Color.red;
            Gizmos.DrawRay(rayOrigin.position, rayOrigin.forward * detectrange);
        }
    }
}
