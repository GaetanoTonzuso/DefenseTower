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

    private Coroutine _checkWeaponDestroyed;

    private void OnEnable()
    {
        EventService.Instance.OnItemSelected.AddListener(OnItemSelected);
        EventService.Instance.OnItemSpawned.AddListener(OnItemSpawned);
        EventService.Instance.OnWeaponDestroyed.AddListener(OnWeaponDestroyed);
        EventService.Instance.OnActionCancel.AddListener(CancelAction);
    }

    private void OnDisable()
    {
        EventService.Instance.OnItemSelected.RemoveListener(OnItemSelected);
        EventService.Instance.OnItemSpawned.RemoveListener(OnItemSpawned);
        EventService.Instance.OnWeaponDestroyed.RemoveListener(OnWeaponDestroyed);
        EventService.Instance.OnActionCancel.RemoveListener(CancelAction);
    }

    private void Start()
    {
        _redPlaceEffect.Play();
    }

    //Set our Prefab for previewing when on Place
    private void OnItemSelected(GameObject weaponSelected , int itemCost)
    {
        _itemPreviewPrefab = weaponSelected;

        //Disable all sphere colliders for our Weapons
        foreach (Collider col in _itemPreviewPrefab.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

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
                
                //Instantiate weapon selected
                _itemClone = Instantiate(_itemPreviewPrefab,transform.position, Quaternion.identity);
                _itemClone.transform.SetParent(this.transform);
                              
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
        EventService.Instance.OnActionCancel.RemoveListener(CancelAction);

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
        //Enable all sphere colliders for our Weapons
        if(_itemClone != null)
        {
            foreach (Collider col in _itemClone.GetComponentsInChildren<Collider>())
            {
                if(col != null)
                col.enabled = true;
            }
            GameManager.instance.RemoveWarfunds(_currentItemCost);
            EventService.Instance.OnItemSpawned.InvokeEvent();      
            _canPlace = false;     

        }
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
        Debug.Log("Cancel Action called");
            //Cancel preview item and reset cost
            Destroy(_itemClone);
            _previewSpawned = false;
            _currentItemCost = 0;
            _itemPreviewPrefab = null;
            EventService.Instance.OnItemNotPreview.InvokeEvent();
    }

    private void OnWeaponDestroyed()
    {
        if (_checkWeaponDestroyed == null)
            _checkWeaponDestroyed = StartCoroutine(WeaponDestroyedRoutine());
    }

    //Check if weapon has been destroyed
    private IEnumerator WeaponDestroyedRoutine()
    {
        yield return null;

        IWeapon[] weapons = transform.GetComponentsInChildren<IWeapon>();

        if (weapons != null && weapons.Length > 0)
        {
            _checkWeaponDestroyed = null;
            yield break;
        }

        _canPlace = true;

        if (_greenHoverEffect != null)
            _greenHoverEffect.Stop();

        if (_redPlaceEffect != null)
            _redPlaceEffect.Play();

        _checkWeaponDestroyed = null;
    }
}
