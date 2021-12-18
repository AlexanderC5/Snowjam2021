using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//plays script

public class DialogueManager : MonoBehaviour
{
    private SceneManager m_SceneManager;
    private DialogueSystem dialogue;

    //script stores text to be displayed
    private List<string> script = new List<string>();
    //lineType stores what kind of line it is (music, bg, scene, name of character, action, etc.)
    private List<char> lineType = new List<char>();
    //speaking stores who is speaking
    private List <string> speaking = new List<string>();

    [SerializeField] private TextAsset[] txtAsset; // Array of text file assets
    private string txt;

    int index = 0; // keeps track of which line is being read in the script
    int temp;
    bool isLine = false; // used to make sure a line is shown after each input

    void Awake()
    {
        m_SceneManager = GameObject.FindObjectOfType<SceneManager>();
        dialogue = GameObject.FindObjectOfType<DialogueSystem>();
    }

    public void beginDialogueSegment(int n) // This is called by Scene Manager when a dialogue scene is loaded
    {
        index = 0;
        temp = 0;
        isLine = false;
        //dialogue = DialogueSystem.instance;
        script.Clear();
        lineType.Clear();
        speaking.Clear();
        //txtAsset = new TextAsset(textDirectory + n + ".txt");
        txt = txtAsset[n].text; // Grab the correct text file for the dialogue segment
        ReadTextFile();
        advanceText(); // Start the first line
    }

