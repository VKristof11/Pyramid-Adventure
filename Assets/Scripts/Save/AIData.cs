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
    /// <param name="id">Az id �rt�ke</param>
    /// <param name="health">Az �let �rt�ke</param>
    /// <param name="position">A poz�ci� �rt�ke</param>
    public AIData(string id, float health, float[] position)
    {
        this.id = id;
        this.health = health;
        this.position = position;
    }
}
