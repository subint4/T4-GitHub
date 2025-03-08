using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSpeedController : MonoBehaviour
{
    // 현재 속도 단계 (0: x1, 1: x2, 2: x3)
    private int speedLevel = 0;
    private float[] speedValues = { 1.0f, 1.5f, 2.0f };
    private string[] speedLabels = { "X 1 배속", "X 1.5 배속", "X 2 배속" };

    // UI 버튼
    public Button speedButton;
    // 버튼 텍스트
    public TextMeshProUGUI speedButtonText;

    void Start()
    {
        if (speedButton != null && speedButtonText != null)
        {
            speedButton.onClick.AddListener(ToggleSpeed);
            UpdateButtonText();
        }
    }

    public void ToggleSpeed()
    {
        speedLevel = (speedLevel + 1) % speedValues.Length;
        Time.timeScale = speedValues[speedLevel];

        UpdateButtonText();
        Debug.Log("현재 속도: " + speedLabels[speedLevel]);
    }

    private void UpdateButtonText()
    {
        if (speedButtonText != null)
        {
            speedButtonText.text = speedLabels[speedLevel];
        }
    }
}
