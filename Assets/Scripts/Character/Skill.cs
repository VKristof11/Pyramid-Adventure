using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;
using System.Runtime.InteropServices;
using TMPro;
using System;
using Unity.VisualScripting;

public class Skill : MonoBehaviour
{
    public EventSystem system;
    public Stats stats;
    public Move move;
    public Bullet bullet;
    public float holdTime;
    [Header("Health")]
    public int healthLevel;
    public float[] HLStat = new float[5];
    public int[] HLCoast = new int[5];
    public GameObject[] HLObj = new GameObject[5];
    [Header("Speed")]
    public int speedLevel;
    public float[] SLStat = new float[5];
    public int[] SLCoast = new int[5];
    public GameObject[] SLObj = new GameObject[5];
    [Header("BulletDamage")]
    public int bulletDamageLevel;
    public float[] BDLStat = new float[5];
    public int[] BDLCoast = new int[5];
    public GameObject[] BDLObj = new GameObject[5];
    [Header("BulletSpeed")]
    public int bulletSpeedLevel;
    public float[] BSLStat = new float[5];
    public int[] BSLCoast = new int[5]; 
    public GameObject[] BSLObj = new GameObject[5];


    private float start;
    private bool free = true;

    private void Start()
    {
        for (int i = 1; i < HLObj.Length; i++)
        {
            HLObj[i].transform.Find("Coast").GetComponent<TextMeshProUGUI>().text = Convert.ToString(HLCoast[i]);
        }
        for (int i = 1; i < SLObj.Length; i++)
        {
            SLObj[i].transform.Find("Coast").GetComponent<TextMeshProUGUI>().text = Convert.ToString(SLCoast[i]);
        }
        for (int i = 1; i < BDLObj.Length; i++)
        {
            BDLObj[i].transform.Find("Coast").GetComponent<TextMeshProUGUI>().text = Convert.ToString(BDLCoast[i]);
        }
        for (int i = 1; i < BSLObj.Length; i++)
        {
            BSLObj[i].transform.Find("Coast").GetComponent<TextMeshProUGUI>().text = Convert.ToString(BSLCoast[i]);
        }
    }

    /// <summary>
    /// Figyeli hogy melyik upgrade van kiv�lasztva �s fejleszti ha nyomvatartja az "F" gombot
    /// </summary>
    private void Update()
    {
        stats.PointUpdate();
        // Ameddig nem engedi fel az "F" gombot addig nem lehet �j fejleszt�st elkezdeni
        if (Input.GetKeyUp(KeyCode.F))
        {
            free = true;
        }
        // Ha van kiv�lasztva egy objekt akkor megn�zi, hogy milyen szint �s t�pus�
        if (system.currentSelectedGameObject is not null && free)
        {
            int level = system.currentSelectedGameObject.GetComponent<ButtonTag>().level;
            switch (system.currentSelectedGameObject.GetComponent<ButtonTag>().type)
            {
                case "Health":
                    switch (level)
                    {
                        case 1:
                            Health(level);
                            break;
                        case 2:
                            Health(level);
                            break;
                        case 3:
                            Health(level);
                            break;
                        case 4:
                            Health(level);
                            break;
                    }
                    break;
                case "Speed":
                    switch (level)
                    {
                        case 1:
                            Speed(level);
                            break;
                        case 2:
                            Speed(level);
                            break;
                        case 3:
                            Speed(level);
                            break;
                    }
                    break;
                case "BulletDamage":
                    switch (level)
                    {
                        case 1:
                            BulletDamage(level);
                            break;
                        case 2:
                            BulletDamage(level);
                            break;
                        case 3:
                            BulletDamage(level);
                            break;
                        case 4:
                            BulletDamage(level);
                            break;
                    }
                    break;
                case "BulletSpeed":
                    switch (level)
                    {
                        case 1:
                            BulletSpeed(level);
                            break;
                        case 2:
                            BulletSpeed(level);
                            break;
                        case 3:
                            BulletSpeed(level);
                            break;
                    }
                    break;
            }
        }
    }


