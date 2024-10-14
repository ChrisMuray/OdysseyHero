using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Cannons : MonoBehaviour
{

    [SerializeField] private GameObject m_cannonballPrefab;
    [SerializeField] private GameObject m_graphicsPrefab;
    [SerializeField] private GameObject m_uiPrefab;

    [SerializeField] private float m_reloadTime;
    [SerializeField] private int m_numCannonballs;
    private float m_lastFireTime;

    private CannonUI m_cannonUI;

    private InputManager m_inputManager;
    private ShipControllerTest player;

    private void Awake()
    {
        player = GetComponent<ShipControllerTest>();
        Instantiate(m_graphicsPrefab, transform);
        m_cannonUI = Instantiate(m_uiPrefab, transform).GetComponent<CannonUI>();
        m_cannonUI.SetFill(1f);
    }

    private void Start()
    {
        m_inputManager = GameManager.Instance.GetInputManager();
        m_inputManager.OnShootDown += TryFire;
    }

    private void TryFire(object sender, EventArgs e)
    {
        if (player.GetCurrentView() == CameraManager.View.ThirdPerson)
        {
            if (m_lastFireTime == -1f || Time.time - m_lastFireTime > m_reloadTime)
            {
                m_lastFireTime = Time.time;
                bool lookingRight = Vector3.Dot(Camera.main.transform.forward, player.transform.right) > 0;
                float spawnHeight = 0.5f;
                float spawnDist = 1f;
                Vector3 spawnPoint = transform.position + (lookingRight ? spawnDist : -spawnDist) * transform.right + spawnHeight * transform.up;
                for (int i = 0; i < m_numCannonballs; i++)
                {
                    float cannonBallSpread = 0.1f;
                    Vector3 offset = Random.insideUnitSphere * cannonBallSpread;
                    Vector3 thisSpawnPoint = spawnPoint + offset;
                    Rigidbody ball = Instantiate(m_cannonballPrefab, thisSpawnPoint, Quaternion.identity).GetComponent<Rigidbody>();
                    float explosionForce = 300f;
                    ball.AddExplosionForce(explosionForce, transform.position, (thisSpawnPoint - transform.position).magnitude, -0.4f, ForceMode.VelocityChange);
                }
            }
        }
    }

    private void Update()
    {
        float fill = m_lastFireTime == -1f ? 1f : (Time.time - m_lastFireTime) / m_reloadTime;
        m_cannonUI.SetFill(fill);
    }



}
