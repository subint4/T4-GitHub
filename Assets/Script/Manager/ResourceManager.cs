using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;
    public int currentGold = 25;
    public TMP_Text goldText;
    public int goldIncrement = 10;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        UpdateGoldUI();
        StartCoroutine(AutoAddGold());
    }
    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateGoldUI();
    }
    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            UpdateGoldUI();
            Debug.Log($"°ñµå ¼Ò¸ð : {amount}, ÀÜ·® : {currentGold}");
            return true;
        }
        else
        {
            Debug.Log("°ñµåºÎÁ·");
            return false;
        }
    }
    public void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = $"{currentGold}";
        }
    }
    private IEnumerator AutoAddGold()
    {
        while (true)
        {
            //5ÃÊ ÈÄ
            yield return new WaitForSeconds(5f);
            AddGold(goldIncrement);
            Debug.Log($"5ÃÊ¸¶´Ù {goldIncrement} °ñµå Ãß°¡µÊ. ÇöÀç °ñµå: {currentGold}");
        }
    }
}