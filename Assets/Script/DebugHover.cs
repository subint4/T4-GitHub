using UnityEngine;
using UnityEngine.UI;

public class DebugHover : MonoBehaviour
{
    void Start()
    {
        var image = GetComponent<Image>();
        if (image != null)
        {
            Debug.Log($"Alpha °ª: {image.color.a}");
        }
    }
}
