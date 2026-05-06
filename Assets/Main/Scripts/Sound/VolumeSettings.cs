using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider UISlider;
    private void Awake()
    {
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        UISlider.onValueChanged.AddListener(SetUiVolume);
    }
    private void Start()
    {
        LoadVolumes();
    }

    private void OnDestroy()
    {
        musicSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
        UISlider.onValueChanged.RemoveAllListeners();
    }
    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }
    public void SetSFXVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }
    public void SetUiVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat("UI", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("uiVolume", volume);
    }
    private void LoadVolumes()
    {
        float music = PlayerPrefs.GetFloat("musicVolume");
        float sfx = PlayerPrefs.GetFloat("sfxVolume");
        float ui = PlayerPrefs.GetFloat("uiVolume");

        musicSlider.value = music;
        sfxSlider.value = sfx;
        UISlider.value = ui;

        SetMusicVolume(music);
        SetSFXVolume(sfx);
        SetUiVolume(ui);
    }
}