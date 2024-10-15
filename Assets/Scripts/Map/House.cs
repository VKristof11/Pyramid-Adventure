using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class House : MonoBehaviour
{
    public string id;
    public bool saveHouse = false;
    public bool end = false;
    public float minDistance;
    public float maxDistance;
    public Vector2 gridPos;
    public GameObject player;
    public float armorRegenDistance;
    [Header("AI Settings")]
    public GameObject preAI;
    public int minAI;
    public int maxAI;
    private int countAI;
    public List<GameObject> listAI = new List<GameObject>();
    public Map map;
    public SaveSystem saveSystem;

    /// <summary>
    /// Megkeresi a map-ot, generál egy random számot hogy hányszor fusson le az "SpawnAI()" metódus
    /// </summary>
    private void Start()
    {
        // Leállítja a ParticleSystem-et
        GetComponent<ParticleSystem>().Stop();
        // Megkeresi a mapot, játékost, savesystemet
        map = GameObject.Find("Map").GetComponent<Map>();
        player = GameObject.Find("Character");
        saveSystem = GameObject.Find("SceneManager").GetComponent<SaveSystem>();
        // Generál egy random AI mennyiséget
        countAI = Random.Range(minAI, maxAI + 1);
        // Meghívja a SpawnAI metódust a random mennyiségnek megfelelõen
        for (int i = 0; i < countAI; i++)
        {
            SpawnAI(i);
        }
    }


    /// <summary>
    /// Ha elfogynak az AI-ok akkor feloldaj a háza, csinál egy mentést és ha egy endHouse akkor törli a listából
    /// </summary>
    private void LateUpdate()
    {
        if (listAI.Count <= 0 && !saveHouse)
        {
            // Ha elfogynak az AI-k akkor beállítja saveHouse-nak és csinál egy mentést
            saveHouse = true;
            // Elindítja a ParticleSystem-et
            GetComponent<ParticleSystem>().Play();
            // Elmenti a játék állását
            saveSystem.AutoSaveData();
            if (end)
            {
                // Ha egy endHouse akkor törli a listából
                map.endHouses.Remove(gameObject);
            }
        }
        // Ha elég közel van a játékos akkor elindul az armor visszatöltés
        if (Vector3.Distance(transform.position, player.transform.position) < armorRegenDistance && saveHouse)
        {
            player.GetComponent<Stats>().ArmorRegen();
        }

    }


    /// <summary>
    /// Létrehoz egy új AI-t aminek beállítja hogy hol van a ház amihez tartozik, egy random poziciót 
    /// hogy honnan induljon a háztól egy megadott területen belül. Megadja az id-ját és hogy melyik házhoz tartozik.
    /// Majd Hozzáadja a "listAI" listához.
    /// </summary>
    /// <param name="idAI">Az idja</param>
    private void SpawnAI(int idAI)
    {
        Vector3 housePos = gameObject.transform.position;
        Vector3 spawnPos = new Vector3(Random.Range(
            housePos.x + minDistance, housePos.x + maxDistance),
            1f, Random.Range(housePos.z + minDistance,
            housePos.z + maxDistance));
        GameObject newAI = Instantiate(preAI, spawnPos, Quaternion.identity);
        newAI.GetComponent<EnemyAI>().Default(gameObject.transform.position,
                                              maxDistance, maxDistance);
        newAI.GetComponent<EnemyStats>().id = $"{id}-{idAI}";
        newAI.GetComponent<EnemyStats>().isHouse = true;
        newAI.GetComponent<EnemyStats>().house = gameObject;
        listAI.Add(newAI);
    }



    public void Load(List<AIData> seged)
    {
        foreach (var item in listAI)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in seged)
        {
            Vector3 housePos = gameObject.transform.position;
            Vector3 spawnPos = new Vector3(item.position[0], item.position[1], item.position[2]);
            GameObject newAI = Instantiate(preAI, spawnPos, Quaternion.identity);
            newAI.GetComponent<EnemyAI>().Default(gameObject.transform.position, maxDistance, maxDistance);
            newAI.GetComponent<EnemyStats>().id = item.id;
            newAI.GetComponent<EnemyStats>().health = item.health;
            newAI.GetComponent<EnemyStats>().house = gameObject;
            listAI.Add(newAI);
        }
    }
}
