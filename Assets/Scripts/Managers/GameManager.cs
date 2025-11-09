using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if(_instance == null )
            {
                Debug.LogError("Gamemaneger instance is NULL");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    private void OnEnable()
    {
        EventService.Instance.OnEnemyDie.AddListener(OnEnemyDie);
    }

    [Header("Game Settings")]
    [SerializeField] private int _lives = 5;
    public int Lives { get { return _lives; }}
    [SerializeField] private int _warfunds = 500;
    public int currentWarfunds { get {  return _warfunds; } }

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

    private void OnEnemyDie(Enemy enemy)
    {
        AddWarfunds(enemy.WarfundsCredits);
    }
}
