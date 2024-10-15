using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;

public class FightSystem : MonoBehaviour
{
    public GameObject Player;
    public Stats stats;
    public GameObject pfBullet;
    public float cooldown;
    public float shotCost;
    private GameObject bullet;
    public bool menu;

    /// <summary>
    /// Ha van elegendõ energy, majd lenyobjuk a bal egérgombot, akkor létrehozunk egy lövedéket és a nyomvatartásával
    /// tudjuk növelni a méretét amivel lassabb lesz, de nagyobbat fog sebezni.
    /// </summary>
    private void Update()
    {
        if (stats.energy >= shotCost && !menu)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // A bal egér lenyomásával létrehozzunk egy lövedéket, majd beállítjuk hogy mozogjon a játékossal, a helyzetét és a méretét
                bullet = Instantiate(pfBullet, new Vector3(0, 0, 0), Quaternion.identity);
                bullet.transform.parent = Player.transform;
                bullet.transform.localPosition = new Vector3(0, 1.23f, 0);
                bullet.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            }
            if (Input.GetMouseButton(0))
            {
                // Ha nyomvatartjuk a bal egérgombot és nem érte el a maximális méretét akkor növeljük a löcedéket
                if ((bullet.transform.localScale.y < 1 && bullet.transform.localScale.x < 1 && bullet.transform.localScale.z < 1))
                {
                    bullet.transform.localPosition += new Vector3(0, 0.0005f, 0);
                    bullet.transform.localScale += new Vector3(0.001f, 0.001f, 0.001f);
                }
                // Állítjuk a raycast segítségével a paricle effekt irányát
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit);
                bullet.GetComponent<Bullet>().SetParticle(hit.point);
            }
            if (Input.GetMouseButtonUp(0))
            {
                // Ha felengedjük a bal egérgombot akkor levonja a lövéshez szükséges energy-t, készítünk egy raycastet hogy meghatározzuk hogy hova kell lõni,
                // majd leválasztjuk a játékosról, átadjuk a célt a lövedéknek és kilõjük azt.
                stats.energy -= shotCost;
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit);
                bullet.transform.parent = null;
                bullet.GetComponent<Bullet>().SetParameter(hit.point);
                bullet.GetComponent<Bullet>().shooted = true;
            }
        }
    }
}