    /// <summary>
    /// N�veli a Health szintj�t, �rt�k�t �s levonja az �r�t
    /// </summary>
    /// <param name="upgradeLevel">A fejleszteni kiv�nt szint</param>
    private void Health(int upgradeLevel)
    {
        if (stats.points >= HLCoast[upgradeLevel])
        {
            // Ha van elegend� pontunk �s nyomvatartjuk az "F" gombot "holdTime" ideig, akkor v�gbemegy a fejleszt�s
            if (Input.GetKeyDown(KeyCode.F))
            {
                start = Time.time;
                Debug.Log(start);
            }
            if (Input.GetKey(KeyCode.F))
            {
                if (Time.time - start >= holdTime)
                {
                    free = false;
                    if (healthLevel < upgradeLevel)
                    {
                        // Ha kisebb a szint�nk a fejleszteni k�v�ntn�l akkor a jelenleg�t�l v�gigmegy �s levonja az �raikat 
                        for (int i = healthLevel; i < upgradeLevel; i++)
                        {
                            var colors = HLObj[i + 1].GetComponent<Button>().colors;
                            colors.normalColor = new Color(1f, 1f, 1f, 1f);
                            HLObj[i + 1].GetComponent<Button>().colors = colors;
                            PayCoast(stats, HLCoast[i + 1]);
                        }
                        var colors2 = HLObj[upgradeLevel].GetComponent<Button>().colors;
                        colors2.normalColor = new Color(1f, 1f, 1f, 1f);
                        HLObj[upgradeLevel].GetComponent<Button>().colors = colors2;
                        healthLevel = upgradeLevel;
                        stats.HealthUpgrade(HLStat[upgradeLevel]);
                    }
                    else if (healthLevel == upgradeLevel)
                    {
                        // Ha megeggyezik a szint�nk akkor vissza adja az �r�t �s egyel kisebbre �ll�tja a szintet
                        healthLevel = upgradeLevel - 1;
                        stats.HealthUpgrade(HLStat[upgradeLevel - 1]);
                        BackCoast(stats, HLCoast[upgradeLevel]);
                        var colors = HLObj[upgradeLevel].GetComponent<Button>().colors;
                        colors.normalColor = new Color(1f, 1f, 1f, 0f);
                        HLObj[upgradeLevel].GetComponent<Button>().colors = colors;
                    }
                    else if (healthLevel > upgradeLevel)
                    {
                        // Ha nagyobb a szint�nk akkor v�gigmegy a k�v�nt szintt�l �s vissza fizeti az �rakat �s egyel kisebbre �ll�tja a szintet
                        for (int i = upgradeLevel; i < healthLevel + 1; i++)
                        {
                            BackCoast(stats, HLCoast[i]);
                            var colors = HLObj[i].GetComponent<Button>().colors;
                            colors.normalColor = new Color(1f, 1f, 1f, 0f);
                            HLObj[i].GetComponent<Button>().colors = colors;
                        }
                        healthLevel = upgradeLevel - 1;
                        stats.HealthUpgrade(HLStat[upgradeLevel - 1]);
                    }
                }
            }

        }
    }


