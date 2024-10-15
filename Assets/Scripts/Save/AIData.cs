using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AIData
{
    public string id;
    public float health;
    public float[] position;

    /// <summary>
    /// AIData konstuktora
    /// </summary>
    /// <param name="id">Az id értéke</param>
    /// <param name="health">Az élet értéke</param>
    /// <param name="position">A pozíció értéke</param>
    public AIData(string id, float health, float[] position)
    {
        this.id = id;
        this.health = health;
        this.position = position;
    }
}
