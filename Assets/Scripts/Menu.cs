using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    const string MAIN_MENU = "main";
    const string PLAY_MENU = "play";
    const string TEST_MENU = "test";

    const string MUSIC_PREF_NAME = "music";
    const string SCORE_PREF_NAME = "score";


    public GameObject musicCheckBox;

    public List<GameObject> levelButtons;

    public GameObject mainMenu;
    public GameObject levelMenu;

    private int music;
    private int score;

    private string selectedMenu = MAIN_MENU;

    void Start()
    {
        music = PlayerPrefs.GetInt(MUSIC_PREF_NAME, 1);
        score = PlayerPrefs.GetInt(SCORE_PREF_NAME, 0);
        if (score == 0)
            PlayerPrefs.SetInt(SCORE_PREF_NAME, 0);
        musicCheckBox.GetComponent<Toggle>().isOn = music == 1;
        renderLevels();
    }

    private void renderLevels()
    {
        for (int i = 0; i < levelButtons.Count; i++)
        {
            bool isAvailable = i <= score || selectedMenu == TEST_MENU;

            bool isComleted = score > i;

            levelButtons[i].SetActive(isAvailable);
            levelButtons[i].transform.GetChild(1).gameObject.SetActive(isComleted);
        }
    }

    public void selectMenu(string name)
    {
        if (name == MAIN_MENU)
        {
            selectedMenu = name;
            mainMenu.SetActive(true);
            levelMenu.SetActive(false);
        }
        else if (name == PLAY_MENU)
        {
            selectedMenu = name;
            mainMenu.SetActive(false);
            levelMenu.SetActive(true);
            renderLevels();
        }
        else if (name == TEST_MENU)
        {
            selectedMenu = name;
            mainMenu.SetActive(false);
            levelMenu.SetActive(true);
            renderLevels();
        }
    }

    public void loadLevel(int number)
    {
        SceneManager.LoadScene(number);
    }

    public void changeMusic(bool value)
    {
        music = value ? 1 : 0;
        PlayerPrefs.SetInt(MUSIC_PREF_NAME, music);
    }

    public void resetScore()
    {
        PlayerPrefs.SetInt(SCORE_PREF_NAME, 0);
        score = 0;
    }

    public void exit()
    {
        Application.Quit();
    }
}