    /// <summary>
    /// N�veli a Speed szintj�t, �rt�k�t �s levonja az �r�t
    /// </summary>
    /// <param name="upgradeLevel">A fejleszteni kiv�nt szint</param>
    private void Speed(int upgradeLevel)
    {
        if (stats.points >= SLCoast[upgradeLevel])
        {
            // Ha van elegend� pontunk �s nyomvatartjuk az "F" gombot "holdTime" ideig, akkor v�gbemegy a fejleszt�s
            if (Input.GetKeyDown(KeyCode.F))
            {
                start = Time.time;
                Debug.Log(start);
            }
            if (Input.GetKey(KeyCode.F))
            {
                if (Time.time - start >= holdTime)
                {
                    free = false;
                    if (speedLevel < upgradeLevel)
                    {
                        // Ha kisebb a szint�nk a fejleszteni k�v�ntn�l akkor a jelenleg�t�l v�gigmegy �s levonja az �raikat 
                        for (int i = speedLevel; i < upgradeLevel; i++)
                        {
                            var colors = SLObj[i + 1].GetComponent<Button>().colors;
                            colors.normalColor = new Color(1f, 1f, 1f, 1f);
                            SLObj[i + 1].GetComponent<Button>().colors = colors;
                            PayCoast(stats, SLCoast[i + 1]);
                        }
                        speedLevel = upgradeLevel;
                        move.SpeedUpgrade(SLStat[upgradeLevel]);
                    }
                    else if (speedLevel == upgradeLevel)
                    {
                        // Ha megeggyezik a szint�nk akkor vissza adja az �r�t �s egyel kisebbre �ll�tja a szintet
                        speedLevel = upgradeLevel - 1;
                        move.SpeedUpgrade(SLStat[upgradeLevel - 1]);
                        BackCoast(stats, SLCoast[upgradeLevel]);
                        var colors2 = SLObj[upgradeLevel].GetComponent<Button>().colors;
                        colors2.normalColor = new Color(1f, 1f, 1f, 1f);
                        SLObj[upgradeLevel].GetComponent<Button>().colors = colors2;
                    }
                    else if (speedLevel > upgradeLevel)
                    {
                        // Ha nagyobb a szint�nk akkor v�gigmegy a k�v�nt szintt�l �s vissza fizeti az �rakat �s egyel kisebbre �ll�tja a szintet
                        for (int i = upgradeLevel; i < speedLevel + 1; i++)
                        {
                            var colors = SLObj[i].GetComponent<Button>().colors;
                            colors.normalColor = new Color(1f, 1f, 1f, 0f);
                            SLObj[i].GetComponent<Button>().colors = colors;
                            BackCoast(stats, SLCoast[i]);
                        }
                        speedLevel = upgradeLevel - 1;
                        move.SpeedUpgrade(SLStat[upgradeLevel - 1]);
                    }
                }
            }
        }
    }


    /// <summary>
    /// N�veli a BulletDamage szintj�t, �rt�k�t �s levonja az �r�t
    /// </summary>
    /// <param name="upgradeLevel">A fejleszteni kiv�nt szint</param>
    private void BulletDamage(int upgradeLevel)
    {
        if (stats.points >= BDLCoast[upgradeLevel])
        {
            // Ha van elegend� pontunk �s nyomvatartjuk az "F" gombot "holdTime" ideig, akkor v�gbemegy a fejleszt�s
            if (Input.GetKeyDown(KeyCode.F))
            {
                start = Time.time;
                Debug.Log(start);
            }
            if (Input.GetKey(KeyCode.F))
            {
                if (Time.time - start >= holdTime)
                {
                    free = false;
                    if (bulletDamageLevel < upgradeLevel)
                    {
                        // Ha kisebb a szint�nk a fejleszteni k�v�ntn�l akkor a jelenleg�t�l v�gigmegy �s levonja az �raikat 
                        for (int i = bulletDamageLevel; i < upgradeLevel; i++)
                        {
                            var colors = BDLObj[i + 1].GetComponent<Button>().colors;
                            colors.normalColor = new Color(1f, 1f, 1f, 1f);
                            BDLObj[i + 1].GetComponent<Button>().colors = colors;
                            PayCoast(stats, BDLCoast[i + 1]);
                        }
                        bulletDamageLevel = upgradeLevel;
                        bullet.DamageUpgrade(BDLStat[upgradeLevel], upgradeLevel);
                    }
                    else if (bulletDamageLevel == upgradeLevel)
                    {
                        // Ha megeggyezik a szint�nk akkor vissza adja az �r�t �s egyel kisebbre �ll�tja a szintet
                        bulletDamageLevel = upgradeLevel - 1;
                        bullet.DamageUpgrade(BDLStat[upgradeLevel - 1], upgradeLevel - 1);
                        BackCoast(stats, BDLCoast[upgradeLevel]);
                        var colors2 = BDLObj[upgradeLevel].GetComponent<Button>().colors;
                        colors2.normalColor = new Color(1f, 1f, 1f, 1f);
                        BDLObj[upgradeLevel].GetComponent<Button>().colors = colors2;
                    }
                    else if (bulletDamageLevel > upgradeLevel)
                    {
                        // Ha nagyobb a szint�nk akkor v�gigmegy a k�v�nt szintt�l �s vissza fizeti az �rakat �s egyel kisebbre �ll�tja a szintet
                        for (int i = upgradeLevel; i < bulletDamageLevel + 1; i++)
                        {
                            var colors = BDLObj[i].GetComponent<Button>().colors;
                            colors.normalColor = new Color(1f, 1f, 1f, 0f);
                            BDLObj[i].GetComponent<Button>().colors = colors;
                            BackCoast(stats, BDLCoast[i]);
                        }
                        bulletDamageLevel = upgradeLevel - 1;
                        bullet.DamageUpgrade(BDLStat[upgradeLevel - 1], upgradeLevel - 1);
                    }
                }
            }
        }
    }


