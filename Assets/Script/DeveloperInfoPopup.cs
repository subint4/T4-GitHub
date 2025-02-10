using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class DeveloperInfoPopup : MonoBehaviour
{
    public GameObject infoPopupPanel;  // 팝업창 패널
    public Button closeButton;         // 팝업 닫기 버튼
    public Button instagramButton;     // 인스타그램 링크 버튼
    public List<Button> extraLinks;    // 추가 링크 버튼 리스트

    private void Start()
    {
        // 시작할 때 팝업창을 비활성화
        infoPopupPanel.SetActive(false);

        // 닫기 버튼 클릭 시 팝업 닫기
        closeButton.onClick.AddListener(ClosePopup);

        // 인스타그램 버튼 클릭 시 링크 열기 (매개변수 없는 함수 추가)
        instagramButton.onClick.AddListener(OpenInstagram);

        // 추가된 링크 버튼이 있다면 클릭 이벤트 추가
        foreach (Button linkButton in extraLinks)
        {
            string url = linkButton.GetComponentInChildren<Text>().text; // 버튼에 적힌 URL을 가져옴
            linkButton.onClick.AddListener(() => OpenURL(url));
        }
    }

    // 팝업 열기
    public void OpenPopup()
    {
        infoPopupPanel.SetActive(true);
    }

    // 팝업 닫기
    public void ClosePopup()
    {
        infoPopupPanel.SetActive(false);
    }

    // URL 열기 (웹 브라우저)
    private void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    // 인스타그램 링크 열기 (매개변수 없는 함수)
    public void OpenInstagram()
    {
        Application.OpenURL("https://www.instagram.com/evenigame?igsh=dTdkZDh3Z3B6ajBp");
    }
}