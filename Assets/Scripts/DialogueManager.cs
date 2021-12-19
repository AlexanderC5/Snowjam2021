using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DialogueManager : MonoBehaviour
{
    private SceneManager m_sceneManager;
    private DialogueSystem dialogue;

    private List<string> script = new List<string>();    // Stores text to be displayed
    private List<char> lineType = new List<char>();      // Stores what kind of line it is (music, bg, scene, name of character, action, etc.)
    private List <string> speaking = new List<string>(); // Stores stores who is speaking

    [SerializeField] private TextAsset[] txtAsset;       // Array of text file assets. Add text files in the Unity Editor!
    private string txt;

    int index;   // Keeps track of which line is being read in the script. Used as an iterator for each of the Lists above.

    void Awake()
    {
        m_sceneManager = GameObject.FindObjectOfType<SceneManager>();
        dialogue = GameObject.FindObjectOfType<DialogueSystem>();
    }

    public void beginDialogueSegment(int n) // This is called by Scene Manager when a dialogue scene is loaded
    {
        //dialogue = DialogueSystem.instance; // No need for instances with the way the code is laid out
        index = 0;
        script.Clear();
        lineType.Clear();
        speaking.Clear();

        txt = txtAsset[n].text; // Grab the correct text file for the dialogue segment
        ReadTextFile();
        advanceText(); // Start the first line (otherwise the user has to click once to show the first line)
    }

    private void ReadTextFile()
    {
        string[] lines = txt.Split(System.Environment.NewLine.ToCharArray()); // Split txt by newline into an array

        //int counter = 0;

        foreach (string line in lines)
        {            
            if(!string.IsNullOrEmpty(line)) // ERROR - This if statement does not do anything in the WebGL build. It seems like
                                            //  the lines are no longer null once the file is compressed/packaged? I checked
                                            //  for "\n" and "\r" strings as well, but didn't find anything. Though now that I
                                            //  think about it, I should've checked for the chars, not the strings. In any case
                                            //  for now I just removed all of the white space from the dialogue txts.
            {
                //print(line);
                if (line.StartsWith("["))
                {
                    string special = line.Substring(1, line.IndexOf(']')- 1); // special = [Name], [SFX], etc.
                    string curr = line.Substring(line.IndexOf(']') + 1); // curr = The value after the end brace

                    //Sound effect in following format:
                    //[SFX]0
                    //may add text to show user what sound effect it is
                    if (special == "SFX")
                    {
                        lineType.Add('S');
                        script.Add(curr);
                        speaking.Add(speaking[speaking.Count - 1]); // Speaker hasn't been changed, so pass it on to the next line
                    }

                    //Character's facial expression in following format:
                    //[EXPRESSION]expressionType
                    else if (special == "EXPRESSION")
                    {
                        lineType.Add('A');
                        script.Add(curr);
                        speaking.Add(speaking[speaking.Count - 1]);
                    }

                    //Thought in following format:
                    //[THOUGHT]
                    //thought dialouge... (may be multiple lines)
                    else if (special == "THOUGHT")
                    {
                        lineType.Add('E');
                        script.Add(curr);
                        speaking.Add(" ");
                    }

                    //Music change in following format:
                    //[MUSIC]MusicName
                    else if(special == "MUSIC")
                    {
                        lineType.Add('M');
                        script.Add(curr);
                        speaking.Add(speaking[speaking.Count - 1]);
                    }

                    //Scene title in following format:
                    //[SCENE]SceneName
                    else if(special == "SCENE")
                    {
                        lineType.Add('T');
                        script.Add(curr);
                        speaking.Add(" ");
                    }

                    //Character entrance in following format:
                    //[ENTER]0 or 1 (0 = Stuart, 1 = Toa)
                    else if(special == "ENTER")
                    {
                        lineType.Add('N');
                        script.Add(curr);
                        speaking.Add(speaking[speaking.Count - 1]);
                    }

                    //Character exit in following format:
                    //[EXIT]0 or 1 (0 = Stuart, 1 = Toa)
                    else if (special == "EXIT")
                    {
                        lineType.Add('X');
                        script.Add(curr);
                        speaking.Add(speaking[speaking.Count - 1]);
                    }

                    // Scene end
                    else if (special == "END")
                    {
                        lineType.Add('D');
                        script.Add("");
                        speaking.Add(" ");
                    }

                    // Background change
                    else if (special == "BG")
                    {
                        lineType.Add('B');
                        script.Add(curr);
                        speaking.Add(speaking[speaking.Count-1]);
                    }

                    //Speaker is speaking in following format
                    //[NAME]
                    //NAME's dialogue (may be multiple lines)
                    else
                    {
                        lineType.Add('E');
                        script.Add("");
                        speaking.Add(special);
                    }
                }
                else // If line doesn't start with [, it's dialogue
                {
                    script.Add(line);
                    lineType.Add('L');
                    speaking.Add(speaking[speaking.Count-1]);
                }
            }
            /*
            else if (counter % 2 == 0) // Separate new text box by empty line
            {
                script.Add("");
                lineType.Add('E'); //empty = new dialogue
                speaking.Add(speaking[speaking.Count-1]);
            }
            counter++;
            */
        }
    }

    void LateUpdate() // Make sure to update the textbox AFTER any UI button presses (so you don't accidentally advance text)
    {
        if (m_sceneManager.SceneType() != "VN") return; // If not in a VN scene, don't register clicks
        if (!m_sceneManager.IsDialogueOn()) return; // If in a menu, don't register clicks
        
        if (Input.GetMouseButtonUp(0)) { // Click to finish current line or move to next line
            if (!dialogue.isWaitingForUserInput) { StartCoroutine(finishTextBox()); }
            else advanceText();
        }
    }

    private void advanceText()
    {
        bool isLine = false;
        int temp;
        while (!isLine) // Repeat until there's a line that can be displayed by the dialogue box
        {
            // Error check just in case
            if (dialogue.isSpeaking && !dialogue.isWaitingForUserInput) return;

            // END OF SCRIPT
            if (index >= script.Count) return;

            // Dialogue or text
            if (lineType[index] == 'L')
            {
                isLine = true;

                if (speaking[index] == " ") { m_sceneManager.UnloadUI(1); } // Hide the nameplate if it's an inner thought
                else { m_sceneManager.LoadUI(1); }                          // Show nameplate if it's spoken aloud

                dialogue.Say(script[index], speaking[index]); // Clear speech box and output line
            }

            // Character expressions
            else if (lineType[index] == 'A')
            {
                temp = Int32.Parse(script[index]);
                if (speaking[index] == "Toa")
                {
                    //print("TOA EXPRESSION WORKED");
                    //TODO: CHARACTER FACIAL FEATURE CHANGE FROM CHARACTER MANAGER
                }
                else //STUART
                {
                    //print("STUART EXPRESSION WORKED");
                    //TODO: CHARCTER FACIAL FEATURE CHANGE FROM CHARACTER MANAGER
                }
            }

            // SFX
            else if (lineType[index] == 'S')
            {
                temp = Int32.Parse(script[index]);
                //print("SFX WORKED:" + temp);
     //           m_sceneManager.playSFX(temp);
            }

            // Music
            else if (lineType[index] == 'M')
            {
                temp = Int32.Parse(script[index]);
                //print("Music  WORKED: " + temp);
                m_sceneManager.startMusic(temp);
            }

            // Scene Title
            else if (lineType[index] == 'T') { m_sceneManager.DisplaySceneTitle(script[index]); }

            // Enter character
            else if (lineType[index] == 'N')
            {
                temp = Int32.Parse(script[index]);
                //print("ENTER CHARACTER WORKED: " + temp);
                m_sceneManager.LoadCharacter(temp);
            }

            // Exit character
            else if (lineType[index] == 'X')
            {
                temp = Int32.Parse(script[index]);
                //print("EXIT CHARACTER WORKED: " + temp);
                m_sceneManager.UnloadCharacter(temp);
            }

            // End scene
            else if (lineType[index] == 'D')
            {
                //print("END WORKED");
                StartCoroutine(endDialogueScene());
            }

            else if (lineType[index] == 'B')
            {
                temp = Int32.Parse(script[index]);
                //print("BG LOADED: " + temp);

                if (temp == 10) { StartCoroutine(fadeBlack(3f)); } // Background 10 puts a fade to black overlay for 3 secs
                                                                    //m_sceneManager.LoadBackground(temp);
                else { m_sceneManager.LoadBackground(temp); }
            }

            index++; // Move on to the next line!
        }
    }

    IEnumerator finishTextBox() // Quickly fills out the rest of the text if the user clicks while it's loading.
    {
        if (m_sceneManager.instantText == false) // If not instant text already
        {
            m_sceneManager.instantText = true;
            yield return new WaitForEndOfFrame();
            m_sceneManager.instantText = false;
        }
    }

    IEnumerator fadeBlack(float time) // Background ID:10 fades to black
    {
        m_sceneManager.crossFade.SetTrigger("FadeToBlack");
        yield return new WaitForSeconds(time);
        m_sceneManager.crossFade.SetTrigger("UnFade");
    }

    IEnumerator endDialogueScene() // Wait a bit after the final click and then move on to next scene
    {
        yield return new WaitForSeconds(0.5f / m_sceneManager.animationSpeed);
        m_sceneManager.LoadScene(m_sceneManager.getScene() + 1); // Move on to next scene
    }
}
