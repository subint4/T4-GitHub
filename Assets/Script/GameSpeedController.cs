using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSpeedController : MonoBehaviour
{
    //현재 속도
    private bool nowSpeed = false;
    //UI 버튼
    public Button speedButton;
    //버튼 텍스트
    public Text speedButtonText;
        
    void Start()
    {
        //UI 버튼이 널이 아니거나 버튼 텍스트가 널이 아니면 발생
        if (speedButton != null && speedButtonText != null)
        {
            speedButton.onClick.AddListener(ToggleSpeed);
            UpdateButtonText();
        }
        
    }

    //클릭 입력 받음
    public void ToggleSpeed()
    {
        nowSpeed = !nowSpeed;
        //현재 속도
        if (nowSpeed)
        {
            //기본 속도
            Time.timeScale = 2.0f;
        }
        else
        {
            //2배속
            Time.timeScale = 1.0f;
        }

        //업데이트 버튼 텍스트
        UpdateButtonText();
        Debug.Log("현재 속도: X " + Time.timeScale);
    }
    private void UpdateButtonText()
    {
         
        if (speedButtonText != null)
        {
            speedButtonText.text = nowSpeed ? "X 1 배속" : "X 2 배속"; // 상태에 따라 변경
        }
    }
}
