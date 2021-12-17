using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    private SceneManager m_SceneManager;

    // CUT_BOARD and SINK are special ingredients - they're stationary, and don't have box colliders, but rather 
    //  arrays that store their bounds on the scene. This was done since adding colliders to these objects would
    //  sometimes block the other ingredients from being dragged.
    public enum INGRs { CUT_BOARD, SINK, POT, POT_WATER, MILK, BUTTER, SALT, PEPPER, BOWL, KNIFE, POTATO,
                        POTATO_PEELED, BUTTER_SLICE, PEELER };

    private List<int> ingrList = new List<int>(); // List of the ingredients currently available in the current minigame
    private List<int[]> recipeList = new List<int[]>(); // Each internal array stores ingr 1, ingr 2, resulting ingr, sfx
    private List<int[]> sinkList = new List<int[]>(); // Each internal array stores ingr 1, resulting ingr (after adding water)

    int currentRecipePos = 0; // Start at beginning of recipe list

    public float[] ingrXbounds; // Bounds on where the ingredients spawn in upon the start of the minigame
    public float[] ingrYbounds;
    public float[] cutbXbounds; // Bounds on where the cutting board is active
    public float[] cutbYbounds;
    public float[] sinkXbounds; // Bounds on where the sink is active
    public float[] sinkYbounds;

    void Awake() // Link to SceneManager to use its load/unload functions
    {
        m_SceneManager = GameObject.FindObjectOfType<SceneManager>();
    }

    public void startCooking(int n) // Paremeter = which thing are we cooking?
    {
        currentRecipePos = 0; // Start at beginning of recipe
        ingrList.Clear();
        recipeList.Clear();
        sinkList.Clear();
        ingrList.Add((int)INGRs.CUT_BOARD); // Cutting board and sink are always added
        ingrList.Add((int)INGRs.SINK);
        switch (n)
        {
            case 0: // Mashed Potatoes
                ingrList.Add((int)INGRs.POT);
                //ingrList.Add((int)INGRs.POT_WATER); <-- Don't add this one, it's an intermediate ingr added via sinnkList
                ingrList.Add((int)INGRs.MILK);
                ingrList.Add((int)INGRs.BUTTER);
                ingrList.Add((int)INGRs.SALT);
                ingrList.Add((int)INGRs.PEPPER);
                ingrList.Add((int)INGRs.BOWL);
                ingrList.Add((int)INGRs.KNIFE);
                ingrList.Add((int)INGRs.PEELER);
                ingrList.Add((int)INGRs.POTATO);

                recipeList.Add(new int[4]);
                recipeList[0][0] = (int)INGRs.POTATO;
                recipeList[0][1] = (int)INGRs.PEELER;
                recipeList[0][2] = (int)INGRs.POTATO_PEELED;
                recipeList[0][3] = 0; // Combination SFX

                sinkList.Add(new int[2]);
                sinkList[0][0] = (int)INGRs.POT;
                sinkList[0][1] = (int)INGRs.POT_WATER;

                break;
            case 1: // Gingerbread Cookies
                break;
            case 2: // Christmas Ham
                break;
            case 3: // Ham Sandwiches
                break;
        }
        LoadIngredients(ingrList);
    }

    private void LoadIngredients(List<int> ingr) // Spawn ingredients randomly on the shelf
    {
        foreach (int i in ingr)
        {
            if (i < 2) { m_SceneManager.LoadIngredient(i, -1, -1); } // Cutting board and Sink shouldn't be randomly placed
            else m_SceneManager.LoadIngredient(i, Random.Range(ingrXbounds[0], ingrXbounds[1]), Random.Range(ingrYbounds[0], ingrYbounds[1]));
        }
    }

    public void touchingSink(string s) // Ingredient n is touching the sink! Can it be filled with water?
    {
        int n = -100;
        for (int i = 0; i < ingrList.Count; i++) // Find ouch which ingr called this function
        {
            if (m_SceneManager.getIngredient(i).name == s)
            {
                n = i;
                break;
            }
        }
        if (n == -100) return; // No ingredient found
        for (int i = 0; i < sinkList.Count; i++) // Check if the ingr can be filled with water
        {
            if (n == sinkList[i][0])
            {
                m_SceneManager.LoadIngredient(sinkList[i][1],
                                              m_SceneManager.getIngredient(n).transform.position.x / m_SceneManager.getScreenX(),
                                              m_SceneManager.getIngredient(n).transform.position.y / m_SceneManager.getScreenY());
                m_SceneManager.UnloadIngredient(n);
            }
        }
    }

    public void onCuttingBoard(string s) { checkCombination(s); } // Ingredient n was placed on the cutting board!
    private void checkCombination(string s, bool keepFirstIngr = false)
    {
        int n = -100;
        for (int i = 0; i < m_SceneManager.numIngredients(); i++) // Find ouch which ingr n called this function
        {
            if (m_SceneManager.getIngredient(i).name == s)
            {
                n = i;
                break;
            }
        }
        if (n == -100) return; // No ingredient found

        // Check if n is part of a recipe list with m
        int m = -1;
        if (recipeList[currentRecipePos][0] == n) { m = recipeList[currentRecipePos][1]; }
        else if (recipeList[currentRecipePos][1] == n) { m = recipeList[currentRecipePos][0]; }
        else return; // Not part of a recipe list

        // Check if n and m are touching
        if (m > -1 && getIngr(n).GetComponent<Collider2D>().bounds.Intersects(getIngr(m).GetComponent<Collider2D>().bounds))
        {
            m_SceneManager.LoadIngredient(recipeList[currentRecipePos][2],
                                              m_SceneManager.getIngredient(n).transform.position.x / m_SceneManager.getScreenX(),
                                              m_SceneManager.getIngredient(n).transform.position.y / m_SceneManager.getScreenY());
            m_SceneManager.UnloadIngredient(recipeList[currentRecipePos][1]);
            if (!keepFirstIngr) m_SceneManager.UnloadIngredient(recipeList[currentRecipePos][0]);

            //m_SceneManager.playSFX(recipeList[currentRecipePos][3]);

            currentRecipePos++; // Onward to the next recipe!
            Debug.Log("AB" + recipeList.Count);
            if (currentRecipePos >= recipeList.Count) // Check if recipe is complete!
            {
                StopCooking();
                StartCoroutine(minigameClear());
                Debug.Log("A");
            }
        }
    }

    // Pass the ingredient to any child classes
    public GameObject getIngr(int n) { return m_SceneManager.getIngredient(n); }

    // Allows Drag and Drop child class to use screen positioning functions
    public float getScreenX() { return m_SceneManager.getScreenX(); }
    public float getScreenY() { return m_SceneManager.getScreenY(); }

    private void StopCooking()
    {
        currentRecipePos = 0;
        ingrList.Clear();
        recipeList.Clear();
        sinkList.Clear();
    }

    IEnumerator minigameClear()
    {
        m_SceneManager.DisplaySceneTitle("Cooking Complete!"); // Victory Message
        Debug.Log("Minigame clear");
        yield return new WaitForSeconds(5f / m_SceneManager.animationSpeed);
        m_SceneManager.LoadScene(m_SceneManager.getScene() + 1); // Move on to next scene
    }
}