    private void ReadTextFile()
    {
        //string txt = this.TextFileAsset.text;

        bool addedAgain = false;
        
        string[] lines = txt.Split(System.Environment.NewLine.ToCharArray()); // split by newline

        int counter = 0;

        foreach (string line in lines)
        {
            addedAgain = false;
            
            if(!string.IsNullOrEmpty(line))
            {
                //print(line);
                if (line.StartsWith("["))
                {
                    string special = line.Substring(1, line.IndexOf(']')- 1); //special = [Name] or [SFX]
                    string curr = line.Substring(line.IndexOf(']') + 1); //curr = nameofperson or sfx.mp3
                    //sound effect in following format:
                    //[SFX]sfx.mp3
                    //may add text to show user what sound effect it is
                    if (special == "SFX")
                    {
                        lineType.Add('S');
                        script.Add(curr);
                        speaking.Add(" ");
                    }
                    //character expression (facial expression in following format)
                    //[EXPRESSION]expressionType
                    else if (special == "EXPRESSION")
                    {
                        lineType.Add('A');
                        script.Add(curr);
                        speaking.Add(speaking[speaking.Count-1]);
                    }
                    //thought in following format:
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
                        speaking.Add(" ");
                    }
                    //Display Scene title
                    //Scene in following format:
                    //[SCENE]SceneName
                    else if(special == "SCENE")
                    {
                        lineType.Add('T');
                        script.Add(curr);
                        speaking.Add(" ");
                    }
                    //Character shows up
                    //character entrance in following format:
                    //[ENTER]0 or 1 (0 = MC, 1 = LI)
                    else if(special == "ENTER")
                    {
                        lineType.Add('N');
                        script.Add(curr);
                        speaking.Add(" ");
                    }
                    //Character exits
                    //character exit in following format:
                    //[EXIT]0 or 1 (0 = MC, 1 = LI)
                    else if (special == "EXIT")
                    {
                        lineType.Add('X');
                        script.Add(curr);
                        speaking.Add(" ");
                    }
                    else if (special == "END")
                    {
                        lineType.Add('D');
                        script.Add("");
                        speaking.Add(" ");
                    }
                    else if (special == "BG")
                    {
                        lineType.Add('B');
                        script.Add(curr);
                        speaking.Add(speaking[speaking.Count-1]);
                    }
                    //a speaker is speaking in following format
                    //[NAME]
                    //NAME's dialogue (may be multiple lines)
                    else
                    {
                        lineType.Add('E');
                        script.Add("");
                        speaking.Add(special);
                    }
                    

                }
                //dialogue
                else
                {
                    script.Add(line);
                    lineType.Add('L');                   
                    speaking.Add(speaking[speaking.Count-1]);
                }
                
            //separate new text box by empty line
            }            
            else if (counter % 2 == 0)
            {
                script.Add("");
                lineType.Add('E'); //empty = new dialogue
                speaking.Add(speaking[speaking.Count-1]);
            }
            counter++;
        }
    }

    void LateUpdate() // Make sure to update the textbox AFTER any UI button presses (so you don't accidentally advance text)
    {
        if (m_SceneManager.sceneType != "VN") return; // If not in VN segment, don't register clicks
        if (!m_SceneManager.DialogueOn()) return; // If in menu, don't register clicks
        
        //if (dialogue.isSpeaking) return; // If speaking, return to prevent an unknown crash << TODO, figure this out!
        //if (!dialogue.isWaitingForUserInput) return;

        if (Input.GetMouseButtonUp(0)) {
            if (!dialogue.isWaitingForUserInput) // If in the middle of writing text to text box
            {
                if (m_SceneManager.instantText == false) // If not instant text already
                {
                    StartCoroutine(finishTextBox());
                }
            }
            else advanceText();
        }
    }

    IEnumerator finishTextBox() // Quickly fills out the rest of the text if the user clicks while it's loading
    {
        m_SceneManager.instantText = true;
        yield return new WaitForEndOfFrame();
        m_SceneManager.instantText = false;
    }

    private void advanceText()
    {
        isLine = false;
        while (!isLine) // Repeat until there's a line that can be displayed by the dialogue box
        {
            // Error check just in case
            if (dialogue.isSpeaking && !dialogue.isWaitingForUserInput) return;

            //END OF SCRIPT
            if (index >= script.Count) { return; }

            //play SFX
            if (lineType[index] == 'S')
            {
                temp = Int32.Parse(script[index]);
                //print("SFX WORKED");
                //print(temp);
        //        m_SceneManager.playSFX(temp);
            }
            //play music
            else if (lineType[index] == 'M')
            {
                temp = Int32.Parse(script[index]);
                //print("Music  WORKED");
                //print(temp);
                m_SceneManager.startMusic(temp);
            }
            //dialogue/text to display
            else if (lineType[index] == 'L')
            {
                isLine = true;
                if (lineType[index - 1] == 'E')
                {
                    if (speaking[index] == " ") // If nobody is speaking (i.e. an inner thought)
                    {
                        //nameplate disappear
                        m_SceneManager.UnloadUI(1);
                        dialogue.Say(script[index], speaking[index]);
                    }
                    else
                    {
                        // nameplate appear
                        m_SceneManager.LoadUI(1);
                        dialogue.Say(script[index], speaking[index]);
                    }
                    //clear speech box and output line
                    dialogue.Say(script[index], speaking[index]);

                }
                else
                {
                    dialogue.SayAdd(script[index], speaking[index]);
                }

            }
            //character expression
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
            //display title from script
            else if (lineType[index] == 'T')
            {
                m_SceneManager.DisplaySceneTitle(script[index]);
            }
            //enter character
            else if (lineType[index] == 'N')
            {
                temp = Int32.Parse(script[index]);
                //print("ENTER CHARACTER WORKED");
                //print(temp);
                m_SceneManager.LoadCharacter(temp);
            }
            //exit character
            else if (lineType[index] == 'X')
            {
                temp = Int32.Parse(script[index]);
                //print("EXIT CHARACTER WORKED");
                //print(temp);
                m_SceneManager.UnloadCharacter(temp);
            }
            //end scene
            else if (lineType[index] == 'D')
            {
                //print("END WORKED");
                StartCoroutine(endDialogueScene());
            }
            else if (lineType[index] == 'B')
            {
                temp = Int32.Parse(script[index]);
                //print("BG LOADED");
                //print(temp);

                if (temp == 10) { StartCoroutine(fadeBlack(3f)); } // Background 10 puts a fade to black overlay for 3 secs
                                                                    //m_SceneManager.LoadBackground(temp);
                else { m_SceneManager.LoadBackground(temp); }
            }

            index++;
        }
    }

    IEnumerator fadeBlack(float time)
    {
        m_SceneManager.crossFade.SetTrigger("FadeToBlack");
        yield return new WaitForSeconds(time);
        m_SceneManager.crossFade.SetTrigger("UnFade");
    }

    IEnumerator endDialogueScene()
    {
        yield return new WaitForSeconds(0.5f / m_SceneManager.animationSpeed);
        m_SceneManager.LoadScene(m_SceneManager.getScene() + 1); // Move on to next scene
    }
}
