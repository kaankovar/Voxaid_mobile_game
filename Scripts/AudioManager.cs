using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SoundType
{
    MainMenuMusic,
    GameMusic,

    ButtonClick,
    LevelUp,
    Buying,
    NotEnoughMoney,
    Reroll,
    Prestige,
    PrestigeStar,
    Jackpot,

    PickupHealth,
    PickupGold,
    PickupGem,

    Shoot,
    HitMarker,
    Ricochet,
    Execution,

    Explosion,
    HeavyExplosion,

    PlayerHurt,
    ArmorBreak,
    PlayerDeath,
    Revive,
    EnemyDeath,
    Freeze,
    Thorns,

    BossWarning,
    BossLaser,
    ChainLightning,
    OrbitalBlade
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Ses Kütüphanesi")]
    public SoundData[] sounds;

    [Header("Kaynaklar")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    private Dictionary<SoundType, SoundData> soundDictionary;

    private Coroutine currentFadeRoutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeDictionary()
    {
        soundDictionary = new Dictionary<SoundType, SoundData>();
        foreach (var s in sounds)
        {
            if (!soundDictionary.ContainsKey(s.type))
            {
                soundDictionary.Add(s.type, s);
            }
        }
    }

    public void PlayMusic(SoundType type)
    {
        if (soundDictionary.TryGetValue(type, out SoundData s))
        {
            if (musicSource.clip == s.clip) return;

            musicSource.clip = s.clip;
            musicSource.volume = s.volume * PlayerPrefs.GetFloat("MasterVolume", 1f);
            musicSource.loop = true;
            musicSource.Play();
        }
    }


    public void PlayMusicWithFade(SoundType type, float fadeDuration = 10f)
    {
        if (soundDictionary.TryGetValue(type, out SoundData s))
        {
            if (musicSource.clip == s.clip && musicSource.isPlaying) return;

            if (currentFadeRoutine != null)
            {
                StopCoroutine(currentFadeRoutine);
            }

            currentFadeRoutine = StartCoroutine(FadeIn(s, fadeDuration));
        }
    }

    public void StopMusicWithFade(float fadeDuration = 1.5f)
    {
        if (currentFadeRoutine != null)
        {
            StopCoroutine(currentFadeRoutine);
        }
        currentFadeRoutine = StartCoroutine(FadeOut(fadeDuration));
    }

    private IEnumerator FadeIn(SoundData s, float duration)
    {
        float targetVolume = s.volume * PlayerPrefs.GetFloat("MasterVolume", 1f);

        musicSource.clip = s.clip;
        musicSource.loop = true;
        musicSource.volume = 0f;
        musicSource.Play();

        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, targetVolume, currentTime / duration);
            yield return null;
        }

        musicSource.volume = targetVolume;
    }

    private IEnumerator FadeOut(float duration)
    {
        float startVolume = musicSource.volume;
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, currentTime / duration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume;
    }


    public void PlaySFX(SoundType type)
    {
        if (soundDictionary.TryGetValue(type, out SoundData s))
        {
            float basePitch = s.pitch;

            if (type == SoundType.PickupGold || type == SoundType.Shoot ||
               type == SoundType.PickupGem || type == SoundType.EnemyDeath || type == SoundType.HitMarker)
            {
                sfxSource.pitch = basePitch * Random.Range(0.85f, 1.15f);
            }
            else
            {
                sfxSource.pitch = basePitch;
            }

            float finalVolume = s.volume * PlayerPrefs.GetFloat("MasterVolume", 1f);
            sfxSource.PlayOneShot(s.clip, finalVolume);
        }
    }

    public void UpdateVolume(float volume)
    {
        musicSource.volume = volume;
    }
}