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
    /// Ha bel�p a karakter a ter�let�re deBuff-ot rak r� �s sebzi bizonyos id�k�z�nk�nt
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        // Ha a karakter�nk akkor
        if (other.name == "Character")
        {
            // deBuff-ot rak r�
            other.GetComponent<Move>().deBuff = true;
            // Be�ll�tja a deBuff utols� id�pontj�t
            other.GetComponent<Move>().deBuffLast = Time.time;
            // Ha lej�r a cooldown akkor 
            if (Time.time - lastHit > cooldown)
            {
                // �ll�tja az utols� sebz�si id�pontot
                lastHit = Time.time;
                // �s sebzi a karaktert
                other.GetComponent<Stats>().Hit(damage);
            }
        }
    }

}
