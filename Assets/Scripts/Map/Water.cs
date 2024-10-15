using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Water : MonoBehaviour
{
    public float damage;
    public float cooldown;
    private float lastHit;

    /// <summary>
    /// Ha belép a karakter a területére deBuff-ot rak rá és sebzi bizonyos idõközönként
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        // Ha a karakterünk akkor
        if (other.name == "Character")
        {
            // deBuff-ot rak rá
            other.GetComponent<Move>().deBuff = true;
            // Beállítja a deBuff utolsó idõpontját
            other.GetComponent<Move>().deBuffLast = Time.time;
            // Ha lejár a cooldown akkor 
            if (Time.time - lastHit > cooldown)
            {
                // állítja az utolsó sebzési idõpontot
                lastHit = Time.time;
                // és sebzi a karaktert
                other.GetComponent<Stats>().Hit(damage);
            }
        }
    }

}
