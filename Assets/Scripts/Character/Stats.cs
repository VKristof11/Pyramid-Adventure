using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    public float maxHealth;
    public float health;
    public float maxArmor;
    public float armor;
    public float armorRegenAmount;
    public float maxEnergy;
    public float energy;
    public float energyRegen;
    public float energyRegenAmount;
    public int points;
    public float healingAmount;
    public GameObject pointText1;
    public GameObject pointText2;
    public Slider sliderHealth;
    public Slider sliderArmor;
    public Slider sliderEnergy;
    public List<Vector3> spawnPoints = new List<Vector3>();
    public bool attacked;
    public float attackedTime;
    public float attackedCooldown;
    private float energyRegenTime;
    public GameObject death;
    public Move move;
    public FightSystem fight;
    private float Hmoment = 0;
    private float Amoment = 0;
    public GameObject HealthDamage;
    public GameObject ArmoreDamage;
    private float healthSave;
    private float armorSave;
    private float uiTime;
    /// <summary>
    /// Be�ll�tja a UI slider-ek maxim�lis �rt�k�t
    /// </summary>
    private void Start()
    {
        sliderHealth.maxValue = maxHealth;
        sliderEnergy.maxValue = maxEnergy;
        sliderArmor.maxValue = maxArmor;
    }


    /// <summary>
    /// �ll�tja a UI slider-ket az �letnek... 
    /// </summary>
    private void Update()
    {
        sliderHealth.value = health;
        sliderArmor.value = armor;
        sliderEnergy.value = energy;
    }


    /// <summary>
    /// Figyeli a hal�lt, hogy megt�madtak-e �s az energy regener�ci�t
    /// </summary>
    private void LateUpdate()
    {
        // Ha az �let 0 vagy ann�l kevesebb akkor meghalt a karakter
        if (health <= 0)
        {
            Death();
        }
        // Ha letelik az "energyRegenTime" akkor hozz�ad az enegy-hez "energyRegenAmount" mennyis�get
        if (Time.time - energyRegen >= energyRegenTime && energy < maxEnergy)
        {
            energyRegenTime = Time.time;
            energy += energyRegenAmount;
        }
        // Ha megt�madnak �s letelik a "attackedCooldown" akkor elindul a Healing
        if (Time.time - attackedTime >= attackedCooldown)
        {
            attacked = false;
            Healing();
        }

        if (Time.time - uiTime > 0.4f)
        {
            HealthDamage.SetActive(false);
            ArmoreDamage.SetActive(false);
        }
        healthSave = health;
        armorSave = armor;
    }


    /// <summary>
    /// El�j�n a hal�l men� ahol eld�nthetj�k hogy kil�p�nk a men�be vagy visszat�ltj�k a legutols� ment�st
    /// </summary>
    private void Death()
    {
        move.menu = true;
        fight.menu = true;
        death.SetActive(true);

    }


    /*
    /// <summary>
    /// Alap�rtelmezett 
    /// </summary>
    public void Default()
    {
        maxHealth = 100;
        health = 100;
        points = 0;
    }
    */

    /// <summary>
    /// Frissiti a pontok kijelz�j�t a UI-on
    /// </summary>
    public void PointUpdate()
    {
        pointText1.GetComponent<TextMeshProUGUI>().text = $"{points}";
        pointText2.GetComponent<TextMeshProUGUI>().text = $"{points}";
    }


    /// <summary>
    /// Egy megadott �rt�k� sebz�st levon az �letb�l, ha van pajzs el�bb azt sebzi
    /// </summary>
    /// <param name="damage">Sebz�s mennyis�ge</param>
    public void Hit(float damage)
    {
        attackedTime = Time.time;
        attacked = true;
        // Ha van armor akkor el�ssz�r abb�l sebez, viszont itt a 1,5 szeres�t
        if (armor <= 0)
        {
            health -= damage * 1.5f;
            uiTime = Time.time;
            HealthDamage.SetActive(true);
        }
        else
        {
            armor -= damage;
            uiTime = Time.time;
            ArmoreDamage.SetActive(true);
        }
    }


    /// <summary>
    /// Fejleszti a maxim�lis �letet �s be�ll�tja a slider-t
    /// </summary>
    /// <param name="upgradedHealht">Fejlesztett �let mennyis�g</param>
    public void HealthUpgrade(float upgradedHealht)
    {
        maxHealth = upgradedHealht;
        sliderHealth.maxValue = maxHealth;
    }


    /// <summary>
    /// T�lti az �letet bizonyos id�k�z�nk�nt "healingAmount" mennyis�ggel, addig ameddig el nem �ri a maxim�lis �letet
    /// </summary>
    private void Healing()
    {
        if (!attacked)
        {
            if (Time.time - Hmoment > 1 && health < maxHealth)
            {
                Hmoment = Time.time;
                health += healingAmount;
            }
        }
    }


    public void ArmorRegen()
    {
        if (!attacked)
        {
            if (Time.time - Amoment > 1 && armor < maxArmor)
            {
                Amoment = Time.time;
                armor += armorRegenAmount;
            }
        }
    }
}
