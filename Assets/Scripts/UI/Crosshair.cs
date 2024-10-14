using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private float m_crosshairRadius = 50f;

    private InputManager m_inputManager;
    private CameraManager m_cameraManager;
    private Transform m_target;
    private RectTransform m_rectTransform;
    private Image m_image;
    private Camera m_cam;

    private void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();
        m_image = GetComponent<Image>();
    }

    private void Start()
    {
        m_inputManager = GameManager.Instance.GetInputManager();
        m_cameraManager = GameManager.Instance.GetCameraManager();
        m_target = FindAnyObjectByType<ShipControllerTest>().transform;

        m_cam = Camera.main;

        // Lock mouse and make cursor invisible
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void LateUpdate()
    {
        if (m_cameraManager.GetView() != CameraManager.View.Overhead)
        {
            m_image.enabled = false;
            return;
        }
        InputManager.DeviceType device = m_inputManager.GetDeviceType();
        Vector2 look = m_inputManager.GetLook();

        if (device == InputManager.DeviceType.KeyboardMouse)
        {
            m_image.enabled = true;
            m_rectTransform.anchoredPosition = Input.mousePosition;
        }
        else if (device == InputManager.DeviceType.Gamepad && look.magnitude > 0)
        {
            m_image.enabled = true;
            Vector2 center = m_target ? m_cam.WorldToScreenPoint(m_target.position) : new Vector2(Screen.width, Screen.height) / 2;
            m_rectTransform.anchoredPosition = center + m_crosshairRadius * look.normalized;
        }
        else
        {
            m_image.enabled = false;
        }
    }
}
