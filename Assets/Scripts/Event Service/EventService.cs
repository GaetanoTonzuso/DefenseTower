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

    public EventController<GameObject> OnItemSelected { get; private set; } 
    public EventController OnItemPreview { get; private set; }
    public EventController OnItemNotPreview { get; private set; }

    public EventService ()
    {
        OnItemSelected = new EventController<GameObject> ();
        OnItemPreview = new EventController();
        OnItemNotPreview = new EventController();
    }
}
