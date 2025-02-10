using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuUI : MonoBehaviour
{
    public void OnPlayButtonClicked()
    {
        Debug.Log("Play 버튼 클릭됨!");
        // 스테이지 선택 씬으로 이동
    }

    public void OnShopButtonClicked()
    {
        Debug.Log("Shop 버튼 클릭됨!");
        // 상점 씬으로 이동 (추후 추가)
    }

    public void OnTutorialButtonClicked()
    {
        Debug.Log("Tutorial 버튼 클릭됨!");
        // 튜토리얼 씬으로 이동 (추후 추가)
    }
}   
