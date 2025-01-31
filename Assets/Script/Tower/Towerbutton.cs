using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Towerbutton : MonoBehaviour,IPointerClickHandler
{
    public TowerSpawner towerSpawner;
    public int towerIndex;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(towerSpawner != null)
        {
        towerSpawner.SelectedTower(towerIndex);
            Debug.Log($"버튼 클릭됨 : 타워 {towerIndex}선택됨");
        }
        else
        {
            Debug.LogError("스포너가 연결되지 않음");
        }
    }
    
}
