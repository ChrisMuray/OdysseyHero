using System;
using UnityEngine;

public class Health : MonoBehaviour
{

    public event EventHandler OnDeath;

    [SerializeField] private float m_maxHealth;
    [SerializeField] private float m_regenSpeed;
    [SerializeField] private BarUI m_barUI;

    private float m_health;
    private bool m_regen = true;

    public float GetHealth()
    {
        return m_health;
    }

    public void SetHealth(float value)
    {
        m_health = Mathf.Clamp(value, 0, m_maxHealth); ;
        if (m_barUI)
        {
            m_barUI.SetFillAmount(m_health / m_maxHealth);
        }
        if (m_health == 0)
        {
            OnDeath?.Invoke(this, EventArgs.Empty);
            if (m_barUI)
            {
                Destroy(m_barUI.gameObject);
                Destroy(this);
            }
        }
    }

    public void SetBarUI(BarUI ui)
    {
        m_barUI = ui;
    }

    public void TakeDamage(float damage)
    {
        SetHealth(m_health - damage);
    }

    public void SetColor(Color color)
    {
        if (m_barUI)
        {
            m_barUI.SetFillColor(color);
        }
    }

    public void SetVisible(bool visible)
    {
        m_barUI.gameObject.SetActive(visible);
    }

    public bool IsVisible()
    {
        return m_barUI.gameObject.activeSelf;
    }

    private void Awake()
    {
        m_health = m_maxHealth;
    }

    private void Update()
    {
        if (m_regen)
        {
            SetHealth(m_health + m_regenSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent<Damage>(out Damage damage))
        {
            Debug.Log("YOUCH!!");
            SetHealth(m_health - damage.value);
        }
    }
}
