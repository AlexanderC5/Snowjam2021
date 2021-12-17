using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogueManager : MonoBehaviour
{
    DialogueSystem dialogue;

    //script stores text to be displayed
    new List <string> script = new List<string>();
    //lineType stores what kind of line it is (music, bg, scene, name of cahracter, action, etc.)
    new List <char> lineType = new List<char>();
    //speaking stores who is speaking
    new List <string> speaking = new List<string>();

    [SerializeField] private TextAsset txtAsset;
    private string txt;

    // Start is called before the first frame update
    void Start()
    {
        dialogue = DialogueSystem.instance;
        txt = txtAsset.ToString();
        ReadTextFile();
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
                print(line);
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
                    
                    addedAgain = true;
                    script.Add(line);
                    lineType.Add('L');                   
                    speaking.Add(speaking[speaking.Count-1]);
                }
                
            //separate new text box by empty line
            }            
            else if (counter % 2 == 0)
            {
                print(line);
                script.Add("");
                lineType.Add('E'); //empty = new dialogue
                speaking.Add(speaking[speaking.Count-1]);
            }
            counter++;
        }
    }

    int index = 0;
    bool isLine = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown())
        {
            isLine = false;
            while (!isLine){
                if (!dialogue.isSpeaking || dialogue.isWaitingForUserInput)
                {
                    //print(index);
                    //print(index-1);
                    if (index >= script.Count)
                    {
                        return;
                    }
                    if (lineType[index] == 'S')
                    {
                        //TODO: PLAY SOUND EFFECT ASSOCIATED WITH THIS LINE (stored in script at index)
                    }
                    
                    else if (lineType[index] == 'L')
                    {
                        isLine = true;
                        if(lineType[index-1] == 'E')
                        {
                            
                            //clear speech box and output line
                            dialogue.Say(script[index], speaking[index]);
                            while (lineType[index + 1] == 'L')
                            {
                                index++;
                                dialogue.SayAdd(script[index], speaking[index]);
                            }
                        }                        
                        
                    }
                    
                                                     
                    index++;
                    
                }
            }
        }
    }
}
