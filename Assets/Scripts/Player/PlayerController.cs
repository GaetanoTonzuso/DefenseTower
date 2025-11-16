using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private static PlayerController _instance;
    public static PlayerController Instance => _instance;

    public PlayerControls playerInput;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        playerInput = new PlayerControls();
        playerInput.Enable();
    }

    private void OnEnable()
    {
        playerInput.Player.Action.started += Action_performed;
        playerInput.Player.Cancel.started += Action_Cancel;
        playerInput.Player.Quit.started += Quit_started;
    }

    private void Quit_started(InputAction.CallbackContext obj)
    {
        Application.Quit();
    }

    private void OnDisable()
    {
        playerInput.Player.Action.started -= Action_performed;
        playerInput.Player.Cancel.started -= Action_Cancel;

        playerInput.Disable();
    }

    private void OnDestroy()
    {
        if (playerInput != null)
        {
            playerInput.Player.Action.started -= Action_performed;
            playerInput.Player.Cancel.started -= Action_Cancel;
            playerInput.Disable();
        }
    }

    public void Action_performed(InputAction.CallbackContext obj)
    {
        EventService.Instance.OnActionPerformed.InvokeEvent();
    }

    public void Action_Cancel(InputAction.CallbackContext obj)
    {
        EventService.Instance.OnActionCancel.InvokeEvent();
    }
}
