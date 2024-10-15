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
            // deBuff-ot rak rá
            player.GetComponent<Move>().deBuff = true;
            // Beállítja a deBuff utolsó idõpontját
            player.GetComponent<Move>().deBuffLast = Time.time;
            // Ha lejár a cooldown akkor 
            if (Time.time - lastHit > cooldown)
            {
                // állítja az utolsó sebzési idõpontot
                lastHit = Time.time;
                // és sebzi a karaktert
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
