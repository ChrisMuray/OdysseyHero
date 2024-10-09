using System;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [SerializeField] private Transform m_target;
    [SerializeField] private CinemachineVirtualCamera m_overheadCam;
    [SerializeField] private CinemachineFreeLook m_thirdPersonCam;
    [SerializeField] private CinemachineVirtualCamera m_firstPersonCam;

    public event EventHandler<OnViewChangedEventArgs> OnViewChanged;
    public class OnViewChangedEventArgs : EventArgs { public View view; }

    // Enum used by many classes to determine which view we are in
    public enum View { None, Overhead, ThirdPerson, FirstPerson }

    public View GetView()
    {
        CinemachineVirtualCameraBase currentCam = GetCurrentCam();
        if (currentCam == m_overheadCam) { return View.Overhead; }
        if (currentCam == m_thirdPersonCam) { return View.ThirdPerson; }
        if (currentCam == m_firstPersonCam) { return View.FirstPerson; }
        return View.None;
    }

    private InputManager m_inputManager;

    // Array of all virtual cameras
    private CinemachineVirtualCameraBase[] m_cams;

    // Camera priority constants
    private const int PRIORITY_DISABLED = 10;
    private const int PRIORITY_REMEMBER = 15;
    private const int PRIORITY_ENABLED = 20;

    private void Start()
    {
        // Subscribe to input events
        m_inputManager = GameManager.Instance.GetInputManager();
        m_inputManager.OnToggleCamera += (object sender, EventArgs e) => ToggleCamera();
        m_inputManager.OnFirstPersonDown += (object sender, EventArgs e) => EnterFPV();
        m_inputManager.OnFirstPersonUp += (object sender, EventArgs e) => ExitFPV();

        m_cams = new CinemachineVirtualCameraBase[] { m_overheadCam, m_thirdPersonCam, m_firstPersonCam };

        // Set all camera follow and look transforms (don't need to do them individually)
        m_overheadCam.Follow = m_target;
        m_overheadCam.LookAt = m_target;
        m_thirdPersonCam.Follow = m_target;
        m_thirdPersonCam.LookAt = m_target;
        m_firstPersonCam.Follow = m_target;

        Select(m_overheadCam);
    }

    private CinemachineVirtualCameraBase GetCurrentCam()
    {
        return m_cams.OrderByDescending(cam => cam.Priority).First();
    }

    private CinemachineVirtualCameraBase GetRememberedCam()
    {
        return m_cams.Where(cam => cam.Priority == PRIORITY_REMEMBER).First();
    }

    private void Select(CinemachineVirtualCameraBase selectedCam, CinemachineVirtualCameraBase rememberCam = null)
    {
        CinemachineVirtualCameraBase currentCam = GetCurrentCam();

        Debug.Log($"Current: {currentCam}   Selected: {selectedCam}    Remember: {rememberCam}");

        foreach (CinemachineVirtualCameraBase cam in m_cams)
        {
            cam.Priority = cam == selectedCam ? PRIORITY_ENABLED : PRIORITY_DISABLED;
        }
        if (rememberCam)
        {
            rememberCam.Priority = PRIORITY_REMEMBER;
        }
        OnViewChanged?.Invoke(this, new OnViewChangedEventArgs { view = GetView() });
    }

    private void ToggleCamera()
    {
        Select(GetCurrentCam() == m_overheadCam ? m_thirdPersonCam : m_overheadCam);
    }

    private void EnterFPV()
    {
        Select(m_firstPersonCam, rememberCam: GetCurrentCam());
    }

    private void ExitFPV()
    {
        Select(GetRememberedCam());
    }

}
