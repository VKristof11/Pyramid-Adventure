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
    /// Ha van elegend� energy, majd lenyobjuk a bal eg�rgombot, akkor l�trehozunk egy l�ved�ket �s a nyomvatart�s�val
    /// tudjuk n�velni a m�ret�t amivel lassabb lesz, de nagyobbat fog sebezni.
    /// </summary>
    private void Update()
    {
        if (stats.energy >= shotCost && !menu)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // A bal eg�r lenyom�s�val l�trehozzunk egy l�ved�ket, majd be�ll�tjuk hogy mozogjon a j�t�kossal, a helyzet�t �s a m�ret�t
                bullet = Instantiate(pfBullet, new Vector3(0, 0, 0), Quaternion.identity);
                bullet.transform.parent = Player.transform;
                bullet.transform.localPosition = new Vector3(0, 1.23f, 0);
                bullet.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            }
            if (Input.GetMouseButton(0))
            {
                // Ha nyomvatartjuk a bal eg�rgombot �s nem �rte el a maxim�lis m�ret�t akkor n�velj�k a l�ced�ket
                if ((bullet.transform.localScale.y < 1 && bullet.transform.localScale.x < 1 && bullet.transform.localScale.z < 1))
                {
                    bullet.transform.localPosition += new Vector3(0, 0.0005f, 0);
                    bullet.transform.localScale += new Vector3(0.001f, 0.001f, 0.001f);
                }
                // �ll�tjuk a raycast seg�ts�g�vel a paricle effekt ir�ny�t
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit);
                bullet.GetComponent<Bullet>().SetParticle(hit.point);
            }
            if (Input.GetMouseButtonUp(0))
            {
                // Ha felengedj�k a bal eg�rgombot akkor levonja a l�v�shez sz�ks�ges energy-t, k�sz�t�nk egy raycastet hogy meghat�rozzuk hogy hova kell l�ni,
                // majd lev�lasztjuk a j�t�kosr�l, �tadjuk a c�lt a l�ved�knek �s kil�j�k azt.
                stats.energy -= shotCost;
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit);
                bullet.transform.parent = null;
                bullet.GetComponent<Bullet>().SetParameter(hit.point);
                bullet.GetComponent<Bullet>().shooted = true;
            }
        }
    }
}




