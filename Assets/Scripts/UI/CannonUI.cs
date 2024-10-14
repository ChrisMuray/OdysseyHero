using System;
using UnityEngine;
using UnityEngine.UI;

public class CannonUI : MonoBehaviour
{
    [SerializeField] Image m_cannonIcon;
    [SerializeField] Image m_background;

    private CameraManager m_cameraManager;

    private void Awake()
    {
        m_cannonIcon.enabled = false;
        m_background.enabled = false;
    }

    private void Start()
    {
        m_cameraManager = GameManager.Instance.GetCameraManager();
        m_cameraManager.OnViewChanged += ViewChanged;
    }

    private void ViewChanged(object sender, CameraManager.OnViewChangedEventArgs e)
    {
        bool visible = e.view == CameraManager.View.ThirdPerson;
        m_cannonIcon.enabled = visible;
        m_background.enabled = visible;
    }

    public void SetFill(float fill)
    {
        m_cannonIcon.fillAmount = Mathf.Clamp(fill, 0, 1);
    }
}
