using UnityEngine;
using UnityHFSM;
using Sensors;

public class AudienceEnemy : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float windUpTime = 0.8f;
    [SerializeField] private float throwCooldown = 3f;
    [SerializeField] private float damage = 15f;

    [Header("References")]
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private CarSensor sensor;

    public Transform CarTarget { get; private set; }
    private Rigidbody carRigidbody;
    private bool carInRange;

    private StateMachine<AudienceState, AudienceEvent> fsm;

    private void Start()
    {
        if (sensor != null)
        {
            sensor.OnCarEnter += OnCarEnter;
            sensor.OnCarExit += OnCarExit;
        }
        InitFSM();
    }

    private void InitFSM()
    {
        fsm = new StateMachine<AudienceState, AudienceEvent>();

        fsm.AddState(AudienceState.Idle, new AudienceIdleState(false, this));
        fsm.AddState(AudienceState.Aiming, new AudienceAimingState(true, this, windUpTime));
        fsm.AddState(AudienceState.Cooldown, new AudienceCooldownState(true, this, throwCooldown));

        // Idle → Aiming when detects the car
        fsm.AddTriggerTransition(AudienceEvent.DetectCar,
            new Transition<AudienceState>(AudienceState.Idle, AudienceState.Aiming));

        // Loses the car from any state
        fsm.AddTriggerTransition(AudienceEvent.LostCar,
            new Transition<AudienceState>(AudienceState.Aiming, AudienceState.Idle));
        fsm.AddTriggerTransition(AudienceEvent.LostCar,
            new Transition<AudienceState>(AudienceState.Cooldown, AudienceState.Idle));

        // Aiming → Cooldown: fires in transition
        fsm.AddTransition(new Transition<AudienceState>(
            AudienceState.Aiming,
            AudienceState.Cooldown,
            _ => true,
            onTransition: _ => ThrowProjectile()));

        // Cooldown → Aiming or Idle depending on whether the car is still within range
        fsm.AddTransition(new Transition<AudienceState>(
            AudienceState.Cooldown, AudienceState.Aiming, _ => carInRange));
        fsm.AddTransition(new Transition<AudienceState>(
            AudienceState.Cooldown, AudienceState.Idle, _ => !carInRange));

        fsm.Init();
    }
    private void Update() => fsm?.OnLogic();
    private void OnCarEnter(Transform car)
    {
        CarTarget = car;
        carRigidbody = car.GetComponentInParent<Rigidbody>();
        carInRange = true;
        fsm?.Trigger(AudienceEvent.DetectCar);
    }
    private void OnCarExit(Vector3 lastPos)
    {
        carInRange = false;
        CarTarget = null;
        fsm?.Trigger(AudienceEvent.LostCar);
    }
    private void ThrowProjectile()
    {
        if (projectilePrefab == null || throwPoint == null || CarTarget == null) return;

        // Predice posición futura del auto
        Vector3 predictedPos = CarTarget.position;
        if (carRigidbody != null)
            predictedPos += carRigidbody.linearVelocity * 1.2f;

        GameObject obj = Instantiate(projectilePrefab, throwPoint.position, Quaternion.identity);
        obj.GetComponent<ParabolicProjectile>()?.Launch(throwPoint.position, predictedPos, damage);
    }
    private void OnDrawGizmosSelected()
    {
        if (sensor != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, sensor.GetComponent<SphereCollider>().radius);
        }
    }
}