using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    // These use Find() commands to link up with the correct Game Objects/Componenets
    private MinigameManager m_minigameManager;
    private DialogueManager m_dialogueManager;
    private AudioSource music; // Plays music
    private AudioSource sound; // Plays sfx
    
    // These values can be changed directly in the Unity editor, and are public to make life easier
    public int initialScene = 0;
    public float animationSpeed = 1.0f; // (increase to speed up)
    public float textSpeed = 1.0f;      // (increase to speed up)
    public bool instantText;
    public bool sceneSkipEnabled;

    // These objects are manually added in the Unity editor
    public List<GameObject> backgrounds; //
    public List<GameObject> characters;  //
    public List<GameObject> interfaces;  //
    public List<GameObject> ingredients; //
    public List<AudioClip> musics;       // Lists all music tracks - can be added directly in the Unity editor
    public List<AudioClip> sfx;          // List of all sfx - can be added directly in the Unity editor
    public GameObject[] sliders;         // Stores all of the sliders present in the options menu
    public Animator crossFade;           // Cross fade
    public Animator optionsMenu;         //
    public Animator sceneTitle;          //

    // These variables are private
    private int currentScene;               // Scene #
    private IEnumerator loadingScene;       // Prevents two scenes from being loaded at the same time -> crash
    private bool isDialogueEnabled;         // Disables Dialogue when not in a VN segment
    private string sceneType = "menu";      // VN, cooking. Used to disable dialogue when menus are opened.

    private float sceneXSize; // Required to deal with changes in screen resolution
    private float sceneYSize;

    // ========================================================================================= //
    //                                                                                           //
    //                                      START / UPDATE                                       //
    //                                                                                           //
    // ========================================================================================= //

    void Start()
    {
        m_minigameManager = GameObject.FindObjectOfType<MinigameManager>();
        m_dialogueManager = GameObject.FindObjectOfType<DialogueManager>();
        music = GetComponent<AudioSource>();
        sound = GameObject.FindGameObjectWithTag("SFX").GetComponent<AudioSource>();
        
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
        textSpeed = sliders[3].GetComponent<Slider>().value;
        crossFade.speed = animationSpeed;
        optionsMenu.speed = animationSpeed;
        sceneTitle.speed = animationSpeed;
        //windowOpacity = (byte) sliders[3].GetComponent<Slider>().value;
        //interfaces[0].GetComponent<Image>().color = new Color32(255, 255, 255, windowOpacity);

        // Get the dimensions of the player's window. Origin is at bottom left corner.
        sceneXSize = Screen.width;
        sceneYSize = Screen.height;

        if (Input.GetKeyDown("q") && sceneSkipEnabled) LoadScene(currentScene + 1); // Dev skip through scenes
    }

    // ========================================================================================= //
    //                                                                                           //
    //                                     SCENE MANAGEMENT                                      //
    //                                                                                           //
    // ========================================================================================= //

    public void LoadScene(int n) // Prevents loading multiple scenes at once
    {
        if (loadingScene == null) // If not currently loading a scene, load
        {
            loadingScene = LoadScn(n);
            StartCoroutine(loadingScene);
        }
    }

    IEnumerator LoadScn(int n)
    {
        crossFade.SetTrigger("FadeToBlack");
        yield return new WaitForSeconds(1f / animationSpeed);
        crossFade.SetTrigger("UnFade");

        HideAll();
        currentScene = n; // Set the scene!
        switch (n)
        {
            // Visual Novel scenes
            case 1: // Dialogue 0
            case 3: // Dialogue 1
            case 5: // Dialogue 2
            case 7: // Dialogue 3
            case 9: // Dialogue 4
                loadVisualNovel();
                m_dialogueManager.beginDialogueSegment((n + 1) / 2 - 1);
                break;
            
            // Cooking Minigame scenes
            case 2: // Minigame 0
            case 4: // Minigame 1
            case 6: // Minigame 2
            case 8: // Minigame 3
                loadCookingMinigame();
                m_minigameManager.startCooking(n / 2 - 1);
                break;

            // Game Complete!
            case 10:
                sceneType = "credits";
                DisableDialogue(); // Prevents clicking to progress text
                LoadBackground(Values.B_CELEBRATION); // Final CG, yay
                startMusic(4);
                StartCoroutine(gameClear()); // Wait for 50s, go back to title
                break;
            
            // Title screen
            default:
                StopAllCoroutines();
                sceneType = "menu";
                LoadBackground(Values.B_TITLE);
                LoadUI(Values.U_START); // Play Button
                LoadUI(Values.U_OPTIONS); // Options Button
                LoadUI(Values.U_EXIT); // Exit Button
                startMusic(0);
                break;
        }
        loadingScene = null; // no longer loading! Can now load another scene!
    }

    private void loadVisualNovel()
    {
        sceneType = "VN";
        EnableDialogue(); // Allows clicking to progress text?
        LoadUI(Values.U_SETTINGS); // Settings Button
    }

    private void loadCookingMinigame()
    {
        sceneType = "cooking";
        DisableDialogue(); // Prevents clicking to progress text?
        LoadBackground(Values.B_COUNTERTOP); // Cutting board background
        LoadUI(Values.U_SETTINGS); // Settings Button
        startMusic(2);
    }

    public int getScene() { return currentScene; }

    public string SceneType() { return sceneType; }

    public void sceneCleared() { LoadScene(currentScene + 1); }

    IEnumerator gameClear()
    {
        yield return new WaitForSeconds(50f);
        LoadScene(0);
    }

    // ========================================================================================= //
    //                                                                                           //
    //                                     LOADING SPRITES                                       //
    //                                                                                           //
    // ========================================================================================= //

    public void HideAll()
    {
        for (int i = 0; i < backgrounds.Count; i++) { backgrounds[i].SetActive(false); }
        for (int i = 0; i < characters.Count; i++) { characters[i].SetActive(false); }
        for (int i = 0; i < interfaces.Count; i++) { interfaces[i].SetActive(false); }
        for (int i = 0; i < ingredients.Count; i++) { ingredients[i].SetActive(false); }
    }

    public void LoadCharacter(int n) { characters[n].SetActive(true); }
    public void LoadBackground(int n) { backgrounds[n].SetActive(true); }
    public void LoadUI(int n) { interfaces[n].SetActive(true); }
    public void LoadIngredient(int n, float x, float y) { // x and y are percentages (0 to 1) of total screen size
        if (x != -1) ingredients[n].transform.position = new Vector3(x * sceneXSize, y * sceneYSize, 0);
        ingredients[n].SetActive(true);
    }
    public void UnloadCharacter(int n) { characters[n].SetActive(false); }
    public void UnloadBackground(int n) { backgrounds[n].SetActive(false); }
    public void UnloadUI(int n) { interfaces[n].SetActive(false); }
    public void UnloadIngredient(int n) { ingredients[n].SetActive(false); }

    public void UnloadAllCharacters() { for (int n = 0; n < characters.Count; n++) { characters[n].SetActive(false); } }
    public void UnloadAllBackgrounds() { for (int n = 0; n < characters.Count; n++) { backgrounds[n].SetActive(false); } }
    public void UnloadAllUI() { for (int n = 0; n < characters.Count; n++) { interfaces[n].SetActive(false); } }
    public void UnloadAllIngredients(int n) { ingredients[n].SetActive(false); }

    public GameObject getIngredient(int n) { return ingredients[n]; }
    public int numIngredients() { return ingredients.Count; }


    // ========================================================================================= //
    //                                                                                           //
    //                                     TEXT / DIALOGUE                                       //
    //                                                                                           //
    // ========================================================================================= //

    public void DisableDialogue()
    {
        isDialogueEnabled = false;
        UnloadUI(Values.U_DIALOGUE); // Unload dialogue box
        UnloadUI(Values.U_NAMEPLATE); // Unload nameplate
    }
    public void EnableDialogue()
    {
        isDialogueEnabled = true;
        LoadUI(Values.U_DIALOGUE); // Load dialogue box (make sure dialogue box is first UI element)
        LoadUI(Values.U_NAMEPLATE); // Load Nameplate
    }
    public bool IsDialogueOn() { return isDialogueEnabled; }

    public void DisplaySceneTitle(string str) { StartCoroutine(DispSceneTitle(str)); }

    IEnumerator DispSceneTitle(string str)
    {
        yield return new WaitForSeconds(1f / animationSpeed); // Wait for scene fade transition to finish
        LoadUI(Values.U_SCENE_TITLE);
        interfaces[Values.U_SCENE_TITLE].GetComponentInChildren<Text>().text = str;
        yield return new WaitForSeconds(2f / 3 / animationSpeed);
        yield return new WaitForSeconds(3f / animationSpeed); // Scene Title Stays on screen for this long
        sceneTitle.SetTrigger("SlideOff");
        yield return new WaitForSeconds(2f / 3 / animationSpeed);
        UnloadUI(Values.U_SCENE_TITLE);
    }

    public void toggleInstantText() { instantText = !instantText; }


    // ========================================================================================= //
    //                                                                                           //
    //                                      MUSIC / SOUND                                        //
    //                                                                                           //
    // ========================================================================================= //

    public void playSFX(int n) { sound.PlayOneShot(sfx[n]); }

    public void startMusic(int n)
    {
        music.clip = musics[n];
        music.Play();
    }

    public void stopAllMusic() { music.Pause(); }

    // ========================================================================================= //
    //                                                                                           //
    //                                     BUTTONS / MENUS                                       //
    //                                                                                           //
    // ========================================================================================= //

    public void PlayButtonPressed() { LoadScene(1); }

    public void OptionsWindowOpened() { StartCoroutine(OptnOpen()); }
    IEnumerator OptnOpen()
    {
        LoadUI(Values.U_OPT_MENU);
        DisableDialogue();
        yield return new WaitForSeconds(2f / 3 / animationSpeed);
    }
    public void OptionsWindowClosed() { StartCoroutine(OptnClose()); }
    IEnumerator OptnClose()
    {
        optionsMenu.SetTrigger("SlideOff");
        yield return new WaitForSeconds(2f / 3 / animationSpeed);
        UnloadUI(Values.U_OPT_MENU);
        if (sceneType == "VN") EnableDialogue();
    }

    public void ExitButtonPressed() { StartCoroutine(CloseGame()); }

    IEnumerator CloseGame()
    {
        crossFade.SetTrigger("FadeToBlack");
        yield return new WaitForSeconds(1f / animationSpeed);
        Application.Quit();
    }

    public void toggleSceneSkip()
    {
        if (sceneSkipEnabled) sceneSkipEnabled = false;
        else sceneSkipEnabled = true;
    }


    // ========================================================================================= //
    //                                                                                           //
    //                                    SCREEN RESOLUTION                                      //
    //                                                                                           //
    // ========================================================================================= //

    public float getScreenX() { return sceneXSize; }
    public float getScreenY() { return sceneYSize; }

}
