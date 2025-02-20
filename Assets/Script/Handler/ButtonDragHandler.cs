using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject towerPrefab;
    public int towerCost = 10;
    public GameObject draggingObject;
    private Vector3 GetInputPosition()
    {

        if (Input.touchCount > 0)
        {
            return Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        }
        else
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    public void OnBeginDrag(PointerEventData eventdata)
    {
        draggingObject = Instantiate(towerPrefab, transform.position, Quaternion.identity);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (draggingObject != null)
        {
            Vector3 inputPosition = GetInputPosition();
            draggingObject.transform.position = new Vector3(inputPosition.x, inputPosition.y, 0);
        }
    }
    public void OnEndDrag(PointerEventData eventdata)
    {
        if (draggingObject != null)
        {
            Vector3 inputPosition = GetInputPosition();
            Collider2D hit = Physics2D.OverlapPoint(inputPosition);

            if (hit != null && hit.CompareTag("Tile"))
            {
                if (GoldManager.Instance.SpendGold(towerCost))
                {
                    draggingObject.transform.position = hit.transform.position;
                    Debug.Log("설치 완료");
                }
                else
                {
                    Debug.Log("설치 실패. 재화 부족");
                    Destroy(draggingObject);
                }
            }
            else
            {
                Debug.Log("타일을 벗어남");
                Destroy(draggingObject);
            }
            }
            draggingObject = null;
        }
    }

