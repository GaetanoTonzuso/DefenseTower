using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceZone : MonoBehaviour, IInteractable
{
    // Prefab shown when hovering the zone
    [SerializeField] private GameObject _itemPreviewPrefab;

    // Visual feedback effects
    [SerializeField] private ParticleSystem _greenHoverEffect;   // Valid placement indicator
    [SerializeField] private ParticleSystem _redPlaceEffect;     // Invalid placement indicator

    private GameObject _itemClone;   // The preview or placed instance
    private int _currentItemCost;

    private bool _previewSpawned = false; // Has a preview already been instantiated?
    private bool _isInteracting;          // Is the player currently hovering this place zone?
    private bool _canPlace = true;        // Can a weapon be placed here?

    private Coroutine _checkWeaponDestroyed;

    private void OnEnable()
    {
        // Subscribe to global events
        EventService.Instance.OnItemSelected.AddListener(OnItemSelected);
        EventService.Instance.OnItemSpawned.AddListener(OnItemSpawned);
        EventService.Instance.OnWeaponDestroyed.AddListener(OnWeaponDestroyed);
        EventService.Instance.OnActionCancel.AddListener(CancelAction);
    }

    private void OnDisable()
    {
        // Remove event listeners
        EventService.Instance.OnItemSelected.RemoveListener(OnItemSelected);
        EventService.Instance.OnItemSpawned.RemoveListener(OnItemSpawned);
        EventService.Instance.OnWeaponDestroyed.RemoveListener(OnWeaponDestroyed);
        EventService.Instance.OnActionCancel.RemoveListener(CancelAction);
    }

    private void Start()
    {
        // Start with red effect indicating no placement yet
        _redPlaceEffect.Play();
    }

    /// <summary>
    /// Called when the player selects a weapon to place.
    /// Sets the preview prefab and disables its colliders.
    /// </summary>
    private void OnItemSelected(GameObject weaponSelected, int itemCost)
    {
        _itemPreviewPrefab = weaponSelected;

        // Disable colliders so preview doesn't collide with environment
        foreach (Collider col in _itemPreviewPrefab.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        _currentItemCost = itemCost;
    }

    /// <summary>
    /// Called every frame the player interacts (raycast hit).
    /// Instantiates a preview if needed.
    /// </summary>
    public void Interact()
    {
        if (_canPlace && _itemPreviewPrefab != null)
        {
            // Spawn preview only once while interacting
            if (!_previewSpawned && _isInteracting)
            {
                if (GameManager.instance.currentWarfunds >= _currentItemCost)
                {
                    // Listen for confirm action (player pressing the action button)
                    EventService.Instance.OnActionPerformed.AddListener(SpawnWeapon);
                }
                else
                {
                    Debug.Log("Not enough funds");
                }

                _previewSpawned = true;

                // Create the preview clone
                _itemClone = Instantiate(_itemPreviewPrefab, transform.position, Quaternion.identity);
                _itemClone.transform.SetParent(this.transform);

                // Notify UI that a preview is active
                EventService.Instance.OnItemPreview.InvokeEvent();
            }
        }
    }

    /// <summary>
    /// Called when the player enters the zone (hover).
    /// Shows appropriate placement effects.
    /// </summary>
    public void Activate()
    {
        if (!_canPlace) return;

        _isInteracting = true;
        _redPlaceEffect.Stop();

        // Check affordability and show correct VFX
        if (GameManager.instance.currentWarfunds >= _currentItemCost && _itemPreviewPrefab != null)
        {
            _greenHoverEffect.Play();
        }
        else
        {
            _greenHoverEffect.Stop();
            _redPlaceEffect.Play();
        }

        // Listen for cancel action
        EventService.Instance.OnActionCancel.AddListener(CancelAction);
    }

    /// <summary>
    /// Called when the player stops hovering this zone.
    /// Removes preview and resets effects.
    /// </summary>
    public void Deactivate()
    {
        // Remove listeners
        EventService.Instance.OnActionPerformed.RemoveListener(SpawnWeapon);
        EventService.Instance.OnActionCancel.RemoveListener(CancelAction);

        // Notify UI
        EventService.Instance.OnItemNotPreview.InvokeEvent();

        _isInteracting = false;
        _previewSpawned = false;

        if (!_canPlace) return;

        // Reset effects
        _greenHoverEffect.Stop();
        _redPlaceEffect.Play();

        // Remove the preview clone
        Destroy(_itemClone, 0.1f);
    }

    /// <summary>
    /// Finalizes the weapon placement.
    /// </summary>
    private void SpawnWeapon()
    {
        if (_itemClone != null)
        {
            // Enable the weapon's colliders for normal behavior
            foreach (Collider col in _itemClone.GetComponentsInChildren<Collider>())
            {
                if (col != null)
                    col.enabled = true;
            }

            // Deduct currency
            GameManager.instance.RemoveWarfunds(_currentItemCost);

            // Notify system that an item was placed
            EventService.Instance.OnItemSpawned.InvokeEvent();

            _canPlace = false; // This zone now has an item
        }
    }

    /// <summary>
    /// Called when an item finishes spawning.
    /// Clears preview data.
    /// </summary>
    private void OnItemSpawned()
    {
        _previewSpawned = false;
        _currentItemCost = 0;
        _itemPreviewPrefab = null;
        _itemClone = null;
    }

    /// <summary>
    /// Cancels the current placement preview.
    /// </summary>
    private void CancelAction()
    {
        Destroy(_itemClone);
        _previewSpawned = false;
        _currentItemCost = 0;
        _itemPreviewPrefab = null;

        // Notify UI
        EventService.Instance.OnItemNotPreview.InvokeEvent();
    }

    /// <summary>
    /// Called when a weapon on this zone is destroyed.
    /// </summary>
    private void OnWeaponDestroyed()
    {
        if (_checkWeaponDestroyed == null)
            _checkWeaponDestroyed = StartCoroutine(WeaponDestroyedRoutine());
    }

    /// <summary>
    /// Checks if the zone is empty after a weapon is destroyed.
    /// </summary>
    private IEnumerator WeaponDestroyedRoutine()
    {
        yield return null;

        IWeapon[] weapons = transform.GetComponentsInChildren<IWeapon>();

        // If a weapon still exists, stop here
        if (weapons != null && weapons.Length > 0)
        {
            _checkWeaponDestroyed = null;
            yield break;
        }

        // Zone is now free
        _canPlace = true;

        // Reset visual effects
        _greenHoverEffect?.Stop();
        _redPlaceEffect?.Play();

        _checkWeaponDestroyed = null;
    }
}
