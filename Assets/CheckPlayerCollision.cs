using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerCollision : MonoBehaviour
{
    void OnControllerColliderHit(ControllerColliderHit hit) //character controller collision 여기서 처리!
    {
        if(hit.gameObject.tag == "Enemy")
        {
            print("Enemy hit!!");
        }
    }
}
