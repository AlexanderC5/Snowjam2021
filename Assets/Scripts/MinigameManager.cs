using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    private SceneManager m_SceneManager;

    public enum INGRs { CUT_BOARD, SINK, POT, POT_WATER, MILK };

    private List<int> ingrList = new List<int>();
    private List<int[]> recipeList = new List<int[]>(); // Each internal array stores ingr 1, ingr 2, resulting ingr
    private List<int[]> sinkList = new List<int[]>(); // Each internal array stores ingr 1, resulting ingr (after adding water)

    public float[] ingrXbounds; // Bounds on where the ingredients spawn in upon the start of the minigame
    public float[] ingrYbounds;

    void Awake()
    {
        m_SceneManager = GameObject.FindObjectOfType<SceneManager>();
    }

    public void startCooking(int n) // Paremeter = the minigame #
    {
        ingrList.Clear();
        ingrList.Add((int)INGRs.CUT_BOARD);
        ingrList.Add((int)INGRs.SINK);
        recipeList.Clear();
        sinkList.Clear();
        switch (n)
        {
            case 0: // Mashed Potatoes
                ingrList.Add((int)INGRs.POT);
                //ingrList.Add((int)INGRs.POT_WATER); <-- Don't add this one, it's an intermediate ingr
                //ingrList.Add((int)INGRs.MILK);
                recipeList.Add(new int[3]);
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
        for (int i = 0; i < ingrList.Capacity; i++) // Find ouch which ingr called this function
        {
            if (m_SceneManager.getIngredient(i).name == s)
            {
                n = i;
                break;
            }
        }
        for (int i = 0; i < sinkList.Capacity; i++) // Check if the ingr can be filled with water
        {
            if (n == sinkList[0][0])
            {
                m_SceneManager.LoadIngredient(sinkList[0][1], 
                                              m_SceneManager.getIngredient(n).transform.position.x,
                                              m_SceneManager.getIngredient(n).transform.position.y);
                m_SceneManager.UnloadIngredient(n);
                // Remove x, y, x_size, y_size values of n
            }
        }
    }

    public void onCuttingBoard(int n, float x, float y, float x_size, float y_size)
    {

    }
}
