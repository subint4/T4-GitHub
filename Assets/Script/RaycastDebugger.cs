using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("UI가 클릭되었습니다.");
            }
            else
            {
                Debug.Log("UI 외부가 클릭되었습니다.");
            }
        }
    }
}
