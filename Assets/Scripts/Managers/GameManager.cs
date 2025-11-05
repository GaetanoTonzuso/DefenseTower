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
                _instance = new GameManager();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    [Header("Game Settings")]
    [SerializeField] private int _lives = 20;
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
}
