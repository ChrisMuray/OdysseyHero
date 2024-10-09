using System;
using System.Linq;
using UnityEngine;

public class OverheadShoot : MonoBehaviour
{
    [SerializeField] private GameObject m_projectilePrefab;
    [SerializeField] private float m_projectileSpeed;
    [SerializeField] private float m_projectileDamage;

    InputManager m_inputManager;
    ShipControllerTest m_shipController;

    private CameraManager.View[] applicableViews = { CameraManager.View.Overhead };

    private void Awake()
    {
        m_shipController = GetComponent<ShipControllerTest>();
    }

    private void Start()
    {
        m_inputManager = GameManager.Instance.GetInputManager();
        m_inputManager.OnShootDown += OnShoot;
    }

    private void OnShoot(object sender, EventArgs e)
    {
        if (applicableViews.Contains(m_shipController.GetCurrentView()))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        InputManager.DeviceType device = m_inputManager.GetDeviceType();

        if (device == InputManager.DeviceType.KeyboardMouse)
        {
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(camRay, out RaycastHit hit, 100f))
            {
                Vector3 shootDirection = hit.point - transform.position;
                shootDirection.y = 0;
                shootDirection = shootDirection.normalized;
                SpawnProjectile(shootDirection);
            }
        }
        else if (device == InputManager.DeviceType.Gamepad)
        {
            Vector2 shootDir2D = m_inputManager.GetLook();
            Vector3 shootDirection = new Vector3(shootDir2D.x, 0, shootDir2D.y).normalized;
            Debug.Log(shootDirection);
            SpawnProjectile(shootDirection);
        }
    }

    private void SpawnProjectile(Vector3 shootDirection)
    {
        float projectileSpawnRadius = 1f;
        float projectileSpawnHeight = 0.5f;
        Vector3 projectileSpawnPos = transform.position + projectileSpawnRadius * shootDirection + projectileSpawnHeight * Vector3.up;
        GameObject projectile = Instantiate(m_projectilePrefab, projectileSpawnPos, Quaternion.LookRotation(shootDirection));
        if (projectile.TryGetComponent<MoveForwardAndDie>(out MoveForwardAndDie mfad))
        {
            mfad.speed = m_projectileSpeed;
        }
        if (projectile.TryGetComponent<Damage>(out Damage damage))
        {
            damage.value = m_projectileDamage;
        }
    }

}
