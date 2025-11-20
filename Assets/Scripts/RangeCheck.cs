using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeCheck : MonoBehaviour
{
    private IWeapon _weapon;

    private void Start()
    {
        _weapon = transform.parent.GetComponent<IWeapon>();
        if(_weapon == null)
        {
            Debug.LogError("Weapon is null on RangeCheck");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            Debug.Log("Called Update Target");
            _weapon.UpdateTarget(other.transform.GetComponentInChildren<AimTarget>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            _weapon.UpdateEnemiesList(other.GetComponent<Enemy>());
        }
    }
}
