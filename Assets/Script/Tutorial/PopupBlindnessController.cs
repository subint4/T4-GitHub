using UnityEngine;

public class PopupBlindnessController : MonoBehaviour
{
    public GameObject blindBackground;  // 공용 블라인드 배경
    private int openPopupCount = 0;     // 현재 열려있는 팝업 개수

    // 팝업 열기
    public void OpenPopup(GameObject popup)
    {
        popup.SetActive(true);
        openPopupCount++;

        if (openPopupCount > 0)
            blindBackground.SetActive(true);  // 팝업이 하나라도 있으면 블라인드 활성화
    }

    // 팝업 닫기
    public void ClosePopup(GameObject popup)
    {
        popup.SetActive(false);
        openPopupCount--;

        if (openPopupCount <= 0)
        {
            openPopupCount = 0;  // 안전장치
            blindBackground.SetActive(false);  // 모든 팝업이 닫히면 블라인드 비활성화
        }
    }
}
