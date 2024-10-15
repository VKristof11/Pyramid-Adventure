using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int minPoint;
    public int maxPoint;
    public int point;

    /// <summary>
    /// Gener�l egy random sz�mot, hogy mennyi legyen az �rt�ke a coin-nak
    /// </summary>
    void Start()
    {
        point = Random.Range(minPoint, maxPoint);
    }

    /// <summary>
    /// Ha a karakterrel �tk�zik akkor hozz�adja a "Stats" "points" mez�h�z a coin "point" �rt�k�t
    /// </summary>
    /// <param name="other">Az objektum amivel �tk�zik a coin</param>
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
