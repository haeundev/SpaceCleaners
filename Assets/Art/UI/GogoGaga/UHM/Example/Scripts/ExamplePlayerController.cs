using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GogoGaga.UHM {
    public class ExamplePlayerController : MonoBehaviour
    {
        private float y, x;
        private Rigidbody rb;

        public bool grounded;

        public float walkSpeed = 5f, sensitivity = 2f;

        Camera cam;


        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            rb = GetComponent<Rigidbody>();
            cam = Camera.main;
            UltimateHudManager.Instance.CreateCrossHair(1);


            UltimateHudManager.Instance.MoveFadeToBottom();
        }

        
        void Update()
        {
            grounded = Physics.Raycast(rb.transform.position, Vector3.down, Camera.main.transform.localPosition.y + .5f);
            if (Input.GetKey(KeyCode.Space) && grounded)
            {
                rb.velocity = new Vector3(rb.velocity.x, 5, rb.velocity.z);
            }
            Look();

            RaycastHit info = new RaycastHit();

            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out info, 2))
            {
                ExampleButtonInterface e = info.collider.GetComponent<ExampleButtonInterface>();
                if (e != null )
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        e.OnButtonPress();
                    }
                }
                else
                {
                    
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                UltimateHudManager.Instance.ToggleFade(new Color(0.2f, 0.2f, 0.2f));
            }
            else if(Input.GetKeyDown(KeyCode.CapsLock))
            {
                UltimateHudManager.Instance.ToggleFade(Color.white);
            }
        }


        void Look()
        {
            x -= Input.GetAxisRaw("Mouse Y") * sensitivity;
            x = Mathf.Clamp(x, -90, 90);
            y += Input.GetAxisRaw("Mouse X") * sensitivity;
            Camera.main.transform.localRotation = Quaternion.Euler(x, y, 0);
        }

        void FixedUpdate()
        {
            Movement();
        }

        void Movement()
        {
            Vector2 axis = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")).normalized * walkSpeed;
            Vector3 forward = new Vector3(-Camera.main.transform.right.z, 0, Camera.main.transform.right.x);
            Vector3 moveDirection = (forward * axis.x + Camera.main.transform.right * axis.y + Vector3.up * rb.velocity.y);
            rb.velocity = moveDirection;
        }
    }
}