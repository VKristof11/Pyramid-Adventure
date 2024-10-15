using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Data
{
    public string saveName;
    public int point;
    public DateTime time;

    public Data(string saveName, int point, DateTime time)
    {
        this.saveName = saveName;
        this.point = point;
        this.time = time;
    }
}
