using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CheckAboveItem : MonoBehaviour
{
    private RaycastHit hit;
    public Transform spawnPoint;

    void Start()
    {
        
    }

    void Update()
    {
        int layerMask = 1 << 15;

        // if (Physics.SphereCast(spawnPoint.position, 2f, 0.1f, out hit, layerMask))
        // {
        //     print("something above");
        // }
        // else
        // {
        //     print("nothing above");
        // }
        
        
        if (Physics.Raycast(spawnPoint.position, Vector3.up, out hit, 0.2f, layerMask)) //transform.position
        {
            //print("Debris on top");
            // hit.transform.gameObject.GetComponent<SimpleRotator>().enabled = true;
            // if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Debris"))
            // {
            //     print("Debris on top");
            //     hit.transform.gameObject.GetComponent<SimpleRotator>().enabled = true;
            // }
            // else
            // {
            //     print("Something on top");
            // }
            // if (hit.point == gameObject.tag("IngredientHolder"))
            // {
            //     print("Above");
            // }
        }
        else
        {
            //print("nothing on top");
            // hit.transform.gameObject.GetComponent<SimpleRotator>().enabled = false;
        }
    }
}
