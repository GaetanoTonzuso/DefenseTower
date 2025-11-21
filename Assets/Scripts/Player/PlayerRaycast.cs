using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRaycast : MonoBehaviour
{
    // Camera used to cast the ray from the mouse position
    [SerializeField] private Camera _cam;

    private Ray _ray;                   // Ray created from the mouse position
    private RaycastHit _hitInfo;        // Stores raycast collision data
    private Vector2 _mousePos;          // Current mouse position

    IInteractable _lastInteractable;    // Tracks the last interactable object hit by the ray

    private void Update()
    {
        // Continuously check for interactable objects under the mouse
        CheckPlaceZones();
    }

    // Casts a ray from the camera to detect interactable objects
    private void CheckPlaceZones()
    {
        // Get mouse position from Input System
        _mousePos = PlayerController.Instance.playerInput.Player.Mouse.ReadValue<Vector2>();

        // Convert screen mouse position into a ray
        _ray = _cam.ScreenPointToRay(_mousePos);

        // Perform the raycast
        if (Physics.Raycast(_ray, out _hitInfo, Mathf.Infinity))
        {
            // Try to get an IInteractable component from the hit object
            IInteractable interactable = _hitInfo.transform.GetComponent<IInteractable>();

            // If it's the same interactable as last frame, do nothing
            if (interactable == _lastInteractable)
                return;

            // If we hit an interactable object
            if (interactable != null)
            {
                // Exit hover from the previous interactable
                if (_lastInteractable != null)
                    OnHoverExit(_lastInteractable);

                // Enter hover for the new interactable
                OnHoverEnter(interactable);

                // Trigger interaction logic
                interactable.Activate();
                interactable.Interact();
            }
            else
            {
                // If we hit something non-interactable, but were previously hovering an interactable
                if (_lastInteractable != null)
                    OnHoverExit(_lastInteractable);
            }
        }
        else
        {
            // If the raycast hits nothing, exit hover if necessary
            if (_lastInteractable != null)
                OnHoverExit(_lastInteractable);
        }
    }

    // Called when the ray enters a new interactable object
    private void OnHoverEnter(IInteractable newInteractable)
    {
        _lastInteractable = newInteractable;
    }

    // Called when the ray exits the current interactable object
    private void OnHoverExit(IInteractable lastInteractable)
    {
        if (lastInteractable != null)
            lastInteractable.Deactivate();

        _lastInteractable = null;
    }
}
