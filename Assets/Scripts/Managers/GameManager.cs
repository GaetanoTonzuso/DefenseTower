using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton instance for global access
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            // Safety check (mostly for debugging)
            if (_instance == null)
            {
                Debug.LogError("Game Manager instance is NULL");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // Assign singleton
        _instance = this;
    }

    [Header("Game Settings")]
    [SerializeField] private int _lives = 5;     // Player starting lives
    public int Lives { get { return _lives; } }

    [SerializeField] private int _warfunds = 500; // Starting warfunds/currency
    public int currentWarfunds { get { return _warfunds; } }

    // Updating enemies alive logic
    private Coroutine _updateEnemiesRoutine;
    private WaitForSeconds _waitEnemiesUpdateSeconds = new WaitForSeconds(0.5f);
    private Enemy[] _enemies;                                    // Cached enemy list
    private int _enemiesCount;                                   // Total enemies alive

    private void OnEnable()
    {
        // Listen for enemy kill and player hit events
        EventService.Instance.OnEnemyDie.AddListener(OnEnemyDie);
        EventService.Instance.OnPlayerHit.AddListener(OnPlayerHit);
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        EventService.Instance.OnEnemyDie.RemoveListener(OnEnemyDie);
        EventService.Instance.OnPlayerHit.RemoveListener(OnPlayerHit);
    }

    private void Start()
    {
        // Initialize UI warfunds display
        UIManager.instance.UpdateWarfunds(_warfunds);
    }

    // Adds currency and updates UI
    public void AddWarfunds(int fundsToAdd)
    {
        _warfunds += fundsToAdd;
        UIManager.instance.UpdateWarfunds(_warfunds);
    }

    // Removes currency and updates UI
    public void RemoveWarfunds(int fundsToRemove)
    {
        _warfunds -= fundsToRemove;
        UIManager.instance.UpdateWarfunds(_warfunds);
    }

    // Updates how many enemies are currently alive in the scene
    public void InitializeEnemiesAlive()
    {
        _enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.InstanceID);
        _enemiesCount = _enemies.Length;
    }

    // Starts checking if a wave is complete
    public void UpdateEnemiesAlive()
    {
        if (_updateEnemiesRoutine == null)
        {
            _updateEnemiesRoutine = StartCoroutine(UpdateEnemiesRoutine());
        }
    }

    // Called when an enemy dies
    private void OnEnemyDie(Enemy enemy)
    {
        // Reward player with warfunds
        AddWarfunds(enemy.WarfundsCredits);
    }

    // Routine to check if wave is finished
    private IEnumerator UpdateEnemiesRoutine()
    {
        InitializeEnemiesAlive();
        yield return _waitEnemiesUpdateSeconds;

        // If no enemies remain, signal wave end
        if (_enemiesCount < 1)
        {
            EventService.Instance.OnWaveEnd.InvokeEvent();
        }

        _updateEnemiesRoutine = null;
    }

    // Called when the player takes damage
    private void OnPlayerHit()
    {
        UpdateEnemiesAlive();

        _lives--;

        // Update UI status message based on remaining lives
        switch (_lives)
        {
            case 5:
                UIManager.instance.UpdateStatus("Good");
                break;

            case 3:
                UIManager.instance.UpdateStatus("Damaged");
                break;

            case 2:
                UIManager.instance.UpdateStatus("Critical");
                break;
        }

        // Update UI with new life count
        UIManager.instance.UpdateLives(_lives);

        // Player dies → restart scene
        if (_lives < 1)
        {
            SceneManager.LoadScene(0);
        }
    }
}
