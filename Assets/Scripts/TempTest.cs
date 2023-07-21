using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class TempTest : MonoBehaviour
{
    public bool boosting = false;

    Rigidbody rb;
    private Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if(boosting)
        {
            rb.AddForce(mainCam.transform.forward * 2000 * Time.deltaTime);
        }
        
    }

    #region Input Methods
    public void OnBoost(InputAction.CallbackContext context)
    {
        boosting = context.performed;
        Debug.Log("Left Boost pressed!!!");

    }
    #endregion
}
