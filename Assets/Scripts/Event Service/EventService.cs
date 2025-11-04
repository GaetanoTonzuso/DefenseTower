using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventService
{
    private static EventService _instance;
    public static EventService Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new EventService();
            }
            return _instance;
        }
    }

    //Player Inputs
    public EventController OnActionPerformed { get; private set; }
    public EventController OnActionCancel { get; private set; }

    //Weapons/Items
    public EventController<GameObject,int> OnItemSelected { get; private set; } 
    public EventController OnItemPreview { get; private set; }
    public EventController OnItemNotPreview { get; private set; }
    public EventController OnItemSpawned { get; private set; }

    //Enemy
    public EventController<Enemy> OnEnemyDie { get; private set; }

    public EventService ()
    {
        OnItemSelected = new EventController<GameObject,int> ();
        OnItemPreview = new EventController();
        OnItemNotPreview = new EventController();
        OnActionPerformed = new EventController();
        OnItemSpawned = new EventController();
        OnActionCancel = new EventController();
        OnEnemyDie = new EventController<Enemy> ();
    }
}
