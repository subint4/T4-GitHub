using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    public UpgradeSystem upgradeSystem;
    public GameObject upgradeButtonPrefab;
    private GameObject currentUpgradeButton;

    public void ShowUpgradeButton(Tower tower)
    {
        if(currentUpgradeButton != null)
        {
            Destroy(currentUpgradeButton);
        }
        currentUpgradeButton = Instantiate(upgradeButtonPrefab, tower.transform.position, Quaternion.identity);
        currentUpgradeButton.transform.SetParent(GameObject.Find("Canvas").transform, false);
        currentUpgradeButton.SetActive(true);

        Button button = currentUpgradeButton.GetComponent<Button>();
        if(button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                upgradeSystem.UpgradeSelectedTower();
                HideUpgradeButton();
            });
        }
        RectTransform buttonTransform = currentUpgradeButton.GetComponent<RectTransform>();
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(tower.transform.position);
        buttonTransform.position = screenPosition + new Vector3(50, 50, 0);
    }

    public void HideUpgradeButton()
    {
        if(currentUpgradeButton != null)
        {
            currentUpgradeButton.SetActive(false);
            Destroy(currentUpgradeButton);
        }
    }
}
