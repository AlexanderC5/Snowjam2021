using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    private bool isDialogueEnabled = false;
    private int musicVolume;

    private GameObject[] backgrounds;
    private GameObject[] characters;
    private GameObject[] interfaces;

    public int initialScene = 0;

    // Start is called before the first frame update
    void Start()
    {
        backgrounds = GameObject.FindGameObjectsWithTag("BG");
        characters = GameObject.FindGameObjectsWithTag("Char");
        interfaces = GameObject.FindGameObjectsWithTag("UI");
        LoadScene(initialScene); // Load main menu, begin the game
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene(int n)
    {
        switch(n)
        {
            case 0: // Title Scene
                StartMainMenu();
                break;
            case 1: // Scene 1

                break;
            case 2: // Minigame 1

                break;
        }

    }

    public void StartMainMenu()
    {
        hideAll();
        loadBackground(0);
        loadUI(1);
    }
    
    public void ChangeVolume(int n)
    {
        if (n > 100) n = 100;
        if (n < 0) n = 0;
        musicVolume = n;
    }

    public void hideAll()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].SetActive(false);
        }
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(false);
        }
        for (int i = 0; i < interfaces.Length; i++)
        {
            interfaces[i].SetActive(false);
        }
    }

    public void loadCharacter(int n) { characters[n].SetActive(true); }
    public void unloadCharacter(int n) { characters[n].SetActive(false); }
    public void loadBackground(int n) { backgrounds[n].SetActive(true); }
    public void unloadBackground(int n) { backgrounds[n].SetActive(false); }
    public void loadUI(int n) { interfaces[n].SetActive(true); }
    public void unloadUI(int n) { interfaces[n].SetActive(false); }
}
