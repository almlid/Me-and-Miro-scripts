using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerAnimationStateController : MonoBehaviour
{
  Animator animator;
  private CharacterController characterController;
  PlayerMovement playerMovement;
  Player player;
  private float movementSpeed;
  private float animSmoothingSpeed = 3f;
  private Vector3 lastMousePosition;
  private float horizontalSpeed, verticalSpeed;


  // Animator params
  private string p_isGrounded = "IsGrounded";
  private string p_isJumping = "IsJumping";
  private string p_inCombat = "InCombat";
  private string p_isCrouched = "IsCrouched";
  private string p_isTurning = "IsTurning";
  private string p_primaryAttack = "PrimaryAttack";
  private string p_weaponType = "WeaponType";
  private string p_horizontal = "Horizontal";
  private string p_vertical = "Vertical";
  private string p_drawWeapon = "DrawWeapon";
  private string p_sheathWeapon = "SheathWeapon";
  private string p_weaponIsDrawn = "WeaponIsDrawn";
  private string p_cameraDeltaX = "CameraDeltaX";



  void Awake()
  {
    animator = GetComponentInChildren<Animator>();
    Player.OnInCombatModeChange += AnimateCombatModeChange;
    Player.OnWeaponChange += AnimateChangeWeapon;
    Player.OnDeath += AnimateDeath;
  }

  void Start()
  {
    playerMovement = GetComponent<PlayerMovement>();
    characterController = GetComponent<CharacterController>();
    player = GetComponent<Player>();
  }

  void OnDestroy()
  {
    Player.OnInCombatModeChange -= AnimateCombatModeChange;
    Player.OnWeaponChange -= AnimateChangeWeapon;
    Player.OnDeath -= AnimateDeath;

  }

  void Update()
  {
    if (player.inputEnabled == true)
    {

      if (Input.GetButton("Fire1"))
      {
        AnimatePrimaryAttack();
        animator.SetFloat(p_inCombat, 1);
      }

      switch (playerMovement.GetCurrentMovementState())
      {
        case PlayerMovement.MovementState.Walk:
          horizontalSpeed = 1;
          verticalSpeed = 1;
          break;
        case PlayerMovement.MovementState.Run:
          horizontalSpeed = 2;
          verticalSpeed = 2;
          break;
        case PlayerMovement.MovementState.Sprint:
          horizontalSpeed = 3;
          verticalSpeed = 3;
          break;
        case PlayerMovement.MovementState.Crouch:
          horizontalSpeed = 1;
          verticalSpeed = 1;
          break;
      }

      if (animator.GetLayerWeight(2) > 0.01)
      {
        animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 0f, Time.deltaTime * 0.8f));
      }


      movementSpeed = playerMovement.moveDirection.magnitude;
      animator.SetFloat("Speed", movementSpeed);


      if (characterController.isGrounded)
      {
        animator.SetBool(p_isJumping, false);
        animator.SetBool(p_isGrounded, true);
      }
      else
      {
        animator.SetBool(p_isGrounded, false);
      }

      if (Input.GetButtonDown("Jump") && playerMovement.GetCurrentMovementState() != PlayerMovement.MovementState.Crouch && characterController.isGrounded)
      {
        AnimateJump();
      }

      if (playerMovement.GetCurrentMovementState() == PlayerMovement.MovementState.Crouch)
      {
        AnimateCrouchToggle(true);
      }
      else
      {
        AnimateCrouchToggle(false);
      }

      if (playerMovement.GetCurrentMovementState() == PlayerMovement.MovementState.Crouch && Input.GetButtonDown("Jump"))
      {
        AnimateCrouchToggle(false);
      }

      AnimateMovement();
      AnimateIdleTurning();
    }
  }

  private void AnimateDeath()
  {
    animator.SetTrigger("Die");
    StartCoroutine(ResetTrigger(2f, "Die"));
  }

  private void AnimateJump()
  {
    animator.SetBool(p_isJumping, true);
  }

  private void AnimateCrouchToggle(bool value)
  {
    animator.SetBool(p_isCrouched, value);
  }

  private void AnimateMovement()
  {
    float horizontalBlendValue;
    float verticalBlendValue;

    if (playerMovement.horizontalInput < -0.1f || playerMovement.horizontalInput > 0.1f)
    {
      horizontalBlendValue = playerMovement.horizontalInput;
    }
    else
    {
      horizontalBlendValue = 0.01f;
    }

    if (playerMovement.verticalInput < -0.1f || playerMovement.verticalInput > 0.1f)
    {
      verticalBlendValue = playerMovement.verticalInput;
    }
    else
    {
      verticalBlendValue = 0.01f;
    }

    animator.SetFloat(p_horizontal, Mathf.Lerp(animator.GetFloat(p_horizontal), horizontalBlendValue * horizontalSpeed, (Time.deltaTime * animSmoothingSpeed)));
    animator.SetFloat(p_vertical, Mathf.Lerp(animator.GetFloat(p_vertical), verticalBlendValue * verticalSpeed, (Time.deltaTime * animSmoothingSpeed)));
  }

  private void AnimateIdleTurning()
  {
    Vector3 currentMousePosition = Input.mousePosition;
    float deltaX = currentMousePosition.x - lastMousePosition.x;
    if (deltaX != 0f)
    {
      animator.SetBool(p_isTurning, true);
      animator.SetFloat(p_cameraDeltaX, Mathf.Lerp(animator.GetFloat(p_cameraDeltaX), deltaX, Time.deltaTime * 10f));
    }
    else
    {
      animator.SetBool(p_isTurning, false);
    }

    if (deltaX == 0 && movementSpeed >= 0.1f)
    {
      animator.SetFloat(p_cameraDeltaX, deltaX);
    }
    lastMousePosition = currentMousePosition;
  }

  IEnumerator ResetTrigger(float delay, string param)
  {
    yield return new WaitForSeconds(delay);
    animator.ResetTrigger(param);
  }
  IEnumerator ResetLayerWeight(float delay)
  {
    yield return new WaitForSeconds(delay);
    animator.SetLayerWeight(2, 0);
  }

  private void AnimatePrimaryAttack()
  {
    if (animator.GetBool(p_weaponIsDrawn) == false)
    {
      AnimateDrawWeapon();
    }
    animator.SetTrigger(p_primaryAttack);
    StartCoroutine(ResetTrigger(0.5f, p_primaryAttack));
    animator.SetFloat(p_inCombat, 1);
    if (movementSpeed > 0.5)
    {
      animator.SetLayerWeight(2, 1);
    }
  }

  private void AnimateCombatModeChange(bool flag)
  {
    if (flag)
    {
      AnimateDrawWeapon();
    }
    else
    {
      AnimateSheathWeapon();
    }
  }
  private void AnimateDrawWeapon()
  {
    if (animator.GetFloat(p_inCombat) < 0.5 && animator.GetBool(p_weaponIsDrawn) == false)
    {
      animator.SetFloat(p_inCombat, 1);
      animator.SetTrigger(p_drawWeapon);
      animator.SetBool(p_weaponIsDrawn, true);
      animator.ResetTrigger(p_sheathWeapon);
    }
  }

  private void AnimateSheathWeapon()
  {
    animator.SetTrigger(p_sheathWeapon);
    animator.SetFloat(p_inCombat, 0);
    animator.SetBool(p_weaponIsDrawn, false);
    animator.ResetTrigger(p_drawWeapon);
  }

  private void AnimateChangeWeapon(Weapon weapon)
  {
    float weaponTypeValue = 0;
    switch (weapon.weaponType)
    {
      case WeaponType.Sword:
        weaponTypeValue = 1f;
        break;
      case WeaponType.Rifle:
        weaponTypeValue = 2f;
        break;
      case WeaponType.Gun:
        weaponTypeValue = 3f;
        break;
      case WeaponType.Barehanded:
        weaponTypeValue = 0f;
        break;
    }
    animator.SetFloat(p_weaponType, weaponTypeValue);
  }

}
