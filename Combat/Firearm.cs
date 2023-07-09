using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Firearm : Weapon
{
  public GameObject bulletPrefab;
  public Transform bulletSpawnPoint;
  public AudioSource gunshot;
  private Transform aimDirection;
  public int firingForce = 4000;

  private void Start()
  {
    weaponType = WeaponType.Rifle;
    gunshot = GetComponent<AudioSource>();
    Player player = GetComponent<Player>();
    if (player)
    {
      GameObject cam = GameObject.Find("Camera");
      if (cam)
      {
        aimDirection = cam.transform;
      }
    }
    else
    {
      aimDirection = transform;
    }
  }
  public override void Use()
  {
    if (canAttack)
    {
      base.Use();

      gunshot.Play();
      GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, aimDirection.rotation);
      bullet.GetComponent<Projectile>().speed = firingForce;
      bullet.GetComponent<Projectile>().damage = damage;

      canAttack = false;
      Invoke("ResetAttack", attackCooldown / 2);
    }
  }

  public override void EndAttack() { }

  private void ResetAttack()
  {
    canAttack = true;
  }

  public override void SetAim(Transform transform)
  {
    aimDirection = transform;
  }
}

