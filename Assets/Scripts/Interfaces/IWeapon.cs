using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    public void UpdateTarget(AimTarget target);
    public void UpdateEnemiesList(Enemy enemy);
}
