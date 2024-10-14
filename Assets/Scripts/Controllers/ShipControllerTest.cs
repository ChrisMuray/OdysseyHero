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
        return m_currentView;
    }

    public bool GetLookingRight()
    {
        return Vector3.Dot(Camera.main.transform.forward, transform.right) > 0;
    }

    public void SetSpeedModifier(float speedModifier)
    {
        m_speedModifier = speedModifier;
    }

    // Might makes these ScriptableObjects...
    [SerializeField] private MovementStats m_overheadStats;
    [SerializeField] private MovementStats m_thirdPersonStats;
    [SerializeField] private MovementStats m_firstPersonStats;

    private MovementStats m_movementStats;
    private float m_speedModifier = 1f;
    private InputManager m_inputManager;
    private CameraManager m_cameraManager;
    private CameraManager.View m_currentView = CameraManager.View.Overhead;
    private Rigidbody m_rigidBody;
    private Vector2 m_input;
    private Vector3 m_velocity;
    private Vector3 m_angularVelocity;

    private void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_movementStats = m_overheadStats;
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
        m_input = m_inputManager.GetMovement();
        Vector3 movementDir = (m_input.x * rightDir + m_input.y * forwardDir).normalized;
        Vector3 targetVelocity = m_speedModifier * m_movementStats.maxSpeed * movementDir; // Vector3.zero if no input

        // Move towards target velocity and apply force to rigidbody to play nicely with built in physics collisions
        float acc = m_input.magnitude > 0 ? m_movementStats.acceleration : m_movementStats.deceleration;
        m_velocity = Vector3.MoveTowards(m_rigidBody.velocity, targetVelocity, acc);

        if (m_input.magnitude > 0)
        {
            // Rotate rigidbody with torque
            Vector3 newForward = Vector3.RotateTowards(transform.forward, m_velocity.normalized, m_movementStats.turnSpeed * Time.deltaTime, 1f);
            m_angularVelocity = -m_movementStats.turnSpeed * Vector3.up * Vector3.SignedAngle(newForward, transform.forward, Vector3.up) * Mathf.PI / 180f; // convert to radians
        }
    }

    private void FixedUpdate()
    {
        m_rigidBody.AddForce(m_velocity - m_rigidBody.velocity, ForceMode.VelocityChange);
        if (m_input.magnitude > 0)
        {
            m_rigidBody.AddTorque(m_angularVelocity - m_rigidBody.angularVelocity, ForceMode.VelocityChange);
        }
    }

    private void ViewChanged(object sender, CameraManager.OnViewChangedEventArgs e)
    {
        Debug.Log($"View: {e.view}");
        SetCurrentView(e.view);
    }

    private void SetCurrentView(CameraManager.View view)
    {
        m_currentView = view;
        m_movementStats = view switch
        {
            CameraManager.View.Overhead => m_overheadStats,
            CameraManager.View.ThirdPerson => m_thirdPersonStats,
            CameraManager.View.FirstPerson => m_firstPersonStats,
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

