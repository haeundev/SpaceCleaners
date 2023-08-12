using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckPlayerCollision : MonoBehaviour
{
    public event Action<int> OnPlayerDamaged;


    private void OnCollisionEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            return;

        OnPlayerDamaged?.Invoke(20);

        // var enemyObj = other.gameObject;
        // var enemyComp = enemyObj.GetComponentInChildren<EnemyController>();
        // enemyComp.On
    }
}
