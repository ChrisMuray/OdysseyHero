using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{

    [SerializeField] private float m_maxStamina;
    [SerializeField] private float m_regenSpeed;
    [SerializeField] private BarUI m_barUI;

    private float m_stamina;
    private bool m_regen = true;

    public void Update()
    {
        if (m_regen)
        {
            Regen();
        }
    }

    public void SetRegen(bool regen)
    {
        m_regen = regen;
    }

    public float GetStamina()
    {
        return m_stamina;
    }

    public void SetStamina(float value)
    {
        m_stamina = Mathf.Clamp(value, 0, m_maxStamina);
        if (m_barUI)
        {
            m_barUI.SetFillAmount(m_stamina / m_maxStamina);
        }
    }

    public bool IsMax()
    {
        return m_stamina == m_maxStamina;
    }

    public bool IsEmpty()
    {
        return m_stamina == 0;
    }

    public void Drain(float amount)
    {
        SetStamina(m_stamina - amount);
    }

    public void Regen()
    {
        SetStamina(m_stamina + m_regenSpeed * Time.deltaTime);
        SetColor(IsMax() ? Color.yellow : Color.gray);
    }

    public void SetColor(Color color)
    {
        if (m_barUI)
        {
            m_barUI.SetFillColor(color);
        }
    }

    private void Awake()
    {
        m_stamina = m_maxStamina;
    }
}
