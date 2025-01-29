using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    public static PlayerSystem Instance {get; private set;}
    // Start is called before the first frame update
    public int currentMoney = 0;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        Debug.Log($"Current Money : {currentMoney}");
    }
}
