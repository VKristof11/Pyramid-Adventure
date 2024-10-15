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
    /// Megkeresi a map-ot, gener�l egy random sz�mot hogy h�nyszor fusson le az "SpawnAI()" met�dus
    /// </summary>
    private void Start()
    {
        // Le�ll�tja a ParticleSystem-et
        GetComponent<ParticleSystem>().Stop();
        // Megkeresi a mapot, j�t�kost, savesystemet
        map = GameObject.Find("Map").GetComponent<Map>();
        player = GameObject.Find("Character");
        saveSystem = GameObject.Find("SceneManager").GetComponent<SaveSystem>();
        // Gener�l egy random AI mennyis�get
        countAI = Random.Range(minAI, maxAI + 1);
        // Megh�vja a SpawnAI met�dust a random mennyis�gnek megfelel�en
        for (int i = 0; i < countAI; i++)
        {
            SpawnAI(i);
        }
    }


    /// <summary>
    /// Ha elfogynak az AI-ok akkor feloldaj a h�za, csin�l egy ment�st �s ha egy endHouse akkor t�rli a list�b�l
    /// </summary>
    private void LateUpdate()
    {
        if (listAI.Count <= 0 && !saveHouse)
        {
            // Ha elfogynak az AI-k akkor be�ll�tja saveHouse-nak �s csin�l egy ment�st
            saveHouse = true;
            // Elind�tja a ParticleSystem-et
            GetComponent<ParticleSystem>().Play();
            // Elmenti a j�t�k �ll�s�t
            saveSystem.AutoSaveData();
            if (end)
            {
                // Ha egy endHouse akkor t�rli a list�b�l
                map.endHouses.Remove(gameObject);
            }
        }
        // Ha el�g k�zel van a j�t�kos akkor elindul az armor visszat�lt�s
        if (Vector3.Distance(transform.position, player.transform.position) < armorRegenDistance && saveHouse)
        {
            player.GetComponent<Stats>().ArmorRegen();
        }

    }


    /// <summary>
    /// L�trehoz egy �j AI-t aminek be�ll�tja hogy hol van a h�z amihez tartozik, egy random pozici�t 
    /// hogy honnan induljon a h�zt�l egy megadott ter�leten bel�l. Megadja az id-j�t �s hogy melyik h�zhoz tartozik.
    /// Majd Hozz�adja a "listAI" list�hoz.
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
