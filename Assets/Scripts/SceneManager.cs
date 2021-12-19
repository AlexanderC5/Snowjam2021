using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    private MinigameManager m_minigameManager;
    private DialogueManager m_dialogueManager;
        
    private bool isDialogueEnabled = false; // disables Dialogue when not in a VN segment
    public string sceneType = "menu"; // VN, cooking

    // These commented variables are old code used for assigning all of the game objects to these arrays
    //  when Awake() is called. Since the build of the game orders the layers differently (I assume), these
    //  have been replaced with Lists that are just filled out in Unity
    //private GameObject[] backgrounds;
    //private GameObject[] characters;
    //private GameObject[] interfaces;
    //private GameObject[] ingredients;
    public List<GameObject> backgrounds;
    public List<GameObject> characters;
    public List<GameObject> interfaces;
    public List<GameObject> ingredients;

    public int initialScene = 0; // Change this in the Unity Editor to start from a different scene
    private int currentScene;
    private IEnumerator loadingScene; // Prevents two scenes from being loaded at the same time -> crash

    // Animation Animators
    public float animationSpeed = 1.0f; // Cross fade spee (increase to speed up)
    public Animator crossFade; // Cross fade
    public Animator optionsMenu;
    public Animator sceneTitle;
    //private byte windowOpacity;
    public float textSpeed = 1.0f; // Text Speed! (increase to speed up)

    public GameObject[] sliders; // Stores all of the sliders present in the options menu
    private AudioSource music; // Plays music
    private AudioSource sound; // Plays sfx
    public List<AudioClip> musics; // Lists all music tracks - can be added directly in the Unity editor
    public List<AudioClip> sfx; // List of all sfx - can be added directly in the Unity editor
    public bool instantText;
    public bool sceneSkipEnabled;

    private float sceneXSize; // Required to deal with changes in screen resolution
    private float sceneYSize;

    // ===== //////////////////////////////////////////////////////////////////////////////////////
    // START //////////////////////////////////////////////////////////////////////////////////////
    // ===== //////////////////////////////////////////////////////////////////////////////////////

    void Start()
    {
        m_minigameManager = GameObject.FindObjectOfType<MinigameManager>();
        m_dialogueManager = GameObject.FindObjectOfType<DialogueManager>();

        // OLD CODE
        //backgrounds = GameObject.FindGameObjectsWithTag("BG");
        //characters = GameObject.FindGameObjectsWithTag("Char");
        //interfaces = GameObject.FindGameObjectsWithTag("UI");
        //ingredients = GameObject.FindGameObjectsWithTag("Ingr");
        //sliders = GameObject.FindGameObjectsWithTag("Slider");

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
                m_dialogueManager.beginDialogueSegment((n + 1) / 2 - 1); // Load Dialogue #1
                break;
            
            // Cooking Minigame scenes
            case 2: // Minigame 0
            case 4: // Minigame 1
            //case 6: // Minigame 2
            //case 8: // Minigame 3
                loadCookingMinigame();
                m_minigameManager.startCooking(n / 2 - 1);
                break;

            // Credits
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
        sceneTitle.SetTrigger("SlideOff");
        yield return new WaitForSeconds(2f / 3 / animationSpeed);
        UnloadUI((int)UIs.SCENE_TITLE);
    }

    public void toggleInstantText()
    {
        if (instantText) instantText = false;
        else instantText = true;
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
        StartCoroutine(CloseGame());
    }

    IEnumerator CloseGame()
    {
        crossFade.SetTrigger("FadeToBlack");
        yield return new WaitForSeconds(1f / animationSpeed);
        Application.Quit();
    }
    
    public void sceneCleared()
    {
        LoadScene(currentScene + 1);
    }

    IEnumerator gameClear()
    {
        yield return new WaitForSeconds(50f);
        LoadScene(0);
    }

    public void toggleSceneSkip()
    {
        if (sceneSkipEnabled) sceneSkipEnabled = false;
        else sceneSkipEnabled = true;
    }

    // ================= //
    // SCREEN RESOLUTION //
    // ================= //

    public float getScreenX() { return sceneXSize; }
    public float getScreenY() { return sceneYSize; }

}
