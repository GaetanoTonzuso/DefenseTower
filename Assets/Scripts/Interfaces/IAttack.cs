using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttack
{
    public int AtkDamage { get; set; }
    public void Attack();

}
