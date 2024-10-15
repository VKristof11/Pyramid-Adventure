using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public string id;
    public float health;    // 100
    public float speed;     // 4
    public float damage;    // 5
    public GameObject house;
    public bool isHouse = false;
    public GameObject preCoin;

    /// <summary>
    /// Futtatja a Death met�dust
    /// </summary>
    private void Update()
    {
        Death();
    }


    /// <summary>
    /// Figyeli hogy az �let 0 vagy az alatt van �s ha igen, akkor l�trehoz egy coin-t, ha egy h�zhoz tartozik akkor kiveszi a h�z list�j�b�l
    /// </summary>
    private void Death()
    {
        if (health <= 0)
        {
            if (isHouse)
            {
                house.GetComponent<House>().listAI.Remove(gameObject);
            }
            GameObject coin = Instantiate(preCoin, transform.position, new Quaternion(0f, 90f, 90f, 1f));
            Destroy(gameObject);
        }
    }


    /// <summary>
    /// Sebzi a j�t�kost a "damage"-nek megfelel�en
    /// </summary>
    /// <param name="player">J�t�kos Stats-ja</param>
    public void Attack(Stats player) 
    {
        player.Hit(damage);
    }


}
