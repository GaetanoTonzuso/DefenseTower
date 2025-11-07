using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager : MonoBehaviour
{
    private static ObjectPoolingManager _instance;
    public static ObjectPoolingManager Instance
        { get { return _instance; } }

    private void Awake()
    {
        _instance = this;
    }

    private int _startingEnemies = 10;
    [Header("Pool Settings")]
    [SerializeField] private GameObject _enemyPrefab; //Make this as Array if we want multiple Enemies Prefabs
    [SerializeField] private Transform _spawnPos;
    [SerializeField] private GameObject _container;
    [SerializeField] private List<GameObject> _enemiesPool;

    private void Start()
    {
        GenerateNewEnemies(_startingEnemies);
    }

    public List<GameObject> GenerateNewEnemies(int amountOfenemies)
    {
        for (int i = 0; i < amountOfenemies; i++)
        {
            GameObject enemyClone = Instantiate(_enemyPrefab, _spawnPos.position, Quaternion.identity);
            enemyClone.transform.SetParent(_container.transform);
            enemyClone.SetActive(false);
            _enemiesPool.Add(enemyClone);
        }

        return null;
    }

    public GameObject RequestEnemy()
    {
        for (int i = 0; i < _enemiesPool.Count; i++)
        {
            if (_enemiesPool[i].activeInHierarchy == false)
            {
                _enemiesPool[i].SetActive(true);
                return _enemiesPool[i];
            }
        }
        GameObject newEnemy = Instantiate(_enemyPrefab, _spawnPos.position, Quaternion.identity);
        newEnemy.transform.SetParent(_container.transform);
        _enemiesPool.Add(newEnemy);

        return newEnemy;
    }

    public Transform OnActive()
    {
        return _spawnPos;
    }
}
