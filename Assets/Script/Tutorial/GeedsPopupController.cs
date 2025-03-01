using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GeedsPopupController : MonoBehaviour
{
    public GameObject tutorialPanel;
    public Button cloaseButton;

    private void Start()
    {
        tutorialPanel.SetActive(false);
        cloaseButton.onClick.AddListener(ClosePopup);
    }

    public void OpenPopup()
    {
        tutorialPanel.SetActive(true);
    }

    public void ClosePopup()
    {
        tutorialPanel.SetActive(false);
    }
}
