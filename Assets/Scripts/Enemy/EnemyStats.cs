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
    /// Futtatja a Death metódust
    /// </summary>
    private void Update()
    {
        Death();
    }


    /// <summary>
    /// Figyeli hogy az élet 0 vagy az alatt van és ha igen, akkor létrehoz egy coin-t, ha egy házhoz tartozik akkor kiveszi a ház listájából
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
    /// Sebzi a játékost a "damage"-nek megfelelõen
    /// </summary>
    /// <param name="player">Játékos Stats-ja</param>
    public void Attack(Stats player) 
    {
        player.Hit(damage);
    }


}
