using System;
using System.Collections.Generic;
using System.Drawing;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Map : MonoBehaviour
{
    static System.Random rand = new System.Random();
    public NavMeshSurface surface;
    public GameObject preHouse;
    public static int size = 100;
    public float scale;
    public float waterLevel;
    public int houseDistance;
    public float waterDamage;
    public float waterCooldown;

    public Tile[,] grid = new Tile[size, size];
    private GameObject[,] tiles = new GameObject[size, size];
    private List<int[]> cord = new List<int[]>();
    private List<int[]> houseCords = new List<int[]>();
    private List<int[]> endHouseCords = new List<int[]>();
    public List<GameObject> houses = new List<GameObject>();
    public List<GameObject> endHouses = new List<GameObject>();
    private int houseId = 1;
    private bool builded = false;
    public GameObject endUI;
    public Move move;
    public FightSystem fight;
    public List<int[]> roads = new List<int[]>();

    /// <summary>
    /// Egy �j j�t�khoz p�ly�t gener�l
    /// </summary>
    public void NewGame()
    {
        NewMap();

        // Kezd� h�z lerak�sa �s aparam�terei be�ll�t�sa
        GameObject startHouse = Instantiate(preHouse, transform);
        startHouse.GetComponent<House>().id = $"{houseId}";
        houseId++;
        startHouse.GetComponent<House>().minAI = 0;
        startHouse.GetComponent<House>().maxAI = 0;
        houses.Add(startHouse);


        // P�lya tile-ok lerak�sa
        SpawnPlane();
        // Beszinezz�k a Tile-kat, lerakja a h�zakat, meghat�rozza a vizeket
        Build();
        // Fel�p�tj�k a NavMesh-t az AI-nak
        surface.BuildNavMesh();
        builded = true;
    }

    public void Load(List<HouseData> segedHouse, List<HouseData> segedEndHouse) 
    {
        // Kezd� h�z lerak�sa �s aparam�terei be�ll�t�sa
        GameObject startHouse = Instantiate(preHouse, transform);
        startHouse.GetComponent<House>().id = $"{houseId}";
        houseId++;
        startHouse.GetComponent<House>().minAI = 0;
        startHouse.GetComponent<House>().maxAI = 0;
        startHouse.GetComponent<House>().saveHouse = true;
        houses.Add(startHouse);
        // P�lya tile-ok lerak�sa
        SpawnPlane();
        // Beszinezz�k a Tile-kat, lerakja a h�zakat, meghat�rozza a vizeket
        LoadBuild(segedHouse, segedEndHouse);
        // Fel�p�tj�k a NavMesh-t az AI-nak
        surface.BuildNavMesh();
        builded = true;
    }

    private void LateUpdate()
    {
        if (builded && endHouses.Count == 0)
        {
            Debug.Log("END");
            //GameEnd();
        }
    }

    /// <summary>
    /// Egy �j p�ly�t hoz l�tre, megcsin�lja az utakat �s k�rnyezetet
    /// </summary>
    private void NewMap()
    {
        // A grid, tile el�ksz�t�se a size alapj�n
        grid = new Tile[size, size];
        tiles = new GameObject[size, size];


        // Noismap gener�l�sa
        (float xOffset, float yOffset) = (UnityEngine.Random.Range(-10000f, 10000f), UnityEngine.Random.Range(-10000f, 10000f));
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * scale + xOffset, y * scale + yOffset);
                grid[y, x] = new Tile(noiseValue, waterLevel, new Vector3((y - size / 2) * 10, 0, (x - size / 2) * 10));
            }
        }


        // Kezd�pont meghat�roz�sa a p�lya k�zep�n
        cord.Add(new int[] { size / 2, size / 2 });
        grid[size / 2, size / 2].SetPoint();


        // 8 db v�gpont gener�l�sa a p�lya sz�lein 
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                // A p�lya 5*5-ben van felv�gva, kiz�rolag a sarkokon �s az �lek k�zep�n van v�gpont �gy j�n ki a 8db
                if ((i == 0 && j == 2) ||
                    (i == 2 && j == 0) || (i == 2 && j == 4) ||
                                          (i == 4 && j == 2))
                {
                    // Addig keresi a pontot am�g le nem tudja rakni mivel v�zbe nem megy az �t
                    bool spawn;
                    do
                    {
                        // Random kordin�t�t gener�l
                        int y = UnityEngine.Random.Range(size / 5 * i, size / 5 * (i + 1));
                        int x = UnityEngine.Random.Range(size / 5 * j, size / 5 * (j + 1));
                        // Majd megn�zi hogy leteheti-e
                        if (grid[y, x].Type != 2)
                        {

                            // Ha igen akkor elmenti a cord list�ba ahol az �sszek�tend� pontokat t�roljuk
                            cord.Add(new int[] { y, x });           
                            // emelett elmenti a houseCords-ok k�z� mivel itt is lesz h�z
                            houseCords.Add(new int[] { y, x }); 
                            // be�ll�tja hogy sikeres volt �gy kitudunk l�pni a ciklusb�l
                            spawn = true;                           
                        }
                        else
                        {
                            spawn = false; // K�l�nbben sikertelen 
                        }
                    } while (!spawn);
                }
                if ((i == 0 && j == 0) || (i == 0 && j == 4)
                                                  ||
                    (i == 4 && j == 0) || (i == 4 && j == 4))
                {
                    // Addig keresi a pontot am�g le nem tudja rakni mivel v�zbe nem megy az �t
                    bool spawn;
                    do
                    {
                        // Random kordin�t�t gener�l
                        int y = UnityEngine.Random.Range(size / 5 * i, size / 5 * (i + 1));
                        int x = UnityEngine.Random.Range(size / 5 * j, size / 5 * (j + 1));
                        // Majd megn�zi hogy leteheti-e
                        if (grid[y, x].Type != 2)
                        {

                            // Ha igen akkor elmenti a cord list�ba ahol az �sszek�tend� pontokat t�roljuk
                            cord.Add(new int[] { y, x });
                            // emelett elmenti a endHouseCords-ok k�z� mivel itt lesz az egyik v�gpont
                            endHouseCords.Add(new int[] { y, x });
                            // be�ll�tja hogy sikeres volt �gy kitudunk l�pni a ciklusb�l
                            spawn = true;
                        }
                        else
                        {
                            spawn = false; // K�l�nbben sikertelen 
                        }
                    } while (!spawn);
                }
            }
        }


        // �t gener�l�sa a 8 db v�gpont �s a kezd�pont k�z�tt
        for (int i = 1; i <  cord.Count; i++)
        {
            PathFinding2(cord[i], new int[] { size / 2, size / 2 });
        }

        // esetleg kitr�r�lhet�
        grid[size / 2, size / 2].SetPoint();

        // H�zak pontos hely�nek meghat�roz�sa
        foreach (var item in houseCords)
        {
            // Ellen�rizz�k hogy a k�rny�ken nincs-e t�l k�zel h�z
            bool thereHouse = false;
            for (int g = -5; g < 5; g++)
            {
                for (int h = -5; h < 5; h++)
                {
                    // Ellen�rizz�k hogy nem-e indexel�nk ki a grid-b�l
                    if ((item[0] + g >= 0 && item[0] + g < grid.GetLength(0)) && (item[1] + h >= 0 && item[1] + h < grid.GetLength(1)))
                    {
                        // Megn�zz�k hogy nics-e a k�rny�ken h�z
                        if (grid[item[0] + g, item[1] + h].Type == 6)
                        {
                            thereHouse = true;  // Ha van akkor be�ll�tjuk igazra
                            break;              // �s le�ll�tjuk a ciklust
                        }
                    }
                }
                if (thereHouse)
                {
                    break; // Le�ll�tjuk a m�sik ciklust is ha van h�z a k�rny�ken
                }
            }
            // Ha nincs h�z akkor lerakjuk a h�zat 
            if (!thereHouse)
            {
                // Pont meghat�roz�sa az �ton (az �tra nem rakunk h�zat)
                grid[item[0], item[1]].SetPoint();

                // Megn�zz�k hogy nem-e indexel�nk ki a grid-b�l
                if ((item[0] + 1 < grid.GetLength(0) && item[1] + 1 < grid.GetLength(1)) && (item[0] - 1 >= 0 && item[1] - 1 >= 0))
                {
                    // Eld�nj�k hogy ett�l a pont-t�l melyik ir�nyba legyen a h�z
                    if (grid[item[0], item[1] + 1].Type == 1)
                    {
                        grid[item[0], item[1] + 1].SetHousePoint();
                    }         //  0, +1 (jobbra)
                    else if (grid[item[0] + 1, item[1]].Type == 1)
                    {
                        grid[item[0] + 1, item[1]].SetHousePoint();
                    }    // +1,  0 (fel)
                    else if (grid[item[0] + 1, item[1] + 1].Type == 1)
                    {
                        grid[item[0] + 1, item[1] + 1].SetHousePoint();
                    }// +1, +1 (fel, jobbra)
                    else if (grid[item[0], item[1] - 1].Type == 1)
                    {
                        grid[item[0], item[1] - 1].SetHousePoint();
                    }    //  0, -1 (balra)
                    else if (grid[item[0] - 1, item[1] + 1].Type == 1)
                    {
                        grid[item[0] - 1, item[1] + 1].SetHousePoint();
                    }// -1, +1 (le, jobbra)
                    else if (grid[item[0] + 1, item[1] - 1].Type == 1)
                    {
                        grid[item[0] + 1, item[1] - 1].SetHousePoint();
                    }// +1, -1 (fel, balra)
                    else if (grid[item[0] - 1, item[1] - 1].Type == 1)
                    {
                        grid[item[0] - 1, item[1] - 1].SetHousePoint();
                    }// -1, -1 (le, balra)
                    else if (grid[item[0] - 1, item[1]].Type == 1)
                    {
                        grid[item[0] - 1, item[1]].SetHousePoint();
                    }    // -1,  0 (le)
                }
            }

        }

        foreach (var item in endHouseCords)
        {
            // Ellen�rizz�k hogy a k�rny�ken nincs e t�l k�zel h�z
            bool thereHouse = false;
            for (int g = -5; g < 5; g++)
            {
                for (int h = -5; h < 5; h++)
                {
                    // Ellen�rizz�k hogy nem e indexel�nk ki a grid-b�l
                    if ((item[0] + g >= 0 && item[0] + g < grid.GetLength(0)) && (item[1] + h >= 0 && item[1] + h < grid.GetLength(1)))
                    {
                        // Megn�zz�k hogy nics e a k�rny�ken h�z
                        if (grid[item[0] + g, item[1] + h].Type == 6)
                        {
                            thereHouse = true;  // Ha van akkor be�ll�tjuk igazra
                            break;              // �s le�ll�tjuk a ciklust
                        }
                    }
                }
                if (thereHouse)
                {
                    break; // Le�ll�tjuk a m�sik ciklust is ha van h�z a k�rny�ken
                }
            }
            // Ha nincs h�z akkor lerakjuk a h�zat 
            if (!thereHouse)
            {
                // Pont meghat�roz�sa az �ton (az �tra nem rakunk h�zat)
                grid[item[0], item[1]].SetPoint();

                // Megn�zz�k hogy nem-e indexel�nk ki a grid-b�l
                if ((item[0] + 1 < grid.GetLength(0) && item[1] + 1 < grid.GetLength(1)) && (item[0] - 1 >= 0 && item[1] - 1 >= 0))
                {
                    // Eld�nj�k hogy ett�l a pont-t�l melyik ir�nyba legyen a h�z
                    if (grid[item[0], item[1] + 1].Type == 1)
                    {
                        grid[item[0], item[1] + 1].SetEndHousePoint();
                    }         //  0, +1 (jobbra)
                    else if (grid[item[0] + 1, item[1]].Type == 1)
                    {
                        grid[item[0] + 1, item[1]].SetEndHousePoint();
                    }    // +1,  0 (fel)
                    else if (grid[item[0] + 1, item[1] + 1].Type == 1)
                    {
                        grid[item[0] + 1, item[1] + 1].SetEndHousePoint();
                    }// +1, +1 (fel, jobbra)
                    else if (grid[item[0], item[1] - 1].Type == 1)
                    {
                        grid[item[0], item[1] - 1].SetEndHousePoint();
                    }    //  0, -1 (balra)
                    else if (grid[item[0] - 1, item[1] + 1].Type == 1)
                    {
                        grid[item[0] - 1, item[1] + 1].SetEndHousePoint();
                    }// -1, +1 (le, jobbra)
                    else if (grid[item[0] + 1, item[1] - 1].Type == 1)
                    {
                        grid[item[0] + 1, item[1] - 1].SetEndHousePoint();
                    }// +1, -1 (fel, balra)
                    else if (grid[item[0] - 1, item[1] - 1].Type == 1)
                    {
                        grid[item[0] - 1, item[1] - 1].SetEndHousePoint();
                    }// -1, -1 (le, balra)
                    else if (grid[item[0] - 1, item[1]].Type == 1)
                    {
                        grid[item[0] - 1, item[1]].SetEndHousePoint();
                    }    // -1,  0 (le)
                }
            }

        }
    }


    /// <summary>
    /// Utat kerese a kett� kordin�ta k�z�tt
    /// </summary>
    /// <param name="start">Az els� kordin�t</param>
    /// <param name="end">A m�sodik kordin�ta</param>
    private void PathFinding(int[] start, int[] end)
    {
        int db = 0;
        float[,] cost = new float[size, size];
        int i = start[0], j = start[1];
        bool[] prevMove = new bool[] { true, true, true, true };
        // Addidg keresi ameddig el nem �r a start-r�l az end-ig
        while (i != end[0] || j != end[1])
        {
            // Egy kil�p�si felt�tel hogy ne ker�lj�n v�gtelen ciklusba
            if (db++ > 200 * (size + size))
            {
                return;
            }
            for (int k = -1; k < 2; k++)
            {
                for (int l = -1; l < 2; l++)
                {
                    // Csak akkor l�p be ha nem indexel ki a t�mbb�l �s kiz�rja az �tl�kat
                    if ((i + k >= 0 && i + k < cost.GetLength(0) && j + l >= 0 && j + l < cost.GetLength(1)) && (k == 0 || l == 0))
                    {
                        float vx = end[1] - (j + l);
                        float vy = end[0] - (i + k);
                        float vLength = (float)Math.Sqrt(vx * vx + vy * vy);
                        if (cost[i + k, j + l] != 0)
                        {
                            cost[i + k, j + l] *= 5;
                        }
                        cost[i + k, j + l] += vLength * 5 + grid[i + k, j + l].Cost + (float)(rand.NextDouble() % 0.6);
                    }
                }
            }

            List<float> costs = new List<float>();
            List<int[]> cords = new List<int[]>();
            if (i - 1 >= 0 && prevMove[0])
            {
                costs.Add(cost[i - 1, j]);
                cords.Add(new int[] { i - 1, j, 1 });
            }
            if (i + 1 <= cost.GetLength(0) - 1 && prevMove[1])
            {
                costs.Add(cost[i + 1, j]);
                cords.Add(new int[] { i + 1, j, 2 });
            }
            if (j - 1 >= 0 && prevMove[2])
            {
                costs.Add(cost[i, j - 1]);
                cords.Add(new int[] { i, j - 1, 3 });
            }
            if (j + 1 <= cost.GetLength(0) - 1 && prevMove[3])
            {
                costs.Add(cost[i, j + 1]);
                cords.Add(new int[] { i, j + 1, 4 });
            }
            int min = 0;
            for (int n = 1; n < costs.Count; n++)
            {
                if (costs[min] > costs[n])
                {
                    min = n;
                }
            }
            i = cords[min][0];
            j = cords[min][1];
            switch (cords[min][2])
            {
                case 1:
                    prevMove[0] = true;
                    prevMove[1] = false;
                    prevMove[2] = true;
                    prevMove[3] = true;
                    break;
                case 2:
                    prevMove[0] = false;
                    prevMove[1] = true;
                    prevMove[2] = true;
                    prevMove[3] = true;
                    break;
                case 3:
                    prevMove[0] = true;
                    prevMove[1] = true;
                    prevMove[2] = true;
                    prevMove[3] = false;
                    break;
                case 4:
                    prevMove[0] = true;
                    prevMove[1] = true;
                    prevMove[2] = false;
                    prevMove[3] = true;
                    break;
            }
            grid[i, j].SetPath();
            // Ha elment�nk houseDistance t�vols�got akkor lerakunk egy h�zat
            if (db % houseDistance == 0)
            {
                // Elmentj�k a h�z kordin�t�j�t
                houseCords.Add(new int[] { i, j });
            }
            cost[i, j] = int.MaxValue;
        }
    }

    private void PathFinding3(int[] start, int[] end) 
    {
        int db = 0;
        float[,] cost = new float[size, size];
        int i = start[0], j = start[1];
        bool[] prevMove = new bool[] { true, true, true, true };
        // Addidg keresi ameddig el nem �r a start-r�l az end-ig
        while (i != end[0] || j != end[1])
        {
            // Egy kil�p�si felt�tel hogy ne ker�lj�n v�gtelen ciklusba
            if (db++ > 200 * (size + size))
            {
                return;
            }
            for (int k = -1; k < 2; k++)
            {
                for (int l = -1; l < 2; l++)
                {
                    // Csak akkor l�p be ha nem indexel ki a t�mbb�l �s kiz�rja az �tl�kat
                    if ((i + k >= 0 && i + k < cost.GetLength(0) && j + l >= 0 && j + l < cost.GetLength(1)) && (k == 0 || l == 0))
                    {
                        float vx = end[1] - (j + l);
                        float vy = end[0] - (i + k);
                        float vLength = (float)Math.Sqrt(vx * vx + vy * vy);
                        cost[i + k, j + l] += vLength * 100 + grid[i + k, j + l].Cost;
                    }
                }
            }

            List<float> costs = new List<float>();
            List<int[]> cords = new List<int[]>();
            if (i - 1 >= 0 && prevMove[0])
            {
                costs.Add(cost[i - 1, j]);
                cords.Add(new int[] { i - 1, j, 1 });
            }
            if (i + 1 <= cost.GetLength(0) - 1 && prevMove[1])
            {
                costs.Add(cost[i + 1, j]);
                cords.Add(new int[] { i + 1, j, 2 });
            }
            if (j - 1 >= 0 && prevMove[2])
            {
                costs.Add(cost[i, j - 1]);
                cords.Add(new int[] { i, j - 1, 3 });
            }
            if (j + 1 <= cost.GetLength(0) - 1 && prevMove[3])
            {
                costs.Add(cost[i, j + 1]);
                cords.Add(new int[] { i, j + 1, 4 });
            }
            int min = 0;
            for (int n = 1; n < costs.Count; n++)
            {
                if (costs[min] > costs[n])
                {
                    min = n;
                }
            }
            i = cords[min][0];
            j = cords[min][1];
            switch (cords[min][2])
            {
                case 1:
                    prevMove[0] = true;
                    prevMove[1] = false;
                    prevMove[2] = true;
                    prevMove[3] = true;
                    break;
                case 2:
                    prevMove[0] = false;
                    prevMove[1] = true;
                    prevMove[2] = true;
                    prevMove[3] = true;
                    break;
                case 3:
                    prevMove[0] = true;
                    prevMove[1] = true;
                    prevMove[2] = true;
                    prevMove[3] = false;
                    break;
                case 4:
                    prevMove[0] = true;
                    prevMove[1] = true;
                    prevMove[2] = false;
                    prevMove[3] = true;
                    break;
            }
            grid[i, j].SetPath();
            // Ha elment�nk houseDistance t�vols�got akkor lerakunk egy h�zat
            if (db % houseDistance == 0)
            {
                // Elmentj�k a h�z kordin�t�j�t
                houseCords.Add(new int[] { i, j });
            }
            cost[i, j] = int.MaxValue;
        }
    }

    private void PathFinding2(int[] start, int[] end)
    {

        int db = 0;
        float[,] cost = new float[size, size];
        int i = start[0], j = start[1];
        bool[] prevMove = new bool[] { true, true, true, true };
        while (i != end[0] || j != end[1])
        {
            if (db++ > 200 * (size + size))
            {
                return;
            }
            for (int k = -1; k < 2; k++)
            {
                for (int l = -1; l < 2; l++)
                {
                    if ((i + k >= 0 && i + k < cost.GetLength(0) && 
                        j + l >= 0 && j + l < cost.GetLength(1)) && 
                        (k == 0 || l == 0))
                    {
                        float vx = end[1] - (j + l);
                        float vy = end[0] - (i + k);
                        float vLength = (float)Math.Sqrt(vx * vx + vy * vy);
                        cost[i + k, j + l] += vLength * 100 + grid[i + k, j + l].Cost;
                    }
                }
            }

            List<float> costs = new List<float>();
            List<int[]> cords = new List<int[]>();
            if (i - 1 >= 0 && prevMove[0])
            {
                costs.Add(cost[i - 1, j]);
                cords.Add(new int[] { i - 1, j, 1 });
            }
            if (i + 1 <= cost.GetLength(0) - 1 && prevMove[1])
            {
                costs.Add(cost[i + 1, j]);
                cords.Add(new int[] { i + 1, j, 2 });
            }
            if (j - 1 >= 0 && prevMove[2])
            {
                costs.Add(cost[i, j - 1]);
                cords.Add(new int[] { i, j - 1, 3 });
            }
            if (j + 1 <= cost.GetLength(0) - 1 && prevMove[3])
            {
                costs.Add(cost[i, j + 1]);
                cords.Add(new int[] { i, j + 1, 4 });
            }
            int min = 0;
            for (int n = 1; n < costs.Count; n++)
            {
                if (costs[min] > costs[n])
                {
                    min = n;
                }
            }
            i = cords[min][0];
            j = cords[min][1];

            switch (cords[min][2])
            {
                case 1:
                    prevMove[0] = true;
                    prevMove[1] = false;
                    prevMove[2] = true;
                    prevMove[3] = true;
                    break;
                case 2:
                    prevMove[0] = false;
                    prevMove[1] = true;
                    prevMove[2] = true;
                    prevMove[3] = true;
                    break;
                case 3:
                    prevMove[0] = true;
                    prevMove[1] = true;
                    prevMove[2] = true;
                    prevMove[3] = false;
                    break;
                case 4:
                    prevMove[0] = true;
                    prevMove[1] = true;
                    prevMove[2] = false;
                    prevMove[3] = true;
                    break;
            }
            grid[i, j].SetPath();
            if (db % houseDistance == 0)
            {
                houseCords.Add(new int[] { i, j });
            }
            cost[i, j] = int.MaxValue;
        }
    }

    /// <summary>
    /// L�terhozza az alap tile-kat �s elhejezz�k a p�ly�nt �ket 
    /// </summary>
    private void SpawnPlane()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                // L�trehoz egy egysz� Plane-t, majd a pozici�j�t be�ll�tja a p�lya m�rete �s az alapj�n
                // hogy h�nyadikn�l j�r, majd hozz�adja (elmenti) a tiles m�trixban
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
                obj.transform.position = new Vector3((i - size / 2) * 10, 0, (j - size / 2) * 10);
                tiles[i, j] = obj;
            }
        }
    }


    /// <summary>
    /// Be�ll�tja a tile-okat a param�tereknek megfelel�en �s l�trehozza a h�zakat
    /// </summary>
    private void Build()
    {
        // V�gigmegy a tiles m�trixon
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                // Az adott tile-nak megszerzi a renderer-j�t majd a grid-ben t�rolt adatok alapj�n meghat�rozza a sz�n�t
                tiles[i,j].GetComponent<Renderer>().material.color = grid[i, j].Color;
                // Ha a type-ja 2-es (ami vizet jelent), akkor
                if (grid[i,j].Type == 2)
                {
                    // Adneki egy NavMeshObstacle-t hogy ne tudjanak r�l�pni a vizre az AI-k
                    tiles[i, j].AddComponent<NavMeshObstacle>();
                    // kap egy BoxCollider-t aminek be�ll�tja a m�ret�t �s hogy Trigger legyen
                    // (amivel tudjuk �rz�kelni ha valami bel�p a ter�letre)
                    tiles[i, j].AddComponent<BoxCollider>();
                    tiles[i,j].GetComponent<BoxCollider>().size = new Vector3(9, 5, 9);
                    tiles[i,j].GetComponent<BoxCollider>().isTrigger = true;
                    // Megkapja a Water class-t �s be�ll�tja a parm�tereit
                    tiles[i, j].AddComponent<Water>();
                    tiles[i, j].GetComponent<Water>().damage = waterDamage;
                    tiles[i, j].GetComponent<Water>().cooldown = waterCooldown;
                    tiles[i, j].name = "Water";
                }
                else if (grid[i,j].Type == 7)
                {
                    // Ha a type-ja 7-es (ami h�zat jelent), akkor l�trehoz egy h�zat a preHouse alapj�n,
                    // ad neki egy id-t, majd menti a houses list�ban
                    GameObject house = Instantiate(preHouse, tiles[i, j].transform);
                    house.GetComponent<House>().id = $"{houseId}";
                    house.GetComponent<House>().gridPos = new Vector2(tiles[i, j].transform.position.x, tiles[i, j].transform.position.z);
                    houseId++;
                    houses.Add(house);
                }
                else if (grid[i, j].Type == 8)
                {
                    // Ha a type-ja 8-es (ami end h�zat jelent), akkor l�trehoz egy h�zat a preHouse alapj�n,
                    // ad neki egy id-t, be�ll�tja end-nek, majd menti az endHouses list�ban
                    GameObject house = Instantiate(preHouse, tiles[i, j].transform);
                    house.GetComponent<House>().end = true;
                    house.GetComponent<House>().id = $"{houseId}";
                    house.GetComponent<House>().gridPos = new Vector2(tiles[i, j].transform.position.x, tiles[i, j].transform.position.z);
                    endHouses.Add(house);
                }

            }
        }
    }

    /// <summary>
    /// Be�ll�tja a tile-okat a param�tereknek megfelel�en �s l�trehozza a h�zakat a ment�sb�l
    /// </summary>
    private void LoadBuild(List<HouseData> segedHouse, List<HouseData> segedEndHouse)
    {
        // V�gigmegy a tiles m�trixon
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                // Az adott tile-nak megszerzi a renderer-j�t majd a grid-ben t�rolt adatok alapj�n meghat�rozza a sz�n�t
                tiles[i, j].GetComponent<Renderer>().material.color = grid[i, j].Color;
                // Ha a type-ja 2-es (ami vizet jelent), akkor
                if (grid[i, j].Type == 2)
                {
                    // Adneki egy NavMeshObstacle-t hogy ne tudjanak r�l�pni a vizre az AI-k
                    tiles[i, j].AddComponent<NavMeshObstacle>();
                    // kap egy BoxCollider-t aminek be�ll�tja a m�ret�t �s hogy Trigger legyen
                    // (amivel tudjuk �rz�kelni ha valami bel�p a ter�letre)
                    tiles[i, j].AddComponent<BoxCollider>();
                    tiles[i, j].GetComponent<BoxCollider>().size = new Vector3(9, 5, 9);
                    tiles[i, j].GetComponent<BoxCollider>().isTrigger = true;
                    // Megkapja a Water class-t �s be�ll�tja a parm�tereit
                    tiles[i, j].AddComponent<Water>();
                    tiles[i, j].GetComponent<Water>().damage = waterDamage;
                    tiles[i, j].GetComponent<Water>().cooldown = waterCooldown;
                    tiles[i, j].name = "Water";
                }
                else if (grid[i, j].Type == 7)
                {
                    // Ha a type-ja 7-es (ami h�zat jelent), akkor l�trehoz egy h�zat a preHouse alapj�n,
                    // ad neki egy id-t, majd menti a houses list�ban
                    foreach (var item in segedHouse)
                    {
                        if (tiles[i,j].transform.position.x == item.position[0] && tiles[i, j].transform.position.z == item.position[1])
                        {
                            GameObject house = Instantiate(preHouse, tiles[i, j].transform);
                            house.GetComponent<House>().id = item.id;
                            house.GetComponent<House>().saveHouse = item.saveHouse;
                            house.GetComponent<House>().end = item.end;
                            house.GetComponent<House>().gridPos = new Vector2(item.position[0], item.position[1]);
                            house.GetComponent<House>().Load(item.listAI);

                            houseId++;
                            houses.Add(house);
                        }
                    }
                }
                else if (grid[i, j].Type == 8)
                {
                    // Ha a type-ja 8-es (ami end h�zat jelent), akkor l�trehoz egy h�zat a preHouse alapj�n,
                    // ad neki egy id-t, be�ll�tja end-nek, majd menti az endHouses list�ban
                    foreach (var item in segedEndHouse)
                    {
                        if (tiles[i, j].transform.position.x == item.position[0] && tiles[i, j].transform.position.z == item.position[1])
                        {
                            GameObject house = Instantiate(preHouse, tiles[i, j].transform);
                            house.GetComponent<House>().id = item.id;
                            house.GetComponent<House>().saveHouse = item.saveHouse;
                            house.GetComponent<House>().end = item.end;
                            house.GetComponent<House>().gridPos = new Vector2(item.position[0], item.position[1]);
                            house.GetComponent<House>().Load(item.listAI);

                            houseId++;
                            endHouses.Add(house);
                        }
                    }
                }

            }
        }
    }


    private void GameEnd()
    {
        move.menu = true;
        fight.menu = true;
        endUI.SetActive(true);
    }
}