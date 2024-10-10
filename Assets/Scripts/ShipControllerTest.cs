using System;
using UnityEngine;

public class ShipControllerTest : MonoBehaviour
{

    [Serializable]
    public struct MovementStats
    {
        public float maxSpeed;
        public float turnSpeed;
        public float acceleration;
        public float deceleration;
    }

    public CameraManager.View GetCurrentView()
    {
        return currentView;
    }

    public void SetSpeedModifier(float speedModifier)
    {
        m_speedModifier = speedModifier;
    }

    // Might makes these ScriptableObjects...
    [SerializeField]
    private MovementStats overheadStats = new MovementStats { maxSpeed = 10f, turnSpeed = 10f, acceleration = 2f, deceleration = 0.1f };
    [SerializeField]
    private MovementStats thirdPersonStats = new MovementStats { maxSpeed = 5f, turnSpeed = 2f, acceleration = 1f, deceleration = 0.05f };
    [SerializeField]
    private MovementStats firstPersonStats = new MovementStats { maxSpeed = 1f, turnSpeed = 1f, acceleration = 0.5f, deceleration = 0.025f };

    private MovementStats m_movementStats;
    private float m_speedModifier = 1f;
    private InputManager m_inputManager;
    private CameraManager m_cameraManager;
    private CameraManager.View currentView = CameraManager.View.Overhead;
    private Rigidbody m_rigidBody;

    private void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_movementStats = overheadStats;
    }

    private void Start()
    {
        // Get managers from GameManager
        m_inputManager = GameManager.Instance.GetInputManager();
        m_cameraManager = GameManager.Instance.GetCameraManager();
        m_cameraManager.OnViewChanged += ViewChanged;
    }

    private void Update()
    {
        // Establish orientation
        Transform mainCam = Camera.main.transform;
        Vector3 forwardDir = LevelNormalVector(mainCam.forward);
        Vector3 rightDir = LevelNormalVector(mainCam.right);

        // Movement input
        Vector2 input = m_inputManager.GetMovement();
        Vector3 movementDir = (input.x * rightDir + input.y * forwardDir).normalized;
        Vector3 targetVelocity = m_speedModifier * m_movementStats.maxSpeed * movementDir; // Vector3.zero if no input

        // Move towards target velocity and apply force to rigidbody to play nicely with built in physics collisions
        float acc = input.magnitude > 0 ? m_movementStats.acceleration : m_movementStats.deceleration;
        Vector3 velocity = Vector3.MoveTowards(m_rigidBody.velocity, targetVelocity, acc);
        m_rigidBody.AddForce(velocity - m_rigidBody.velocity, ForceMode.VelocityChange);

        if (input.magnitude > 0)
        {
            // Rotate rigidbody with torque
            Vector3 newForward = Vector3.RotateTowards(transform.forward, velocity.normalized, m_movementStats.turnSpeed * Time.deltaTime, 1f);
            Vector3 omega = -m_movementStats.turnSpeed * Vector3.up * Vector3.SignedAngle(newForward, transform.forward, Vector3.up) * Time.deltaTime;
            m_rigidBody.AddTorque(omega - m_rigidBody.angularVelocity, ForceMode.VelocityChange);
        }
    }

    private void ViewChanged(object sender, CameraManager.OnViewChangedEventArgs e)
    {
        Debug.Log($"View: {e.view}");
        SetCurrentView(e.view);
    }

    private void SetCurrentView(CameraManager.View view)
    {
        currentView = view;
        m_movementStats = view switch
        {
            CameraManager.View.Overhead => overheadStats,
            CameraManager.View.ThirdPerson => thirdPersonStats,
            CameraManager.View.FirstPerson => firstPersonStats,
            _ => throw new ArgumentException($"No MovementStats for View {view}.")
        };
        m_rigidBody.velocity = Mathf.Clamp(m_rigidBody.velocity.magnitude, 0, m_movementStats.maxSpeed) * m_rigidBody.velocity.normalized;
    }

    private Vector3 LevelNormalVector(Vector3 vector)
    {
        // Vector projected to xy plane and normalized
        vector.y = 0;
        vector = vector.normalized;
        return vector;
    }

}

