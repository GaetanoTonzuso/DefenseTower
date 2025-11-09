using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private int _targetId;
    public int TargetId { get {  return _targetId; } }
}
