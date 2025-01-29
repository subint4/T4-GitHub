using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedController : MonoBehaviour
{
    public float normalSpeed = 1f;
    public float fastSpeed = 2f;
    public float fastestSpeed = 3f;
    public float slowSpeed = 0.5f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Time.timeScale = normalSpeed;
            Debug.Log("기본 속도");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Time.timeScale = fastSpeed;
            Debug.Log("2배속");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Time.timeScale = fastestSpeed;
            Debug.Log("3배속");
        }
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            Time.timeScale = slowSpeed;
            Debug.Log("0.5배속");
        }
        if (Time.timeScale != 1f)
        {
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
    }
}
