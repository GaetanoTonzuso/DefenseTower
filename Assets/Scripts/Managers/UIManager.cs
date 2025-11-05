using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = this;
    }

    [Header("Weapon prices Settings")]
    [SerializeField] private Text[] _pricesTexts;
    [SerializeField] private WeaponInfo[] _weaponPrices;
    [SerializeField] private Text _currentWarfunds;

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

    public void UpdateWarfunds(int warfunds)
    {
        _currentWarfunds.text = warfunds.ToString();
    }
}
