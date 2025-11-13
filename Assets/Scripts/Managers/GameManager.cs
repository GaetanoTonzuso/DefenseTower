using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if(_instance == null )
            {
                Debug.LogError("Game Manager instance is NULL");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    [Header("Game Settings")]
    [SerializeField] private int _lives = 5;
    public int Lives { get { return _lives; }}
    [SerializeField] private int _warfunds = 500;
    public int currentWarfunds { get {  return _warfunds; } }

    private int _currentEnemiesAlive;

    private void OnEnable()
    {
        EventService.Instance.OnEnemyDie.AddListener(OnEnemyDie);
        EventService.Instance.OnPlayerHit.AddListener(OnPlayerHit);
    }

    private void OnDisable()
    {
        EventService.Instance.OnEnemyDie.RemoveListener(OnEnemyDie);
        EventService.Instance.OnPlayerHit.RemoveListener(OnPlayerHit);
    }


    private void Start()
    {
        UIManager.instance.UpdateWarfunds(_warfunds);
    }

    public void AddWarfunds(int fundsToAdd)
    {
        _warfunds += fundsToAdd;
        UIManager.instance.UpdateWarfunds(_warfunds);
    }

    public void RemoveWarfunds(int fundsToRemove)
    {
        _warfunds -= fundsToRemove;
        UIManager.instance.UpdateWarfunds(_warfunds);
    }

    public void InitializeEnemiesAlive(int currentEnemies)
    {
        _currentEnemiesAlive = currentEnemies;
    }

    public void UpdateEnemiesAlive() //Check if there are other enemies in the current wave
    {
        _currentEnemiesAlive--;
        Debug.Log("Current enemies: " + _currentEnemiesAlive);
        if(_currentEnemiesAlive < 1)
        {
            Debug.Log("Enemies killed");
            EventService.Instance.OnWaveEnd.InvokeEvent();
        }
    }

    private void OnEnemyDie(Enemy enemy)
    {
        AddWarfunds(enemy.WarfundsCredits);
    }

    private void OnPlayerHit()
    {
        UpdateEnemiesAlive();
        _lives--;
        UIManager.instance.UpdateLives(_lives);
        if(_lives < 1)
        {
            SceneManager.LoadScene(0);
        }
    }
}
