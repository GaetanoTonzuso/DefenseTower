using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private static PlayerController _instance;
    public static PlayerController Instance
    {
        get
        {
            return _instance;
        }
    }

    public PlayerInput playerInput;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        playerInput = new PlayerInput();
        playerInput.Enable();
    }

    private void Start()
    {
        playerInput.Player.Action.started += Action_performed;
        playerInput.Player.Cancel.started += Action_Cancel;
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
