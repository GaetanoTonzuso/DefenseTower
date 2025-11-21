using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Singleton instance for global access
    private static PlayerController _instance;
    public static PlayerController Instance => _instance;

    // Input Actions generated from the Input System
    public PlayerControls playerInput;

    private void Awake()
    {
        // Ensure only one PlayerController exists in the scene
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        // Create and enable the input system
        playerInput = new PlayerControls();
        playerInput.Enable();
    }

    private void OnEnable()
    {
        // Subscribe to input action events
        playerInput.Player.Action.started += Action_performed;
        playerInput.Player.Cancel.started += Action_Cancel;
        playerInput.Player.Quit.started += Quit_started;
    }

    private void Quit_started(InputAction.CallbackContext obj)
    {
        // Quit the application (will not work in editor)
        Application.Quit();
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks or duplicated callbacks
        playerInput.Player.Action.started -= Action_performed;
        playerInput.Player.Cancel.started -= Action_Cancel;
        playerInput.Player.Quit.started -= Quit_started;

        // Disable input actions
        playerInput.Disable();
    }

    private void OnDestroy()
    {
        // Additional cleanup in case the object is destroyed unexpectedly
        if (playerInput != null)
        {
            playerInput.Player.Action.started -= Action_performed;
            playerInput.Player.Cancel.started -= Action_Cancel;

            playerInput.Disable();
        }
    }

    // Trigger custom game event when the Action button is pressed
    public void Action_performed(InputAction.CallbackContext obj)
    {
        EventService.Instance.OnActionPerformed.InvokeEvent();
    }

    // Trigger custom game event when the Cancel button is pressed
    public void Action_Cancel(InputAction.CallbackContext obj)
    {
        EventService.Instance.OnActionCancel.InvokeEvent();
    }
}
