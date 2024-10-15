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
    /// Egy új játékhoz pályát generál
    /// </summary>
    public void NewGame()
    {
        NewMap();

        // Kezdõ ház lerakása és aparaméterei beállítása
        GameObject startHouse = Instantiate(preHouse, transform);
        startHouse.GetComponent<House>().id = $"{houseId}";
        houseId++;
        startHouse.GetComponent<House>().minAI = 0;
        startHouse.GetComponent<House>().maxAI = 0;
        houses.Add(startHouse);


        // Pálya tile-ok lerakása
        SpawnPlane();
        // Beszinezzük a Tile-kat, lerakja a házakat, meghatározza a vizeket
        Build();
        // Felépítjük a NavMesh-t az AI-nak
        surface.BuildNavMesh();
        builded = true;
    }

    public void Load(List<HouseData> segedHouse, List<HouseData> segedEndHouse) 
    {
        // Kezdõ ház lerakása és aparaméterei beállítása
        GameObject startHouse = Instantiate(preHouse, transform);
        startHouse.GetComponent<House>().id = $"{houseId}";
        houseId++;
        startHouse.GetComponent<House>().minAI = 0;
        startHouse.GetComponent<House>().maxAI = 0;
        startHouse.GetComponent<House>().saveHouse = true;
        houses.Add(startHouse);
        // Pálya tile-ok lerakása
        SpawnPlane();
        // Beszinezzük a Tile-kat, lerakja a házakat, meghatározza a vizeket
        LoadBuild(segedHouse, segedEndHouse);
        // Felépítjük a NavMesh-t az AI-nak
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
    /// Egy új pályát hoz létre, megcsinálja az utakat és környezetet
    /// </summary>
    private void NewMap()
    {
        // A grid, tile elékszítése a size alapján
        grid = new Tile[size, size];
        tiles = new GameObject[size, size];


        // Noismap generálása
        (float xOffset, float yOffset) = (UnityEngine.Random.Range(-10000f, 10000f), UnityEngine.Random.Range(-10000f, 10000f));
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * scale + xOffset, y * scale + yOffset);
                grid[y, x] = new Tile(noiseValue, waterLevel, new Vector3((y - size / 2) * 10, 0, (x - size / 2) * 10));
            }
        }


        // Kezdõpont meghatározása a pálya közepén
        cord.Add(new int[] { size / 2, size / 2 });
        grid[size / 2, size / 2].SetPoint();


        // 8 db végpont generálása a pálya szélein 
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                // A pálya 5*5-ben van felvágva, kizárolag a sarkokon és az élek közepén van végpont így jön ki a 8db
                if ((i == 0 && j == 2) ||
                    (i == 2 && j == 0) || (i == 2 && j == 4) ||
                                          (i == 4 && j == 2))
                {
                    // Addig keresi a pontot amíg le nem tudja rakni mivel vízbe nem megy az út
                    bool spawn;
                    do
                    {
                        // Random kordinátát generál
                        int y = UnityEngine.Random.Range(size / 5 * i, size / 5 * (i + 1));
                        int x = UnityEngine.Random.Range(size / 5 * j, size / 5 * (j + 1));
                        // Majd megnézi hogy leteheti-e
                        if (grid[y, x].Type != 2)
                        {

                            // Ha igen akkor elmenti a cord listába ahol az összekötendõ pontokat tároljuk
                            cord.Add(new int[] { y, x });           
                            // emelett elmenti a houseCords-ok közé mivel itt is lesz ház
                            houseCords.Add(new int[] { y, x }); 
                            // beállítja hogy sikeres volt így kitudunk lépni a ciklusból
                            spawn = true;                           
                        }
                        else
                        {
                            spawn = false; // Különbben sikertelen 
                        }
                    } while (!spawn);
                }
                if ((i == 0 && j == 0) || (i == 0 && j == 4)
                                                  ||
                    (i == 4 && j == 0) || (i == 4 && j == 4))
                {
                    // Addig keresi a pontot amíg le nem tudja rakni mivel vízbe nem megy az út
                    bool spawn;
                    do
                    {
                        // Random kordinátát generál
                        int y = UnityEngine.Random.Range(size / 5 * i, size / 5 * (i + 1));
                        int x = UnityEngine.Random.Range(size / 5 * j, size / 5 * (j + 1));
                        // Majd megnézi hogy leteheti-e
                        if (grid[y, x].Type != 2)
                        {

                            // Ha igen akkor elmenti a cord listába ahol az összekötendõ pontokat tároljuk
                            cord.Add(new int[] { y, x });
                            // emelett elmenti a endHouseCords-ok közé mivel itt lesz az egyik végpont
                            endHouseCords.Add(new int[] { y, x });
                            // beállítja hogy sikeres volt így kitudunk lépni a ciklusból
                            spawn = true;
                        }
                        else
                        {
                            spawn = false; // Különbben sikertelen 
                        }
                    } while (!spawn);
                }
            }
        }


        // Út generálása a 8 db végpont és a kezdõpont között
        for (int i = 1; i <  cord.Count; i++)
        {
            PathFinding2(cord[i], new int[] { size / 2, size / 2 });
        }

        // esetleg kitrörölhetõ
        grid[size / 2, size / 2].SetPoint();

        // Házak pontos helyének meghatározása
        foreach (var item in houseCords)
        {
            // Ellenörizzük hogy a környéken nincs-e túl közel ház
            bool thereHouse = false;
            for (int g = -5; g < 5; g++)
            {
                for (int h = -5; h < 5; h++)
                {
                    // Ellenõrizzük hogy nem-e indexelünk ki a grid-bõl
                    if ((item[0] + g >= 0 && item[0] + g < grid.GetLength(0)) && (item[1] + h >= 0 && item[1] + h < grid.GetLength(1)))
                    {
                        // Megnézzük hogy nics-e a környéken ház
                        if (grid[item[0] + g, item[1] + h].Type == 6)
                        {
                            thereHouse = true;  // Ha van akkor beállítjuk igazra
                            break;              // és leállítjuk a ciklust
                        }
                    }
                }
                if (thereHouse)
                {
                    break; // Leállítjuk a másik ciklust is ha van ház a környéken
                }
            }
            // Ha nincs ház akkor lerakjuk a házat 
            if (!thereHouse)
            {
                // Pont meghatározása az úton (az útra nem rakunk házat)
                grid[item[0], item[1]].SetPoint();

                // Megnézzük hogy nem-e indexelünk ki a grid-bõl
                if ((item[0] + 1 < grid.GetLength(0) && item[1] + 1 < grid.GetLength(1)) && (item[0] - 1 >= 0 && item[1] - 1 >= 0))
                {
                    // Eldönjük hogy ettõl a pont-tól melyik irányba legyen a ház
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
            // Ellenörizzük hogy a környéken nincs e túl közel ház
            bool thereHouse = false;
            for (int g = -5; g < 5; g++)
            {
                for (int h = -5; h < 5; h++)
                {
                    // Ellenõrizzük hogy nem e indexelünk ki a grid-bõl
                    if ((item[0] + g >= 0 && item[0] + g < grid.GetLength(0)) && (item[1] + h >= 0 && item[1] + h < grid.GetLength(1)))
                    {
                        // Megnézzük hogy nics e a környéken ház
                        if (grid[item[0] + g, item[1] + h].Type == 6)
                        {
                            thereHouse = true;  // Ha van akkor beállítjuk igazra
                            break;              // és leállítjuk a ciklust
                        }
                    }
                }
                if (thereHouse)
                {
                    break; // Leállítjuk a másik ciklust is ha van ház a környéken
                }
            }
            // Ha nincs ház akkor lerakjuk a házat 
            if (!thereHouse)
            {
                // Pont meghatározása az úton (az útra nem rakunk házat)
                grid[item[0], item[1]].SetPoint();

                // Megnézzük hogy nem-e indexelünk ki a grid-bõl
                if ((item[0] + 1 < grid.GetLength(0) && item[1] + 1 < grid.GetLength(1)) && (item[0] - 1 >= 0 && item[1] - 1 >= 0))
                {
                    // Eldönjük hogy ettõl a pont-tól melyik irányba legyen a ház
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
    /// Utat kerese a kettõ kordináta között
    /// </summary>
    /// <param name="start">Az elsõ kordinát</param>
    /// <param name="end">A második kordináta</param>
    private void PathFinding(int[] start, int[] end)
    {
        int db = 0;
        float[,] cost = new float[size, size];
        int i = start[0], j = start[1];
        bool[] prevMove = new bool[] { true, true, true, true };
        // Addidg keresi ameddig el nem ér a start-ról az end-ig
        while (i != end[0] || j != end[1])
        {
            // Egy kilépési feltétel hogy ne kerüljön végtelen ciklusba
            if (db++ > 200 * (size + size))
            {
                return;
            }
            for (int k = -1; k < 2; k++)
            {
                for (int l = -1; l < 2; l++)
                {
                    // Csak akkor lép be ha nem indexel ki a tömbbõl és kizárja az átlókat
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
            // Ha elmentünk houseDistance távolságot akkor lerakunk egy házat
            if (db % houseDistance == 0)
            {
                // Elmentjük a ház kordinátáját
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
        // Addidg keresi ameddig el nem ér a start-ról az end-ig
        while (i != end[0] || j != end[1])
        {
            // Egy kilépési feltétel hogy ne kerüljön végtelen ciklusba
            if (db++ > 200 * (size + size))
            {
                return;
            }
            for (int k = -1; k < 2; k++)
            {
                for (int l = -1; l < 2; l++)
                {
                    // Csak akkor lép be ha nem indexel ki a tömbbõl és kizárja az átlókat
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
            // Ha elmentünk houseDistance távolságot akkor lerakunk egy házat
            if (db % houseDistance == 0)
            {
                // Elmentjük a ház kordinátáját
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
    /// Léterhozza az alap tile-kat és elhejezzük a pályánt õket 
    /// </summary>
    private void SpawnPlane()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                // Létrehoz egy egyszû Plane-t, majd a pozicióját beállítja a pálya mérete és az alapján
                // hogy hányadiknál jár, majd hozzáadja (elmenti) a tiles mátrixban
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
                obj.transform.position = new Vector3((i - size / 2) * 10, 0, (j - size / 2) * 10);
                tiles[i, j] = obj;
            }
        }
    }


    /// <summary>
    /// Beállítja a tile-okat a paramétereknek megfelelõen és létrehozza a házakat
    /// </summary>
    private void Build()
    {
        // Végigmegy a tiles mátrixon
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                // Az adott tile-nak megszerzi a renderer-jét majd a grid-ben tárolt adatok alapján meghatározza a színét
                tiles[i,j].GetComponent<Renderer>().material.color = grid[i, j].Color;
                // Ha a type-ja 2-es (ami vizet jelent), akkor
                if (grid[i,j].Type == 2)
                {
                    // Adneki egy NavMeshObstacle-t hogy ne tudjanak rálépni a vizre az AI-k
                    tiles[i, j].AddComponent<NavMeshObstacle>();
                    // kap egy BoxCollider-t aminek beállítja a méretét és hogy Trigger legyen
                    // (amivel tudjuk érzékelni ha valami belép a területre)
                    tiles[i, j].AddComponent<BoxCollider>();
                    tiles[i,j].GetComponent<BoxCollider>().size = new Vector3(9, 5, 9);
                    tiles[i,j].GetComponent<BoxCollider>().isTrigger = true;
                    // Megkapja a Water class-t és beállítja a parmétereit
                    tiles[i, j].AddComponent<Water>();
                    tiles[i, j].GetComponent<Water>().damage = waterDamage;
                    tiles[i, j].GetComponent<Water>().cooldown = waterCooldown;
                    tiles[i, j].name = "Water";
                }
                else if (grid[i,j].Type == 7)
                {
                    // Ha a type-ja 7-es (ami házat jelent), akkor létrehoz egy házat a preHouse alapján,
                    // ad neki egy id-t, majd menti a houses listában
                    GameObject house = Instantiate(preHouse, tiles[i, j].transform);
                    house.GetComponent<House>().id = $"{houseId}";
                    house.GetComponent<House>().gridPos = new Vector2(tiles[i, j].transform.position.x, tiles[i, j].transform.position.z);
                    houseId++;
                    houses.Add(house);
                }
                else if (grid[i, j].Type == 8)
                {
                    // Ha a type-ja 8-es (ami end házat jelent), akkor létrehoz egy házat a preHouse alapján,
                    // ad neki egy id-t, beállítja end-nek, majd menti az endHouses listában
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
    /// Beállítja a tile-okat a paramétereknek megfelelõen és létrehozza a házakat a mentésbõl
    /// </summary>
    private void LoadBuild(List<HouseData> segedHouse, List<HouseData> segedEndHouse)
    {
        // Végigmegy a tiles mátrixon
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                // Az adott tile-nak megszerzi a renderer-jét majd a grid-ben tárolt adatok alapján meghatározza a színét
                tiles[i, j].GetComponent<Renderer>().material.color = grid[i, j].Color;
                // Ha a type-ja 2-es (ami vizet jelent), akkor
                if (grid[i, j].Type == 2)
                {
                    // Adneki egy NavMeshObstacle-t hogy ne tudjanak rálépni a vizre az AI-k
                    tiles[i, j].AddComponent<NavMeshObstacle>();
                    // kap egy BoxCollider-t aminek beállítja a méretét és hogy Trigger legyen
                    // (amivel tudjuk érzékelni ha valami belép a területre)
                    tiles[i, j].AddComponent<BoxCollider>();
                    tiles[i, j].GetComponent<BoxCollider>().size = new Vector3(9, 5, 9);
                    tiles[i, j].GetComponent<BoxCollider>().isTrigger = true;
                    // Megkapja a Water class-t és beállítja a parmétereit
                    tiles[i, j].AddComponent<Water>();
                    tiles[i, j].GetComponent<Water>().damage = waterDamage;
                    tiles[i, j].GetComponent<Water>().cooldown = waterCooldown;
                    tiles[i, j].name = "Water";
                }
                else if (grid[i, j].Type == 7)
                {
                    // Ha a type-ja 7-es (ami házat jelent), akkor létrehoz egy házat a preHouse alapján,
                    // ad neki egy id-t, majd menti a houses listában
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
                    // Ha a type-ja 8-es (ami end házat jelent), akkor létrehoz egy házat a preHouse alapján,
                    // ad neki egy id-t, beállítja end-nek, majd menti az endHouses listában
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