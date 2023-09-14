using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlIngredientRotation : MonoBehaviour
{
    private RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int layerMask = 1 << 28;
        
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 0.3f, layerMask)) //transform.position
        {
            print("Debris on top");
            gameObject.GetComponent<SimpleRotator>().enabled = true;
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
            print("nothing on top");
            gameObject.GetComponent<SimpleRotator>().enabled = false;
        }

    }
}
