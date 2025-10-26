using System.Collections;
using System.Collections.Generic;
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
    }

    private void OnDisable()
    {
        EventService.Instance.OnItemSelected.RemoveListener(OnItemSelected);
        EventService.Instance.OnItemPreview.RemoveListener(OnItemPreview);
        EventService.Instance.OnItemNotPreview.RemoveListener(OnItemNotPreview);
    }

    private void Update()
    {
        _mousePos = Mouse.current.position.ReadValue();
        if (_currentItem == null) return;
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

    private void OnItemSelected(GameObject itemSelected)
    {
        if(_currentItem != null)
        {
            Destroy(_currentItem);
            _currentItem = null;
            _currentItem = itemSelected;
            _currentItem = Instantiate(_currentItem, Camera.main.ScreenToWorldPoint(new Vector3(_mousePos.x, _mousePos.y, _z)), Quaternion.identity);
        }
        else
        {
            _currentItem = itemSelected;
            _currentItem = Instantiate(_currentItem, Camera.main.ScreenToWorldPoint(new Vector3(_mousePos.x, _mousePos.y, _z)), Quaternion.identity);
        }
    }

    private void OnItemPreview()
    {
        _isPreviewing = true;
    }

    private void OnItemNotPreview()
    {
        _isPreviewing = false;
    }
}
