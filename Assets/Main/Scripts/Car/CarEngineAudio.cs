using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CarEngineAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CarStats carStats;

    [Header("Engine Audio")]
    [SerializeField] private AudioClip engineLoop;  
    [SerializeField] private float minPitch = 0.6f; 
    [SerializeField] private float maxPitch = 2.2f; 
    [SerializeField] private float pitchSmoothSpeed = 3f;

    [Header("Reference Speed")]
    [SerializeField] private float maxSpeed = 20f;    

    private AudioSource _engineSource;
    private Rigidbody _rb;
    private float _targetPitch;

    private void Awake()
    {
        _engineSource = GetComponent<AudioSource>();
        _rb = GetComponent<Rigidbody>();

        _engineSource.clip = engineLoop;
        _engineSource.loop = true;
        _engineSource.playOnAwake = false;
    }
    private void Start()
    {
        if (engineLoop == null) return;
        _engineSource.pitch = minPitch;
        _engineSource.Play();
    }
    private void Update()
    {
        if (!carStats.HasFuel() || carStats.IsDead)
        {
            FadeOutEngine();
            return;
        }

        UpdateEnginePitch();
    }
    private void UpdateEnginePitch()
    {
        // Current normalized speed between 0 and 1
        float speed = _rb != null ? _rb.linearVelocity.magnitude : 0f;
        float speedNormalized = Mathf.Clamp01(speed / maxSpeed);

        // Target pitch based on speed
        _targetPitch = Mathf.Lerp(minPitch, maxPitch, speedNormalized);

        // Smooth the pitch change so that it is not abrupt
        _engineSource.pitch = Mathf.Lerp(
            _engineSource.pitch,
            _targetPitch,
            Time.deltaTime * pitchSmoothSpeed
        );
        // Reactivate if it was off
        if (!_engineSource.isPlaying)
            _engineSource.Play();
    }
    private void FadeOutEngine()
    {
        // The pitch gradually decreases when it dies or runs out of fuel.
        _engineSource.pitch = Mathf.Lerp(
            _engineSource.pitch,
            minPitch * 0.5f,
            Time.deltaTime * pitchSmoothSpeed
        );
        if (_engineSource.pitch <= minPitch * 0.55f)
            _engineSource.Stop();
    }
}