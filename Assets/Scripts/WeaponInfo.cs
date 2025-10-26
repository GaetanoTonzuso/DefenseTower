using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponInfo : MonoBehaviour , IPointerClickHandler
{
    [SerializeField] private GameObject _itemPrefab;

    public void OnPointerClick(PointerEventData eventData)
    {
        EventService.Instance.OnItemSelected.InvokeEvent(_itemPrefab);
    }
}
