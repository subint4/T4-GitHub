using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GameObject packagePanel;
    public GameObject diamondPanel;
    public GameObject heartPanel;
    public GameObject itemPanel;

    public Button btn_pack;
    public Button btn_dia;
    public Button btn_heart;
    public Button btn_item;

    public Sprite btn_pack_on;
    public Sprite btn_pack_off;
    public Sprite btn_dia_on;
    public Sprite btn_dia_off;
    public Sprite btn_heart_on;
    public Sprite btn_heart_off;
    public Sprite btn_item_on;
    public Sprite btn_item_off;

    private GameObject currentPanel;
    private Button currentButton;

    void Start()
    {
        // 시작 시 기본 패널 설정 및 버튼 상태
        ShowPanel(packagePanel);
        SetButtonState(btn_pack, btn_pack_on);  // Package는 초록색으로
        currentButton = btn_pack;               // 현재 선택된 버튼 저장

        btn_pack.onClick.AddListener(() => {
            ShowPanel(packagePanel);
            UpdateButtonImages(btn_pack);
        });

        btn_dia.onClick.AddListener(() => {
            ShowPanel(diamondPanel);
            UpdateButtonImages(btn_dia);
        });

        btn_heart.onClick.AddListener(() => {
            ShowPanel(heartPanel);
            UpdateButtonImages(btn_heart);
        });

        btn_item.onClick.AddListener(() => {
            ShowPanel(itemPanel);
            UpdateButtonImages(btn_item);
        });
    }

    // 패널 전환 메서드
    void ShowPanel(GameObject panel)
    {
        if (currentPanel != null)
            currentPanel.SetActive(false);

        panel.SetActive(true);
        currentPanel = panel;
    }

    // 버튼 상태 업데이트 메서드
    void UpdateButtonImages(Button selectedButton)
    {
        // 현재 선택된 버튼이 Package가 아니면 Package를 OFF로 변경
        if (selectedButton != btn_pack && currentButton == btn_pack)
            SetButtonState(btn_pack, btn_pack_off);
        else if (selectedButton == btn_pack)
            SetButtonState(btn_pack, btn_pack_on);  // Package가 선택되면 ON

        // 다른 버튼들의 상태 설정
        SetButtonState(btn_dia, selectedButton == btn_dia ? btn_dia_on : btn_dia_off);
        SetButtonState(btn_heart, selectedButton == btn_heart ? btn_heart_on : btn_heart_off);
        SetButtonState(btn_item, selectedButton == btn_item ? btn_item_on : btn_item_off);

        // 현재 선택된 버튼 저장
        currentButton = selectedButton;
    }

    // 버튼 스프라이트 변경 메서드
    void SetButtonState(Button button, Sprite sprite)
    {
        button.GetComponent<Image>().sprite = sprite;
    }
}
