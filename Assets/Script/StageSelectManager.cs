using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    public Button[] StageButtons;  // 스테이지 버튼 (9개만 사용하여 페이지별로 업데이트)
    public Image[] StagePlayImages; // 플레이 버튼 아이콘 (9개만 사용)

    public Button FowardArrow;  // 다음 페이지 버튼
    public Button BackArrow;  // 이전 페이지 버튼

    private int currentPage = 0;
    private int stagesPerPage = 9;
    private int maxStage = 18;  // 전체 스테이지 개수
    private int unlockedStage = 1; // 플레이어가 해금한 최대 스테이지 (기본값 1)
    private int maxUnlockedStage = 6; // 최대 해금 가능 스테이지 (6으로 제한)

    void Start()
    {
        
        LoadPlayerProgress(); // 저장된 플레이어 진행도 불러오기
        UpdateStageButtons();
        FowardArrow.onClick.AddListener(NextPage);
        BackArrow.onClick.AddListener(PreviousPage);
    }

    private void LoadPlayerProgress()
    {
        unlockedStage = PlayerPrefs.GetInt("UnlockedStage"); 

        Debug.Log($" 현재 저장된 해금된 스테이지: {unlockedStage}");

        if (unlockedStage < 1)
        {
            unlockedStage = 1;
            PlayerPrefs.SetInt("UnlockedStage", 1);
            PlayerPrefs.Save();
        }

        if (unlockedStage > maxUnlockedStage)
        {
            unlockedStage = maxUnlockedStage;
        }

        Debug.Log($" 최종 적용된 해금된 스테이지: {unlockedStage}");
    }

    public void NextPage()
    {
        if ((currentPage + 1) * stagesPerPage < maxStage)
        {
            currentPage++;
            UpdateStageButtons();
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateStageButtons();
        }
    }

    public void UpdateStageButtons()
    {
        int buttonCount = Mathf.Min(StageButtons.Length, StagePlayImages.Length, stagesPerPage); // 배열 크기 초과 방지

        // 🔥 추가한 디버그 로그
        Debug.Log($" buttonCount : {StageButtons.Length} / {StagePlayImages.Length} / {stagesPerPage} / {buttonCount}");

        for (int i = 0; i < buttonCount; i++)
        {
            int stageNumber = currentPage * stagesPerPage + i + 1;

            // 🔥 스테이지 번호 예외처리 추가
            if (stageNumber <= 0)
            {
                Debug.LogError($" 잘못된 스테이지 번호 계산됨: {stageNumber}");
                continue;
            }

            if (StageButtons[i] == null || StagePlayImages[i] == null)
            {
                Debug.LogError($" StageButtons[{i}] 또는 StagePlayImages[{i}]이(가) null입니다! Inspector에서 연결 확인 필요.");
                continue;
            }

            if (stageNumber <= maxStage)
            {
                StageButtons[i].gameObject.SetActive(true);

                bool isUnlocked = (stageNumber <= unlockedStage);
                StageButtons[i].interactable = isUnlocked;
                StagePlayImages[i].gameObject.SetActive(stageNumber == unlockedStage);

                Debug.Log($" Stage {stageNumber}: isUnlocked = {isUnlocked}, PlayImage 활성화 여부 - {stageNumber == unlockedStage}");

                Text buttonText = StageButtons[i].GetComponentInChildren<Text>();
                TMP_Text tmpText = StageButtons[i].GetComponentInChildren<TMP_Text>();

                if (buttonText != null)
                    buttonText.text = stageNumber.ToString();
                else if (tmpText != null)
                    tmpText.text = stageNumber.ToString();

                int capturedStageNumber = stageNumber;
                StageButtons[i].onClick.RemoveAllListeners();
                if (isUnlocked)
                {
                    StageButtons[i].onClick.AddListener(() => LoadStage(capturedStageNumber));
                }
            }
            else
            {
                StageButtons[i].gameObject.SetActive(false);
                StagePlayImages[i].gameObject.SetActive(false);
            }
        }
    }

    public void LoadStage(int stageNumber)
    {
        if (stageNumber <= 0)
        {
            Debug.LogError($" 잘못된 스테이지 번호: {stageNumber} - 1 이상이어야 합니다!");
            return;
        }

        string sceneName = "Stage" + stageNumber;
        Debug.Log($" 스테이지 {sceneName} 이동 시도");
        SceneManager.LoadScene(sceneName);
    }

    public void UnlockNextStage(int stageNumber)
    {
        if (stageNumber >= unlockedStage && stageNumber < maxUnlockedStage)
        {
            unlockedStage = stageNumber + 1;
            PlayerPrefs.SetInt("UnlockedStage", unlockedStage);
            PlayerPrefs.Save();
        }
    }
}
