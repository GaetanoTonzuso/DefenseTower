using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // Singleton instance for easy global access
    private static SpawnManager _instance;
    public static SpawnManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        // Assign the Singleton instance
        _instance = this;
    }

    private Coroutine _waveRoutine;

    [SerializeField] private int _maxWaves = 5;         // Total number of waves
    [SerializeField] private int _enemiesToSpawn;       // Number of enemies to spawn in the current wave
    [SerializeField] private int _baseEnemyCount = 2;   // Base count used to calculate enemies per wave
    [SerializeField] private GameObject _winPanel;      // Win screen UI
    private int _currentWave = 0;                       // Current wave index
    private int _enemiesSpawned = 0;                    // How many enemies have been spawned so far

    private void OnEnable()
    {
        // Subscribe to the event triggered when a wave ends
        EventService.Instance.OnWaveEnd.AddListener(OnWaveEnd);
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        EventService.Instance.OnWaveEnd.RemoveListener(OnWaveEnd);
    }

    private void Start()
    {
        // Start the first wave if no routine is running
        if (_waveRoutine == null)
        {
            _waveRoutine = StartCoroutine(StartWave());
        }
    }

    private IEnumerator StartWave()
    {
        _enemiesSpawned = 0;
        _currentWave++;

        // Calculate how many enemies this wave should spawn
        _enemiesToSpawn = _baseEnemyCount * _currentWave;

        // Small delay before spawning begins
        yield return new WaitForSeconds(0.5f);

        // Spawn enemies until we reach the required amount
        while (_enemiesSpawned < _enemiesToSpawn)
        {
            // Notify UI that a new wave has begun
            EventService.Instance.OnWaveBegin.InvokeEvent(_currentWave, _maxWaves);

            for (int i = 0; i < _enemiesToSpawn; i++)
            {
                // Request an enemy from the object pool
                ObjectPoolingManager.Instance.RequestEnemy();

                _enemiesSpawned++;

                // Update GameManager enemy counter
                GameManager.instance.InitializeEnemiesAlive();

                // Random delay between spawns
                yield return new WaitForSeconds(Random.Range(1.5f, 6));
            }

            yield return null;
        }

        // Reset routine so OnWaveEnd can start a new one
        _waveRoutine = null;
    }

    private void OnWaveEnd()
    {
        // Only trigger if the wave routine has finished
        if (_waveRoutine == null)
        {
            if (_currentWave < _maxWaves)
            {
                // Start next wave
                _waveRoutine = StartCoroutine(StartWave());
            }
            else
            {
                // All waves completed → player wins
                _winPanel.SetActive(true);
            }
        }
    }
}
