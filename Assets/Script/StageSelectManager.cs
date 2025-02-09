using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour
{
    public Button[] StageButtons;  // 스테이지 버튼 (9개)
    public Button FowardArrow;  // 다음 페이지 버튼
    public Button BackArrow;  // 이전 페이지 버튼

    private int currentPage = 0;
    private int stagesPerPage = 9;
    private int maxStage = 18;  // 전체 스테이지 개수
    private int unlockedStage = 1; // 플레이어가 해금한 최대 스테이지 (기본값 1)
    private int maxUnlockedStage = 6; // **최대 해금 가능 스테이지 (6으로 제한)**

    void Start()
    {
        LoadPlayerProgress(); // 저장된 플레이어 진행도 불러오기
        UpdateStageButtons();
        FowardArrow.onClick.AddListener(NextPage);
        BackArrow.onClick.AddListener(PreviousPage);
    }

    private void LoadPlayerProgress()
    {
        // 플레이어 진행도 로드, 단 최대 해금 가능 스테이지(6) 이상으로는 못 올라감
        unlockedStage = PlayerPrefs.GetInt("UnlockedStage", 1);
        if (unlockedStage > maxUnlockedStage)
        {
            unlockedStage = maxUnlockedStage; // 최대 6까지만 해금 가능
        }
    }

    public void NextPage()
    {
        if ((currentPage + 1) * stagesPerPage < maxStage)
        {
            currentPage++; // 다음 페이지로 이동
            UpdateStageButtons(); // 버튼 내용 업데이트
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--; // 이전 페이지로 이동
            UpdateStageButtons(); // 버튼 내용 업데이트
        }
    }

    public void UpdateStageButtons()
    {
        for (int i = 0; i < StageButtons.Length; i++)
        {
            if (StageButtons[i] == null)
            {
                Debug.LogError($"StageButtons[{i}]이(가) null입니다! Inspector에서 연결 확인 필요.");
                continue;
            }

            int stageNumber = currentPage * stagesPerPage + i + 1;

            if (stageNumber <= maxStage)
            {
                StageButtons[i].gameObject.SetActive(true);
                Text buttonText = StageButtons[i].GetComponentInChildren<Text>();
                TMP_Text tmpText = StageButtons[i].GetComponentInChildren<TMP_Text>(); // TMP_Text 확인

                if (buttonText == null && tmpText == null)
                {
                    Debug.LogError($"StageButtons[{i}]에 Text 또는 TMP_Text 컴포넌트가 없습니다! 버튼 안에 UI Text 추가 필요.");
                    continue;
                }

                if (buttonText != null)
                    buttonText.text = stageNumber.ToString();
                else if (tmpText != null)
                    tmpText.text = stageNumber.ToString();

                StageButtons[i].interactable = (stageNumber <= unlockedStage);
            }
            else
            {
                StageButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // 플레이어가 새로운 스테이지를 클리어하면 진행도 저장 (단, 최대 6까지만 해금 가능)
    public void UnlockNextStage(int stageNumber)
    {
        if (stageNumber >= unlockedStage && stageNumber < maxUnlockedStage) // 6 이상은 해금 불가능
        {
            unlockedStage = stageNumber + 1;
            PlayerPrefs.SetInt("UnlockedStage", unlockedStage);
            PlayerPrefs.Save(); // 데이터 저장
        }
    }
}