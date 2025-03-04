using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerDataSO", menuName = "SO/MainMenu/PlayerData")]
public class PlayerDataSO : ScriptableObject
{
    public List<PlayerData> playerList;
}
