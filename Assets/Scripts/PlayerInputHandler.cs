using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Events;


public class PlayerInputHandler : MonoBehaviour
{
    private Vector2 movementInput;

    private float turnInput;

    [SerializeField] private float jumpHeight = 2;

    [SerializeField] private float turnSpeed = 0.1f;
    [SerializeField] private float maxTurn = 0.1f;

    private Rigidbody rb;

    [SerializeField] private int jumpCount = 1;

    [SerializeField] private float movementSpeed = 10;

    [SerializeField]private float dumping=0.5f;

    [SerializeField] private float maxSpeed=10;

    [SerializeField][Range(0,1)] private float midairMovementSpeed=1;

    public UnityEvent Shoot;

    public UnityEvent ChangeWeaponUp;

    public UnityEvent ChangeWeaponDown;


    private void Start() {
        Cursor.visible=false;
        SetupInput(InputSystem.devices.ToArray());  
    }
    private void OnEnable()
    {
        //initialize local variables
        rb = GetComponent<Rigidbody>();
    }

    private PlayerControls input;

    public void SetupInput(InputDevice device)
    {
        InputDevice[] deviceArray = new InputDevice[1];
        deviceArray[0] = device;
        SetupInput(deviceArray);
    }

    public void SetupInput(InputDevice[] devices)
    {
        input = new PlayerControls();
        input.devices = new ReadOnlyArray<InputDevice>(devices);
        input.Gameplay.Jump.performed += ctx => Jump();
        input.Gameplay.Shoot.performed += ctx => Shoot.Invoke();
        input.Gameplay.Enable();
    }

    public void Jump()
    {
        if (jumpsSinceGrounded < jumpCount)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpHeight,rb.velocity.z);
            jumpsSinceGrounded += 1;
        }
    }

    private int jumpsSinceGrounded = 0;

    public LayerMask resetJumpLayers;

    private void OnTriggerEnter(Collider coll)
    {
        if (((1 << coll.gameObject.layer) & resetJumpLayers) != 0)
        {
            jumpsSinceGrounded = 0;
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if (((1 << coll.gameObject.layer) & resetJumpLayers) != 0)
        {
            //leaving ground is considered jumping
            if ( jumpsSinceGrounded == 0 ) jumpsSinceGrounded += 1;
        }
    }


    Vector3 velocity;
    Vector3 refVelocity;
    private void FixedUpdate()
    {
        // reading input and setting horizontal velocity
        movementInput = input.Gameplay.Move.ReadValue<Vector2>();
        turnInput = Mathf.Clamp(input.Gameplay.Turn.ReadValue<float>()*turnSpeed,-maxTurn,maxTurn);

        if (jumpsSinceGrounded == 0)
            velocity = transform.rotation*(new Vector3(movementInput.x * movementSpeed,rb.velocity.y, movementSpeed*movementInput.y));
        else{
            Vector3 midairMovement = Vector3.Lerp(rb.velocity/movementSpeed, transform.rotation*(new Vector3(movementInput.x,0,movementInput.y)), midairMovementSpeed);
            velocity = new Vector3(midairMovement.x * movementSpeed, rb.velocity.y,midairMovement.z*movementSpeed);
        }

        rb.velocity = Vector3.Lerp(rb.velocity,velocity,dumping);
        transform.Rotate(Vector3.up*turnInput,Space.World);
    }
}
