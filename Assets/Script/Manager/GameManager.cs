using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public WaveManager waveManager;
    public EnemyManager enemyManager;

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // **자동으로 WaveManager와 EnemyManager 찾기**
        if (waveManager == null)
        {
            waveManager = FindObjectOfType<WaveManager>();
        }
        if (enemyManager == null)
        {
            enemyManager = FindObjectOfType<EnemyManager>();
        }
    }

    private void Start()
    {
        if (waveManager == null || enemyManager == null)
        {
            Debug.LogError("GameManager: WaveManager 또는 EnemyManager를 찾을 수 없습니다! 씬에 존재하는지 확인하세요.");
            return;
        }

        Debug.Log("게임 시작! 10초 후 웨이브 시작...");
        StartCoroutine(GameStartCountdown());
    }

    private IEnumerator GameStartCountdown()
    {
        int countdown = 10;

        while (countdown > 0)
        {
            Debug.Log($"웨이브 시작까지 {countdown}초...");
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        Debug.Log("웨이브 시작!");
        waveManager.StartWave(); // 10초 후 웨이브 시작
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("게임 오버!");
    }
}
