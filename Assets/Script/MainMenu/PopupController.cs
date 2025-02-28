using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    public GameObject popupPenel;
    public Button closeButton;
    void Start()
    {
        popupPenel.SetActive(false);
        closeButton.onClick.AddListener(ClosePopup);
    }
        
    public void OpenPopup()
    {
        Debug.Log("OpenPopup() ½ÇÇàµÊ!");
        popupPenel.SetActive(true);
    }

    public void ClosePopup()
    {
        Debug.Log("ClosePopup() ½ÇÇàµÊ!");
        popupPenel.SetActive(false);
    }
}
