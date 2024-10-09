using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShipControllerTest))]
[RequireComponent(typeof(Stamina))]
public class Boost : MonoBehaviour
{

    [SerializeField] private float m_speedModifier = 2f;
    [SerializeField] private float m_staminaDrainRate;

    private InputManager m_inputManager;
    private ShipControllerTest m_shipController;
    private Stamina m_stamina;
    private bool m_boosting = false;


    private void Awake()
    {
        m_shipController = GetComponent<ShipControllerTest>();
        m_stamina = GetComponent<Stamina>();
    }

    private void Start()
    {
        m_inputManager = GameManager.Instance.GetInputManager();
        m_inputManager.OnBoostDown += (context, e) => StartBoost();
        m_inputManager.OnBoostUp += (context, e) => StopBoost();
    }

    private void Update()
    {
        if (m_boosting)
        {
            m_stamina.Drain(m_staminaDrainRate * Time.deltaTime);
            if (m_stamina.IsEmpty())
            {
                StopBoost();
            }
        }
    }

    private void StartBoost()
    {
        if (m_stamina.IsMax() && m_inputManager.GetMovement().magnitude > 0)
        {
            m_boosting = true;
            m_stamina.SetRegen(false);
            m_shipController.SetSpeedModifier(m_speedModifier);
        }
    }

    private void StopBoost()
    {
        m_boosting = false;
        m_stamina.SetRegen(true);
        m_shipController.SetSpeedModifier(1f);
    }
}
