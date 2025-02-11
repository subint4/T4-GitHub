using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "ScriptableObjects/WaveSO", order = 2)]
public class WaveSO : ScriptableObject
{
    public int waveCount;
    public float spawnRate;
    public float timeBetweenWaves = 5f;

    // 이제 Unity에서 직렬화 가능
    public SerializableDictionary<int, int> enemyCounts = new SerializableDictionary<int, int>();

    public int GetTotalEnemies()
    {
        int totalEnemies = 0;
        foreach (var enemy in enemyCounts)
        {
            totalEnemies += enemy.Value;
        }
        return totalEnemies;
    }
}
