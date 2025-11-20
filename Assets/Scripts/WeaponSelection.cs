using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class WeaponSelection : MonoBehaviour
{
    Vector2 _mousePos;
    private GameObject _currentItem;
    private bool _isPreviewing;

    [SerializeField] private float _z = 18;

    private void OnEnable()
    {
        EventService.Instance.OnItemSelected.AddListener(OnItemSelected);
        EventService.Instance.OnItemPreview.AddListener(OnItemPreview);
        EventService.Instance.OnItemNotPreview.AddListener(OnItemNotPreview);
        EventService.Instance.OnItemSpawned.AddListener(OnItemSpawned);
        EventService.Instance.OnActionCancel.AddListener(OnCancel);
    }

    private void OnDisable()
    {
        EventService.Instance.OnItemSelected.RemoveListener(OnItemSelected);
        EventService.Instance.OnItemPreview.RemoveListener(OnItemPreview);
        EventService.Instance.OnItemNotPreview.RemoveListener(OnItemNotPreview);
        EventService.Instance.OnItemSpawned.RemoveListener(OnItemSpawned);
        EventService.Instance.OnActionCancel.RemoveListener(OnCancel);
    }

    private void Update()
    {
        _mousePos = Mouse.current.position.ReadValue();
        if (_currentItem != null)
        {
            if(_isPreviewing)
            {
                _currentItem.SetActive(false);
            }
            else
            {
                _currentItem.SetActive(true);
            }
                
           _currentItem.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(_mousePos.x, _mousePos.y, _z));
        }
    }

    private void OnItemSelected(GameObject itemSelected, int cost)
    {
        if (_currentItem != null)
            Destroy(_currentItem);
        
        Vector3 spawnPos = Camera.main.ScreenToWorldPoint(new Vector3(_mousePos.x, _mousePos.y, _z));

        _currentItem = Instantiate(itemSelected, spawnPos, Quaternion.identity);
        foreach(Collider col in _currentItem.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
    }

    private void OnItemPreview() //Inform Place Zone that we are previewing
    {
        _isPreviewing = true;       
    }

    private void OnItemNotPreview() //Inform Place Zone that we are not previewing
    {
        _isPreviewing = false;       
    }

    private void OnCancel()
    {
        if (_currentItem != null)
            Destroy(_currentItem);

        _isPreviewing = false;
        _currentItem = null;
    }

    private void OnItemSpawned()
    {
        Destroy(_currentItem);
    }
}
