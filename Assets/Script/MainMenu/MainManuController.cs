using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MainManuController : MonoBehaviour
{
    public PlayerDataSO playerDataSO;

    private Dictionary<string, PlayerData> playerDictionary = new Dictionary<string, PlayerData>();

    public TextMeshProUGUI diamondText;
    public TextMeshProUGUI heartText;
    public TextMeshProUGUI starText;

    private const int maxhearts = 30;
    void Start()
    {
        //primary key 로 데이터 저장
        if (playerDataSO != null && playerDataSO.playerList.Count > 0)
        {
            foreach (var player in playerDataSO.playerList)
            {
                if (!playerDictionary.ContainsKey(player.PlayerID))
                {
                    playerDictionary.Add(player.PlayerID, player);
                }
            }
        }
        else
        {
            Debug.LogWarning("PlayerDataSO가 비어 있거나 없습니다!");
            return;
        }

        // 프라이머리 키 사용 해서 데이터 불러오기
        string targetPlayerID = "1";
        if (playerDictionary.ContainsKey(targetPlayerID))
        {
            PlayerData player = playerDictionary[targetPlayerID];
            diamondText.text = player.Diamonds.ToString();
            heartText.text = player.Hearts.ToString();
            starText.text = player.Shars.ToString();
        }
        else
        {
            Debug.LogWarning("해당 PlayerID를 찾을 수 없습니다!");
        }
    }
}
