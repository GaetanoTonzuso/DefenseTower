using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventService
{
    // Singleton instance for global event access
    private static EventService _instance;
    public static EventService Instance
    {
        get
        {
            // Lazy initialization of the event service
            if (_instance == null)
            {
                _instance = new EventService();
            }
            return _instance;
        }
    }

    // ------------------------
    // PLAYER INPUT EVENTS
    // ------------------------

    public EventController OnActionPerformed { get; private set; }   // Called when the player confirms an action (e.g., place weapon)
    public EventController OnActionCancel { get; private set; }      // Called when the player cancels an action

    // ------------------------
    // WEAPONS / ITEMS EVENTS
    // ------------------------

    public EventController<GameObject, int> OnItemSelected { get; private set; } // Triggered when a weapon/item is selected (sends prefab and cost)
    public EventController OnItemPreview { get; private set; }                  // Triggered when a preview item appears on a placement zone
    public EventController OnItemNotPreview { get; private set; }               // Triggered when preview is removed
    public EventController OnItemSpawned { get; private set; }                  // Triggered when an item is placed/spawned
    public EventController OnWeaponDestroyed { get; private set; }              // Triggered when a weapon is destroyed
    public EventController<Transform, GameObject> OnUpdateWeapon { get; private set; } // Used to update a weapon at a target position

    // ------------------------
    // ENEMY EVENTS
    // ------------------------

    public EventController<Enemy> OnEnemyDie { get; private set; }   // Triggered when an enemy dies (passes enemy reference)
    public EventController OnPlayerHit { get; private set; }         // Triggered when the player takes damage

    // ------------------------
    // WAVE EVENTS
    // ------------------------

    public EventController<int, int> OnWaveBegin { get; private set; } // Triggered at the start of a wave (currentWave, maxWaves)
    public EventController OnWaveEnd { get; private set; }            // Triggered when all enemies in a wave are defeated

    // Constructor initializes all event controllers
    public EventService()
    {
        OnItemSelected = new EventController<GameObject, int>();
        OnItemPreview = new EventController();
        OnItemNotPreview = new EventController();
        OnActionPerformed = new EventController();
        OnItemSpawned = new EventController();
        OnActionCancel = new EventController();
        OnEnemyDie = new EventController<Enemy>();
        OnWaveBegin = new EventController<int, int>();
        OnPlayerHit = new EventController();
        OnWaveEnd = new EventController();
        OnWeaponDestroyed = new EventController();
        OnUpdateWeapon = new EventController<Transform, GameObject>();
    }
}
