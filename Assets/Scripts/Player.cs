using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Events;


public class Player : MonoBehaviour
{
    private Vector2 movementInput;

    private Vector3Int gridPosition;

    public Ability ability;

    [SerializeField] private float turnTime = 0.5f;
    [SerializeField] private float fallTime = 0.25f;
    [SerializeField] private float turnJumpHeight = 0.5f;
    [SerializeField] private float moveJumpHeight = 0.25f;
    [SerializeField] private float gridSize = 1f;

    private Rigidbody rb;

    public ParticleSystem stepParticles;

    public event Action<Vector3Int> reachedBlock;

    public event Action PlayerDiedCallback;

    private Animator animator;

    private void Start()
    {
        animator=GetComponent<Animator>();
        gridPosition = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
        ReachedTile(gridPosition);
        facing = Dir.forward;
        SetupInput(InputSystem.devices.ToArray());
    }
    private void OnEnable()
    {
        //initialize local variables
        rb = GetComponent<Rigidbody>();
    }

    private PlayerControls input;

    public GameObject deathParticles;

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
        input.Gameplay.Shoot.performed += ctx => { if (!moving){ 
                                                        moving=true;
                                                        ability.Perform(gridPosition,DirToVector(facing),AbilityCallback);
                                                        } };
        input.Gameplay.Enable();
    }

    public void AbilityCallback(Ability.AbilityOutput output){
        Debug.Log(output);
        switch (output)
        {
            case Ability.AbilityOutput.abilityPerformed:
                Die();
                break;
            default:

                break;
        }
        moving=false;
    }

    Vector3 velocity;
    Vector3 refVelocity;

    private enum Dir { none, forward, back, left, right }

    private Dir facing;

    private bool moving = false;

    private Vector3Int DirToVector(Dir dir)
    {
        Vector3Int movementDirection = Vector3Int.zero;
        switch (dir)
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
        return movementDirection;
    }
    private void FixedUpdate()
    {
        if (!moving)
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
                    moving = true;
                    StartCoroutine(Rotate(inputDirection));
                }
                else
                {
                    Vector3Int movementDirection = DirToVector(inputDirection);
                    Grid grid=Grid.instance;
                    if (grid.WhatIsThere(gridPosition + movementDirection + Vector3Int.up) == Grid.TileType.tile )
                    {
                        if(grid.WhatIsThere(gridPosition + movementDirection + 2*Vector3Int.up) == Grid.TileType.none && grid.WhatIsThere(gridPosition + (2* Vector3Int.up)) == Grid.TileType.none){
                            moving = true;
                            StartCoroutine(Move(movementDirection + Vector3Int.up));
                        }
                    }
                    else if (grid.WhatIsThere(gridPosition + movementDirection) != Grid.TileType.barrier && grid.WhatIsThere(gridPosition + movementDirection + Vector3Int.up) != Grid.TileType.barrier)
                    {
                        Debug.Log(grid.WhatIsThere(gridPosition + movementDirection));
                        moving = true;
                        StartCoroutine(Move(movementDirection));
                    }
                }
            }
        }
    }

    private void PlayerAction()
    {

    }

    IEnumerator Rotate(Dir direction)
    {
        animator.SetBool("IsFalling",true);
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
        moving = false;
        facing = direction;
        stepParticles.Play();
        animator.SetBool("IsFalling",false);
        yield return null;
    }

    IEnumerator Move(Vector3Int direction)
    {
        animator.SetBool("IsFalling",true);
        Vector3 from = transform.position;

        Vector3 position = transform.position;
        for (float t = 0; t < 1; t += Time.fixedDeltaTime / turnTime)
        {
            position = Vector3.Lerp(from, from + ((Vector3)direction * gridSize), t);
            transform.position = new Vector3(position.x, from.y + (4 * t * (1 - t) * moveJumpHeight) + (Mathf.Sqrt(t) * direction.y), position.z);
            yield return new WaitForFixedUpdate();
        }
        transform.position = from + ((Vector3)direction * gridSize);
        gridPosition = gridPosition + direction;
        moving = false;
        ReachedTile(gridPosition);
        yield return null;
    }

    IEnumerator Fall()
    {
        animator.SetBool("IsFalling",true);
        Vector3 from = transform.position;
        for (float t = 0; t < 1; t += Time.fixedDeltaTime / fallTime)
        {
            transform.position = new Vector3(from.x, from.y - t, from.z);
            yield return new WaitForFixedUpdate();
        }
        transform.position = from + Vector3.down;
        gridPosition = gridPosition + Vector3Int.down;
        moving = false;
        ReachedTile(gridPosition);
        yield return null;
    }

    private void ReachedTile(Vector3Int position)
    {
        if (position.y < -3)
        {
            Die();
            return;
        }
        switch (Grid.instance.WhatIsThere(position))
        {
            case Grid.TileType.none:
                moving = true;
                StartCoroutine(Fall());
                break;
            case Grid.TileType.tile:
                animator.SetBool("IsFalling",false);
                stepParticles.Play();
                break;
            default:
                break;
        }
        Grid.instance.PlayerIsHere(position);
    }

    private void Die(){
        input.Dispose();
        Debug.Log("PlayerDied");
        Destroy(Instantiate(deathParticles,transform.position,Quaternion.identity,LevelController.level.transform),3);
        gridPosition=Vector3Int.zero;
        Destroy(this.gameObject);
        transform.position=Vector3.zero;
        PlayerDiedCallback.Invoke();
    }
}
