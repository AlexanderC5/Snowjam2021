using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    private bool isDialogueEnabled = false; // VN mode or Minigame mode?
    private int musicVolume;

    private GameObject[] backgrounds;
    private GameObject[] characters;
    private GameObject[] interfaces;

    public int initialScene = 0; // Change this in the Unity Editor to start from a different scene
    public Image fadeImage;

    // Start is called before the first frame update
    void Start()
    {
        backgrounds = GameObject.FindGameObjectsWithTag("BG");
        characters = GameObject.FindGameObjectsWithTag("Char");
        interfaces = GameObject.FindGameObjectsWithTag("UI");
        fadeImage.rectTransform.sizeDelta = new Vector2(0, 0);
        LoadScene(initialScene); // Load main menu, begin the game
    }

    public void LoadScene(int n)
    {
        StartCoroutine(fadeToast(0.5f));
        HideAll();
        switch (n)
        {
            case 0: // Title Scene
                LoadBackground(0);
                LoadUI(1); // Play Button
                LoadUI(2); // Options Button
                LoadUI(3); // Exit Button
                break;
            case 1: // Scene 1
                EnableDialogue(); // Allows clicking to progress text?
                LoadBackground(1);
                LoadCharacter(0);
                // Load first UI/BG/characters
                break;
            case 2: // Minigame 1
                DisableDialogue(); // Prevents clicking to progress text?
                LoadBackground(3); // Cutting board background
                // Load first UI/BG/characters
                break;
        }
    }
    
    public void SetVolume(int n)
    {
        if (n > 100) n = 100;
        if (n < 0) n = 0;
        musicVolume = n;
    }

    public void ChangeVolume(int n)
    {
        musicVolume += n;
        if (musicVolume > 100) musicVolume = 100;
        if (musicVolume < 0) musicVolume = 0;
    }

    public void HideAll()
    {
        for (int i = 0; i < backgrounds.Length; i++) { backgrounds[i].SetActive(false); }
        for (int i = 0; i < characters.Length; i++) { characters[i].SetActive(false); }
        for (int i = 0; i < interfaces.Length; i++) { interfaces[i].SetActive(false); }
    }

    public void LoadCharacter(int n) { characters[n].SetActive(true); }
    public void UnloadCharacter(int n) { characters[n].SetActive(false); }
    public void LoadBackground(int n) { backgrounds[n].SetActive(true); }
    public void UnloadBackground(int n) { backgrounds[n].SetActive(false); }
    public void LoadUI(int n) { interfaces[n].SetActive(true); }
    public void UnloadUI(int n) { interfaces[n].SetActive(false); }

    public void UnloadAllCharacters() { for (int n = 0; n < characters.Length; n++) { characters[n].SetActive(false); } }
    public void UnloadAllBackgrounds() { for (int n = 0; n < characters.Length; n++) { backgrounds[n].SetActive(false); } }
    public void UnloadAllUI() { for (int n = 0; n < characters.Length; n++) { interfaces[n].SetActive(false); } }

    public void DisableDialogue() {
        isDialogueEnabled = false;
        UnloadUI(0); // Unload dialogue box
    }
    public void EnableDialogue() {
        isDialogueEnabled = true;
        LoadUI(0); // Load dialogue box (make sure dialogue box is first UI element)
    }
    public bool DialogueOn() { return isDialogueEnabled; }

    public void PlayButtonPressed()
    {
        LoadScene(1);
    }

    public void OptionsButtonPressed()
    {
        
    }

    public void ExitButtonPressed() // This button will only work in a Built Application (i.e. WebGL version posted to itch)
    {
        Application.Quit();
    }

    IEnumerator fadeToast(float duration) // Toast fade effect between scenes (Completely unnecessary)
    {
        float totalDur = duration;

        fadeImage.gameObject.SetActive(true);

        /** // Grow the toast!
        for (; duration >= 0; duration -= Time.deltaTime)
        {
            fadeImage.rectTransform.sizeDelta = new Vector2(1500 * (1 - duration / totalDur), 1000 * (1 - duration / totalDur));
            yield return null;
        }
        duration = totalDur;
        **/

        // Shrink the toast!
        for (; duration >= 0; duration -= Time.deltaTime)
        {
            fadeImage.rectTransform.sizeDelta = new Vector2(1500 * (duration / totalDur), 1000 * (duration / totalDur));
            yield return null;  
        }
        fadeImage.gameObject.SetActive(false);
    }
}
