using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponInfo : MonoBehaviour , IPointerClickHandler
{
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private int _cost = 250;
    public int WeaponCost { get { return _cost; } }

    public void OnPointerClick(PointerEventData eventData)
    {
        EventService.Instance.OnItemSelected.InvokeEvent(_itemPrefab,_cost);
    }
}
