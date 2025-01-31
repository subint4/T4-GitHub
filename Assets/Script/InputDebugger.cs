using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputDebugger : MonoBehaviour
{
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("UIÅ¬¸¯µÊ");
            }
            else
            {
                Debug.Log("UI¿ÜºÎ Å¬¸¯µÊ");
            }
        }
    }
}
