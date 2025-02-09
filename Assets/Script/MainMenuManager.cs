using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuManager : MonoBehaviour
{
    public void LoadStageScene()
    {
        SceneManager.LoadScene("StageMenu"); // 스테이지 씬으로 이동
    }

    public void LoadShopScene()
    {
        // 아직 Shop 씬이 없으므로, 추후 생성될 씬을 대비해서 코드만 작성
        SceneManager.LoadScene("Shop");
    }

    public void LoadTutorialScene()
    {
        SceneManager.LoadScene("Tutorial"); // 튜토리얼 씬으로 이동
    }
}