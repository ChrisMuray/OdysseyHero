using UnityEngine;

public class ControlsMenu : MonoBehaviour
{
    [SerializeField] private GameObject m_hint;
    [SerializeField] private GameObject m_controlsPanel;

    private InputManager m_inputManager;
    private bool m_showing = false;

    private void Start()
    {
        m_inputManager = GameManager.Instance.GetInputManager();
        m_inputManager.OnToggleControlsMenu += (sender, e) => m_showing = !m_showing;
    }

    private void Update()
    {
        m_hint.SetActive(!m_showing);
        m_controlsPanel.SetActive(m_showing);
    }

}
