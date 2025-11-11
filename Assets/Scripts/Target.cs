using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private int _targetId;
    public int TargetId { get {  return _targetId; } }

    [SerializeField] private bool _isPlayerBase;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            other.gameObject.SetActive(false);
            EventService.Instance.OnPlayerHit.InvokeEvent();
        }
    }
}
