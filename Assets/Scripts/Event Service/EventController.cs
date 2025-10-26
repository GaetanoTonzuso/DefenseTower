using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController
{
    public Action baseAction;

    public void AddListener(Action listener)
    {
        baseAction += listener;
    }

    public void RemoveListener(Action listener)
    {
        baseAction -= listener;
    }

    public void InvokeEvent()
    {
        baseAction?.Invoke();
    }
}

public class EventController<T>
{
    public Action<T> baseAction;

    public void AddListener(Action<T> listener)
    {
        baseAction += listener;
    }

    public void RemoveListener(Action<T> listener)
    {
        baseAction -= listener;
    }

    public void InvokeEvent(T type)
    {
        baseAction?.Invoke(type);
    }
}
