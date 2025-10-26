using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRaycast : MonoBehaviour
{
    [SerializeField] private Camera _cam;

    private PlayerInput _playerInput;
    private Ray _ray;
    private RaycastHit _hitInfo;
    private Vector2 _mousePos;

    IInteractable _lastInteractable;

    private void Start()
    {
        _playerInput = new PlayerInput();
        _playerInput.Enable();
    }

    private void Update()
    {
        CheckPlaceZones();
    }

    private void CheckPlaceZones()
    {
        _mousePos = _playerInput.Player.Mouse.ReadValue<Vector2>();
        _ray = _cam.ScreenPointToRay(_mousePos);

        if (Physics.Raycast(_ray, out _hitInfo, Mathf.Infinity))
        {
            IInteractable interactable = _hitInfo.transform.GetComponent<IInteractable>();

            // Se stai ancora puntando lo stesso oggetto, non fare nulla
            if (interactable == _lastInteractable)
                return;

            // Se colpisci un nuovo oggetto interagibile
            if (interactable != null)
            {
                // Se prima ne stavi puntando un altro, notificagli l'uscita
                if (_lastInteractable != null)
                    OnHoverExit(_lastInteractable);

                // Entra nel nuovo oggetto
                OnHoverEnter(interactable);
                interactable.Activate();
                interactable.Interact();
            }
            else
            {
                // Se colpisci qualcosa non interagibile
                if (_lastInteractable != null)
                    OnHoverExit(_lastInteractable);
            }
        }
        else
        {
            // Se non colpisci nulla
            if (_lastInteractable != null)
                OnHoverExit(_lastInteractable);
        }
    }

    private void OnHoverEnter(IInteractable newInteractable)
    {
        _lastInteractable = newInteractable;
    }

    private void OnHoverExit(IInteractable lastInteractable)
    {
        if (lastInteractable != null)
            lastInteractable.Deactivate();

        _lastInteractable = null;
    }
}
