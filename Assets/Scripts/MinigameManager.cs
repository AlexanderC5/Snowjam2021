using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    private SceneManager m_SceneManager;

    public enum INGRs { WATER, POT, MILK };

    private List<int> ingrList = new List<int>();

    public float[] ingrXbounds;
    public float[] ingrYbounds;

    // Start is called before the first frame update
    void Awake()
    {
        m_SceneManager = GameObject.FindObjectOfType<SceneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startCooking(int n)
    {
        ingrList.Clear();
        switch(n)
        {
            case 0: // Mashed Potatoes
                ingrList.Add((int)INGRs.WATER);
                //ingrList.Add((int)INGRs.MILK);
                //ingrList.Add((int)INGRs.POT);
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
            m_SceneManager.LoadIngredient(i, Random.Range(ingrXbounds[0], ingrXbounds[1]), Random.Range(ingrYbounds[0], ingrYbounds[1]));
        }
    }
}
