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
    /// Beállítja a UI slider-ek maximális értékét
    /// </summary>
    private void Start()
    {
        sliderHealth.maxValue = maxHealth;
        sliderEnergy.maxValue = maxEnergy;
        sliderArmor.maxValue = maxArmor;
    }


    /// <summary>
    /// Állítja a UI slider-ket az életnek... 
    /// </summary>
    private void Update()
    {
        sliderHealth.value = health;
        sliderArmor.value = armor;
        sliderEnergy.value = energy;
    }


    /// <summary>
    /// Figyeli a halált, hogy megtámadtak-e és az energy regenerációt
    /// </summary>
    private void LateUpdate()
    {
        // Ha az élet 0 vagy annál kevesebb akkor meghalt a karakter
        if (health <= 0)
        {
            Death();
        }
        // Ha letelik az "energyRegenTime" akkor hozzáad az enegy-hez "energyRegenAmount" mennyiséget
        if (Time.time - energyRegen >= energyRegenTime && energy < maxEnergy)
        {
            energyRegenTime = Time.time;
            energy += energyRegenAmount;
        }
        // Ha megtámadnak és letelik a "attackedCooldown" akkor elindul a Healing
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
    /// Elöjön a halál menü ahol eldönthetjük hogy kilépünk a menübe vagy visszatöltjük a legutolsó mentést
    /// </summary>
    private void Death()
    {
        move.menu = true;
        fight.menu = true;
        death.SetActive(true);

    }


    /*
    /// <summary>
    /// Alapértelmezett 
    /// </summary>
    public void Default()
    {
        maxHealth = 100;
        health = 100;
        points = 0;
    }
    */

    /// <summary>
    /// Frissiti a pontok kijelzõjét a UI-on
    /// </summary>
    public void PointUpdate()
    {
        pointText1.GetComponent<TextMeshProUGUI>().text = $"{points}";
        pointText2.GetComponent<TextMeshProUGUI>().text = $"{points}";
    }


    /// <summary>
    /// Egy megadott értékû sebzést levon az életbõl, ha van pajzs elöbb azt sebzi
    /// </summary>
    /// <param name="damage">Sebzés mennyisége</param>
    public void Hit(float damage)
    {
        attackedTime = Time.time;
        attacked = true;
        // Ha van armor akkor elösször abból sebez, viszont itt a 1,5 szeresét
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
    /// Fejleszti a maximális életet és beállítja a slider-t
    /// </summary>
    /// <param name="upgradedHealht">Fejlesztett élet mennyiség</param>
    public void HealthUpgrade(float upgradedHealht)
    {
        maxHealth = upgradedHealht;
        sliderHealth.maxValue = maxHealth;
    }


    /// <summary>
    /// Tölti az életet bizonyos idõközönként "healingAmount" mennyiséggel, addig ameddig el nem éri a maximális életet
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
