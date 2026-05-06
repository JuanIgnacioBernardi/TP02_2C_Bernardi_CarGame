using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("-------- Audio Source --------")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource uiSource;

    [Header("-------- Audio Clip --------")]
    public AudioClip backgroundMusic;
    public AudioClip WinSfx;
    public AudioClip LooseSfx;
    public AudioClip LaserShootSfx;
    public AudioClip LauncherShootSfx;
    public AudioClip EnemyAttackSfx;
    public AudioClip EnemyDeathSfx;
    public AudioClip PlayerDamageSfx;
    public AudioClip PlayerDeathSfx;
    [Header("-------- Audio Clip UI --------")]
    public AudioClip ButtonUI;
    public AudioClip HoverUi;
    private void Start()
    {
        // Play background music on start
        if (musicSource == null || backgroundMusic == null) return;
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    public void Stop()
    {
        // Stop all audio sources
        if (musicSource != null)
        {
            musicSource.Stop();
            musicSource.clip = null;
        }
        if (sfxSource != null)
        {
            sfxSource.Stop();
            sfxSource.clip = null;
        }
    }
    public void PlaySFX(AudioClip clip)
    {
        // Play a one-shot sound effect
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }
    public void PlayUI(AudioClip clip)
    {
        // Play a one-shot UI sound effect
        if (sfxSource == null || clip == null) return;
        uiSource.PlayOneShot(clip);
    }
    public void PlayLoopedSfx(AudioClip clip)
    {
        // Play a looped sound effect
        if (sfxSource == null || clip == null) return;
        sfxSource.clip = clip;
        sfxSource.loop = true;
        sfxSource.Play();
    }
}