using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("마우스가 버튼 위에 있습니다.");
        // 버튼 상태를 변경하거나 효과 추가
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("마우스가 버튼에서 벗어났습니다.");
        // 버튼 상태 복구

    }
}
