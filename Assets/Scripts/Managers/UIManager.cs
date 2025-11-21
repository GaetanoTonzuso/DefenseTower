using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Singleton instance for global UI access
    private static UIManager _instance;
    public static UIManager instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        // Assign singleton instance
        _instance = this;
    }

    private void OnEnable()
    {
        // Listen for wave start event
        EventService.Instance.OnWaveBegin.AddListener(OnWaveBegin);
    }

    private void OnDisable()
    {
        // Stop listening to avoid memory leaks
        EventService.Instance.OnWaveBegin.RemoveListener(OnWaveBegin);
    }

    [Header("Lives")]
    [SerializeField] private Text _livesText;    // Displays player lives
    [SerializeField] private Text _status;       // Displays status messages

    [Header("Weapon prices Settings")]
    [SerializeField] private Text[] _pricesTexts;     // UI text elements for weapon prices
    [SerializeField] private WeaponInfo[] _weaponPrices; // ScriptableObjects holding weapon costs
    [SerializeField] private Text _currentWarfunds;   // Displays current warfunds

    [Header("Wave UI")]
    [SerializeField] private Text _currentWaveText;   // Displays current wave progression

    private void Start()
    {
        // Initialize UI with current game data
        UpdateLives(GameManager.instance.Lives);
        InitializeWeaponPrices();
        UpdateWarfunds(GameManager.instance.currentWarfunds);
    }

    // Sets the weapon price text for each UI slot
    private void InitializeWeaponPrices()
    {
        for (int i = 0; i < _pricesTexts.Length; i++)
        {
            int currentPrice = _weaponPrices[i].WeaponCost;
            _pricesTexts[i].text = "$" + currentPrice;
        }
    }

    // Updates the displayed amount of warfunds
    public void UpdateWarfunds(int warfunds)
    {
        _currentWarfunds.text = warfunds.ToString();
    }

    // Updates the wave text when a new wave begins
    private void OnWaveBegin(int currentWave, int maxWaves)
    {
        _currentWaveText.text = currentWave.ToString() + " / " + maxWaves.ToString();
    }

    // Updates the UI text that shows player lives
    public void UpdateLives(int lives)
    {
        _livesText.text = lives.ToString();
    }

    // Controls game speed (0 = pause, 1 = normal, 2 = fast)
    public void GameSpeed(int value)
    {
        switch (value)
        {
            case 0:
                Time.timeScale = 0;
                break;

            case 1:
                Time.timeScale = 1;
                break;

            case 2:
                Time.timeScale = 2;
                break;

            default:
                Time.timeScale = 1;
                break;
        }
    }

    // Reloads the first scene, effectively restarting the game
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    // Updates a generic status message (e.g., "Wave Incoming", "You Win")
    public void UpdateStatus(string status)
    {
        _status.text = status;
    }
}
