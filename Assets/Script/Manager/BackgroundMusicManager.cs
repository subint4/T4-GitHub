using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    private static BackgroundMusicManager instance;

    public AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // 씬 변경 시에도 삭제되지 않음
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 중복된 객체 제거
            Destroy(gameObject); 
        }
    }
    public void PlayMusic()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
            Debug.Log("배경 음악 재생 시작");
        }
    }

    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("배경 음악 멈춤");
        }
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume); 
    }
}
