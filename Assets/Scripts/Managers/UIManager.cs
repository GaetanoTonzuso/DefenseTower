using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Weapon prices Settings")]
    [SerializeField] private Text[] _pricesTexts;
    [SerializeField] private WeaponInfo[] _weaponPrices;

    private void Start()
    {
        InitializeWeaponPrices();
    }

    private void InitializeWeaponPrices()
    {
        for (int i = 0; i < _pricesTexts.Length; i++)
        {
            int currentPrice = _weaponPrices[i].WeaponCost;
            _pricesTexts[i].text = "$" + currentPrice;
        }
    }
}
