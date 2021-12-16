using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Testing : MonoBehaviour
{
    DialogueSystem dialogue;

    new List <string> script = new List<string>();
    new List <char> lineType = new List<char>();
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

    /*
    public string[] s = new string[]
    {
        "Hi, how are you?:Avira",
        "It's lovely weather today.",
        "To be honest, I'm glad its not snowing any more!"
    };*/

    
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
        if (Input.GetKeyDown(KeyCode.Space))
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
                        }
                        else
                        {
                            print("sayAdd went through");
                            //add line below previous line
                            dialogue.SayAdd(script[index], speaking[index]);
                        }
                        
                    }
                    
                                                     
                    index++;
                    
                }
            }
        }
    }
}
