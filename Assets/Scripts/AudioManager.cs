using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Sound
{
    public string name; // ��Ч�����֣����ڱ�ʶ
    public AudioClip clip; // ��Ӧ����Ƶ�ļ�
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Background Music")]
    public AudioClip bgmClip; // ���������ļ�
    private AudioSource bgmSource; // �������ֵ� AudioSource
    public float bgmVolume = 1f; // ������������

    [Header("Sound Effects")]
    public List<Sound> soundEffects; // ��Ϸ��Ч�б�
    private Dictionary<string, AudioClip> sfxDictionary; // ��Ч�ֵ�
    private List<AudioSource> sfxSources = new List<AudioSource>(); // ��Ч��
    public float sfxVolume = 1f; // ��Ϸ��Ч����
    public int sfxPoolSize = 8; // ��Ч�ش�С

    private bool canPlaySFX = true; // ������Ч�Ƿ��ܲ���

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitAudioSources();
        InitSoundEffects();

        // �Զ����ű������֣���������ã�
        if (bgmClip != null)
        {
            PlayBGM();
        }
    }

    private void InitAudioSources()
    {
        // ��ʼ���������� AudioSource
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;

        // ��ʼ����Ч��
        for (int i = 0; i < sfxPoolSize; i++)
        {
            AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.volume = sfxVolume;
            sfxSources.Add(sfxSource);
        }
    }

    private void InitSoundEffects()
    {
        // ����Ч�б�ת��Ϊ�ֵ�
        sfxDictionary = new Dictionary<string, AudioClip>();
        foreach (var sound in soundEffects)
        {
            if (!sfxDictionary.ContainsKey(sound.name))
            {
                sfxDictionary.Add(sound.name, sound.clip);
            }
            else
            {
                Debug.LogWarning($"Duplicate sound name found: {sound.name}");
            }
        }
    }

    #region BGM Methods
    public void PlayBGM()
    {
        if (bgmClip == null)
        {
            Debug.LogWarning("No background music clip assigned!");
            return;
        }

        bgmSource.clip = bgmClip;
        bgmSource.volume = bgmVolume;
        bgmSource.Play();
    }

    public void PauseBGM()
    {
        bgmSource.Pause();
    }

    public void ResumeBGM()
    {
        bgmSource.UnPause();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }
    #endregion

    #region SFX Methods
    public void PlaySFX(string name)
    {
        // �����Ч�����ã��򲻲�����Ч
        if (!canPlaySFX) return;

        if (sfxDictionary.TryGetValue(name, out AudioClip clip))
        {
            foreach (var sfxSource in sfxSources)
            {
                if (!sfxSource.isPlaying)
                {
                    sfxSource.clip = clip;
                    sfxSource.volume = sfxVolume;
                    sfxSource.Play();
                    return;
                }
            }

            Debug.LogWarning("All SFX sources are busy. Consider increasing the pool size.");
        }
        else
        {
            Debug.LogWarning($"Sound effect not found: {name}");
        }
    }

    public void StopAllSFX()
    {
        foreach (var sfxSource in sfxSources)
        {
            if (sfxSource.isPlaying)
            {
                sfxSource.Stop();
            }
        }
    }

    // ����������Ч
    public void DisableSFX()
    {
        canPlaySFX = false;
    }

    // ����������Ч
    public void EnableSFX()
    {
        canPlaySFX = true;
    }
    #endregion

    #region Volume Control
    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        bgmSource.volume = bgmVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        foreach (var sfxSource in sfxSources)
        {
            sfxSource.volume = sfxVolume;
        }
    }
    #endregion

    //[ContextMenu("ɾ���洢����Ϸ����")] // �Ҽ��ű�ʱ��ʾ�˵���
    //public void DeleteData()
    //{
    //    JsonDataManager.DeleteData();
    //}
}
