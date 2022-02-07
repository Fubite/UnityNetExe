using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MusicData
{
    public string name;
    public AudioClip clip;
}

///// <summary>
///// オーディオプレイヤーインターフェース
///// </summary>
//interface IAudioPlayer
//{
//    /// <summary>
//    /// Musicを再生する
//    /// </summary>
//    /// <param name="no">no = 登録順</param>
//    public void Play(int no);

//    /// <summary>
//    /// Musicを再生する
//    /// </summary>
//    /// <param name="name">自身で付けた名前</param>
//    public void Play(string name);
//}

public abstract class BasePlayer// : IAudioPlayer
{
    public BasePlayer(AudioSource audioSource, List<MusicData> audioData, float volume)
    {
        this.audioSource = audioSource;
        this.audioData = audioData;
        Volume = volume;
    }
    protected readonly AudioSource audioSource;    //自身の持つオーディオソース
    protected readonly List<MusicData> audioData;　  //自身の持つオーディオデータ
    protected float volume;   //BGMのボリューム
    public bool IsPlay { get => audioSource.isPlaying; }    //プレイ中か否か
    //音量調整用のプロパティ
    public float Volume
    {
        get => volume;
        set
        {
            volume = value;
            audioSource.volume = volume;
        }
    }
    public abstract void Play(int id);
    public abstract void Play(string name);
}

public class BGMPlayer : BasePlayer
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public BGMPlayer(AudioSource audioSource, List<MusicData> audioData, float volume = 1f) : base(audioSource,audioData,volume)
    {}

    public override void Play(int no)
    {
        if(audioData.Count <= no)
        {
            Debug.Log("指定された番号のBGMはありません");
            return;
        }
        audioSource.clip = audioData[no].clip;
        audioSource.Play();
    }
    public override void Play(string name)
    {
        foreach(var music in audioData)
        {
            if(music.name == name)
            {
                audioSource.clip = music.clip;
                audioSource.Play();
                return;
            }
        }
        Debug.Log(name + ":BGMはありません");
    }

    /// <summary>
    /// Musicを一時停止させる
    /// </summary>
    public void Pause()
    {
        if (IsPlay)
            audioSource.Pause();
    }

    /// <summary>
    /// Musicを再開させる
    /// </summary>
    public void Resume()
    {
        if (IsPlay)
            audioSource.Play();
    }

    /// <summary>
    /// Musicを停止させる
    /// </summary>
    public void Stop()
    {
        if (IsPlay)
            audioSource.Stop();
    }
}

public class SEPlayer : BasePlayer
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public SEPlayer(AudioSource audioSource, List<MusicData> audioData,float volume = 1f) : base(audioSource, audioData, volume)
    {}

    public override void Play(int no)
    {
        if (audioData.Count <= no)
        {
            Debug.Log("指定された番号のSEはありません");
            return;
        }
        audioSource.PlayOneShot(audioData[no].clip);
    }
    public override void Play(string name)
    {
        foreach (var music in audioData)
        {
            if (music.name == name)
            {
                audioSource.PlayOneShot(music.clip);
                return;
            }
        }
        Debug.Log(name + ":SEはありません");
    }
}

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    [SerializeField]
    List<MusicData> bgmData = new List<MusicData>();
    [SerializeField]
    List<MusicData> seData = new List<MusicData>();
    [SerializeField, Range(0f, 1f)] float mainVolume = 1f;
    public float MainVolume
    {
        get => mainVolume;
        set 
        {
            bgmPlayer.Volume = mainVolume * bgmVolume;
            sePlayer.Volume = mainVolume * seVolume;
            mainVolume = value; 
        }
    }
    [SerializeField, Range(0f, 1f)] float bgmVolume = 1f;
    [SerializeField, Range(0f, 1f)] float seVolume = 1f;

    BGMPlayer bgmPlayer;        //BGMプレイヤー
    public BGMPlayer BgmPlayer { get { return bgmPlayer; } }
    SEPlayer sePlayer;          //SEプレイヤー
    public SEPlayer SePlayer { get { return sePlayer; } }
    protected override void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }
        AudioSource bgmSource = gameObject.AddComponent<AudioSource>(); //BGM用オーディオソース作成
        AudioSource seSource = gameObject.AddComponent<AudioSource>();  //SE用オーディオソース作成
        bgmPlayer = new BGMPlayer(bgmSource, bgmData, mainVolume * bgmVolume);
        sePlayer = new SEPlayer(seSource, seData, mainVolume * seVolume);
    }
}
