using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonManager : MonoBehaviour
{
    public void GoToMainMenu()
    {
        Debug.Log("뒤로가기 버튼 클릭! MainMenu 씬으로 이동");
        SceneManager.LoadScene("MainMenu");
    }
}
