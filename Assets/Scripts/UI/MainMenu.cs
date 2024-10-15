using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using TMPro;
using System;

public class MainMenu : MonoBehaviour
{
    public SceneLoad sceneLoad;
    public List<GameObject> buttons;
    public GameObject gameNameInput;
    public GameObject preButton;
    public GameObject saves;
    public GameObject savesList;
    public GameObject loadingtext1;
    public GameObject loadingtext2;
    public GameObject optionMenu;
    public int count = 0;
    public TMP_Dropdown resDrop;
    private Resolution[] resolutions;
    private bool resDuplicate;

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutions = resolutions.OrderByDescending(x => x.width).ToArray();
        resDrop.ClearOptions();
        List<string> options = new List<string>();
        int currentResIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            resDuplicate = false;
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            foreach (var item in options)
            {
                if (item == option)
                {
                    resDuplicate = true;
                    break;
                }
            }
            if (!resDuplicate)
            {
                options.Add(option);
                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResIndex = i;
                }
            }
        }
        resDrop.AddOptions(options);
        resDrop.value = currentResIndex;
        resDrop.RefreshShownValue();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && count != 2)
        {
            count++;
            sceneLoad.NewGame(this);
        }
    }

    public void NewGame()
    {
        count++;
        buttons[1].SetActive(false);
        buttons[2].SetActive(false);
        buttons[3].SetActive(false);
        buttons[4].SetActive(true);
        gameNameInput.SetActive(true);
        if (count == 2)
        {
            sceneLoad.NewGame(this);
        }
    }

    public void Back() 
    {
        count = 0;
        buttons[0].SetActive(true);
        buttons[1].SetActive(true);
        buttons[2].SetActive(true);
        buttons[3].SetActive(true);
        buttons[4].SetActive(false);
        gameNameInput.SetActive(false);
        optionMenu.SetActive(false);
        saves.SetActive(false);
        gameNameInput.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        foreach (Transform item in savesList.transform)
        {
            Destroy(item.gameObject);
        }
    }

    public void LoadFolders()
    {
        buttons[0].SetActive(false);
        buttons[1].SetActive(false);
        buttons[2].SetActive(false);
        buttons[3].SetActive(false);
        buttons[4].SetActive(true);
        saves.SetActive(true);
        string[] saveFolders = Directory.GetDirectories(Application.persistentDataPath);
        for (int i = 0; i < saveFolders.Length; i++)
        {
            string folder = saveFolders[i].Split('\\').Last();
            GameObject button = Instantiate(preButton);
            button.GetComponent<RectTransform>().SetParent(savesList.GetComponent<RectTransform>());
            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            button.GetComponentInChildren<TextMeshProUGUI>().text = saveFolders[i].Split('\\').Last();
            button.GetComponent<Button>().onClick.AddListener(() => LoadSaves(folder));
        }
    }

    public void LoadSaves(string saveFolderName)
    {
        foreach (Transform item in savesList.transform)
        {
            Destroy(item.gameObject);
        }
        string[] saveNames = Directory.GetFiles(Application.persistentDataPath + $"/{saveFolderName}");
        for (int i = 0; i < saveNames.Length; i++)
        {
            string save = saveNames[i].Split('\\').Last();
            GameObject button = Instantiate(preButton);

            button.GetComponent<RectTransform>().SetParent(savesList.GetComponent<RectTransform>());
            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            button.GetComponentInChildren<TextMeshProUGUI>().text = saveNames[i].Split('\\').Last();
            button.GetComponent<Button>().onClick.AddListener(() => sceneLoad.SaveLoad(save, this, null));
        }
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
        var a = resolutions.Select(x=>x.refreshRate).Distinct().ToArray();
        float s = 0.5f;
        foreach (var item in a)
        {
            s += 0.5f;
        }
        Resolution res = resolutions[Convert.ToInt32(resolutionsIndex * s)];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void Option() 
    {
        optionMenu.SetActive(true);
        buttons[0].SetActive(false);
        buttons[1].SetActive(false);
        buttons[2].SetActive(false);
        buttons[3].SetActive(false);
        buttons[4].SetActive(true);
    }

    public void Quit() 
    {
        Application.Quit();
    }



}
