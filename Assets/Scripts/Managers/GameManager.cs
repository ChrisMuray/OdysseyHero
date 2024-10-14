using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private InputManager inputManager;
    public InputManager GetInputManager() { return inputManager; }

    private CameraManager cameraManager;
    public CameraManager GetCameraManager() { return cameraManager; }

    private void Awake()
    {
        // Singeton
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Persists between scenes
        DontDestroyOnLoad(gameObject);

        // Stores other managers
        inputManager = GetComponent<InputManager>();
        cameraManager = GetComponent<CameraManager>();
    }

    // Temporary way to close game w/ escape
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

}