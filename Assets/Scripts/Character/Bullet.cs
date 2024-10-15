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
    /// Ha kilõtük a lövedéket, akkor elindítja a meghatározott irányba és egy meghatározott idõ után törli
    /// </summary>
    private void Update()
    {
        if (shooted)
        {
            // Törli a distanceTime idõ után a lövedéket
            Destroy(gameObject, distanceTime);
            // Mozgatja a shootDirection irányba a lövedéket
            transform.position += shootDirection * speed * Time.deltaTime;
        }
    }

    /// <summary>
    /// Ha ütközik egy olyan objektummal, ami nem esik bele a kizártak közé, akkor törli a lövedéket és ha van EnemyStats komponense, akkor levonja a sebzést.
    /// </summary>
    /// <param name="other">Az objektum amivel ütközik a lövedék</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.name != "Character" && other.name != "pfBullet(Clone)" && other.name != "Water")
        {
            Destroy(gameObject);
            if (other.gameObject.GetComponent<EnemyStats>())
            {
                // Ha olyan objektet találunk el aminek van EnemyStats-ja akkor sebzi azt
                other.gameObject.GetComponent<EnemyStats>().health -= damage;
            }
        }
    }

    /// <summary>
    /// Beállítja egy új értékre a sebbességet.
    /// </summary>
    /// <param name="upgradedSpeed">Az új értéke a sebességnek</param>
    public void SpeedUpgrade(float upgradedSpeed) 
    {
        baseSpeed = upgradedSpeed;
    }

    /// <summary>
    /// Beállítja egy új értékre a sebzést.
    /// </summary>
    /// <param name="upgradedDamage">Az új értéke a sebzésnek</param>
    public void DamageUpgrade(float upgradedDamage, int level) 
    {
        baseDamage = upgradedDamage;
        GetComponent<ParticleSystemRenderer>().material = material[level];
    }

    /// <summary>
    /// Beállítja a particle irányát a haladási iránynak megfelelõen.
    /// </summary>
    /// <param name="target">A haladási irány</param>
    public void SetParticle(Vector3 target) 
    {
        GetComponent<ParticleSystem>().transform.rotation = Quaternion.LookRotation(transform.position - target, Vector3.down);
    }

    /// <summary>
    /// Beállítja hogy milyen irányba kell mozognia, a sebességét és hogy mekkora legyen a sebzése.
    /// </summary>
    /// <param name="target">A haladási irány</param>
    public void SetParameter(Vector3 target)
    {

        // A lövés irányának meghatározása
        shootDirection = (target - transform.position).normalized;
        // Beállítjuk a lövedék méretéhez a sebzését és sebességét, ha még nincs kilõve
        damage = baseDamage * (transform.localScale.y * 2);
        speed = baseSpeed / (transform.localScale.y * 2);
    }
}
