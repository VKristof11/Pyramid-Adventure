using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyAI : MonoBehaviour
{
    public GameObject player;
    public float targetRange;
    public float attackRange;
    public float cooldown;

    private float lastAttack;
    public Vector3 playerCord;
    public Vector3 origoPosition;
    public float minDistance;
    public float maxDistance;
    public NavMeshAgent agent;

    /// <summary>
    /// Megkeresi a karaktert
    /// </summary>
    private void Start()
    {
        player = GameObject.Find("Character");
    }


    /// <summary>
    /// Beállítja a megadott értékeknek és az alapértelmezetre
    /// </summary>
    /// <param name="origoPosition">Ház középpontja</param>
    /// <param name="minDistance">A minimum távolság az origo-tól</param>
    /// <param name="maxDistance">A maximum távolság az origo-tól</param>
    public void Default(Vector3 origoPosition, float minDistance, float maxDistance)
    {
        this.origoPosition = origoPosition;
        this.minDistance = minDistance;
        this.maxDistance = maxDistance;
        targetRange = 15;
        attackRange = 5;
        cooldown = 5;
    }

    /// <summary>
    /// Ha nincs utvonala az AI-nak akkor elinduil egy random irányba, ha viszont a játékos a "targetRange"-en belül van akkor felé indul el
    /// </summary>
    private void Update()
    {
        playerCord = player.gameObject.transform.position;
        if (Vector3.Distance(transform.position, playerCord) <= targetRange && 
            CheckIn(transform.position, origoPosition, maxDistance))
        {
            agent.destination = playerCord;
            Attack();
        }
        else if (!agent.hasPath)
        {
            Vector3 randDest = new Vector3(Random.Range(
                origoPosition.x - maxDistance, (origoPosition.x + maxDistance)),
                1.1f, Random.Range(origoPosition.z - maxDistance,
                origoPosition.z + maxDistance));
            agent.destination = randDest;
        }
    }


    /// <summary>
    /// Elenörzi hogy a kettõ vektor távolsága meghaladja-e a maximálisat
    /// </summary>
    /// <param name="position">Az egyik vector</param>
    /// <param name="origo">A másik vector</param>
    /// <param name="max">Maximális távolság</param>
    /// <returns></returns>
    private bool CheckIn(Vector3 position, Vector3 origo, float max) 
    {
        if (Vector3.Distance(position, origo) < max)
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// Ha lejárt a "cooldown" és az "attackRange"-en belül van akkor megsebzi
    /// </summary>
    private void Attack() 
    {
        if (Time.time - lastAttack > cooldown)
        {
            if (CheckIn(transform.position, playerCord, attackRange))
            {
                lastAttack = Time.time;
                gameObject.GetComponent<EnemyStats>().Attack(player.GetComponent<Stats>());
            }
        }

    }
}
