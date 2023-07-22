using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using UnityEngine;

public class RayGazable : MonoBehaviour
{
    
    public void OnGazeEnter()
    {
        gameObject.GetComponent<Outlinable>().enabled = true;
    }
    
    public void OnGazeExit()
    {
        gameObject.GetComponent<Outlinable>().enabled = false;
    }

    public void OnGazeStay()
    {
        
    }
}
