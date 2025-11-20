using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRaycast : MonoBehaviour
{
    [SerializeField] private Camera _cam;

    private Ray _ray;
    private RaycastHit _hitInfo;
    private Vector2 _mousePos;

    IInteractable _lastInteractable;

    private void Update()
    {
        CheckPlaceZones();
    }

    private void CheckPlaceZones()
    {
        _mousePos = PlayerController.Instance.playerInput.Player.Mouse.ReadValue<Vector2>();
        _ray = _cam.ScreenPointToRay(_mousePos);

        if (Physics.Raycast(_ray, out _hitInfo, Mathf.Infinity))
        {
            IInteractable interactable = _hitInfo.transform.GetComponent<IInteractable>();
            if (interactable == _lastInteractable)
                return;

            if (interactable != null)
            {
                if (_lastInteractable != null)
                    OnHoverExit(_lastInteractable);

                OnHoverEnter(interactable);
                interactable.Activate();
                interactable.Interact();
            }
            else
            {
                if (_lastInteractable != null)
                    OnHoverExit(_lastInteractable);
            }
        }
        else
        {
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
