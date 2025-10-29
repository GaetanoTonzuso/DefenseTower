using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceZone : MonoBehaviour, IInteractable
{

    [SerializeField] private GameObject _itemPreviewPrefab;
    [SerializeField] private ParticleSystem _greenHoverEffect;
    [SerializeField] private ParticleSystem _redPlaceEffect;

    private GameObject _itemClone;
    private int _currentItemCost;

    private bool _previewSpawned = false;
    private bool _isInteracting;
    private bool _canPlace = true;

    private void OnEnable()
    {
        EventService.Instance.OnItemSelected.AddListener(OnItemSelected);
        EventService.Instance.OnItemSpawned.AddListener(OnItemSpawned);
    }

    private void OnDisable()
    {
        EventService.Instance.OnItemSelected.RemoveListener(OnItemSelected);
        EventService.Instance.OnItemSpawned.RemoveListener(OnItemSpawned);
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

    //When we are interacting (Raycast script)
    public void Interact()
    {
        if (_canPlace && _itemPreviewPrefab != null)
        {
            if (!_previewSpawned && _isInteracting == true)
            {
                if(GameManager.instance.currentWarfunds >= _currentItemCost && _itemPreviewPrefab != null)
                {
                    EventService.Instance.OnActionPerformed.AddListener(SpawnWeapon);
                }
                else
                {
                    Debug.Log("Not funds");
                }
                _previewSpawned = true;
                _itemClone = Instantiate(_itemPreviewPrefab,transform.position, Quaternion.identity);

                //Send signal to WeaponSelection that the place is previewing the Tower/Weapon
                EventService.Instance.OnItemPreview.InvokeEvent();
            }
        }
    }

    public void Activate()
    {
        if (!_canPlace) return;       

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

        if(_isInteracting)
        {
            EventService.Instance.OnActionCancel.AddListener(CancelAction);
        }
    }

    public void Deactivate()
    {
        EventService.Instance.OnActionPerformed.RemoveListener(SpawnWeapon);
        EventService.Instance.OnActionCancel.AddListener(CancelAction);

        //Send signal to Weapon Selection script
        EventService.Instance.OnItemNotPreview.InvokeEvent();
        _isInteracting = false;
        _previewSpawned = false;

        if (!_canPlace) return;
        _greenHoverEffect.Stop();
        _redPlaceEffect.Play();
        Destroy(_itemClone,0.1f);
    }

    private void SpawnWeapon()
    {
        Debug.Log("Placed");
        //Se clicco mi deve togliere l arma selezionata e lasciare 
        GameManager.instance.RemoveWarfunds(_currentItemCost);
        EventService.Instance.OnItemSpawned.InvokeEvent();
        _canPlace = false;     
    }

    private void OnItemSpawned()
    {
        _previewSpawned = false;
        _currentItemCost = 0;
        _itemPreviewPrefab = null;
        _itemClone = null;
    }

    private void CancelAction()
    {
        //Cancel preview item and reset cost
        _previewSpawned = false;
        _currentItemCost = 0;
        _itemPreviewPrefab = null;
        _itemClone = null;
        EventService.Instance.OnItemSpawned.InvokeEvent();
    }
}
