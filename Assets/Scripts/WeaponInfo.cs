using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponInfo : MonoBehaviour, IPointerClickHandler
{
    // Prefab of the weapon/item associated with this UI button
    [SerializeField] private GameObject _itemPrefab;

    // Cost of the weapon (displayed in UI and checked before placement)
    [SerializeField] private int _cost = 250;

    // Public read-only access to weapon cost
    public int WeaponCost { get { return _cost; } }

    // Called when the UI element is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        // Notify WeaponSelection and other systems that an item was selected,
        // sending the prefab and its cost through the event system
        EventService.Instance.OnItemSelected.InvokeEvent(_itemPrefab, _cost);
    }
}
