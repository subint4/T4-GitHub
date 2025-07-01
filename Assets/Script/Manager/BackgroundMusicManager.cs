using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundMusicManager : MonoBehaviour
{
    private static BackgroundMusicManager instance;

    public AudioSource audioSource;

    [Header("스테이지별 BGM (순서대로 등록하세요)")]
    public AudioClip[] bgmClips; // 인스펙터에서 미리 지정

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource.clip = null;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetStageBGM(int stageNum, int subStageNum)
    {
        int index = subStageNum - 1;

        if (index < 0 || index >= bgmClips.Length)
        {
            Debug.LogError($"[BackgroundMusicManager] 잘못된 subStageNum: {subStageNum} → 인덱스 범위 초과");
            return;
        }

        AudioClip selectedBGM = bgmClips[index];

        if (selectedBGM == null)
        {
            Debug.LogError($"[BackgroundMusicManager] 인스펙터에서 BGM이 설정되지 않았습니다. (index: {index})");
            return;
        }

        if (audioSource.clip != selectedBGM)
        {
            Debug.Log($"[BackgroundMusicManager] 새로운 BGM으로 변경: {selectedBGM.name}");
            audioSource.clip = selectedBGM;
            audioSource.Play();
        }
        else if (!audioSource.isPlaying)
        {
            Debug.Log($"[BackgroundMusicManager] BGM이 중지된 상태 → 다시 재생: {selectedBGM.name}");
            audioSource.Play();
        }
        else
        {
            Debug.Log($"[BackgroundMusicManager] 동일한 BGM이 이미 재생 중입니다: {selectedBGM.name}");
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Debug.Log("[BackgroundMusicManager] 앱 포커스 복귀 - BGM 체크");

            if (audioSource != null)
            {
                audioSource.mute = false;
                audioSource.volume = 1.0f;

                if (audioSource.clip != null && !audioSource.isPlaying)
                {
                    Debug.Log("[BackgroundMusicManager] 앱 포커스 복귀 - BGM 다시 재생");
                    audioSource.Play();
                }
            }
        }
    }
}
