using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceZone : MonoBehaviour, IInteractable
{

    [SerializeField] private bool _canPlace = true;
    [SerializeField] private GameObject _itemPreviewPrefab;
    private GameObject _itemClone;

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

    private void OnItemSelected(GameObject weaponSelected)
    {
        _itemPreviewPrefab = weaponSelected;
    }

    public void Interact()
    {
        if (_canPlace && _itemPreviewPrefab != null)
        {
            Debug.Log("Can place tower");
            if (!_previewSpawned && _isInteracting == true)
            {
                Debug.Log("Should spawn");
                _previewSpawned = true;
                _itemClone = Instantiate(_itemPreviewPrefab,transform.position, Quaternion.identity);
                EventService.Instance.OnItemPreview.InvokeEvent();
            }
            
            //Show Tower 
            //Green

        }
        else
        {
            Debug.Log("Unable to place");
            //Show Tower
            //Red
        }
    }

    public void Activate()
    {
        _isInteracting = true;
    }

    public void Deactivate()
    {
        _isInteracting = false;
        _previewSpawned = false;
        EventService.Instance.OnItemNotPreview.InvokeEvent();
        Destroy(_itemClone,0.1f);

    }
}