    /// <summary>
    /// N�veli a BulletSpeed szintj�t, �rt�k�t �s levonja az �r�t
    /// </summary>
    /// <param name="upgradeLevel">A fejleszteni kiv�nt szint</param>
    private void BulletSpeed(int upgradeLevel)
    {
        if (stats.points >= BSLCoast[upgradeLevel])
        {
            // Ha van elegend� pontunk �s nyomvatartjuk az "F" gombot "holdTime" ideig, akkor v�gbemegy a fejleszt�s
            if (Input.GetKeyDown(KeyCode.F))
            {
                start = Time.time;
                Debug.Log(start);
            }
            if (Input.GetKey(KeyCode.F))
            {
                if (Time.time - start >= holdTime)
                {
                    free = false;
                    if (bulletSpeedLevel < upgradeLevel)
                    {
                        // Ha kisebb a szint�nk a fejleszteni k�v�ntn�l akkor a jelenleg�t�l v�gigmegy �s levonja az �raikat 
                        for (int i = bulletSpeedLevel; i < upgradeLevel; i++)
                        {
                            var colors = BSLObj[i + 1].GetComponent<Button>().colors;
                            colors.normalColor = new Color(1f, 1f, 1f, 1f);
                            BSLObj[i + 1].GetComponent<Button>().colors = colors;
                            PayCoast(stats, BSLCoast[i + 1]);
                        }
                        bulletSpeedLevel = upgradeLevel;
                        bullet.SpeedUpgrade(BSLStat[upgradeLevel]);
                    }
                    else if (bulletSpeedLevel == upgradeLevel)
                    {
                        // Ha megeggyezik a szint�nk akkor vissza adja az �r�t �s egyel kisebbre �ll�tja a szintet
                        bulletSpeedLevel = upgradeLevel - 1;
                        bullet.SpeedUpgrade(BSLStat[upgradeLevel - 1]);
                        BackCoast(stats, BSLCoast[upgradeLevel]);
                        var colors2 = BSLObj[upgradeLevel].GetComponent<Button>().colors;
                        colors2.normalColor = new Color(1f, 1f, 1f, 1f);
                        BSLObj[upgradeLevel].GetComponent<Button>().colors = colors2;
                    }
                    else if (bulletSpeedLevel > upgradeLevel)
                    {
                        // Ha nagyobb a szint�nk akkor v�gigmegy a k�v�nt szintt�l �s vissza fizeti az �rakat �s egyel kisebbre �ll�tja a szintet
                        for (int i = upgradeLevel; i < bulletSpeedLevel + 1; i++)
                        {
                            var colors = BSLObj[i].GetComponent<Button>().colors;
                            colors.normalColor = new Color(1f, 1f, 1f, 0f);
                            BSLObj[i].GetComponent<Button>().colors = colors;
                            BackCoast(stats, BSLCoast[i]);
                        }
                        bulletSpeedLevel = upgradeLevel - 1;
                        bullet.SpeedUpgrade(BSLStat[upgradeLevel - 1]);
                    }
                }
            }
        }
    }


    /// <summary>
    /// Levonja a meghat�rozott pont mennyis�get
    /// </summary>
    /// <param name="stats">A stat amelyr�l levonjuk a pontot</param>
    /// <param name="coast">Az �rt�k amennyit levonunk</param>
    private void PayCoast(Stats stats, int coast)
    {
        stats.points -= coast;
    }


    /// <summary>
    /// Hozz�adunk egy meghat�rozott pont mennyis�get
    /// </summary>
    /// <param name="stats">A stat amelyhez hozz�adjuk a pontot</param>
    /// <param name="coast">Az �rt�k amennyit hozz�adunk</param>
    private void BackCoast(Stats stats, int coast)
    {
        stats.points += coast;
    }

}