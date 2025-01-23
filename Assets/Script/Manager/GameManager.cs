using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private bool isPlaying = false;
    public bool IsPlaying { get => isPlaying; }
    
    // Start is called before the first frame update
    void Start()
    {
        isPlaying = true;    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
