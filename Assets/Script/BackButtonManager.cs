using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonManager : MonoBehaviour
{
    public GameObject popupPanel; // 팝업 패널이 있는 경우 참조

    public void GoToMainMenu()
    {
        Debug.Log("뒤로가기 버튼 클릭! MainMenu 씬으로 이동");
        SceneManager.LoadScene("MainMenu");
    }

    public void ClosePopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
            Debug.Log("팝업 닫기 버튼 클릭! 팝업 비활성화");
        }
    }
}
