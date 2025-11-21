using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class WeaponSelection : MonoBehaviour
{
    Vector2 _mousePos;                    // Current mouse position on screen
    private GameObject _currentItem;      // The weapon/item currently being dragged by the player
    private bool _isPreviewing;           // Whether the player is previewing over a PlaceZone

    [SerializeField] private float _z = 18;  // Distance from camera to spawn the preview item

    private void OnEnable()
    {
        // Listen to item selection and placement preview events
        EventService.Instance.OnItemSelected.AddListener(OnItemSelected);
        EventService.Instance.OnItemPreview.AddListener(OnItemPreview);
        EventService.Instance.OnItemNotPreview.AddListener(OnItemNotPreview);
        EventService.Instance.OnItemSpawned.AddListener(OnItemSpawned);
        EventService.Instance.OnActionCancel.AddListener(OnCancel);
    }

    private void OnDisable()
    {
        // Remove event listeners
        EventService.Instance.OnItemSelected.RemoveListener(OnItemSelected);
        EventService.Instance.OnItemPreview.RemoveListener(OnItemPreview);
        EventService.Instance.OnItemNotPreview.RemoveListener(OnItemNotPreview);
        EventService.Instance.OnItemSpawned.RemoveListener(OnItemSpawned);
        EventService.Instance.OnActionCancel.RemoveListener(OnCancel);
    }

    private void Update()
    {
        // Always track the mouse cursor position
        _mousePos = Mouse.current.position.ReadValue();

        // If the player has selected a weapon to drag around
        if (_currentItem != null)
        {
            // Hide weapon preview when hovering a valid PlaceZone
            if (_isPreviewing)
            {
                _currentItem.SetActive(false);
            }
            else
            {
                _currentItem.SetActive(true);
            }

            // Move the held weapon to match cursor position in world space
            _currentItem.transform.position = Camera.main.ScreenToWorldPoint(
                new Vector3(_mousePos.x, _mousePos.y, _z)
            );
        }
    }

    // Called when a weapon is selected from UI
    private void OnItemSelected(GameObject itemSelected, int cost)
    {
        // If the player already had a dragged item, remove it
        if (_currentItem != null)
            Destroy(_currentItem);

        // Calculate spawn position based on cursor location
        Vector3 spawnPos = Camera.main.ScreenToWorldPoint(
            new Vector3(_mousePos.x, _mousePos.y, _z)
        );

        // Spawn a copy of the item for dragging
        _currentItem = Instantiate(itemSelected, spawnPos, Quaternion.identity);

        // Disable colliders so the dragged item doesn't interact with the world
        foreach (Collider col in _currentItem.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
    }

    // Fired when hovering a PlaceZone for preview placement
    private void OnItemPreview()
    {
        _isPreviewing = true;
    }

    // Fired when leaving a PlaceZone
    private void OnItemNotPreview()
    {
        _isPreviewing = false;
    }

    // Called when the player cancels the placement action
    private void OnCancel()
    {
        if (_currentItem != null)
            Destroy(_currentItem);

        _isPreviewing = false;
        _currentItem = null;
    }

    // Called when the player successfully places the weapon
    private void OnItemSpawned()
    {
        Destroy(_currentItem);
    }
}
