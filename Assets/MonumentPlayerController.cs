using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonumentPlayerController : MonoBehaviour
{


    private void OnCollisionEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            return;


        // var enemyObj = other.gameObject;
        // var enemyComp = enemyObj.GetComponentInChildren<EnemyController>();
        // enemyComp.On
    }
}
