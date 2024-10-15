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
    /// L�trehoz egy �j j�t�kot 
    /// </summary>
    public void NewGame(MainMenu menu)
    {
        // Ellen�rizz�k hogy megefelel� form�tum� a ment�s neve
        string name = text.GetComponent<TextMeshProUGUI>().text;
        int i = 0;
        while (i < name.Length-1 && ((name[i] >= 'A' && name[i] <= 'Z') || (name[i] >= '0' && name[i] <= '9') || (name[i] >= 'a' && name[i] <= 'z')))
        {
            i++;
        }
        if (i >= name.Length - 1 && name.Length - 1 > 1 && name.Length - 1 <= 20)
        {
            // Ha megfelel akkor elindul az �j j�t�k l�trehoz�sa
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
    /// Elinditja a ment�s bet�lt�s�t
    /// </summary>
    /// <param name="saveName">A ment�s neve</param>
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
    /// Elinditja a menu met�lt�s�t
    /// </summary>
    public void Menu()
    {
        StartCoroutine(MenuLoad());
    }

    /// <summary>
    /// L�trehoz egy egy �j ment�st amit bet�lt �s elmenti 
    /// </summary>
    /// <returns>Null</returns>
    private IEnumerator LoadNewGame()
    {
        // Elmentj�k a jelenlegi Scene-t
        Scene currentScene = SceneManager.GetActiveScene();
        // Elind�tunk egy async bet�lt�st ami bet�lti a "Game" scene-t, ek�zben
        // ugyan �gy m�k�dik a jelenlegi oldalunk
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        // Ha v�gzett a bet�lt�sel �tviszi ezt a gameObject-et a "Game" scene-res
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("Game"));
        // Majd kit�lti a "currentScene"-t
        SceneManager.UnloadSceneAsync(currentScene);

        // Megkeress�k a map-ot
        map = GameObject.Find("Map");
        // L�trehozunk egy �j p�ly�t
        map.GetComponent<Map>().NewGame();
        // �tadjuk a h�zaknak a save-et
        foreach (var item in map.GetComponent<Map>().houses)
        {
            item.GetComponent<House>().saveSystem = gameObject.GetComponent<SaveSystem>();
        }
        // Be�ll�tjuk a save-nek az adatait
        save.SetDatas(saveName);
        // Majd csin�lunk egy ment�st
        save.SaveData();
    }


    /// <summary>
    /// Bet�lti a ment�st
    /// </summary>
    /// <returns>Null</returns>
    private IEnumerator Load()
    {
        // Elmentj�k a jelenlegi Scene-t
        Scene currentScene = SceneManager.GetActiveScene();
        // Elind�tunk egy async bet�lt�st ami bet�lti a "Game" scene-t, ek�zben
        // ugyan �gy m�k�dik a jelenlegi oldalunk

        if (currentScene.name == "Game")
        {
            // V�gig megy�nk az �sszes h�zon
            foreach (var house in map.GetComponent<Map>().houses)
            {
                // Majd a h�zak AI list�j�n is
                foreach (var ai in house.GetComponent<House>().listAI)
                {
                    // T�r�lj�k az �sszes AI-t
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
            // Ha v�gzett a bet�lt�sel �tviszi ezt a gameObject-et a "Game" scene-res
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("Game"));
            // Majd kit�lti a "currentScene"-t
            SceneManager.UnloadSceneAsync(currentScene);

            // Megkeress�k a map-ot
            map = GameObject.Find("Map");
            // Be�ll�tjuk a save-nek az adatait
            save.SetDatas(saveName);
            // Bet�ltj�k a ment�st
            save.LoadData(saveName);
        }
    }


    /// <summary>
    /// Bet�lti a men�t
    /// </summary>
    /// <returns>Null</returns>
    private IEnumerator MenuLoad()
    {
        // Elmentj�k a jelenlegi Scene-t
        Scene currentScene = SceneManager.GetActiveScene();
        // Elind�tunk egy async bet�lt�st ami bet�lti a "Menu" scene-t, ek�zben
        // ugyan �gy m�k�dik a jelenlegi oldalunk
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        // Ha v�gzett a bet�lt�sel �tviszi ezt a gameObject-et a "Menu" scene-res
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("Menu"));
        // Majd kit�lti a "currentScene"-t
        SceneManager.UnloadSceneAsync(currentScene);
    }
}
