using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            audioSource.clip = null;
        }
        else
        {
            // 중복된 객체 제거
            Destroy(gameObject);
        }
    }
    public void SetStageBGM(int stageNum, int subStageNum)
    {
        string bgmPath = $"Sound/Stage{subStageNum}_BGM";
        Debug.Log($"[BackgroundMusicManager] BGM 로드 시도: {bgmPath}");
        AudioClip loadedBGM = Resources.Load<AudioClip>(bgmPath);

        if (loadedBGM != null)
        {
            if (audioSource.clip != loadedBGM)
            {
                if (!audioSource.isPlaying)
                {
                    Debug.Log($"[BackgroundMusicManager] 새로운 BGM으로 변경: {bgmPath}");
                    audioSource.clip = loadedBGM;
                    audioSource.Play(); // 반드시 실행되도록 보장
                    return;
                }
                if (!audioSource.isPlaying)
                {
                    Debug.Log($"[BackgroundMusicManager] BGM이 중지된 상태 -> 다시 재생: {audioSource.clip.name}");
                    audioSource.Play();
                }
                else
                {
                    Debug.Log($"[BackgroundMusicManager] 동일한 BGM이 이미 재생 중입니다: {audioSource.clip.name}");
                }
            }
            else
            {
                Debug.LogError($"[BackgroundMusicManager] BGM 파일을 찾을 수 없습니다! (경로: {bgmPath})");
            }
        }
    }
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) // 앱이 다시 포커스를 얻었을 때
        {
            Debug.Log("[BackgroundMusicManager] 앱 포커스 복귀 - BGM 체크");

            if (audioSource != null)
            {
                audioSource.mute = false; // 음소거 해제
                audioSource.volume = 1.0f; // 볼륨 복구

                if (audioSource.clip != null && !audioSource.isPlaying)
                {
                    Debug.Log("[BackgroundMusicManager] 앱 포커스 복귀 - BGM 다시 재생");
                    audioSource.Play();
                }
            }
        }
    }
}
