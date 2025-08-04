using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    private UserData userData;

    public TextMeshProUGUI diamondText;
    public TextMeshProUGUI heartText;
    public TextMeshProUGUI starText;

    private const int maxHearts = 30;

    void Start()
    {
        // 유저 데이터 로드
        userData = UserDataManager.LoadUserData();
        if (userData == null)
        {
            Debug.LogError("[MainMenuController] 유저 데이터를 불러오는 데 실패했습니다.");
            return;
        }

        // 화면에 데이터 반영
        UpdateUI();

        AudioListener.pause = true; // 전체 소리 정지
        Debug.Log("메인화면 - 소리 자동 음소거");
    }

    private void UpdateUI()
    {
        if (userData == null)
        {
            Debug.LogError("[MainMenuController] 유저 데이터가 없습니다!");
            return;
        }

        // **아이템 데이터에서 다이아몬드, 하트, 스타 개수 찾기**
        int diamonds = GetItemQuantity(1); // 예제: 201번 아이템이 다이아몬드
        int hearts = GetItemQuantity(2); // 예제: 202번 아이템이 하트
        int stars = GetItemQuantity(3); // 예제: 203번 아이템이 스타

        // UI 업데이트
        diamondText.text = diamonds.ToString();
        heartText.text = $"{hearts}/{maxHearts}";
        starText.text = stars.ToString();
    }

    /// <summary>
    /// 아이템 ID를 기반으로 개수를 가져오는 함수
    /// </summary>
    private int GetItemQuantity(int itemID)
    {
        UserItemData item = userData.OwnedItems.Find(x => x.ItemID == itemID);
        return item != null ? item.Quantity : 0;
    }
}
