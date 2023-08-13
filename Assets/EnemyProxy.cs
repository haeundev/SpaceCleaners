using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProxy : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            print("player collided!");
            var mm = transform.parent.GetComponentInChildren<MonumentMonster>();
            mm.OnPlayerNear(other.gameObject);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            print("player triggered");
            var mm = transform.parent.GetComponentInChildren<MonumentMonster>();
            mm.OnPlayerNear(other.gameObject);
        }
    }
}