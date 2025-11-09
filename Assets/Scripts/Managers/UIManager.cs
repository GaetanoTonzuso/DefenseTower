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

    private void OnEnable()
    {
        EventService.Instance.OnWaveBegin.AddListener(OnWaveBegin);
        
    }

    private void OnDisable()
    {
        EventService.Instance.OnWaveBegin.RemoveListener(OnWaveBegin);
    }

    [Header("Lives")]
    [SerializeField] private Text _livesText;

    [Header("Weapon prices Settings")]
    [SerializeField] private Text[] _pricesTexts;
    [SerializeField] private WeaponInfo[] _weaponPrices;
    [SerializeField] private Text _currentWarfunds;

    [Header("Wave UI")]
    [SerializeField] private Text _currentWaveText;

    private void Start()
    {
        UpdateLives(GameManager.instance.Lives);
        InitializeWeaponPrices();
        UpdateWarfunds(GameManager.instance.currentWarfunds);
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
    
    private void OnWaveBegin(int currentWave, int maxWaves)
    {
        _currentWaveText.text = currentWave.ToString() + " / " + maxWaves.ToString();
    }

    private void UpdateLives(int lives)
    {
        _livesText.text = lives.ToString();
    }
}
