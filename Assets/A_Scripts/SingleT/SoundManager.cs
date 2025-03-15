using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float bgmVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 0.7f;

    [Header("Extra Settings")]
    [SerializeField] bool noisy_yeah;
    float originalBgmVolume = 0.7f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        if (noisy_yeah)
        {
            bgmVolume = 0;
            sfxVolume = 0;
        }
        else
        {
            if (PlayerPrefs.HasKey("BGMVOL"))
            {
                bgmVolume = PlayerPrefs.GetFloat("BGMVOL");
            }
            else
            {
                PlayerPrefs.SetFloat("BGMVOL", 0.7f);
                bgmVolume = 0.7f;
            }
            if (PlayerPrefs.HasKey("SFXVOL"))
            {
                sfxVolume = PlayerPrefs.GetFloat("SFXVOL");
            }
            else
            {
                PlayerPrefs.SetFloat("SFXVOL", 0.7f);
                sfxVolume = 0.7f;
            }
        }
    }

    private void Update()
    {
        bgmSource.volume = bgmVolume;
        sfxSource.volume = sfxVolume;
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip) return;
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void OnSceneLoading()
    {
        StartCoroutine(FadeOutBGM());
    }

    public void OnSceneLoaded()
    {
        StartCoroutine(FadeInBGM());
    }

    IEnumerator FadeOutBGM()
    {
        originalBgmVolume = bgmVolume;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bgmVolume = Mathf.Lerp(originalBgmVolume, 0f, elapsed / duration);
            yield return null;
        }

        bgmVolume = 0f;
    }
    IEnumerator FadeInBGM()
    {
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bgmVolume = Mathf.Lerp(0f, originalBgmVolume, elapsed / duration);
            yield return null;
        }
        bgmVolume = originalBgmVolume;
    }

    public void ChangeBGM_Vol(Slider slider)
    {
        PlayerPrefs.SetFloat("BGMVOL", slider.value);
        bgmVolume = slider.value;
    }
    public void ChangeSFX_Vol(Slider slider)
    {
        PlayerPrefs.SetFloat("SFXVOL", slider.value);
        sfxVolume = slider.value;
    }
}
