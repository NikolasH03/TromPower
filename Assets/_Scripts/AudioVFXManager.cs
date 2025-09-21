using UnityEngine;

public class AudioVFXManager : MonoBehaviour
{
    public static AudioVFXManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;   // Para efectos de sonido
    public AudioSource musicSource; // Para m�sica
    public AudioSource giroSource;

    public AudioClip musicaMenu;
    public AudioClip musicaJuego;
    public AudioClip musicaGameOver;
    public AudioClip musicaWin;
    public AudioClip musicaUltimosMinutos;

    public AudioClip sonidoChoque;
    public AudioClip sonidoGiro;
    public AudioClip sonidoPerder;

    [Header("VFX")]
    public GameObject VFXChoque; // Puedes asignar un prefab de part�culas por defecto

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Funci�n para reproducir efectos de sonido (SFX)
    public void PlaySound(AudioClip clip, float volume = 0.6f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }
    public void PlayGiro(float volume = 0.6f)
    {
        if (sonidoGiro == null || sfxSource == null) return;
        sfxSource.PlayOneShot(sonidoGiro, volume);
    }
    //Funci�n para reproducir m�sica
    public void PlayMusic(AudioClip clip, float volume = 0.3f, bool loop = true)
    {
        if (clip == null || musicSource == null) return;
        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.loop = loop;
        musicSource.Play();
    }

    //Funci�n para instanciar VFX en una posici�n
    public void PlayVFX(GameObject vfxPrefab, Transform target)
    {
        if (target == null) return;
        GameObject prefabToUse = vfxPrefab != null ? vfxPrefab : VFXChoque;
        if (prefabToUse == null) return;

        Instantiate(prefabToUse, target.position, target.rotation);
    }
}

