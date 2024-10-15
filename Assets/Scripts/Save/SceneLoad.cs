using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using UnityEditor;

public class SceneLoad : MonoBehaviour
{
    public string saveName;
    public SaveSystem save;
    public GameObject text;
    public GameObject map;

    /// <summary>
    /// Létrehoz egy új játékot 
    /// </summary>
    public void NewGame(MainMenu menu)
    {
        // Ellenörizzük hogy megefelelõ formátumú a mentés neve
        string name = text.GetComponent<TextMeshProUGUI>().text;
        int i = 0;
        while (i < name.Length-1 && ((name[i] >= 'A' && name[i] <= 'Z') || (name[i] >= '0' && name[i] <= '9') || (name[i] >= 'a' && name[i] <= 'z')))
        {
            i++;
        }
        if (i >= name.Length - 1 && name.Length - 1 > 1 && name.Length - 1 <= 20)
        {
            // Ha megfelel akkor elindul az új játék létrehozása
            menu.loadingtext1.SetActive(true);
            saveName = text.GetComponent<TextMeshProUGUI>().text;
            StartCoroutine(LoadNewGame());
        }
        else
        {
            menu.count--;
            menu.gameNameInput.GetComponent<Image>().color = new Color(1f, 0.58f, 0.58f, 1f);
        }
    }

    /// <summary>
    /// Elinditja a mentés betöltését
    /// </summary>
    /// <param name="saveName">A mentés neve</param>
    public void SaveLoad(string saveName, MainMenu mainmenu, Menu menu)
    {
        if (mainmenu != null) 
        {
            mainmenu.loadingtext2.SetActive(true);
        }
        if (menu != null)
        {
            menu.loadingtext.SetActive(true);
        }
        this.saveName = saveName;
        StartCoroutine(Load());
    }

    /// <summary>
    /// Elinditja a menu metöltését
    /// </summary>
    public void Menu()
    {
        StartCoroutine(MenuLoad());
    }

    /// <summary>
    /// Létrehoz egy egy új mentést amit betölt és elmenti 
    /// </summary>
    /// <returns>Null</returns>
    private IEnumerator LoadNewGame()
    {
        // Elmentjük a jelenlegi Scene-t
        Scene currentScene = SceneManager.GetActiveScene();
        // Elindítunk egy async betöltést ami betölti a "Game" scene-t, eközben
        // ugyan úgy mûködik a jelenlegi oldalunk
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        // Ha végzett a betöltésel átviszi ezt a gameObject-et a "Game" scene-res
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("Game"));
        // Majd kitölti a "currentScene"-t
        SceneManager.UnloadSceneAsync(currentScene);

        // Megkeressük a map-ot
        map = GameObject.Find("Map");
        // Létrehozunk egy új pályát
        map.GetComponent<Map>().NewGame();
        // Átadjuk a házaknak a save-et
        foreach (var item in map.GetComponent<Map>().houses)
        {
            item.GetComponent<House>().saveSystem = gameObject.GetComponent<SaveSystem>();
        }
        // Beállítjuk a save-nek az adatait
        save.SetDatas(saveName);
        // Majd csinálunk egy mentést
        save.SaveData();
    }


    /// <summary>
    /// Betölti a mentést
    /// </summary>
    /// <returns>Null</returns>
    private IEnumerator Load()
    {
        // Elmentjük a jelenlegi Scene-t
        Scene currentScene = SceneManager.GetActiveScene();
        // Elindítunk egy async betöltést ami betölti a "Game" scene-t, eközben
        // ugyan úgy mûködik a jelenlegi oldalunk

        if (currentScene.name == "Game")
        {
            // Végig megyünk az összes házon
            foreach (var house in map.GetComponent<Map>().houses)
            {
                // Majd a házak AI listáján is
                foreach (var ai in house.GetComponent<House>().listAI)
                {
                    // Töröljük az összes AI-t
                    Destroy(ai);
                }
            }
            save.LoadData(saveName);
        }
        else
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            // Ha végzett a betöltésel átviszi ezt a gameObject-et a "Game" scene-res
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("Game"));
            // Majd kitölti a "currentScene"-t
            SceneManager.UnloadSceneAsync(currentScene);

            // Megkeressük a map-ot
            map = GameObject.Find("Map");
            // Beállítjuk a save-nek az adatait
            save.SetDatas(saveName);
            // Betöltjük a mentést
            save.LoadData(saveName);
        }
    }


    /// <summary>
    /// Betölti a menüt
    /// </summary>
    /// <returns>Null</returns>
    private IEnumerator MenuLoad()
    {
        // Elmentjük a jelenlegi Scene-t
        Scene currentScene = SceneManager.GetActiveScene();
        // Elindítunk egy async betöltést ami betölti a "Menu" scene-t, eközben
        // ugyan úgy mûködik a jelenlegi oldalunk
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        // Ha végzett a betöltésel átviszi ezt a gameObject-et a "Menu" scene-res
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("Menu"));
        // Majd kitölti a "currentScene"-t
        SceneManager.UnloadSceneAsync(currentScene);
    }
}
