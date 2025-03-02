using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesShopChange : MonoBehaviour
{
    public void LoadSceneShop(string Shop)
    {
        SceneManager.LoadScene(Shop);
    }

    public void LoadSceneShopdia(string dia)
    {
        SceneManager.LoadScene(dia);
    }

    public void LoadSceneShopheart(string heart)
    {
        SceneManager.LoadScene(heart);
    }

    public void LoadSceneShopitem(string item)
    {
        SceneManager.LoadScene(item);
    }
}
