using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int minPoint;
    public int maxPoint;
    public int point;

    /// <summary>
    /// Generál egy random számot, hogy mennyi legyen az értéke a coin-nak
    /// </summary>
    void Start()
    {
        point = Random.Range(minPoint, maxPoint);
    }

    /// <summary>
    /// Ha a karakterrel ütközik akkor hozzáadja a "Stats" "points" mezõhöz a coin "point" értékét
    /// </summary>
    /// <param name="other">Az objektum amivel ütközik a coin</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Character")
        {
            other.GetComponent<Stats>().points += point;
            other.GetComponent<Stats>().PointUpdate();
            Destroy(gameObject);
        }
    }
}
