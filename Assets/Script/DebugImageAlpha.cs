using UnityEngine;
using UnityEngine.UI;

public class DebugImageAlpha : MonoBehaviour
{
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        Debug.Log($"Image Alpha °ª: {image.color.a}");
    }
}