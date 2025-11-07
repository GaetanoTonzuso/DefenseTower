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

   
}
