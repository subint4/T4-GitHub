using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private bool isGameOver = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 유지

        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if (isGameOver && Input.GetMouseButtonDown(0))
        {
            RestartGame();
        }
    }
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("게임 오버. 적이 목표지점에 도달했습니다.");

        Time.timeScale = 0;
    }
    public void RestartGame()
    {
        Time.timeScale = 1;
        isGameOver=false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
