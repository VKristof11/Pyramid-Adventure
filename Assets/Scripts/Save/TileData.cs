using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileData
{
    public int type;
    public float cost;
    public float noiseValue;
    public float[] color;
    public float[] cord;

    /// <summary>
    /// A TileData konstruktora
    /// </summary>
    /// <param name="type">A type értéke</param>
    /// <param name="cost">A cost érétke</param>
    /// <param name="noiseValue">A noisValue értéke</param>
    /// <param name="color">A color értéke</param>
    /// <param name="cord">A cord értéke</param>
    public TileData(int type, float cost, float noiseValue, float[] color, float[] cord)
    {
        this.type = type;
        this.cost = cost;
        this.noiseValue = noiseValue;
        this.color = color;
        this.cord = cord;
    }
}
