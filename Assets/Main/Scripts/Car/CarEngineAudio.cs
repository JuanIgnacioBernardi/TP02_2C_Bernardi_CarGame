using UnityEngine;

public class CarEngineAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CarController carController;
    [SerializeField] private CarDataSO carData;

    [Header("Clips")]
    [SerializeField] private AudioClip startup;
    [SerializeField] private AudioClip idleClip;
    [SerializeField] private AudioClip lowClip;
    [SerializeField] private AudioClip medClip;
    [SerializeField] private AudioClip highClip;
    [SerializeField] private AudioClip maxRpmClip;

    [Header("Transition")]
    [SerializeField] private float crossfadeSpeed = 2f;
    [SerializeField] private float hysteresis = 5f;

    private float lowThreshold;
    private float medThreshold;
    private float highThreshold;
    private float maxRpmThreshold;

    private AudioSource _sourceA;
    private AudioSource _sourceB;
    private bool _usingA = true;
    private AudioClip _currentClip;
    private bool _ready = false;
    private void Awake()
    {
        carController = GetComponent<CarController>();

        _sourceA = gameObject.AddComponent<AudioSource>();
        _sourceB = gameObject.AddComponent<AudioSource>();

        ConfigSource(_sourceA);
        ConfigSource(_sourceB);
    }
    private void ConfigSource(AudioSource src)
    {
        src.loop = true;
        src.playOnAwake = false;
        src.volume = 0f;
        src.spatialBlend = 1f;
    }
    private void Start()
    {
        if (carData != null)
        {
            float maxKmh = carData.maxSpeed * 3.6f;
            lowThreshold = maxKmh * 0.20f;
            medThreshold = maxKmh * 0.40f;
            highThreshold = maxKmh * 0.65f;
            maxRpmThreshold = maxKmh * 0.85f;
        }
        else
        {
            lowThreshold = 40f;
            medThreshold = 80f;
            highThreshold = 130f;
            maxRpmThreshold = 170f;
        }

        if (startup != null)
        {
            var startupSrc = gameObject.AddComponent<AudioSource>();
            startupSrc.loop = false;
            startupSrc.clip = startup;
            startupSrc.volume = 1f;
            startupSrc.Play();
            Destroy(startupSrc, startup.length);
            Invoke(nameof(StartEngine), startup.length);
        }
        else
        {
            StartEngine();
        }
    }
    private void StartEngine()
    {
        _ready = true;
        CrossfadeTo(idleClip, instant: true);
    }
    private void Update()
    {
        if (!_ready) return;

        float speedKmh = carController.CurrentSpeed * 3.6f;
        AudioClip target = GetClipForSpeed(speedKmh);

        if (target != _currentClip)
            CrossfadeTo(target);

        AudioSource active = _usingA ? _sourceA : _sourceB;
        AudioSource inactive = _usingA ? _sourceB : _sourceA;

        active.volume = Mathf.MoveTowards(active.volume, 1f, Time.deltaTime * crossfadeSpeed);
        inactive.volume = Mathf.MoveTowards(inactive.volume, 0f, Time.deltaTime * crossfadeSpeed);
    }
    private AudioClip GetClipForSpeed(float speedKmh)
    {
        if (_currentClip == idleClip)
        {
            if (speedKmh > lowThreshold + hysteresis) return lowClip;
        }
        else if (_currentClip == lowClip)
        {
            if (speedKmh < lowThreshold - hysteresis) return idleClip;
            if (speedKmh > medThreshold + hysteresis) return medClip;
        }
        else if (_currentClip == medClip)
        {
            if (speedKmh < medThreshold - hysteresis) return lowClip;
            if (speedKmh > highThreshold + hysteresis) return highClip;
        }
        else if (_currentClip == highClip)
        {
            if (speedKmh < highThreshold - hysteresis) return medClip;
            if (speedKmh > maxRpmThreshold + hysteresis) return maxRpmClip;
        }
        else if (_currentClip == maxRpmClip)
        {
            if (speedKmh < maxRpmThreshold - hysteresis) return highClip;
        }

        return _currentClip;
    }
    private void CrossfadeTo(AudioClip clip, bool instant = false)
    {
        _currentClip = clip;
        _usingA = !_usingA;

        AudioSource next = _usingA ? _sourceA : _sourceB;
        next.clip = clip;
        next.volume = instant ? 1f : 0f;
        next.Play();
    }
    public void StopEngine()
    {
        _ready = false;
        _sourceA.Stop();
        _sourceB.Stop();
    }
}