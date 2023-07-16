using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestSpaceMovement : MonoBehaviour
{
    [Header("=== Player Movement Settings ===")]
    [SerializeField]
    private float thrust = 100f;

    private Vector3 moveDir;

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
    public bool isLeftBoost = false;
    public bool isRightBoost = false;
    public float currentBoostAmount;

    [SerializeField, Range(0.001f, 0.999f)]
    private float thrustGlideReduction = 0.999f;
    // [SerializeField, Range(0.001f, 0.999f)]
    // private float upDownGlideReduction = 0.111f;
    // [SerializeField, Range(0.001f, 0.999f)]
    // private float leftRightGlideReduction = 0.111f;
    float glide = 0f;

    Rigidbody rb;

    //Input Values
    private Vector2 move;

    [SerializeField] private InputActionReference playerMoveActionRef;
    [SerializeField] private InputActionReference boostLeftActionRef;
    [SerializeField] private InputActionReference boostRightActionRef;


    private void OnEnable()
    {
        playerMoveActionRef.action.performed += PlayerMove_performed;
        playerMoveActionRef.action.canceled += PlayerMove_canceled;

        boostLeftActionRef.action.performed += BoostLeft_performed;
        boostLeftActionRef.action.canceled += BoostLeft_canceled;

        boostRightActionRef.action.performed += BoostRight_performed;
        boostRightActionRef.action.canceled += BoostRight_canceled;

    }

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

    private void OnDisable()
    {
        playerMoveActionRef.action.performed -= PlayerMove_performed;
        playerMoveActionRef.action.canceled -= PlayerMove_canceled;

        boostLeftActionRef.action.performed -= BoostLeft_performed;
        boostLeftActionRef.action.canceled -= BoostLeft_canceled;

        boostRightActionRef.action.performed -= BoostRight_performed;
        boostRightActionRef.action.canceled -= BoostRight_canceled;

    }

    void HandleBoosting()
    {
        if(isLeftBoost && isRightBoost)
        {
            boosting = true;
        }

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
        
        if(move.x > 0.1f || move.y > 0.1f || move.x < -0.1f || move.y < -0.1f) //checking if we're pressing any stick
        {
            moveDir = new Vector3(move.x, 0, move.y);//new Vector3(mainCam.transform.forward.x + move.x, mainCam.transform.forward.y + 0, mainCam.transform.forward.z + move.y);

            float currentThrust;

            if(boosting)
            {
                currentThrust = thrust * boostMultiplier;
            }
            else
            {
                currentThrust = thrust;
            }

            rb.AddForce(moveDir * currentThrust * Time.deltaTime); //currentThrust for boosting
            glide = thrust;
        }
        else
        {
            rb.AddForce(moveDir * glide * Time.deltaTime); //eventually glide will be 0 and won't be moving
            glide *= thrustGlideReduction;
        }
        

    }

    private void PlayerMove_performed(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    private void PlayerMove_canceled(InputAction.CallbackContext context)
    {
        // move = context.ReadValue<Vector2>();
    }

    private void BoostLeft_performed(InputAction.CallbackContext context)
    {
        isLeftBoost = context.performed;
    }

    private void BoostLeft_canceled(InputAction.CallbackContext context)
    {
        isLeftBoost = context.performed;
    }

    private void BoostRight_performed(InputAction.CallbackContext context)
    {
        isRightBoost = context.performed;
    }

    private void BoostRight_canceled(InputAction.CallbackContext context)
    {
        isRightBoost = context.performed;
        
    }
  
}
