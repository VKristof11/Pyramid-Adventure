using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Move move;
    public FightSystem fight;
    public GameObject skill;
    public bool skillShow;
    public SceneLoad sceneLoad;
    public SaveSystem saveSystem;
    public GameObject savespanel;
    public GameObject savesList;
    public GameObject preButton;
    public GameObject loadingtext;
    public GameObject menuText;
    public GameObject menu;
    public GameObject optionsMenu;
    public GameObject optionsButton;
    public GameObject saveButton;
    public GameObject quitButton;
    public GameObject death;

    public TMP_Dropdown resDrop;
    private Resolution[] resolutions;

    private void Start()
    {
        sceneLoad = GameObject.Find("SceneManager").GetComponent<SceneLoad>();
        saveSystem = GameObject.Find("SceneManager").GetComponent<SaveSystem>();

        resolutions = Screen.resolutions;
        resDrop.ClearOptions();
        List<string> options = new List<string>();
        int currentResIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }
        resDrop.AddOptions(options);
        resDrop.value = currentResIndex;
        resDrop.RefreshShownValue();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (skill.activeSelf)
            {
                move.menu = false;
                fight.menu = false;
                skill.SetActive(false);
            }
            else if (!menu.activeSelf)
            {
                move.menu = true;
                fight.menu = true;
                skill.SetActive(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menu.activeSelf)
            {
                move.menu = false;
                fight.menu = false;
                menu.SetActive(false);
                optionsMenu.SetActive(false);
                menuText.SetActive(true);
                optionsButton.SetActive(true);
                saveButton.SetActive(true);
                quitButton.SetActive(true);
            }
            else
            {
                if (skill.activeSelf)
                {
                    skill.SetActive(false);
                }
                move.menu = true;
                fight.menu = true;
                menu.SetActive(true);
            }

        }
    }

    public void Load()
    {
        savespanel.SetActive(true);
        string[] saveNames = Directory.GetFiles(Application.persistentDataPath+$"/{sceneLoad.saveName}");
        foreach (Transform item in savesList.transform)
        {
            Destroy(item.gameObject);
        }
        for (int i = 0; i < saveNames.Length; i++)
        {
            string save = saveNames[i].Split('\\').Last();
            GameObject button = Instantiate(preButton);

            button.GetComponent<RectTransform>().SetParent(savesList.GetComponent<RectTransform>());
            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            button.GetComponentInChildren<TextMeshProUGUI>().text = saveNames[i].Split('\\').Last();
            button.GetComponent<Button>().onClick.AddListener(() => LoadHelp(save));
        }
    }

    private void LoadHelp(string save) 
    {
        sceneLoad.SaveLoad(save, null, this);
        death.SetActive(false);
        move.menu = false;
        fight.menu = false;
    }

    public void Save()
    {
        saveSystem.SaveData();
    }

    public void Options()
    {
        optionsMenu.SetActive(true);
        menuText.SetActive(false);
        optionsButton.SetActive(false);
        saveButton.SetActive(false);
        quitButton.SetActive(false);
    }

    public void Back()
    {
        optionsMenu.SetActive(false);
        menuText.SetActive(true);
        optionsButton.SetActive(true);
        saveButton.SetActive(true);
        quitButton.SetActive(true);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFull)
    {
        Screen.fullScreen = isFull;
    }

    public void SetResolution(int resolutionsIndex)
    {
        Resolution res = resolutions[resolutionsIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void Quit()
    {
        sceneLoad.Menu();
    }



}
