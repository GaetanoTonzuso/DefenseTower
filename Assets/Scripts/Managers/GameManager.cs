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

    private Coroutine _updateEnemiesRoutine;
    private WaitForSeconds _waitEnemiesUpdateSeconds = new WaitForSeconds(0.5f);
    private Enemy[] _enemies;
    private int _enemiesCount;

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

    public void InitializeEnemiesAlive()
    {
        _enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.InstanceID);
        _enemiesCount = _enemies.Length;
    }

    public void UpdateEnemiesAlive() //Check if there are other enemies in the current wave
    {
        if(_updateEnemiesRoutine == null)
        {
            _updateEnemiesRoutine = StartCoroutine(UpdateEnemiesRoutine());
        }
    }

    private void OnEnemyDie(Enemy enemy)
    {
        AddWarfunds(enemy.WarfundsCredits);
    }

    private IEnumerator UpdateEnemiesRoutine()
    {
        InitializeEnemiesAlive();
        yield return _waitEnemiesUpdateSeconds;

        if (_enemiesCount < 1)
        {
            EventService.Instance.OnWaveEnd.InvokeEvent();
        }

        _updateEnemiesRoutine = null;
    }

    private void OnPlayerHit()
    {
        UpdateEnemiesAlive();
        _lives--;

        switch(_lives)
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

        UIManager.instance.UpdateLives(_lives);
        if(_lives < 1)
        {
            SceneManager.LoadScene(0);
        }
    }

}
