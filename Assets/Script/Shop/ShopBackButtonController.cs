using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopBackButtonController : MonoBehaviour
{

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");  // "MainMenu"´Â ¾À ÀÌ¸§
    }
}
