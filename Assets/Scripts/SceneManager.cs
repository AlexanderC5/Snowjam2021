using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    private bool isDialogueEnabled = false; // disables Dialogue when not in a VN segment
    private string sceneType = "menu"; // VN, cooking

    private GameObject[] backgrounds;
    private GameObject[] characters;
    private GameObject[] interfaces;

    // These enum types should list the game objects in the order they appear in Unity!
    public enum BGs { TITLE, KITCHEN, COUNTERTOP };
    public enum CHARs { };
    public enum UIs { DIALOGUE, NAMEPLATE, START, OPTIONS, EXIT, OPT_MENU, SETTINGS, SCENE_TITLE };

    public int initialScene = 0; // Change this in the Unity Editor to start from a different scene

    // Animation Animators
    public float animationSpeed = 1.0f; // Cross fade spee (increase to speed up)
    public Animator crossFade; // Cross fade
    public Animator optionsMenu;
    public Animator sceneTitle;
    //private byte windowOpacity;

    private GameObject[] sliders; // Stores all of the sliders present in the options menu
    private AudioSource music; // Plays music
    private AudioSource sound; // Plays sfx
    public List<AudioClip> musics; // Lists all music tracks - can be added directly in the Unity editor
    public List<AudioClip> sfx; // List of all sfx - can be added directly in the Unity editor

    // ===== //////////////////////////////////////////////////////////////////////////////////////
    // START //////////////////////////////////////////////////////////////////////////////////////
    // ===== //////////////////////////////////////////////////////////////////////////////////////

    void Start()
    {
        backgrounds = GameObject.FindGameObjectsWithTag("BG");
        characters = GameObject.FindGameObjectsWithTag("Char");
        interfaces = GameObject.FindGameObjectsWithTag("UI");

        music = GetComponent<AudioSource>();
        sound = GameObject.FindGameObjectWithTag("SFX").GetComponent<AudioSource>();
        sliders = GameObject.FindGameObjectsWithTag("Slider");

        crossFade.gameObject.SetActive(true); // Do this upon start so we can disable the black screen during testing
        crossFade.speed = animationSpeed;
        optionsMenu.gameObject.SetActive(true);
        optionsMenu.speed = animationSpeed;
        sceneTitle.gameObject.SetActive(true);
        sceneTitle.speed = animationSpeed;

        HideAll();
        LoadScene(initialScene); // Load main menu, begin the game
    }

    void Update()
    {
        music.volume = 0.01f * sliders[0].GetComponent<Slider>().value; // Make sure the music volume slider is sorted above the sfx vol slider!
        sound.volume = 0.01f * sliders[1].GetComponent<Slider>().value;
        animationSpeed = sliders[2].GetComponent<Slider>().value;
        crossFade.speed = animationSpeed;
        optionsMenu.speed = animationSpeed;
        sceneTitle.speed = animationSpeed;
        //windowOpacity = (byte) sliders[3].GetComponent<Slider>().value;
        //interfaces[0].GetComponent<Image>().color = new Color32(255, 255, 255, windowOpacity);
    }

    public void LoadScene(int n) { StartCoroutine(LoadScn(n)); } // This function is just to make life easier

    IEnumerator LoadScn(int n)
    {
        crossFade.SetTrigger("FadeToBlack");
        yield return new WaitForSeconds(1f / animationSpeed);
        crossFade.SetTrigger("UnFade");

        HideAll();
        switch (n)
        {

            case 0: // Title Scene
                sceneType = "menu";
                LoadBackground((int)BGs.TITLE);
                LoadUI((int)UIs.START); // Play Button
                LoadUI((int)UIs.OPTIONS); // Options Button
                LoadUI((int)UIs.EXIT); // Exit Button
                startMusic(0); // Play Title Screen Music
                break;
            case 1: // Scene 1
                sceneType = "VN";
                EnableDialogue(); // Allows clicking to progress text?
                LoadBackground((int)BGs.KITCHEN);
                LoadUI((int)UIs.SETTINGS); // Settings Button
                LoadCharacter(0);
                // Load first UI/BG/characters
                break;
            case 2: // Minigame 1
                sceneType = "cooking";
                DisableDialogue(); // Prevents clicking to progress text?
                LoadBackground((int)BGs.COUNTERTOP); // Cutting board background
                LoadUI((int)UIs.SETTINGS); // Settings Button
                break;
        }
    }

    // ============= //
    // MUSIC & SOUND //
    // ============= //

    public void playSFX(int n)
    {
        sound.PlayOneShot(sfx[n]);
    }

    public void startMusic(int n)
    {
        music.clip = musics[n];
        music.Play();
    }

    public void stopAllMusic()
    {
        music.Pause();
    }

    // ======================== //
    // LOADING SPRITES & IMAGES //
    // ======================== //

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

    // =============== //
    // TEXT & DIALOGUE //
    // =============== //

    public void DisableDialogue()
    {
        isDialogueEnabled = false;
        UnloadUI((int)UIs.DIALOGUE); // Unload dialogue box
        UnloadUI((int)UIs.NAMEPLATE); // Unload nameplate
    }
    public void EnableDialogue()
    {
        isDialogueEnabled = true;
        LoadUI((int)UIs.DIALOGUE); // Load dialogue box (make sure dialogue box is first UI element)
        LoadUI((int)UIs.NAMEPLATE); // Load Nameplate
    }
    public bool DialogueOn() { return isDialogueEnabled; }

    public void DisplaySceneTitle(string str)
    {
        StartCoroutine(DispSceneTitle(str));
    }

    IEnumerator DispSceneTitle(string str)
    {
        yield return new WaitForSeconds(1f / animationSpeed); // Wait for scene fade transition to finish
        LoadUI((int)UIs.SCENE_TITLE);
        interfaces[(int)UIs.SCENE_TITLE].GetComponentInChildren<Text>().text = str;
        yield return new WaitForSeconds(2f / 3 / animationSpeed);
        yield return new WaitForSeconds(3f / animationSpeed); // Scene Title Stays on screen for this long
        optionsMenu.SetTrigger("SlideOff");
        yield return new WaitForSeconds(2f / 3 / animationSpeed);
        UnloadUI((int)UIs.SCENE_TITLE);
    }

    // =========================== //
    // BUTTONS & SCENE TRANSITIONS //
    // =========================== //
    

    public void PlayButtonPressed()
    {
        LoadScene(1);
    }

    public void OptionsWindowOpened() { StartCoroutine(OptnOpen()); }
    IEnumerator OptnOpen()
    {
        LoadUI((int)UIs.OPT_MENU);
        DisableDialogue();
        yield return new WaitForSeconds(2f / 3 / animationSpeed);
    }
    public void OptionsWindowClosed() { StartCoroutine(OptnClose()); }
    IEnumerator OptnClose()
    {
        optionsMenu.SetTrigger("SlideOff");
        yield return new WaitForSeconds(2f / 3 / animationSpeed);
        UnloadUI((int)UIs.OPT_MENU);
        if (sceneType == "VN") EnableDialogue();
    }

    public void ExitButtonPressed() // This button will only work in a Built Application (i.e. WebGL version posted to itch)
    {
        Application.Quit();
    }
}
