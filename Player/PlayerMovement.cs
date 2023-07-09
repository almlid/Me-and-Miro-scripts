using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
  public float rotationSpeed = 200.0f;
  public float gravity = 30.0f;
  private CharacterController characterController;
  public Transform cameraTransform;
  public float horizontalInput, verticalInput, mouseInputY, mouseInputX;
  public Vector3 moveDirection;
  private MovementState movementState;
  private PlayerAttributes attributes;
  Vector3 velocity;
  Vector3 lastGroundedMoveDirection;
  private bool canJump = true;

  private KeyCode sprintKey = KeyCode.LeftShift;
  private KeyCode crouchToggleKey = KeyCode.LeftControl;
  private KeyCode walkToggleKey = KeyCode.LeftAlt;
  public event Action OnJump;
  public event Action<Vector3> OnMove;
  public event Action OnWalkToggle;
  public event Action OnCrouchToggle;

  Animator animator;
  GameObject cameraObject;

  public enum MovementState
  {
    Walk,
    Run,
    Sprint,
    Crouch
  }


  void Awake()
  {
    characterController = GetComponent<CharacterController>();
    attributes = GetComponent<PlayerAttributes>();
    animator = GetComponentInChildren<Animator>();
    cameraObject = GameObject.Find("Camera");
    cameraTransform = cameraObject.GetComponent<Transform>();

    moveDirection = Vector3.zero;
    movementState = MovementState.Run;
    OnMove += SetMoveDirection;
    OnJump += Jump;
    OnWalkToggle += ToggleWalk;
    OnCrouchToggle += ToggleCrouch;
  }

  void Update()
  {
    mouseInputX = Input.GetAxis("Mouse X");
    mouseInputY = Input.GetAxis("Mouse Y");
    transform.rotation *= Quaternion.Euler(0, mouseInputX * rotationSpeed * Time.deltaTime, 0);

    cameraTransform.rotation *= Quaternion.Euler(-mouseInputY * rotationSpeed * Time.deltaTime, 0, 0);
    float cameraRotationX = cameraTransform.localEulerAngles.x;
    if (cameraRotationX > 180f) cameraRotationX -= 360f;

    cameraRotationX = Mathf.Clamp(cameraRotationX, -70f, 70f);
    cameraTransform.localEulerAngles = new Vector3(cameraRotationX, 0, 0);

    mouseInputX = Input.GetAxis("Mouse X");
    transform.rotation *= Quaternion.Euler(0, mouseInputX * rotationSpeed * Time.deltaTime, 0);

    horizontalInput = Input.GetAxis("Horizontal");
    mouseInputY = Input.GetAxis("Mouse Y");
    verticalInput = Input.GetAxis("Vertical");

    Vector3 moveInput = new Vector3(horizontalInput, 0, verticalInput);
    if (moveInput.magnitude > 0.0f)
    {
      OnMove?.Invoke(moveInput);
    }
    else
    {
      moveDirection = Vector3.zero;
    }

    if (characterController.isGrounded)
    {
      if (velocity.y < 0)
      {
        velocity.y = -2.0f;
      }

      if (Input.GetKeyDown(walkToggleKey))
      {
        OnWalkToggle?.Invoke();
      }

      if (Input.GetKeyDown(crouchToggleKey))
      {
        OnCrouchToggle?.Invoke();
      }

      if (Input.GetKey(sprintKey))
      {
        movementState = MovementState.Sprint;
      }
      else if (Input.GetKeyUp(sprintKey) && movementState == MovementState.Sprint)
      {
        movementState = MovementState.Run;
      }

      SetMovementSpeed();
      lastGroundedMoveDirection = moveDirection;
    }
    else
    {
      float airControlFactor = 0.5f;
      moveDirection = Vector3.Lerp(lastGroundedMoveDirection, moveDirection, airControlFactor);
    }

    if (Input.GetButtonDown("Jump") && canJump == true && movementState != MovementState.Crouch && characterController.isGrounded)
    {
      OnJump?.Invoke();
      EnableState(0.3f, canJump);
    }

    if (Input.GetButtonDown("Jump") && movementState == MovementState.Crouch)
    {
      movementState = MovementState.Run;
    }

    if (!canJump && characterController.isGrounded)
    {
      StartCoroutine(EnableState(1f, true));
    }

    velocity.y -= gravity * Time.deltaTime;
    characterController.Move((velocity + moveDirection) * Time.deltaTime);
    SetMoveDirection(moveInput);
  }

  private void SetMoveDirection(Vector3 moveInput)
  {
    if (cameraObject == null)
    {
      cameraObject = GameObject.Find("Camera");
    }

    if (cameraObject.GetComponent<Transform>() != null)
    {
      Transform cameraTransform = cameraObject.GetComponent<Transform>();

      Vector3 cameraDirection = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z);
      moveDirection = Vector3.ClampMagnitude(moveInput, 1);
      moveDirection = cameraDirection * moveDirection.z + cameraTransform.right * moveDirection.x;
      moveDirection.Normalize();
    }
  }

  IEnumerator JumpWithDelay(float delay)
  {
    canJump = false;
    yield return new WaitForSeconds(delay);
    velocity.y = Mathf.Sqrt(attributes.jumpForce * 2f * gravity);
  }

  IEnumerator EnableState(float delay, bool state)
  {
    yield return new WaitForSeconds(delay);
    canJump = state;
  }

  private void Jump()
  {
    StartCoroutine(JumpWithDelay(0.2f));
  }

  private void ToggleWalk()
  {
    movementState = movementState == MovementState.Walk ? MovementState.Run : MovementState.Walk;
  }

  private void ToggleCrouch()
  {
    movementState = movementState == MovementState.Crouch ? MovementState.Run : MovementState.Crouch;
  }

  private void SetMovementSpeed()
  {
    switch (movementState)
    {
      case MovementState.Walk:
        moveDirection *= attributes.walkSpeed;
        break;
      case MovementState.Run:
        moveDirection *= attributes.runSpeed;
        break;
      case MovementState.Sprint:
        moveDirection *= attributes.sprintSpeed;
        break;
      case MovementState.Crouch:
        moveDirection *= attributes.sneakSpeed;
        break;
    }
  }

  public MovementState GetCurrentMovementState()
  {
    return movementState;
  }

  public Vector3 GetVelocity()
  {
    return moveDirection + velocity;
  }
}