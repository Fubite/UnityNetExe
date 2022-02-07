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
///// �I�[�f�B�I�v���C���[�C���^�[�t�F�[�X
///// </summary>
//interface IAudioPlayer
//{
//    /// <summary>
//    /// Music���Đ�����
//    /// </summary>
//    /// <param name="no">no = �o�^��</param>
//    public void Play(int no);

//    /// <summary>
//    /// Music���Đ�����
//    /// </summary>
//    /// <param name="name">���g�ŕt�������O</param>
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
    protected readonly AudioSource audioSource;    //���g�̎��I�[�f�B�I�\�[�X
    protected readonly List<MusicData> audioData;�@  //���g�̎��I�[�f�B�I�f�[�^
    protected float volume;   //BGM�̃{�����[��
    public bool IsPlay { get => audioSource.isPlaying; }    //�v���C�����ۂ�
    //���ʒ����p�̃v���p�e�B
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
    /// �R���X�g���N�^
    /// </summary>
    public BGMPlayer(AudioSource audioSource, List<MusicData> audioData, float volume = 1f) : base(audioSource,audioData,volume)
    {}

    public override void Play(int no)
    {
        if(audioData.Count <= no)
        {
            Debug.Log("�w�肳�ꂽ�ԍ���BGM�͂���܂���");
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
        Debug.Log(name + ":BGM�͂���܂���");
    }

    /// <summary>
    /// Music���ꎞ��~������
    /// </summary>
    public void Pause()
    {
        if (IsPlay)
            audioSource.Pause();
    }

    /// <summary>
    /// Music���ĊJ������
    /// </summary>
    public void Resume()
    {
        if (IsPlay)
            audioSource.Play();
    }

    /// <summary>
    /// Music���~������
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
    /// �R���X�g���N�^
    /// </summary>
    public SEPlayer(AudioSource audioSource, List<MusicData> audioData,float volume = 1f) : base(audioSource, audioData, volume)
    {}

    public override void Play(int no)
    {
        if (audioData.Count <= no)
        {
            Debug.Log("�w�肳�ꂽ�ԍ���SE�͂���܂���");
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
        Debug.Log(name + ":SE�͂���܂���");
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

    BGMPlayer bgmPlayer;        //BGM�v���C���[
    public BGMPlayer BgmPlayer { get { return bgmPlayer; } }
    SEPlayer sePlayer;          //SE�v���C���[
    public SEPlayer SePlayer { get { return sePlayer; } }
    protected override void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }
        AudioSource bgmSource = gameObject.AddComponent<AudioSource>(); //BGM�p�I�[�f�B�I�\�[�X�쐬
        AudioSource seSource = gameObject.AddComponent<AudioSource>();  //SE�p�I�[�f�B�I�\�[�X�쐬
        bgmPlayer = new BGMPlayer(bgmSource, bgmData, mainVolume * bgmVolume);
        sePlayer = new SEPlayer(seSource, seData, mainVolume * seVolume);
    }
}
