using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// *************************************************
/// 制作者 三澤裕樹
/// *************************************************
/// SEやBGMを管理するクラス
/// *************************************************
/// </summary>
public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get { return instance; }
    }

    Dictionary<string, AudioClip> seDict = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> bgmDict = new Dictionary<string, AudioClip>();
    
    [SerializeField]
    AudioSource[] seSource;

    [SerializeField]
    AudioSource[] bgmSource;

    [SerializeField]
    AudioClip[] seClips;

    [SerializeField]
    AudioClip[] bgmClips;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        foreach(AudioClip clip in seClips)
        {
            seDict.Add(clip.name, clip);
        }
        foreach (AudioClip clip in bgmClips)
        {
            bgmDict.Add(clip.name, clip);
        }
    }

    /// <summary>
    /// SEの音量を変更する
    /// </summary>
    /// <param name="volume">音量</param>
    public void ChangeSEVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0f, 1f);

        foreach (AudioSource source in seSource)
        {
            source.volume = volume;
        }
    }

    /// <summary>
    /// SEの音量を取得
    /// </summary>
    /// <returns>SEの音量</returns>
    public float GetSEVolume()
    {
        return seSource[0].volume;
    }

    /// <summary>
    /// SEを再生する
    /// </summary>
    /// <param name="seName">再生したいSEのファイル名</param>
    public void PlaySE(string seName)
    {
        //使用していないseSourceを探す
        for (int i = 0; i < seSource.Length; i++)
        {
            if (seSource[i].isPlaying == false) {
                if(seSource[i].clip == null || seSource[i].clip.name != seName)
                {
                    seSource[i].clip = seDict[seName];
                }
                seSource[i].Play();
                return;
            }
        }
        Debug.LogWarning("同時に再生できる音の数を超えたので鳴らせませんでした。");
    }

    /// <summary>
    /// SEを停止
    /// </summary>
    /// <param name="isPause">一時停止か</param>
    public void StopSE(bool isPause = false)
    {
        if (isPause == false)
        {
            foreach (AudioSource source in seSource)
            {
                source.Stop();
            }
        }
        else
        {
            foreach (AudioSource source in seSource)
            {
                source.Pause();
            }
        }
    }

    /// <summary>
    /// BGMの音量を変更する
    /// </summary>
    /// <param name="volume">音量</param>
    public void ChangeBGMVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0f, 1f);

        foreach (AudioSource source in bgmSource)
        {
            source.volume = volume;
        }
    }

    /// <summary>
    /// BGMのフェードアウト
    /// </summary>
    /// <param name="time">フェード時間</param>
    /// <returns></returns>
    public IEnumerator BGMFadeOut(float time)
    {
        float startTime = Time.timeSinceLevelLoad;
        float diff = Time.timeSinceLevelLoad - startTime;
        float initVolume = 0.4f;
        while (diff < time)
        {
            diff = Time.timeSinceLevelLoad - startTime;
            ChangeBGMVolume(initVolume*(1-diff / time));
            yield return null;
        }
        foreach (AudioSource source in bgmSource)
        {
            source.Stop();
        }
    }

    /// <summary>
    /// BGMのフェードイン
    /// </summary>
    /// <param name="time">フェード時間</param>
    /// <param name="bgmName">再生するBGMの名前</param>
    /// <param name="targetVolume">どこまで音量上げるか</param>
    /// <returns></returns>
    public IEnumerator BGMFadeIn(float time , string bgmName, float targetVolume)
    {
        PlayBGM(bgmName, 0f);
        float startTime = Time.timeSinceLevelLoad;
        float diff = Time.timeSinceLevelLoad - startTime;

        diff = Time.timeSinceLevelLoad - startTime;
        while (targetVolume > diff / time)
        {
            diff = Time.timeSinceLevelLoad - startTime;
            ChangeBGMVolume((diff / time));
            yield return null;
        }
    }

    /// <summary>
    /// BGMの音量を取得
    /// </summary>
    /// <returns>BGMの音量</returns>
    public float GetBGMVolume()
    {
        return bgmSource[0].volume;
    }

    /// <summary>
    /// BGMを再生する
    /// </summary>
    /// <param name="bgmName">再生したいBGM名</param>
    public void PlayBGM(string bgmName,float volime = 1)
    {
        {
            //使用していないseSourceを探す
            for (int i = 0; i < bgmSource.Length; i++)
            {
                if (bgmSource[i].clip != null && bgmSource[i].clip.name == bgmName)
                {
                    if (bgmSource[i].isPlaying == false)
                    {
                        bgmSource[i].volume = volime;
                        bgmSource[i].Play();
                    }
                    return;
                }
                else if (bgmSource[i].isPlaying == false)
                {
                    bgmSource[i].clip = bgmDict[bgmName];

                    bgmSource[i].volume = volime;
                    bgmSource[i].Play();
                    return;
                }
            }
            Debug.LogWarning("同時に再生できる音の数を超えたので鳴らせませんでした。");
        }        
    }

    /// <summary>
    /// 特定のBGMが再生中か？
    /// </summary>
    /// <param name="bgmName">BGM名</param>
    /// <returns>再生中か</returns>
    public bool IsPlayBGM(string bgmName)
    {
        foreach (AudioSource source in bgmSource)
        {
            if (source.clip != null && source.clip.name == bgmName)
            {
                return source.isPlaying;
            }
        }
        return false;
    }
    
    /// <summary>
    /// BGMを停止
    /// </summary>
    /// <param name="isPause">一時停止か</param>
    public void StopBGM(string bgmName, bool isPause = false)
    {
        if (isPause == false)
        {
            foreach (AudioSource source in bgmSource)
            {
                if (source.clip != null && source.clip.name == bgmName)
                {
                    source.Stop();
                }
            }
        }
        else
        {
            foreach (AudioSource source in bgmSource)
            {
                if (source.name == bgmName) source.Pause();
            }
        }
    }

    /// <summary>
    /// 全てのBGMとSEを停止する
    /// </summary>
    public void StopAll()
    {
        foreach (AudioSource source in bgmSource)
        {
            source.Stop();
        }
        foreach (AudioSource source in seSource)
        {
            source.Stop();
        }
    }
}