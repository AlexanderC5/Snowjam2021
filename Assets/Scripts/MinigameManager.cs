using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    private SceneManager m_SceneManager;

    // CUT_BOARD and SINK are special ingredients - they're stationary, and don't have box colliders, but rather 
    //  arrays that store their bounds on the scene. This was done since adding colliders to these objects would
    //  sometimes block the other ingredients from being dragged.
    public enum INGRs { CUT_BOARD, SINK, POT, POT_WATER, MILK, BUTTER, SALT, PEPPER, BOWL, KNIFE,
                        POTATO, POTATO_PEELED, BUTTER_SLICE, PEELER, POTATO_STEAM, POTATO_BOWL, MIXER, POTATO_MASH, POTATO_GRAVY, GRAVY, 
                        BOWL_FILLED, SUGAR, SPICES, FLOUR, EGG, ICING, GINGERBREAD, GINGER_ICED };

    private List<int> ingrList = new List<int>(); // List of the ingredients currently available in the current minigame
    private List<int[]> recipeList = new List<int[]>(); // Each internal array stores ingr 1, ingr 2, resulting ingr, sfx
    private List<int[]> sinkList = new List<int[]>(); // Each internal array stores ingr 1, resulting ingr (after adding water)

    private List<List<int>> recipePrereqs = new List<List<int>>(); // Stores prerequisites for each recipe step.

    int remainingRecipeSteps = 0; // How many steps until the recipe is complete?

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
        remainingRecipeSteps = 0; // Start at beginning of recipe
        ingrList.Clear();
        recipeList.Clear();
        sinkList.Clear();
        recipePrereqs.Clear();
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
                ingrList.Add((int)INGRs.MIXER);
                ingrList.Add((int)INGRs.GRAVY);

                for (int i = 0; i < 10; i++) // 10 steps for recipe 0
                {
                    recipeList.Add(new int[4]);
                    recipePrereqs.Add(new List<int>());
                    remainingRecipeSteps++;
                }

                // Step 0
                recipeList[0][0] = (int)INGRs.POTATO;        // Ingredient #1
                recipeList[0][1] = (int)INGRs.PEELER;        // Ingredient #2
                recipeList[0][2] = (int)INGRs.POTATO_PEELED; // Resulting ingredient
                recipeList[0][3] = 2;                        // Combination SFX

                // Step 1
                recipeList[1][0] = (int)INGRs.POTATO_PEELED;
                recipeList[1][1] = (int)INGRs.POT_WATER;
                recipeList[1][2] = (int)INGRs.POTATO_STEAM;
                recipeList[1][3] = 2; // Combination SFX
                recipePrereqs[1].Add(0);

                // Step 2
                recipeList[2][0] = (int)INGRs.POTATO_STEAM;
                recipeList[2][1] = (int)INGRs.BOWL;
                recipeList[2][2] = (int)INGRs.POTATO_BOWL;
                recipeList[2][3] = 2; // Combination SFX
                recipePrereqs[2].Add(1);

                // Step 3
                recipeList[3][0] = (int)INGRs.POTATO_BOWL;
                recipeList[3][1] = (int)INGRs.MIXER;
                recipeList[3][2] = (int)INGRs.POTATO_MASH;
                recipeList[3][3] = 2; // Combination SFX
                recipePrereqs[3].Add(2);

                // Step 4
                recipeList[4][0] = (int)INGRs.POTATO_MASH;
                recipeList[4][1] = (int)INGRs.BUTTER_SLICE;
                recipeList[4][2] = (int)INGRs.POTATO_MASH;
                recipeList[4][3] = 2; // Combination SFX
                recipePrereqs[4].Add(3);
                recipePrereqs[4].Add(9);

                // Step 5
                recipeList[5][0] = (int)INGRs.POTATO_MASH;
                recipeList[5][1] = (int)INGRs.MILK;
                recipeList[5][2] = (int)INGRs.POTATO_MASH;
                recipeList[5][3] = 2; // Combination SFX
                recipePrereqs[5].Add(3);

                // Step 6
                recipeList[6][0] = (int)INGRs.POTATO_MASH;
                recipeList[6][1] = (int)INGRs.SALT;
                recipeList[6][2] = (int)INGRs.POTATO_MASH;
                recipeList[6][3] = 2; // Combination SFX
                recipePrereqs[6].Add(3);

                // Step 7
                recipeList[7][0] = (int)INGRs.POTATO_MASH;
                recipeList[7][1] = (int)INGRs.PEPPER;
                recipeList[7][2] = (int)INGRs.POTATO_MASH;
                recipeList[7][3] = 2; // Combination SFX
                recipePrereqs[7].Add(3);

                // Step 8
                recipeList[8][0] = (int)INGRs.POTATO_MASH;
                recipeList[8][1] = (int)INGRs.GRAVY;
                recipeList[8][2] = (int)INGRs.POTATO_GRAVY;
                recipeList[8][3] = 2; // Combination SFX
                recipePrereqs[8].Add(4);
                recipePrereqs[8].Add(5);
                recipePrereqs[8].Add(6);
                recipePrereqs[8].Add(7);

                // Step 9
                recipeList[9][0] = (int)INGRs.KNIFE;
                recipeList[9][1] = (int)INGRs.BUTTER;
                recipeList[9][2] = (int)INGRs.BUTTER_SLICE;
                recipeList[9][3] = 2; // Combination SFX

                sinkList.Add(new int[2]);
                sinkList[0][0] = (int)INGRs.POT;
                sinkList[0][1] = (int)INGRs.POT_WATER;
                break;

            case 1: // Gingerbread Cookies
                ingrList.Add((int)INGRs.BOWL);
                ingrList.Add((int)INGRs.SUGAR);
                ingrList.Add((int)INGRs.SPICES);
                ingrList.Add((int)INGRs.FLOUR);
                ingrList.Add((int)INGRs.SALT);
                ingrList.Add((int)INGRs.BUTTER);
                ingrList.Add((int)INGRs.KNIFE);
                ingrList.Add((int)INGRs.MILK);
                ingrList.Add((int)INGRs.EGG);
                ingrList.Add((int)INGRs.ICING);
                ingrList.Add((int)INGRs.MIXER);

                remainingRecipeSteps = 10;

                // Step 0
                recipeList.Add(new int[4] { (int)INGRs.BOWL, (int)INGRs.FLOUR, (int)INGRs.BOWL_FILLED, 2 }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 1
                recipeList.Add(new int[4] { (int)INGRs.BOWL_FILLED, (int)INGRs.SUGAR, (int)INGRs.BOWL_FILLED, 2 }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 2
                recipeList.Add(new int[4] { (int)INGRs.BOWL_FILLED, (int)INGRs.SPICES, (int)INGRs.BOWL_FILLED, 2 }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());
                                
                // Step 3
                recipeList.Add(new int[4] { (int)INGRs.BOWL_FILLED, (int)INGRs.SALT, (int)INGRs.BOWL_FILLED, 2 }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 4
                recipeList.Add(new int[4] { (int)INGRs.BOWL_FILLED, (int)INGRs.BUTTER_SLICE, (int)INGRs.BOWL_FILLED, 2 }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 5
                recipeList.Add(new int[4] { (int)INGRs.BOWL_FILLED, (int)INGRs.MILK, (int)INGRs.BOWL_FILLED, 2 }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 6
                recipeList.Add(new int[4] { (int)INGRs.BOWL_FILLED, (int)INGRs.EGG, (int)INGRs.BOWL_FILLED, 2 }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 7
                recipeList.Add(new int[4] { (int)INGRs.BOWL_FILLED, (int)INGRs.MIXER, (int)INGRs.GINGERBREAD, 2 }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>() { 1, 2, 3, 4, 5, 6 });

                // Step 8
                recipeList.Add(new int[4] { (int)INGRs.GINGERBREAD, (int)INGRs.ICING, (int)INGRs.GINGER_ICED, 2 }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 9
                recipeList.Add(new int[4] { (int)INGRs.BUTTER, (int)INGRs.KNIFE, (int)INGRs.BUTTER_SLICE, 2 }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

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
    private void checkCombination(string s)
    {
        if (remainingRecipeSteps <= 0) return; // Already completed the recipe! Do nothing!

        int n = -100;
        for (int i = 0; i < m_SceneManager.numIngredients(); i++) // Find ouch which ingredient n called this function
        {
            if (m_SceneManager.getIngredient(i).name == s)
            {
                n = i;
                break;
            }
        }
        if (n == -100) return; // No ingredient found

        for (int i = 0; i < recipePrereqs.Count; i++) // For each recipe step
        {
            if (recipePrereqs[i].Count == 0) // If recipe step prereqs have already been satisfied
            {
                int m = -1;
                if (recipeList[i][0] == n) { m = recipeList[i][1]; } // For each recipe step, find the other ingredient if it exists
                else if (recipeList[i][1] == n) { m = recipeList[i][0]; }
                
                if (m != -1) // If other ingredient m exists in a combination with n
                {
                    // Check if n and m are touching
                    if (getIngr(n).GetComponent<Collider2D>().bounds.Intersects(getIngr(m).GetComponent<Collider2D>().bounds))
                    {
                        m_SceneManager.LoadIngredient(recipeList[i][2], // Load the ingredient that is made
                                                          m_SceneManager.getIngredient(n).transform.position.x / m_SceneManager.getScreenX(),
                                                          m_SceneManager.getIngredient(n).transform.position.y / m_SceneManager.getScreenY());
                        if (recipeList[i][1] != recipeList[i][2]) // Unload the ingredients that combine
                        {
                            m_SceneManager.UnloadIngredient(recipeList[i][1]);
                        }
                        if (recipeList[i][0] != recipeList[i][2])
                        {
                            m_SceneManager.UnloadIngredient(recipeList[i][0]);
                        }

                        //m_SceneManager.playSFX(recipeList[currentRecipePos][3]); // Play Combination SFX

                        remainingRecipeSteps--; // Onward to the next recipe!

                        // Remove prereqs!
                        for (int j = 0; j < recipePrereqs.Count; j++)
                        {
                            if (recipePrereqs[j].Contains(i))
                            {
                                recipePrereqs[j].Remove(i);
                                // Debug.Log("removed " + i + " from " + j);
                            }
                        }

                        if (remainingRecipeSteps == 0) // Check if recipe is complete!
                        {
                            StopCooking();
                            StartCoroutine(minigameClear());
                        }
                        break;
                    }
                }
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
        ingrList.Clear();
        recipeList.Clear();
        sinkList.Clear();
        recipePrereqs.Clear();
    }

    IEnumerator minigameClear()
    {
        m_SceneManager.DisplaySceneTitle("Cooking Complete!"); // Victory Message
        //Debug.Log("Minigame clear");
        yield return new WaitForSeconds(5f / m_SceneManager.animationSpeed);
        m_SceneManager.LoadScene(m_SceneManager.getScene() + 1); // Move on to next scene
    }
}
