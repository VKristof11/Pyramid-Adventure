using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveFormat
{
    // Játékos adatai
    public float[] position;
    public float health;
    public int healthLevel;
    public float[] HLStat;
    public int[] HLCoast;
    // Speed
    public int speedLevel;
    public float[] SLStat;
    public int[] SLCoast;
    // BulletDamage
    public int bulletDamageLevel;
    public float[] BDLStat;
    public int[] BDLCoast;
    // BulletSpeed
    public int bulletSpeedLevel;
    public float[] BSLStat;
    public int[] BSLCoast;
    public float armor;
    public float energy;
    public int points;

    // Pálya adatai
    public TileData[,] grid;
    public List<HouseData> houses;
    public List<HouseData> endHouses;

    /// <summary>
    /// A SaveFormat konstruktora, elmeti egy játék adatait egy olyan formátumba ami késöbb kiírható egy file-ba
    /// </summary>
    /// <param name="position">A position értéke</param>
    /// <param name="stats">A stats értéke</param>
    /// <param name="skill">A skill értéke</param>
    /// <param name="map">A mapp értéke</param>
    public SaveFormat(Vector3 position, Stats stats, Skill skill, Map map) 
    {
        // Létrehozunk egy 3 hosszúségú tömböt majd lebontjuk a Vector-t 
        this.position = new float[3];
        this.position[0] = position.x;
        this.position[1] = position.y;
        this.position[2] = position.z;
        health = stats.health;
        healthLevel = skill.healthLevel;
        HLStat = skill.HLStat;
        HLCoast = skill.HLCoast;
        speedLevel = skill.speedLevel;
        SLStat = skill.SLStat;
        SLCoast = skill.SLCoast;
        bulletDamageLevel = skill.bulletDamageLevel;
        BDLStat = skill.BDLStat;
        BDLCoast = skill.BDLCoast;
        bulletSpeedLevel = skill.bulletSpeedLevel;
        BSLStat = skill.BSLStat;
        BSLCoast = skill.BSLCoast;
        armor = stats.armor;
        energy = stats.energy;
        points = stats.points;
        // Létrhozzuk a gridet
        grid = new TileData[map.grid.GetLength(0), map.grid.GetLength(1)];
        // Végig megyünk a griden
        for (int x = 0; x < map.grid.GetLength(0); x++)
        {
            for (int y = 0; y < map.grid.GetLength(1); y++)
            {
                // Lebontjuk a színt, cordinátát tömbökre majd létrhozunk egy segéd változot amit
                // aztán feltöltünk adatokkal és hozzáadjuk a gridhez
                float[] color = { map.grid[x, y].Color.r, map.grid[x, y].Color.g, map.grid[x, y].Color.b, map.grid[x, y].Color.a };
                float[] cord = { map.grid[x, y].Cord.x, map.grid[x, y].Cord.y, map.grid[x, y].Cord.z };
                TileData seged = new TileData(map.grid[x, y].Type, map.grid[x, y].Cost, map.grid[x, y].NoiseValue, color, cord);
                grid[x, y] = seged;
            }
        }
        // Létrrehozzuk a houses listát
        houses = new List<HouseData>();
        // Végigmegyünk a map.houses listán és lementjük a houses listába
        foreach (var itemHouse in map.houses)
        {
            // Létrehozunk egy segédváltozót
            House segedHouse = itemHouse.GetComponent<House>();
            // Lebontjuk a pozicióját egy segéd tömbbe
            float[] segedPoz = new float[] { segedHouse.gridPos.x, segedHouse.gridPos.y};
            // Lementjük az AI-kat
            List<AIData> AIs = new List<AIData>();
            foreach (var itemAI in segedHouse.listAI)
            {
                // Létrehozunk egy segét változót amiben tároljuk az AI stat-ját 
                EnemyStats segedAI = itemAI.GetComponent<EnemyStats>();
                // Egy segéd tömbbe lementjük a pozicióját
                float[] segedAIPoz = new float[] { segedAI.transform.position.x, segedAI.transform.position.y, segedAI.transform.position.z };
                // Létrehozunk egy segéd AIData-t, majd mentjük azt a listánkba
                AIData aidata = new AIData(segedAI.id, segedAI.health, segedAIPoz);
                AIs.Add(aidata);
            }
            // Létrehozzuk egy segéd HouseData-t és mentjük a listába
            HouseData hdata = new HouseData(segedHouse.id, segedHouse.saveHouse, segedHouse.end, segedPoz, AIs);
            houses.Add(hdata);
        }
        // Létrrehozzuk az endHouses listát
        endHouses = new List<HouseData>();
        foreach (var itemHouse in map.endHouses)
        {
            // Létrehozunk egy segédváltozót
            House segedHouse = itemHouse.GetComponent<House>();
            // Lebontjuk a pozicióját egy segéd tömbbe
            float[] segedPoz = new float[] { segedHouse.gridPos.x, segedHouse.gridPos.y };
            // Lementjük az AI-kat
            List<AIData> AIs = new List<AIData>();
            foreach (var itemAI in segedHouse.listAI)
            {
                // Létrehozunk egy segét változót amiben tároljuk az AI stat-ját 
                EnemyStats segedAI = itemAI.GetComponent<EnemyStats>();
                // Egy segéd tömbbe lementjük a pozicióját
                float[] segedAIPoz = new float[] { segedAI.transform.position.x, segedAI.transform.position.y, segedAI.transform.position.z };
                // Létrehozunk egy segéd AIData-t, majd mentjük azt a listánkba
                AIData aidata = new AIData(segedAI.id, segedAI.health, segedAIPoz);
                AIs.Add(aidata);
            }
            // Létrehozzuk egy segéd HouseData-t és mentjük a listába
            HouseData hdata = new HouseData(segedHouse.id, segedHouse.saveHouse, segedHouse.end, segedPoz, AIs);
            endHouses.Add(hdata);
        }
    }
}
