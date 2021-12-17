using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    private SceneManager m_SceneManager;

    // CUT_BOARD and SINK are special ingredients - they're stationary, and don't have box colliders, but rather 
    //  arrays that store their bounds on the scene.
    public enum INGRs { CUT_BOARD, SINK, POT, POT_WATER, MILK, BUTTER, SALT, PEPPER, BOWL, KNIFE, POTATO,
                        POTATO_PEELED, BUTTER_SLICE, PEELER };

    private List<int> ingrList = new List<int>();
    private List<int[]> recipeList = new List<int[]>(); // Each internal array stores ingr 1, ingr 2, resulting ingr
    private List<int[]> sinkList = new List<int[]>(); // Each internal array stores ingr 1, resulting ingr (after adding water)

    int currentRecipePos = 0; // Start at beginning of recipe list

    public float[] ingrXbounds; // Bounds on where the ingredients spawn in upon the start of the minigame
    public float[] ingrYbounds;
    public float[] cutbXbounds; // Bounds on where the cutting board is active
    public float[] cutbYbounds;
    public float[] sinkXbounds; // Bounds on where the sink is active
    public float[] sinkYbounds;

    void Awake()
    {
        m_SceneManager = GameObject.FindObjectOfType<SceneManager>();
    }

    public void startCooking(int n) // Paremeter = the minigame #
    {
        currentRecipePos = 0; // Start at beginning of recipe
        ingrList.Clear();
        recipeList.Clear();
        sinkList.Clear();
        ingrList.Add((int)INGRs.CUT_BOARD);
        ingrList.Add((int)INGRs.SINK);
        switch (n)
        {
            case 0: // Mashed Potatoes
                ingrList.Add((int)INGRs.POT);
                //ingrList.Add((int)INGRs.POT_WATER); <-- Don't add this one, it's an intermediate ingr
                ingrList.Add((int)INGRs.MILK);
                ingrList.Add((int)INGRs.BUTTER);
                ingrList.Add((int)INGRs.SALT);
                ingrList.Add((int)INGRs.PEPPER);
                ingrList.Add((int)INGRs.BOWL);
                ingrList.Add((int)INGRs.KNIFE);
                ingrList.Add((int)INGRs.PEELER);
                ingrList.Add((int)INGRs.POTATO);

                recipeList.Add(new int[3]);
                recipeList[0][0] = (int)INGRs.POTATO;
                recipeList[0][1] = (int)INGRs.PEELER;
                recipeList[0][2] = (int)INGRs.POTATO_PEELED;

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

    private void LoadIngredients(List<int> ingr)
    {
        foreach (int i in ingr)
        {
            if (i < 2) // For things that aren't randomly spawned
            {
                m_SceneManager.LoadIngredient(i, -1, -1);
            }
            else
            {
                m_SceneManager.LoadIngredient(i, Random.Range(ingrXbounds[0], ingrXbounds[1]), Random.Range(ingrYbounds[0], ingrYbounds[1]));
            }
        }
    }

    public GameObject getIngr(int n)
    {
        return m_SceneManager.getIngredient(n);
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
        if (n == -100) return;
        for (int i = 0; i < sinkList.Count; i++) // Check if the ingr can be filled with water
        {
            if (n == sinkList[i][0])
            {
                Debug.Log("awy89e " + (m_SceneManager.getIngredient(n).transform.position.x - (m_SceneManager.getScreenX() / 2)) / m_SceneManager.getScreenX());
                m_SceneManager.LoadIngredient(sinkList[i][1],
                                              m_SceneManager.getIngredient(n).transform.position.x / m_SceneManager.getScreenX(),
                                              m_SceneManager.getIngredient(n).transform.position.y / m_SceneManager.getScreenY());
                m_SceneManager.UnloadIngredient(n);
                // Remove x, y, x_size, y_size values of n
            }
        }
    }

    public void onCuttingBoard(string s)
    {
        checkCombination(s);
    }

    private void checkCombination(string s, bool keepFirstIngr = false)
    {
        int n = -100;
        for (int i = 0; i < ingrList.Count; i++) // Find ouch which ingr n called this function
        {
            Debug.Log("A " + i);
            if (m_SceneManager.getIngredient(i).name == s)
            {
                n = i;
                break;
            }
        }
        if (n == -100) return;
        Debug.Log("B " + currentRecipePos);

        // Check if n is part of a recipe list with m
        int m = -1;
        if (recipeList[currentRecipePos][0] == n)
        {
            m = recipeList[currentRecipePos][1];
        }
        else if (recipeList[currentRecipePos][1] == n)
        {
            m = recipeList[currentRecipePos][1];

        }
        else return;

        Debug.Log("C");

        // Check if n and m are touching
        if (m > -1 && getIngr(n).GetComponent<Collider2D>().bounds.Intersects(getIngr(m).GetComponent<Collider2D>().bounds))
        {
            // Transform.position.x/y are based on (0,0) at the center of the screen. We need to convert that to a percentage
            //  from the bottom right corner of the screen in order to spawn the ingredient at the correct location
            m_SceneManager.LoadIngredient(recipeList[currentRecipePos][2],
                                              m_SceneManager.getIngredient(n).transform.position.x / m_SceneManager.getScreenX(),
                                              m_SceneManager.getIngredient(n).transform.position.y / m_SceneManager.getScreenY());
            m_SceneManager.UnloadIngredient(recipeList[currentRecipePos][1]);
            if (!keepFirstIngr) m_SceneManager.UnloadIngredient(recipeList[currentRecipePos][0]);
        }
    }

    // Allows Drag and Drop child class to use screen positioning functions
    public float getScreenX() { return m_SceneManager.getScreenX(); }
    public float getScreenY() { return m_SceneManager.getScreenY(); }
}
