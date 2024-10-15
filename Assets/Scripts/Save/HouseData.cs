using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HouseData
{
    public string id;
    public bool saveHouse;
    public bool end;
    public float[] position;
    public List<AIData> listAI;

    /// <summary>
    /// A HouseData konstruktora
    /// </summary>
    /// <param name="id">Az id értéke</param>
    /// <param name="saveHouse">A saveHouse értéke</param>
    /// <param name="end">Az end értéke</param>
    /// <param name="position">A position értéke</param>
    /// <param name="listAI">A listAI értéke</param>
    public HouseData(string id, bool saveHouse, bool end, float[] position, List<AIData> listAI)
    {
        this.id = id;
        this.saveHouse = saveHouse;
        this.end = end;
        this.position = position;
        this.listAI = listAI;
    }
}
