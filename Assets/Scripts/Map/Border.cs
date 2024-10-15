using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour
{
    public GameObject player;
    public float damage;
    public float cooldown;
    public float lastHit;
    public bool outChar;

    private void Update()
    {
        if (outChar)
        {
            // deBuff-ot rak r�
            player.GetComponent<Move>().deBuff = true;
            // Be�ll�tja a deBuff utols� id�pontj�t
            player.GetComponent<Move>().deBuffLast = Time.time;
            // Ha lej�r a cooldown akkor 
            if (Time.time - lastHit > cooldown)
            {
                // �ll�tja az utols� sebz�si id�pontot
                lastHit = Time.time;
                // �s sebzi a karaktert
                player.GetComponent<Stats>().Hit(damage);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Character")
        {
            outChar = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Character")
        {
            outChar = false;
        }
    }
}
