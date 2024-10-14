using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

public class InputManager : MonoBehaviour
{

    public static InputManager instance { get; private set; }

    public event EventHandler OnToggleCamera;
    public event EventHandler OnFirstPersonDown;
    public event EventHandler OnFirstPersonUp;
    public event EventHandler OnBoostDown;
    public event EventHandler OnBoostUp;
    public event EventHandler OnShootDown;
    public event EventHandler OnShootUp;

    public event EventHandler OnToggleControlsMenu;

    private Inputs m_inputs;
    public enum DeviceType { None, KeyboardMouse, Gamepad }
    private DeviceType m_currentDevice = DeviceType.None;

    public DeviceType GetDeviceType()
    {
        return m_currentDevice;
    }

    public Vector3 GetMovement()
    {
        return m_inputs.Ship.Movement.ReadValue<Vector2>();
    }

    public Vector2 GetLook()
    {
        return m_inputs.Ship.Look.ReadValue<Vector2>();
    }

    private void Awake()
    {
        // Remove limit on event bytes per update
        InputSystem.settings.maxEventBytesPerUpdate = 0;

        // Set up input events for discrete things like button presses
        m_inputs = new Inputs();

        m_inputs.Ship.ToggleCamera.performed += (context) => OnToggleCamera?.Invoke(this, EventArgs.Empty);

        m_inputs.Ship.FirstPerson.performed += (context) => OnFirstPersonDown?.Invoke(this, EventArgs.Empty);
        m_inputs.Ship.FirstPerson.canceled += (context) => OnFirstPersonUp?.Invoke(this, EventArgs.Empty);

        m_inputs.Ship.Boost.performed += (context) => OnBoostDown?.Invoke(this, EventArgs.Empty);
        m_inputs.Ship.Boost.canceled += (context) => OnBoostUp?.Invoke(this, EventArgs.Empty);

        m_inputs.Ship.Shoot.performed += (context) => OnShootDown?.Invoke(this, EventArgs.Empty);
        m_inputs.Ship.Shoot.canceled += (context) => OnShootUp?.Invoke(this, EventArgs.Empty);

        m_inputs.Ship.ToggleControlsMenu.performed += (context) => OnToggleControlsMenu?.Invoke(this, EventArgs.Empty);
    }

    private void OnEnable()
    {
        m_inputs.Enable();
        InputSystem.onEvent += OnInputEvent;
    }

    private void OnDisable()
    {
        m_inputs.Disable();
        InputSystem.onEvent -= OnInputEvent;
    }

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        // Check for mouse movement or button presses from a keyboard, mouse, or gamepad and assign currentDevice
        if (device is Mouse mouse)
        {
            if (mouse.delta.ReadValue() != Vector2.zero)
            {
                UpdateInputDevice(DeviceType.KeyboardMouse);
            }
        }
        else if (device is Gamepad gamepad)
        {
            if (gamepad.leftStick.ReadValue() != Vector2.zero || gamepad.rightStick.ReadValue() != Vector2.zero)
            {
                UpdateInputDevice(DeviceType.Gamepad);
            }
        }
        else if (device is Keyboard)
        {
            UpdateInputDevice(DeviceType.KeyboardMouse);
        }
    }

    private void UpdateInputDevice(DeviceType newDevice)
    {
        if (m_currentDevice != newDevice)
        {
            m_currentDevice = newDevice;
            Debug.Log($"Switched device to {m_currentDevice}!");
        }
    }
}
