using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public CharacterController controller;
    public GameObject character;
    public float baseSpeed;     // 6
    public float speed;         // 6
    public float deBuffSpeed;   // 3
    public bool deBuff = false;
    public float deBuffCooldown;
    public float deBuffLast;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public bool menu;


    /// <summary>
    /// Mozgatja és állítja a karakterünk sebességét
    /// </summary>
    private void Update()
    {
        DeBuff();
        Moveing();
    }


    /// <summary>
    /// Mozgatja a karakterünket a WASD vagy nyílak segítségével és a mozgási irányba állítja a karakterünket
    /// </summary>
    private void Moveing()
    {
        if (!menu)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(character.transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                character.transform.rotation = Quaternion.Euler(-90f, angle, 0f);
                controller.Move(direction * speed * Time.deltaTime);
            }
        }
    }


    /// <summary>
    /// Ha van a karakterünkön debuff effekt akkor átállítja a sebességet a deBuffSpeed-re, amit deBuffCooldown ideig rajtahagy a karakteren
    /// </summary>
    private void DeBuff()
    {
        if (deBuff)
        {
            speed = deBuffSpeed;
        }
        if (Time.time - deBuffLast >= deBuffCooldown)
        {
            deBuff = false;
            speed = baseSpeed;
        }
    }


    /// <summary>
    /// Beállítja egy új értékre a sebbességet.
    /// </summary>
    /// <param name="upgradedSpeed">Az új értéke a sebességnek</param>
    public void SpeedUpgrade(float upgradedSpeed)
    {
        baseSpeed = upgradedSpeed;
        speed = baseSpeed;
    }

}
