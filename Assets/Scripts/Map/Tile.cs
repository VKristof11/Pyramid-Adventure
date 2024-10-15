using System;
using UnityEngine;

public class Tile
{
    /*
    * 1: Land
    * 2: Water
    * 3: House
    * 4: Tower
    * 5: Road
    * 6: Points
    * 7: HousePoint
    * 8: EndHousePoint
    */

    private int type;
    private float cost;
    private float noiseValue;
    private Color color;
    private Vector3 cord;

    public int Type { get => type; set => type = value; }
    public float Cost { get => cost; set => cost = value; }
    public float NoiseValue { get => noiseValue; set => noiseValue = value; }
    public Color Color { get => color; set => color = value; }
    public Vector3 Cord { get => cord; set => cord = value; }

    /// <summary>
    /// A Tile konstruktora
    /// </summary>
    /// <param name="noiseValue">A noiseValue értéke</param>
    /// <param name="WaterLevel">A víz szintjének értéke</param>
    /// <param name="cord">A helyzete a tile-nek</param>
    public Tile(float noiseValue, float WaterLevel, Vector3 cord)
    {
        this.cord = cord;
        this.noiseValue = noiseValue;
        if (noiseValue < WaterLevel)
        {
            type = 2;
            cost = 100000;
            color = new Color(0.05f, 0.3f, 1f, 0.5f);
        }
        else
        {
            type = 1;
            cost = ( noiseValue) * 15;
            color = new Color(noiseValue / 2f + 0.1f, noiseValue / 2f + 0.2f, 0.01f, 1f);
        }

    }

    /// <summary>
    /// Utra állítja az adott tile-t
    /// </summary>
    public void SetPath() 
    {
        type = 5;
        cost = 10;
        color = new Color(0.6f,0.6f,0.6f);
    }

    /// <summary>
    /// Összekötendõ pontnak állítja a tile-t
    /// </summary>
    public void SetPoint() 
    {
        type = 6;
        cost = 0;
        color = new Color(0.6f,0.6f,0.6f);
        //color = new Color(1f, 0.949f, 0.039f); // Test color
    }

    /// <summary>
    /// Egy ház pointnak állítja be az adott tile-t
    /// </summary>
    public void SetHousePoint() 
    {
        type = 7;
        cost = 0;
        color = new Color(noiseValue / 2f + 0.1f, noiseValue / 2f + 0.2f, 0.01f, 1f);
    }

    /// <summary>
    /// Egy végpont háznak állítja be az adott tile-t
    /// </summary>
    public void SetEndHousePoint()
    {
        type = 8;
        cost = 0;
        color = new Color(noiseValue / 2f + 0.1f, noiseValue / 2f + 0.2f, 0.01f, 1f);
    }
}
