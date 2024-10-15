using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveFormat
{
    // J�t�kos adatai
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

    // P�lya adatai
    public TileData[,] grid;
    public List<HouseData> houses;
    public List<HouseData> endHouses;

    /// <summary>
    /// A SaveFormat konstruktora, elmeti egy j�t�k adatait egy olyan form�tumba ami k�s�bb ki�rhat� egy file-ba
    /// </summary>
    /// <param name="position">A position �rt�ke</param>
    /// <param name="stats">A stats �rt�ke</param>
    /// <param name="skill">A skill �rt�ke</param>
    /// <param name="map">A mapp �rt�ke</param>
    public SaveFormat(Vector3 position, Stats stats, Skill skill, Map map) 
    {
        // L�trehozunk egy 3 hossz�s�g� t�mb�t majd lebontjuk a Vector-t 
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
        // L�trhozzuk a gridet
        grid = new TileData[map.grid.GetLength(0), map.grid.GetLength(1)];
        // V�gig megy�nk a griden
        for (int x = 0; x < map.grid.GetLength(0); x++)
        {
            for (int y = 0; y < map.grid.GetLength(1); y++)
            {
                // Lebontjuk a sz�nt, cordin�t�t t�mb�kre majd l�trhozunk egy seg�d v�ltozot amit
                // azt�n felt�lt�nk adatokkal �s hozz�adjuk a gridhez
                float[] color = { map.grid[x, y].Color.r, map.grid[x, y].Color.g, map.grid[x, y].Color.b, map.grid[x, y].Color.a };
                float[] cord = { map.grid[x, y].Cord.x, map.grid[x, y].Cord.y, map.grid[x, y].Cord.z };
                TileData seged = new TileData(map.grid[x, y].Type, map.grid[x, y].Cost, map.grid[x, y].NoiseValue, color, cord);
                grid[x, y] = seged;
            }
        }
        // L�trrehozzuk a houses list�t
        houses = new List<HouseData>();
        // V�gigmegy�nk a map.houses list�n �s lementj�k a houses list�ba
        foreach (var itemHouse in map.houses)
        {
            // L�trehozunk egy seg�dv�ltoz�t
            House segedHouse = itemHouse.GetComponent<House>();
            // Lebontjuk a pozici�j�t egy seg�d t�mbbe
            float[] segedPoz = new float[] { segedHouse.gridPos.x, segedHouse.gridPos.y};
            // Lementj�k az AI-kat
            List<AIData> AIs = new List<AIData>();
            foreach (var itemAI in segedHouse.listAI)
            {
                // L�trehozunk egy seg�t v�ltoz�t amiben t�roljuk az AI stat-j�t 
                EnemyStats segedAI = itemAI.GetComponent<EnemyStats>();
                // Egy seg�d t�mbbe lementj�k a pozici�j�t
                float[] segedAIPoz = new float[] { segedAI.transform.position.x, segedAI.transform.position.y, segedAI.transform.position.z };
                // L�trehozunk egy seg�d AIData-t, majd mentj�k azt a list�nkba
                AIData aidata = new AIData(segedAI.id, segedAI.health, segedAIPoz);
                AIs.Add(aidata);
            }
            // L�trehozzuk egy seg�d HouseData-t �s mentj�k a list�ba
            HouseData hdata = new HouseData(segedHouse.id, segedHouse.saveHouse, segedHouse.end, segedPoz, AIs);
            houses.Add(hdata);
        }
        // L�trrehozzuk az endHouses list�t
        endHouses = new List<HouseData>();
        foreach (var itemHouse in map.endHouses)
        {
            // L�trehozunk egy seg�dv�ltoz�t
            House segedHouse = itemHouse.GetComponent<House>();
            // Lebontjuk a pozici�j�t egy seg�d t�mbbe
            float[] segedPoz = new float[] { segedHouse.gridPos.x, segedHouse.gridPos.y };
            // Lementj�k az AI-kat
            List<AIData> AIs = new List<AIData>();
            foreach (var itemAI in segedHouse.listAI)
            {
                // L�trehozunk egy seg�t v�ltoz�t amiben t�roljuk az AI stat-j�t 
                EnemyStats segedAI = itemAI.GetComponent<EnemyStats>();
                // Egy seg�d t�mbbe lementj�k a pozici�j�t
                float[] segedAIPoz = new float[] { segedAI.transform.position.x, segedAI.transform.position.y, segedAI.transform.position.z };
                // L�trehozunk egy seg�d AIData-t, majd mentj�k azt a list�nkba
                AIData aidata = new AIData(segedAI.id, segedAI.health, segedAIPoz);
                AIs.Add(aidata);
            }
            // L�trehozzuk egy seg�d HouseData-t �s mentj�k a list�ba
            HouseData hdata = new HouseData(segedHouse.id, segedHouse.saveHouse, segedHouse.end, segedPoz, AIs);
            endHouses.Add(hdata);
        }
    }
}
