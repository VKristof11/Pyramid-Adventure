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
    /// Mozgatja �s �ll�tja a karakter�nk sebess�g�t
    /// </summary>
    private void Update()
    {
        DeBuff();
        Moveing();
    }


    /// <summary>
    /// Mozgatja a karakter�nket a WASD vagy ny�lak seg�ts�g�vel �s a mozg�si ir�nyba �ll�tja a karakter�nket
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
    /// Ha van a karakter�nk�n debuff effekt akkor �t�ll�tja a sebess�get a deBuffSpeed-re, amit deBuffCooldown ideig rajtahagy a karakteren
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
    /// Be�ll�tja egy �j �rt�kre a sebbess�get.
    /// </summary>
    /// <param name="upgradedSpeed">Az �j �rt�ke a sebess�gnek</param>
    public void SpeedUpgrade(float upgradedSpeed)
    {
        baseSpeed = upgradedSpeed;
        speed = baseSpeed;
    }

}
