using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    private SceneManager m_SceneManager;

    public enum INGRs { CUT_BOARD, SINK, WATER, POT, MILK };

    private List<int> ingrList = new List<int>();

    public float[] ingrXbounds; // Bounds on where the ingredients spawn in upon the start of the minigame
    public float[] ingrYbounds;

    void Awake()
    {
        m_SceneManager = GameObject.FindObjectOfType<SceneManager>();
    }

    void Update()
    {
        
    }

    public void startCooking(int n) // Paremeter = the minigame #
    {
        ingrList.Clear();
        ingrList.Add((int)INGRs.CUT_BOARD);
        ingrList.Add((int)INGRs.SINK);
        switch (n)
        {
            case 0: // Mashed Potatoes
                ingrList.Add((int)INGRs.WATER);
                ingrList.Add((int)INGRs.POT);
                //ingrList.Add((int)INGRs.MILK);
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
}
