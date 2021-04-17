using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Events;


public class PlayerInputHandler2 : MonoBehaviour
{
    private Vector2 movementInput;


    [SerializeField] private float turnTime = 0.5f;
    [SerializeField] private float turnJumpHeight = 0.5f;
    [SerializeField] private float moveJumpHeight = 0.25f;
    [SerializeField] private float gridSize = 1f;

    private Rigidbody rb;


    public UnityEvent Shoot;

    public event Action<Vector3Int> reachedBlock;


    private void Start()
    {
        facing=Dir.forward;
        Cursor.visible = false;
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
        input.Gameplay.Shoot.performed += ctx => Shoot.Invoke();
        input.Gameplay.Enable();
    }


    Vector3 velocity;
    Vector3 refVelocity;

    private enum Dir { none, forward, back, left, right }

    private Dir facing;

    private bool walking=false;
    private void FixedUpdate()
    {
        if (!walking)
        {

            movementInput = input.Gameplay.Move.ReadValue<Vector2>();
            Dir inputDirection = Dir.none;

            if (movementInput.x > 0.5f)
                inputDirection = Dir.right;
            else if (movementInput.x < -0.5f)
                inputDirection = Dir.left;
            else if (movementInput.y > 0.5f)
                inputDirection = Dir.forward;
            else if (movementInput.y < -0.5f)
                inputDirection = Dir.back;

            if (inputDirection != Dir.none)
            {
                if (facing != inputDirection)
                {
                    walking = true;
                    StartCoroutine(Rotate(inputDirection));
                }
                else{
                    walking=true;
                    StartCoroutine(Move(inputDirection));
                }
            }
        }
    }

    IEnumerator Rotate(Dir direction)
    {
        Quaternion targetRotation = Quaternion.identity;
        switch (direction)
        {
            case Dir.forward:
                targetRotation = Quaternion.Euler(0, 0, 0);
                break;
            case Dir.back:
                targetRotation = Quaternion.Euler(0, 180, 0);
                break;
            case Dir.left:
                targetRotation = Quaternion.Euler(0, 270, 0);
                break;
            case Dir.right:
                targetRotation = Quaternion.Euler(0, 90, 0);
                break;
        }

        Quaternion from = transform.rotation;
        Vector3 position = transform.position;


        for (float t = 0; t < 1; t += Time.fixedDeltaTime / turnTime)
        {
            transform.position = position + (Vector3.up * 4 * t * (1-t) * turnJumpHeight);
            transform.rotation = Quaternion.Lerp(from, targetRotation, t);
            yield return new WaitForFixedUpdate();
        }
        transform.rotation = targetRotation;
        transform.position = position;
        walking = false;
        facing=direction;
        yield return null;
    }

    IEnumerator Move(Dir direction)
    {
        Vector3 movementDirection = Vector3.zero;
        switch (direction)
        {
            case Dir.forward:
                movementDirection = Vector3.forward;
                break;
            case Dir.back:
                movementDirection = Vector3.back;
                break;
            case Dir.left:
                movementDirection = Vector3.left;
                break;
            case Dir.right:
                movementDirection = Vector3.right;
                break;
        }
        Vector3 from = transform.position;

        Vector3 position=transform.position;
        for (float t = 0; t < 1; t += Time.fixedDeltaTime / turnTime)
        {
            position=Vector3.Lerp(from, from + (movementDirection*gridSize), t);
            transform.position = new Vector3(position.x,from.y + ( 4 * t * (1-t) * moveJumpHeight),position.z);
            yield return new WaitForFixedUpdate();
        }
        transform.position = from + (movementDirection*gridSize);
        walking = false;
        yield return null;
    }
}
