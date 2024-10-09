using System;
using UnityEngine;
using UnityEngine.AI;

public class ShipController : MonoBehaviour
{

    public event EventHandler OnDeath;

    [Header("Basic Movement")]
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float turnSpeed = 1f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float deceleration = 1f;

    [Header("Boost")]
    [SerializeField] private float boostSpeed;
    [SerializeField] private float boostDuration;
    [SerializeField] private float boostCooldown;

    [Header("Spear")]
    [SerializeField] private GameObject spearPrefab;

    private enum State
    {
        Movement,
        Boosting
    }

    private State state;
    private Health health;
    private Stamina stamina;

    private Vector3 forwardDir;
    private Vector3 rightDir;

    private NavMeshAgent navMeshAgent;
    private CapsuleCollider boostCollider;

    public CapsuleCollider GetBoostCollider()
    {
        return boostCollider;
    }

    public Vector3 GetVelocity()
    {
        return navMeshAgent.velocity;
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        boostCollider = GetComponent<CapsuleCollider>();
        boostCollider.enabled = false;
        health = GetComponent<Health>();
        health.OnDeath += Die;
        stamina = GetComponent<Stamina>();
    }

    private void Die(object sender, EventArgs e)
    {
        OnDeath?.Invoke(this, EventArgs.Empty);
        Destroy(gameObject);
    }

    private void Start()
    {
        Transform cam = Camera.main.transform;
        forwardDir = cam.forward;
        forwardDir.y = 0;
        forwardDir = forwardDir.normalized;
        rightDir = cam.right;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        switch (state)
        {
            case State.Movement:

                MovementFromInput();
                stamina.Regen();

                if (Input.GetMouseButtonDown(0))
                {
                    ShootSpear();
                }

                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    if (stamina.IsMax() && navMeshAgent.velocity.magnitude > 0)
                    {
                        state = State.Boosting;
                        boostCollider.enabled = true;
                    }
                }
                break;

            case State.Boosting:
                if (stamina.IsEmpty() || Input.GetKeyUp(KeyCode.LeftShift))
                {
                    state = State.Movement;
                    boostCollider.enabled = false;
                }
                navMeshAgent.velocity = Vector3.MoveTowards(navMeshAgent.velocity, boostSpeed * navMeshAgent.velocity.normalized, acceleration);
                // stamina.Drain();
                break;
        }
    }

    private void MovementFromInput()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        float acc = input.magnitude > 0 ? acceleration : deceleration;

        // Movement input
        Vector3 movementDir = (input.x * rightDir + input.y * forwardDir).normalized;
        Vector3 targetVelocity = maxSpeed * movementDir; // Vector3.Zero if no input
        navMeshAgent.velocity = Vector3.MoveTowards(navMeshAgent.velocity, targetVelocity, acc);

        if (input.magnitude > 0)
        {
            transform.forward = Vector3.RotateTowards(transform.forward, navMeshAgent.velocity.normalized, turnSpeed, 1f);
        }
    }

    private void ShootSpear()
    {
        Vector3 targetPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Physics.Raycast(targetPoint, Camera.main.transform.forward, out RaycastHit hitInfo))
        {
            targetPoint = hitInfo.point;
            targetPoint.y = transform.position.y;
            Vector3 targetDirection = (targetPoint - transform.position).normalized;
            float spearSpawnRadius = 2f;
            float spearSpawnHeight = 0.5f;
            Instantiate(
                spearPrefab,
                transform.position + spearSpawnRadius * targetDirection + spearSpawnHeight * Vector3.up,
                Quaternion.LookRotation(targetDirection));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (state != State.Boosting && other.TryGetComponent<Damage>(out Damage damage))
        {
            health.TakeDamage(damage.value);
        }
        // if (state == State.Boosting)
        // {
        //     boostCollider.enabled = false;
        //     state = State.Movement;
        //     navMeshAgent.velocity = Vector3.zero;
        // }
    }
}
