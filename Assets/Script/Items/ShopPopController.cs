using UnityEngine;
using UnityEngine.UI;

public class ShopPopupController : MonoBehaviour
{
    public GameObject popupPanel;
    public Button closeButton;
    void Start()
    {
        popupPanel.SetActive(false);
        closeButton.onClick.AddListener(ClosePopup);
    }

    public void OpenPopup()
    {
        Debug.Log("OpenPopup() ½ÇÇàµÊ!");
        popupPanel.SetActive(true);
    }

    public void ClosePopup()
    {
        Debug.Log("ClosePopup() ½ÇÇàµÊ!");
        popupPanel.SetActive(false);
    }
}
