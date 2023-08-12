using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonumentPlayer : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            return;
        
        
        // var enemyObj = other.gameObject;
        // var enemyComp = enemyObj.GetComponentInChildren<EnemyController>();
        // enemyComp.On
    }

    public void OnAttacked()
    {
        // the player's behaviour when attacked
        
    }
}
