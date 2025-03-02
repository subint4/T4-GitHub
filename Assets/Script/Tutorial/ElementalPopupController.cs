using UnityEngine;
using UnityEngine.UI;

public class ElementalPopupController : MonoBehaviour
{
    public GameObject tutorialPanel;
    public GameObject[] panels;
    public Button cloaseButton;
    public Button leftButton;
    public Button rightButton;

    private int currentIndex = 0;
    private bool isPopupActive = false;
    private void Start()
    {
        tutorialPanel.SetActive(false);
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }
        if (panels.Length > 0)
        {
            //첫 패널 활성화
            panels[0].SetActive(true);
        }

        //버튼 이벤트들
        cloaseButton.onClick.AddListener(ClosePopup);
        leftButton.onClick.AddListener(ShowPreviousPanel);
        rightButton.onClick.AddListener(ShowNextPanel);
               
    }

    public void OpenPopup()
    {
        tutorialPanel.SetActive(true);
    }

    public void ClosePopup()
    {
        tutorialPanel.SetActive(false);
    }
    //중복방지용
    private bool isTransitioning = false;

    public void ShowPreviousPanel()
    {
        if (isTransitioning || panels.Length == 0) return;
        panels[currentIndex].SetActive(false);
        currentIndex = (currentIndex - 1 + panels.Length) % panels.Length;

        panels[currentIndex].SetActive(true);
    }

    public void ShowNextPanel()
    {
        if (isTransitioning || panels.Length == 0) return;
        panels[currentIndex].SetActive(false);
        currentIndex = (currentIndex + 1) % panels.Length;

        panels[currentIndex].SetActive(true);
        Debug.Log("오른쪽 버튼: 현재 활성화된 패널 = " + panels[currentIndex].name);  // 디버그 로그
    }
}
