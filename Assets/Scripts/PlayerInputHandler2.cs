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

    private Vector3Int gridPosition;


    [SerializeField] private float turnTime = 0.5f;
    [SerializeField] private float fallTime = 0.25f;
    [SerializeField] private float turnJumpHeight = 0.5f;
    [SerializeField] private float moveJumpHeight = 0.25f;
    [SerializeField] private float gridSize = 1f;

    private Rigidbody rb;


    public UnityEvent Shoot;

    public event Action<Vector3Int> reachedBlock;


    private void Start()
    {
        gridPosition = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
        facing = Dir.forward;
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

    private bool walking = false;
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
                else
                {
                    Vector3Int movementDirection = Vector3Int.zero;
                    switch (inputDirection)
                    {
                        case Dir.forward:
                            movementDirection = Vector3Int.forward;
                            break;
                        case Dir.back:
                            movementDirection = Vector3Int.back;
                            break;
                        case Dir.left:
                            movementDirection = Vector3Int.left;
                            break;
                        case Dir.right:
                            movementDirection = Vector3Int.right;
                            break;
                    }
                    if (Grid.instance.WhatIsThere(gridPosition + movementDirection + Vector3Int.up) == Grid.TileType.tile)
                    {
                        walking = true;
                        StartCoroutine(Move(movementDirection + Vector3Int.up));
                    }
                    else if (Grid.instance.WhatIsThere(gridPosition + movementDirection) != Grid.TileType.barrier)
                    {
                        walking = true;
                        StartCoroutine(Move(movementDirection));
                    }
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
            transform.position = position + (Vector3.up * 4 * t * (1 - t) * turnJumpHeight);
            transform.rotation = Quaternion.Lerp(from, targetRotation, t);
            yield return new WaitForFixedUpdate();
        }
        transform.rotation = targetRotation;
        transform.position = position;
        walking = false;
        facing = direction;
        yield return null;
    }

    IEnumerator Move(Vector3Int direction)
    {

        Vector3 from = transform.position;

        Vector3 position = transform.position;
        for (float t = 0; t < 1; t += Time.fixedDeltaTime / turnTime)
        {
            position = Vector3.Lerp(from, from + ((Vector3)direction * gridSize), t);
            transform.position = new Vector3(position.x, from.y + (4 * t * (1 - t) * moveJumpHeight)+(Mathf.Sqrt(t)*direction.y), position.z);
            yield return new WaitForFixedUpdate();
        }
        transform.position = from + ((Vector3)direction * gridSize);
        gridPosition = gridPosition + direction;
        walking = false;
        ReachedTile(gridPosition);
        yield return null;
    }

    IEnumerator Fall()
    {

        Vector3 from = transform.position;
        for (float t = 0; t < 1; t += Time.fixedDeltaTime / fallTime)
        {
            transform.position = new Vector3(from.x, from.y - t, from.z);
            yield return new WaitForFixedUpdate();
        }
        transform.position = from + Vector3.down;
        gridPosition = gridPosition + Vector3Int.down;
        walking = false;
        ReachedTile(gridPosition);
        yield return null;
    }

    private void ReachedTile(Vector3Int position){
        switch (Grid.instance.WhatIsThere(position))
        {
            case Grid.TileType.none:
                walking = true;
                StartCoroutine(Fall());
                break;
            default:
                break;
        }
        Grid.instance.PlayerIsHere(position);
        
        if(position.y<-3){
            Debug.Log("PlayerDied");
            Destroy(this.gameObject);
        }
    }
}
