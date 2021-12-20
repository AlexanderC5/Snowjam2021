using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    private SceneManager m_SceneManager;
        
    private List<int> ingrList = new List<int>(); // List of the ingredients currently available in the current minigame
    private List<int[]> recipeList = new List<int[]>(); // Each internal array stores ingr 1, ingr 2, resulting ingr, sfx
    private List<int[]> sinkList = new List<int[]>(); // Each internal array stores ingr 1, resulting ingr (after adding water)
    private List<List<int>> recipePrereqs = new List<List<int>>(); // Stores prerequisites for each recipe step.

    private int remainingRecipeSteps = 0; // How many steps until the recipe is complete?

    public float[] ingrXbounds; // Bounds on where the ingredients spawn in upon the start of the minigame
    public float[] ingrYbounds;
    public float[] cutbXbounds; // Bounds on where the cutting board is active
    public float[] cutbYbounds;
    public float[] sinkXbounds; // Bounds on where the sink is active
    public float[] sinkYbounds;

    private int knifeUses = 1; // Number of times the knife can be used before it disappears
                               //  Only really needed for minigame #4, where it's used 3 times

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
        ingrList.Add(Values.I_CUT_BOARD); // Cutting board and sink are always added
        ingrList.Add(Values.I_SINK);
        knifeUses = 1;

        switch (n)
        {
            case 0: // Mashed Potatoes
                ingrList.Add(Values.I_POT);
                //ingrList.Add(Values.I_POT_WATER); <-- Don't add this one, it's an intermediate ingr added via sinnkList
                ingrList.Add(Values.I_MILK);
                ingrList.Add(Values.I_BUTTER);
                ingrList.Add(Values.I_SALT);
                ingrList.Add(Values.I_PEPPER);
                ingrList.Add(Values.I_BOWL);
                ingrList.Add(Values.I_KNIFE);
                ingrList.Add(Values.I_PEELER);
                ingrList.Add(Values.I_POTATO);
                ingrList.Add(Values.I_MIXER);
                ingrList.Add(Values.I_GRAVY);

                for (int i = 0; i < 10; i++) // 10 steps for recipe 0
                {
                    recipeList.Add(new int[4]);
                    recipePrereqs.Add(new List<int>());
                    remainingRecipeSteps++;
                }

                // Step 0
                recipeList[0][0] = Values.I_POTATO;        // Ingredient #1
                recipeList[0][1] = Values.I_PEELER;        // Ingredient #2
                recipeList[0][2] = Values.I_POTATO_PEELED; // Resulting ingredient
                recipeList[0][3] = Values.S_PEELING;                        // Combination SFX

                // Step 1
                recipeList[1][0] = Values.I_POTATO_PEELED;
                recipeList[1][1] = Values.I_POT_WATER;
                recipeList[1][2] = Values.I_POTATO_STEAM;
                recipeList[1][3] = Values.S_POP; // Combination SFX
                recipePrereqs[1].Add(0);

                // Step 2
                recipeList[2][0] = Values.I_POTATO_STEAM;
                recipeList[2][1] = Values.I_BOWL;
                recipeList[2][2] = Values.I_POTATO_BOWL;
                recipeList[2][3] = Values.S_POP; // Combination SFX
                recipePrereqs[2].Add(1);

                // Step 3
                recipeList[3][0] = Values.I_POTATO_BOWL;
                recipeList[3][1] = Values.I_MIXER;
                recipeList[3][2] = Values.I_POTATO_MASH;
                recipeList[3][3] = Values.S_POTATO_MASH; // Combination SFX
                recipePrereqs[3].Add(2);

                // Step 4
                recipeList[4][0] = Values.I_POTATO_MASH;
                recipeList[4][1] = Values.I_BUTTER_SLICE;
                recipeList[4][2] = Values.I_POTATO_MASH;
                recipeList[4][3] = Values.S_POP; // Combination SFX
                recipePrereqs[4].Add(3);
                recipePrereqs[4].Add(9);

                // Step 5
                recipeList[5][0] = Values.I_POTATO_MASH;
                recipeList[5][1] = Values.I_MILK;
                recipeList[5][2] = Values.I_POTATO_MASH;
                recipeList[5][3] = Values.S_POP; // Combination SFX
                recipePrereqs[5].Add(3);

                // Step 6
                recipeList[6][0] = Values.I_POTATO_MASH;
                recipeList[6][1] = Values.I_SALT;
                recipeList[6][2] = Values.I_POTATO_MASH;
                recipeList[6][3] = Values.S_POP; // Combination SFX
                recipePrereqs[6].Add(3);

                // Step 7
                recipeList[7][0] = Values.I_POTATO_MASH;
                recipeList[7][1] = Values.I_PEPPER;
                recipeList[7][2] = Values.I_POTATO_MASH;
                recipeList[7][3] = Values.S_POP; // Combination SFX
                recipePrereqs[7].Add(3);

                // Step 8
                recipeList[8][0] = Values.I_POTATO_MASH;
                recipeList[8][1] = Values.I_GRAVY;
                recipeList[8][2] = Values.I_POTATO_GRAVY;
                recipeList[8][3] = Values.S_POP; // Combination SFX
                recipePrereqs[8].Add(4);
                recipePrereqs[8].Add(5);
                recipePrereqs[8].Add(6);
                recipePrereqs[8].Add(7);

                // Step 9
                recipeList[9][0] = Values.I_KNIFE;
                recipeList[9][1] = Values.I_BUTTER;
                recipeList[9][2] = Values.I_BUTTER_SLICE;
                recipeList[9][3] = Values.S_KNIFE_CHOP; // Combination SFX

                sinkList.Add(new int[2]);
                sinkList[0][0] = Values.I_POT;
                sinkList[0][1] = Values.I_POT_WATER;
                break;

            case 1: // Gingerbread Cookies
                ingrList.Add(Values.I_BOWL);
                ingrList.Add(Values.I_SUGAR);
                ingrList.Add(Values.I_SPICES);
                ingrList.Add(Values.I_FLOUR);
                ingrList.Add(Values.I_SALT);
                ingrList.Add(Values.I_BUTTER);
                ingrList.Add(Values.I_KNIFE);
                ingrList.Add(Values.I_MILK);
                ingrList.Add(Values.I_EGG);
                ingrList.Add(Values.I_ICING);
                ingrList.Add(Values.I_MIXER);

                remainingRecipeSteps = 10;

                // Step 0
                recipeList.Add(new int[4] { Values.I_BOWL, Values.I_FLOUR, Values.I_BOWL_FILLED, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 1
                recipeList.Add(new int[4] { Values.I_BOWL_FILLED, Values.I_SUGAR, Values.I_BOWL_FILLED, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 2
                recipeList.Add(new int[4] { Values.I_BOWL_FILLED, Values.I_SPICES, Values.I_BOWL_FILLED, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());
                                
                // Step 3
                recipeList.Add(new int[4] { Values.I_BOWL_FILLED, Values.I_SALT, Values.I_BOWL_FILLED, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 4
                recipeList.Add(new int[4] { Values.I_BOWL_FILLED, Values.I_BUTTER_SLICE, Values.I_BOWL_FILLED, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 5
                recipeList.Add(new int[4] { Values.I_BOWL_FILLED, Values.I_MILK, Values.I_BOWL_FILLED, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 6
                recipeList.Add(new int[4] { Values.I_BOWL_FILLED, Values.I_EGG, Values.I_BOWL_FILLED, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 7
                recipeList.Add(new int[4] { Values.I_BOWL_FILLED, Values.I_MIXER, Values.I_GINGERBREAD, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>() { 1, 2, 3, 4, 5, 6 });

                // Step 8
                recipeList.Add(new int[4] { Values.I_GINGERBREAD, Values.I_ICING, Values.I_GINGER_ICED, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 9
                recipeList.Add(new int[4] { Values.I_BUTTER, Values.I_KNIFE, Values.I_BUTTER_SLICE, Values.S_KNIFE_CHOP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                break;

            case 2: // Christmas Ham
                ingrList.Add(Values.I_HAM);
                ingrList.Add(Values.I_PINEAPPLE);
                ingrList.Add(Values.I_CLOVES);
                ingrList.Add(Values.I_SALT);
                ingrList.Add(Values.I_PEPPER);

                remainingRecipeSteps = 4;

                // Step 0
                recipeList.Add(new int[4] { Values.I_HAM, Values.I_SALT, Values.I_HAM, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 1
                recipeList.Add(new int[4] { Values.I_PINE_HAM, Values.I_SALT, Values.I_PINE_HAM, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 2
                recipeList.Add(new int[4] { Values.I_PINE_HAM_CLOVE, Values.I_SALT, Values.I_PINE_HAM_CLOVE, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 3
                recipeList.Add(new int[4] { Values.I_HAM, Values.I_PEPPER, Values.I_HAM, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 4
                recipeList.Add(new int[4] { Values.I_PINE_HAM, Values.I_PEPPER, Values.I_PINE_HAM, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 5
                recipeList.Add(new int[4] { Values.I_PINE_HAM_CLOVE, Values.I_PEPPER, Values.I_PINE_HAM_CLOVE, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 6
                recipeList.Add(new int[4] { Values.I_HAM, Values.I_PINEAPPLE, Values.I_PINE_HAM, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 7
                recipeList.Add(new int[4] { Values.I_PINE_HAM, Values.I_CLOVES, Values.I_PINE_HAM_CLOVE, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                break;

            case 3: // Ham Sandwiches
                knifeUses = 4000; // The knife is now indestructible just to make life easier. (i.e. fix a bug)

                ingrList.Add(Values.I_BREAD_LOAF);
                ingrList.Add(Values.I_MAYO);
                ingrList.Add(Values.I_SPOON);
                ingrList.Add(Values.I_KNIFE);
                ingrList.Add(Values.I_TOMATO);
                ingrList.Add(Values.I_LETTUCE);
                ingrList.Add(Values.I_CHEESE_SLICE);
                ingrList.Add(Values.I_PINE_HAM_CLOVE);

                remainingRecipeSteps = 10;

                // Step 0
                recipeList.Add(new int[4] { Values.I_MAYO, Values.I_SPOON, Values.I_SPOON_MAYO, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 1
                recipeList.Add(new int[4] { Values.I_KNIFE, Values.I_TOMATO, Values.I_TOMATO_SLICE, Values.S_KNIFE_CHOP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 2
                recipeList.Add(new int[4] { Values.I_KNIFE, Values.I_LETTUCE, Values.I_LETTUCE_LEAF, Values.S_KNIFE_CHOP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 3
                recipeList.Add(new int[4] { Values.I_KNIFE, Values.I_PINE_HAM_CLOVE, Values.I_HAM_SLICE, Values.S_KNIFE_CHOP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 4
                recipeList.Add(new int[4] { Values.I_KNIFE, Values.I_BREAD_LOAF, Values.I_BREAD_A, Values.S_KNIFE_CHOP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 5
                recipeList.Add(new int[4] { Values.I_BREAD_A, Values.I_SPOON_MAYO, Values.I_BREAD_B, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 6
                recipeList.Add(new int[4] { Values.I_BREAD_B, Values.I_TOMATO_SLICE, Values.I_BREAD_C, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 7
                recipeList.Add(new int[4] { Values.I_BREAD_C, Values.I_LETTUCE_LEAF, Values.I_BREAD_D, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 8
                recipeList.Add(new int[4] { Values.I_BREAD_D, Values.I_CHEESE_SLICE, Values.I_BREAD_E, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

                // Step 9
                recipeList.Add(new int[4] { Values.I_BREAD_E, Values.I_HAM_SLICE, Values.I_SANDWICH, Values.S_POP }); // Ingr1, Ingr2, IngrResult, SFX
                recipePrereqs.Add(new List<int>());

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
                            // If the first tool is a knife and thre are still knife uses left, decrease knife durability:
                            if (recipeList[i][0] == Values.I_KNIFE && knifeUses > 1) { knifeUses--; }

                            else m_SceneManager.UnloadIngredient(recipeList[i][0]);
                        }

                        m_SceneManager.playSFX(recipeList[i][3]); // Play Combination SFX

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
        m_SceneManager.playSFX(Values.S_COOKING_COMPLETE);
        yield return new WaitForSeconds(5f / m_SceneManager.animationSpeed);
        m_SceneManager.LoadScene(m_SceneManager.getScene() + 1); // Move on to next scene
    }
}
