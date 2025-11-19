using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private static SpawnManager _instance;
    public static SpawnManager Instance
    {
        get { return _instance; }

    }

    private void Awake()
    {
        _instance = this;
    }

    private Coroutine _waveRoutine;
    [SerializeField] private int _maxWaves = 5;
    [SerializeField] private int _enemiesToSpawn;
    [SerializeField] private int _baseEnemyCount = 2;
    private int _enemiesSpawned = 0;
    private int _currentWave = 0;
    [SerializeField] private GameObject _winPanel;

    private void OnEnable()
    {
        EventService.Instance.OnWaveEnd.AddListener(OnWaveEnd);
    }

    private void OnDisable()
    {
        EventService.Instance.OnWaveEnd.RemoveListener(OnWaveEnd);
    }

    private void Start()
    {
        if(_waveRoutine == null)
        {
            _waveRoutine = StartCoroutine(StartWave());
        }
    }

    private IEnumerator StartWave()
    {
        _enemiesSpawned = 0;
        _currentWave++;
        Debug.Log("Current Wave: " + _currentWave);
        _enemiesToSpawn = _baseEnemyCount * _currentWave;
        Debug.Log("Enemies to spawn: " + _enemiesToSpawn);

        yield return new WaitForSeconds(0.5f);
        while(_enemiesSpawned < _enemiesToSpawn)
        {
            EventService.Instance.OnWaveBegin.InvokeEvent(_currentWave,_maxWaves); //Send info to UI

            for(int i = 0; i < _enemiesToSpawn; i++)
            {
                ObjectPoolingManager.Instance.RequestEnemy();
                _enemiesSpawned++;
                GameManager.instance.InitializeEnemiesAlive();
                yield return new WaitForSeconds(Random.Range(1.5f, 6));
            }
            yield return null;
        }
            _waveRoutine = null;
    }

    private void OnWaveEnd()
    {      
        if (_waveRoutine == null)
        {
            if(_currentWave < _maxWaves)
            {
                _waveRoutine = StartCoroutine(StartWave());
            }

            else
            {
                Debug.Log("You Win");
                _winPanel.SetActive(true);
            }
        }
    }
}
