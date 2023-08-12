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
            var mm = transform.parent.GetComponentInChildren<MonumentMonster>();
            mm.OnPlayerNear(other.gameObject);
        }
    }
}