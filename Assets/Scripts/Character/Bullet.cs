using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float baseSpeed;     // 4
    public float baseDamage;    // 5
    public float distanceTime;  // 5
    private float speed;
    private float damage;

    public Material[] material;

    [HideInInspector]
    public bool shooted;
    private Vector3 shootDirection;

    /// <summary>
    /// Ha kil�t�k a l�ved�ket, akkor elind�tja a meghat�rozott ir�nyba �s egy meghat�rozott id� ut�n t�rli
    /// </summary>
    private void Update()
    {
        if (shooted)
        {
            // T�rli a distanceTime id� ut�n a l�ved�ket
            Destroy(gameObject, distanceTime);
            // Mozgatja a shootDirection ir�nyba a l�ved�ket
            transform.position += shootDirection * speed * Time.deltaTime;
        }
    }

    /// <summary>
    /// Ha �tk�zik egy olyan objektummal, ami nem esik bele a kiz�rtak k�z�, akkor t�rli a l�ved�ket �s ha van EnemyStats komponense, akkor levonja a sebz�st.
    /// </summary>
    /// <param name="other">Az objektum amivel �tk�zik a l�ved�k</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.name != "Character" && other.name != "pfBullet(Clone)" && other.name != "Water")
        {
            Destroy(gameObject);
            if (other.gameObject.GetComponent<EnemyStats>())
            {
                // Ha olyan objektet tal�lunk el aminek van EnemyStats-ja akkor sebzi azt
                other.gameObject.GetComponent<EnemyStats>().health -= damage;
            }
        }
    }

    /// <summary>
    /// Be�ll�tja egy �j �rt�kre a sebbess�get.
    /// </summary>
    /// <param name="upgradedSpeed">Az �j �rt�ke a sebess�gnek</param>
    public void SpeedUpgrade(float upgradedSpeed) 
    {
        baseSpeed = upgradedSpeed;
    }

    /// <summary>
    /// Be�ll�tja egy �j �rt�kre a sebz�st.
    /// </summary>
    /// <param name="upgradedDamage">Az �j �rt�ke a sebz�snek</param>
    public void DamageUpgrade(float upgradedDamage, int level) 
    {
        baseDamage = upgradedDamage;
        GetComponent<ParticleSystemRenderer>().material = material[level];
    }

    /// <summary>
    /// Be�ll�tja a particle ir�ny�t a halad�si ir�nynak megfelel�en.
    /// </summary>
    /// <param name="target">A halad�si ir�ny</param>
    public void SetParticle(Vector3 target) 
    {
        GetComponent<ParticleSystem>().transform.rotation = Quaternion.LookRotation(transform.position - target, Vector3.down);
    }

    /// <summary>
    /// Be�ll�tja hogy milyen ir�nyba kell mozognia, a sebess�g�t �s hogy mekkora legyen a sebz�se.
    /// </summary>
    /// <param name="target">A halad�si ir�ny</param>
    public void SetParameter(Vector3 target)
    {

        // A l�v�s ir�ny�nak meghat�roz�sa
        shootDirection = (target - transform.position).normalized;
        // Be�ll�tjuk a l�ved�k m�ret�hez a sebz�s�t �s sebess�g�t, ha m�g nincs kil�ve
        damage = baseDamage * (transform.localScale.y * 2);
        speed = baseSpeed / (transform.localScale.y * 2);
    }
}
