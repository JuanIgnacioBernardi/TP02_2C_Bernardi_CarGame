using UnityEngine;

public class SceneAudio : MonoBehaviour
{
    [SerializeField] private AudioClip sceneMusic;
    private void Start()
    {
        AudioManager.Instance?.PlayMusic(sceneMusic);
    }
}