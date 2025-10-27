using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceZone : MonoBehaviour, IInteractable
{

    [SerializeField] private bool _canPlace = true;
    [SerializeField] private GameObject _itemPreviewPrefab;
    [SerializeField] private ParticleSystem _greenHoverEffect;
    [SerializeField] private ParticleSystem _redPlaceEffect;

    private GameObject _itemClone;
    private int _currentItemCost;

    private bool _previewSpawned = false;
    private bool _isInteracting;

    private void OnEnable()
    {
        EventService.Instance.OnItemSelected.AddListener(OnItemSelected);
    }

    private void OnDisable()
    {
        EventService.Instance.OnItemSelected.RemoveListener(OnItemSelected);
    }

    private void Start()
    {
        _redPlaceEffect.Play();
    }

    private void OnItemSelected(GameObject weaponSelected , int itemCost)
    {
        _itemPreviewPrefab = weaponSelected;
        _currentItemCost = itemCost;
    }

    public void Interact()
    {
        if (_canPlace && _itemPreviewPrefab != null)
        {
            if (!_previewSpawned && _isInteracting == true)
            {
                _previewSpawned = true;
                _itemClone = Instantiate(_itemPreviewPrefab,transform.position, Quaternion.identity);
                //Send signal to WeaponSelection that the place is previewing the Tower/Weapon
                EventService.Instance.OnItemPreview.InvokeEvent();
            }
        }
    }

    public void Activate()
    {
        _isInteracting = true;
        _redPlaceEffect.Stop();
        if(GameManager.instance.currentWarfunds >= _currentItemCost && _itemPreviewPrefab != null)
        {
            _redPlaceEffect.Stop();
            _greenHoverEffect.Play();
        }
        else
        {
            _greenHoverEffect.Stop();
            _redPlaceEffect.Play();
        }
    }

    public void Deactivate()
    {
        _isInteracting = false;
        _previewSpawned = false;
        _greenHoverEffect.Stop();
        _redPlaceEffect.Play();
        EventService.Instance.OnItemNotPreview.InvokeEvent();
        Destroy(_itemClone,0.1f);

    }
}
