using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class SaveSystem : MonoBehaviour
{
    public string saveName;
    public int saveCount = 0;
    public GameObject character;
    public Stats stats;
    public Skill skill;
    public Map map;
    public DBConnect db = new DBConnect("127.0.0.1", "pyramid_adv", "root", "");

    /// <summary>
    /// Beállítja az osztály adat tagjait  
    /// </summary>
    /// <param name="savename">A mentés neve</param>
    public void SetDatas(string savename)
    {
        saveName = savename;
        character = GameObject.Find("Character");
        stats = GameObject.Find("Character").GetComponent<Stats>();
        skill = GameObject.Find("UI").transform.Find("Skill").gameObject.GetComponent<Skill>();
        map = GameObject.Find("Map").GetComponent<Map>();
    }


    /// <summary>
    /// Készít egy mentést az adatok alapján
    /// </summary>
    public void SaveData()
    {
        // Létrehozunk egy BinaryFormatter-t 
        BinaryFormatter formatter = new BinaryFormatter();
        // Ha még nem létezik a mentés nevű mappánk akkor 
        if (!Directory.Exists(Application.persistentDataPath + $"/{saveName}"))
        {
            // létrehozzuk azt a mappát
            Directory.CreateDirectory(Application.persistentDataPath + $"/{saveName}");
        }
        // Létrehozunk egy segéd változót amiben tároljuk az elérési útvonalat 
        string path = Application.persistentDataPath + $"/{saveName}/{saveName}";
        // Létrehozunk egy FileStream-ot
        FileStream fs = new FileStream(path, FileMode.Create);
        // Létrehozzuk a menteni kivánt adatokat SaveFormat alapján
        SaveFormat save = new SaveFormat(character.transform.position, stats, skill, map);
        // A formatter átírja bináris formátumba az adatunkat
        formatter.Serialize(fs, save);
        Data data = new Data(saveName, stats.points, DateTime.Now);
        db.InsertInto(data);
        // Bezárjuk a FileStream-ot
        fs.Close();
    }


    /// <summary>
    /// Készít egy mentést az adatok alapján amihez hoozáfűzi hogy ez egy AutoSave és a sorszámát
    /// </summary>
    public void AutoSaveData()
    {
        if (saveCount >= 10)
        {
            saveCount = 0;
        }
        // Létrehozunk egy BinaryFormatter-t 
        BinaryFormatter formatter = new BinaryFormatter();
        // Létrehozunk egy segéd változót amiben tároljuk az elérési útvonalat 
        string path = Application.persistentDataPath + $"/{saveName}/{saveName}_AutoSave_{saveCount}";
        // Létrehozunk egy FileStream-ot
        FileStream fs = new FileStream(path, FileMode.Create);
        // Létrehozzuk a menteni kivánt adatokat SaveFormat alapján
        SaveFormat save = new SaveFormat(character.transform.position, stats, skill, map);
        // A formatter átírja bináris formátumba az adatunkat
        formatter.Serialize(fs, save);
        Data data = new Data(saveName, stats.points, DateTime.Now);
        db.InsertInto(data);
        // Növeljük a mentések sorszámát
        saveCount++;
        // Bezárjuk a FileStream-ot
        fs.Close();
    }


    /// <summary>
    /// Betölti a mentést és beállítja az adatokat
    /// </summary>
    /// <param name="saveName">A mentés neve</param>
    public void LoadData(string saveName)
    {
        // Létrehozunk egy segéd változót amiben tároljuk az elérési útvonalat 
        string path = Application.persistentDataPath + $"/{saveName}/{saveName}";
        // Ha az elérési úton talál mentést akkor
        if (File.Exists(path))
        {
            // Létrehozunk egy BinaryFormatter-t 
            BinaryFormatter formatter = new BinaryFormatter();
            // Létrehozunk egy FileStream-ot
            FileStream fs = new FileStream(path, FileMode.Open);
            // Létrehozzuk a betölteni kivánt adatokat a formatter segítségével visszakódoljuk a file-t
            SaveFormat save = (SaveFormat)formatter.Deserialize(fs);
            // Majd minden adatot beállítunk ez alapján

            character.transform.position = new Vector3(save.position[0], save.position[1], save.position[2]);
            stats.health = save.health;
            skill.healthLevel = save.healthLevel;
            skill.HLStat = save.HLStat;
            skill.HLCoast = save.HLCoast;
            skill.speedLevel = save.speedLevel;
            skill.SLStat = save.SLStat;
            skill.SLCoast = save.SLCoast;
            skill.bulletDamageLevel = save.bulletDamageLevel;
            skill.BDLStat = save.BDLStat;
            skill.BDLCoast = save.BDLCoast;
            skill.bulletSpeedLevel = save.bulletSpeedLevel;
            skill.BSLStat = save.BSLStat;
            skill.BSLCoast = save.BSLCoast;
            stats.armor = save.armor;
            stats.energy = save.energy;
            stats.points = save.points;
            for (int x = 0; x < map.grid.GetLength(0); x++)
            {
                for (int y = 0; y < map.grid.GetLength(1); y++)
                {
                    Tile seged = new Tile(0, 0, new Vector3(0, 0, 0));
                    map.grid[x, y] = seged;
                    map.grid[x, y].Type = save.grid[x, y].type;
                    map.grid[x, y].Cost = save.grid[x, y].cost;
                    map.grid[x, y].NoiseValue = save.grid[x, y].noiseValue;
                    map.grid[x, y].Color = new Color(save.grid[x, y].color[0], save.grid[x, y].color[1], save.grid[x, y].color[2], save.grid[x, y].color[3]);
                    map.grid[x, y].Cord = new Vector3(save.grid[x, y].cord[0], save.grid[x, y].cord[1], save.grid[x, y].cord[2]);
                }
            }
            map.Load(save.houses, save.endHouses);
            fs.Close();
        }
    }
}
