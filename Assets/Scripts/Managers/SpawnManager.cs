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
    [SerializeField] private int _enemiesToSpawn = 1;
    private int _enemiesSpawned = 0;
    private int _currentWave = 0;
    private int _nextWave;

    private void Start()
    {
        //quando parte la partita
        //mentre siamo vivi deve partire con la prima ondata

        if(_waveRoutine == null)
        {
            _waveRoutine = StartCoroutine(StartWave());
        }
    }

    private IEnumerator StartWave()
    {
        yield return new WaitForSeconds(1f);
        while(_enemiesSpawned < _enemiesToSpawn)
        {
            _currentWave++;
            EventService.Instance.OnWaveBegin.InvokeEvent(_currentWave,_maxWaves); //Send info to UI

            for(int i = 0; i < _enemiesToSpawn; i++)
            {
                ObjectPoolingManager.Instance.RequestEnemy();
                _enemiesSpawned++;
                yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
            }

            yield return null;
            _waveRoutine = null;
        }
    }
}
