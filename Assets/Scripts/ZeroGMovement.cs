using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(Rigidbody))]
public class ZeroGMovement : MonoBehaviour
{
    [Header("=== Player Movement Settings ===")]
    [SerializeField]
    private float rollTorque = 1000f;
    [SerializeField]
    private float thrust = 100f;
    [SerializeField]
    private float upThrust = 50f;
    [SerializeField]
    private float strafeThrust = 50f;

    private Camera mainCam;

    [Header("=== Boost Settings ===")]
    [SerializeField]
    private float maxBoostAmount = 2f; //how big the boost tank is
    [SerializeField]
    private float boostDeprecationRate = 0.25f; //how quickly the tank is depleted while player is holding the boost button
    [SerializeField]
    private float boostRechargeRate = 0.5f; //how quickly it refills while not pressing the button
    [SerializeField]
    private float boostMultiplier = 5f; //how fast the boost actually makes the player go
    public bool boosting = false;
    public float currentBoostAmount;

    [SerializeField, Range(0.001f, 0.999f)]
    private float thrustGlideReduction = 0.999f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float upDownGlideReduction = 0.111f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float leftRightGlideReduction = 0.111f;
    float glide, verticalGlide, horizontalGlide = 0f;


    Rigidbody rb;

    //Input Values
    private float thrust1D;
    private float upDown1D;
    private float strafe1D;
    private float roll1D;
    private Vector2 pitchYaw;


    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        currentBoostAmount = maxBoostAmount;
    }

    void FixedUpdate()
    {
        HandleBoosting();
        HandleMovement();
    }

    void HandleBoosting()
    {
        if(boosting && currentBoostAmount > 0f) //if we have boost left in the tank
        {
            currentBoostAmount -= boostDeprecationRate;
            if(currentBoostAmount <= 0f)
            {
                boosting = false;
            }
        }
        else
        {
            if(currentBoostAmount < maxBoostAmount) //when not boosting replenish
            {
                currentBoostAmount += boostRechargeRate;
            }
        }
    }

    void HandleMovement()
    {
        //Roll
        rb.AddTorque(-mainCam.transform.forward * roll1D * rollTorque * Time.deltaTime); //transform.back가 없기 때문에 -transform.forward
        // //Pitch
        // rb.AddRelativeTorque(Vector3.right * Mathf.Clamp(-pitchYaw.y, -1f, 1f) * pitchTorque * Time.deltaTime); //up을 했을 때 nose of the ship도 up으로 가야 하므로
        // //Yaw
        // rb.AddRelativeTorque(Vector3.up * Mathf.Clamp(pitchYaw.x, -1f, 1f) * yawTorque * Time.deltaTime);//.x is going left and right. .y is going up and down

        // //Thrust
        // if(thrust1D > 0.1f || thrust1D < -0.1f) //checking if we're pressing the stick
        // {
        //     // float currentThrust = thrust;

        //     rb.AddRelativeForce(Vector3.forward * thrust1D * thrust * Time.fixedDeltaTime); //currentThrust come in handy when doing boosting
        //     glide = thrust1D * thrust;

        // }
        // else //not pressing anything
        // {
        //     rb.AddRelativeForce(Vector3.forward * glide * Time.fixedDeltaTime); //eventually glide will be 0 and won't be moving
        //     glide *= thrustGlideReduction;
        // }

        //Thrust. take in consideration boosting
        if(thrust1D > 0.1f || thrust1D < -0.1f) //checking if we're pressing the stick
        {
            float currentThrust;

            if(boosting)
            {
                currentThrust = thrust * boostMultiplier;
            }
            else
            {
                currentThrust = thrust;
            }



            rb.AddForce(mainCam.transform.forward * thrust1D * currentThrust * Time.deltaTime); //currentThrust come in handy when doing boosting
            glide = thrust;

        }
        else //not pressing anything
        {
            rb.AddForce(mainCam.transform.forward * glide * Time.deltaTime); //eventually glide will be 0 and won't be moving
            glide *= thrustGlideReduction;
        }

        //Up/Down
        if(upDown1D > 0.1f || upDown1D < -0.1f) //checking if we're pressing the stick
        {
            rb.AddRelativeForce(Vector3.up * upDown1D * upThrust * Time.fixedDeltaTime); //currentThrust come in handy when doing boosting
            verticalGlide = upDown1D * upThrust;

        }
        else //not pressing anything
        {
            rb.AddRelativeForce(Vector3.up * verticalGlide * Time.fixedDeltaTime); //eventually glide will be 0 and won't be moving
            verticalGlide *= upDownGlideReduction;
        }

        //Strafing
        if(strafe1D > 0.1f || strafe1D < -0.1f) //checking if we're pressing the stick
        {
            rb.AddForce(mainCam.transform.right * strafe1D * strafeThrust * Time.fixedDeltaTime); //currentThrust come in handy when doing boosting
            horizontalGlide = strafe1D * strafeThrust;

        }
        else //not pressing anything
        {
            rb.AddForce(mainCam.transform.right * horizontalGlide * Time.fixedDeltaTime); //eventually glide will be 0 and won't be moving
            horizontalGlide *= leftRightGlideReduction;
        }
    }

    #region Input Methods
    public void OnThrust(InputAction.CallbackContext context)
    {
        thrust1D = context.ReadValue<float>(); //w 누르면 +1, s 누르면 -1, 아무것도 안누르면 0
    }

    public void OnStrafe(InputAction.CallbackContext context)
    {
        strafe1D = context.ReadValue<float>();
    }

    public void OnUpDown(InputAction.CallbackContext context)
    {
        upDown1D = context.ReadValue<float>();
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        roll1D = context.ReadValue<float>();
    }

    // public void OnPitchYaw(InputAction.CallbackContext context)
    // {
    //     pitchYaw = context.ReadValue<Vector2>();
    // }

    public void OnBoost(InputAction.CallbackContext context)
    {
        boosting = context.performed;
    }
    #endregion
}
