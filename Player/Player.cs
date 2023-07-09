using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
  public static Player Instance { get; private set; }

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else
    {
      Destroy(gameObject);
    }

    if (Instance != null && Instance != this)
    {
      Destroy(gameObject);
    }
  }

  public Weapon currentWeapon;
  Weapon[] weapons;
  private int currentWeaponIndex;
  public PlayerAnimationStateController animatorController;
  public PlayerMovement playerMovement;
  CharacterController characterController;

  public bool inCombatMode = false;
  PlayerHealth playerHealth;
  public bool isAlive = true;

  public bool inputEnabled = true;

  public static event Action OnDeath;
  public static event Action OnAttack;
  public static event Action<bool> OnInCombatModeChange;
  public static event Action<Weapon> OnWeaponChange;
  GameObject camera;
  public bool gotObjective = false;
  public static event Action<bool> OnObjectiveChange;
  public void UpdateOnObjectiveChange(bool flag)
  {
    gotObjective = flag;
    OnObjectiveChange?.Invoke(flag);
    audioSource.PlayOneShot(gotObjectiveSFX);
  }

  public GameObject weaponSlot_sheathed, weaponSlot_drawn, weaponSlot_unequipped;


  public AudioSource audioSource;
  public AudioClip changeWeaponSFX;
  public AudioClip gotObjectiveSFX;
  public AudioClip test;

  public void UpdateInCombatMode(bool flag)
  {
    inCombatMode = flag;

    if (flag)
    {
      SetDrawnWeaponTransform();
    }
    else
    {
      SetSheathedWeaponTransform();
    }
    OnInCombatModeChange?.Invoke(flag);
  }

  public void UpdateWeapon(Weapon newWeapon)
  {
    currentWeapon = newWeapon;
    if (inCombatMode)
    {
      SetDrawnWeaponTransform();
    }
    else
    {
      SetSheathedWeaponTransform();
    }
    OnWeaponChange?.Invoke(newWeapon);
  }

  private void Start()
  {
    weapons = GetComponentsInChildren<Weapon>();
    animatorController = GetComponent<PlayerAnimationStateController>();

    playerHealth = gameObject.AddComponent<PlayerHealth>();
    playerMovement = gameObject.AddComponent<PlayerMovement>();
    animatorController = gameObject.AddComponent<PlayerAnimationStateController>();
    characterController = GetComponent<CharacterController>();
    audioSource = GetComponent<AudioSource>();

    UpdateWeapon(weapons[currentWeaponIndex]);
    DisableOtherWeapons();
    OnAttack += Attack;
    GameManager.OnGameStateChange += PlayerOnGameStateChange;

    UpdateInCombatMode(false);
    camera = GameObject.Find("Camera");
  }

  public void PlayerOnGameStateChange(GameManager.GameState state)
  {
    if (state == GameManager.GameState.ReloadGame)
    {
      RestartPlayer();
    }
    SetInputEnabled(!(state == GameManager.GameState.Paused));
  }


  private void Update()
  {
    if (inputEnabled == true)
    {
      if (Input.GetButton("Fire1"))
      {
        OnAttack?.Invoke();
        UpdateInCombatMode(true);
      }

      if (Input.GetKeyDown(KeyCode.E))
      {
        ChangeWeapon();
      }

      if (Input.GetKeyDown(KeyCode.Q))
      {
        UpdateInCombatMode(inCombatMode = !inCombatMode);
      }
    }
  }

  public void Attack()
  {
    if (currentWeapon != null)
    {
      currentWeapon.Use();
    }
  }

  public void Die()
  {
    if (isAlive)
    {
      OnDeath?.Invoke();
      isAlive = false;
      playerMovement.enabled = false;
      characterController.enabled = false;
      GameManager.Instance?.UpdateGameState(GameManager.GameState.GameOver);
    }
  }

  public void ChangeWeapon()
  {
    currentWeapon.gameObject.SetActive(false);
    currentWeaponIndex++;
    if (currentWeaponIndex >= weapons.Length)
    {
      currentWeaponIndex = 0;
    }
    currentWeapon = weapons[currentWeaponIndex];
    currentWeapon.gameObject.SetActive(true);
    currentWeapon.SetAim(camera.transform);
    UpdateWeapon(currentWeapon);
    audioSource.PlayOneShot(changeWeaponSFX);
  }

  public void DisableOtherWeapons()
  {
    foreach (Weapon w in weapons)
    {
      if (w != currentWeapon)
      {
        UnEquipWeapon(w);
        w.gameObject.SetActive(false);
      }
    }
  }

  private void SetSheathedWeaponTransform()
  {
    currentWeapon.transform.parent = weaponSlot_sheathed.transform;
    currentWeapon.transform.localPosition = currentWeapon.position_sheathed;
    currentWeapon.transform.localRotation = Quaternion.Euler(currentWeapon.rotationEuler_sheathed);
  }
  private void SetDrawnWeaponTransform()
  {
    currentWeapon.transform.parent = weaponSlot_drawn.transform;
    currentWeapon.transform.localPosition = currentWeapon.position_drawn;
    currentWeapon.transform.localRotation = Quaternion.Euler(currentWeapon.rotationEuler_drawn);
  }

  private void UnEquipWeapon(Weapon weapon)
  {
    weapon.gameObject.transform.parent = weaponSlot_unequipped.transform;
  }

  private void OnDestroy()
  {
    OnAttack -= Attack;
    GameManager.OnGameStateChange -= PlayerOnGameStateChange;
  }

  public void RestartPlayer()
  {
    isAlive = true;
    inCombatMode = false;
    DisableOtherWeapons();
    currentWeapon = weapons[0];

    GameManager.OnGameStateChange += PlayerOnGameStateChange;
    playerHealth.ResetHealth();
  }

  public void SetInputEnabled(bool flag)
  {
    inputEnabled = flag;
  }

  public void PickUpObjective()
  {
    audioSource.PlayOneShot(gotObjectiveSFX);
  }

}

