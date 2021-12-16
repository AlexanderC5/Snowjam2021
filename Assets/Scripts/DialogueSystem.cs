using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using TMPro;


public class DialogueSystem : MonoBehaviour
{
    //used to located dialogue in other systems
    public static DialogueSystem instance;
    public ELEMENTS elements;


    // Start is called before the first frame update
    //used for initialization
    void Start()
    {
        
    }

    void Awake()
    {
        //instance is only dialogue system in this scene
        instance = this;
    }

    

    // Say something and show it on the speech box
    public void Say(string speech, string speaker = "")
    {
        StopSpeaking();
        speaking = StartCoroutine(Speaking(speech, false, speaker));
        
    }

    //add to what is already in speech box
    public void SayAdd(string speech, string speaker = "")
    {
        StopSpeaking();
        speechText.text = targetSpeech;
        speaking = StartCoroutine(Speaking(speech, true, speaker));
    }

    public void StopSpeaking()
    {
        if (isSpeaking)
        {
            StopCoroutine(speaking);
        }
        speaking = null;
    }

    public bool isSpeaking {get{return speaking != null;}}
    [HideInInspector] public bool isWaitingForUserInput = false;

    public string targetSpeech = "";
    Coroutine speaking = null;
    IEnumerator Speaking(string speech, bool additive, string speaker = "")
    {
        speechPanel.SetActive(true);
        targetSpeech = speech;

        if (!additive)
            speechText.text = "";
        else
            targetSpeech = speechText.text + "\n" + targetSpeech;


        speakerNameText.text = DetermineSpeaker(speaker); //temporary

        isWaitingForUserInput = false;

        while(speechText.text != targetSpeech)
        {
            speechText.text += targetSpeech[speechText.text.Length];
            yield return new WaitForEndOfFrame();
        }

        //text finished
        isWaitingForUserInput = true;
        while(isWaitingForUserInput)
            yield return new WaitForEndOfFrame();

        StopSpeaking();
    }

    string DetermineSpeaker(string s)
    {
        string retVAl = speakerNameText.text;
        if (s != speakerNameText.text && s != "")
            retVAl = (s.ToLower().Contains("narrator")) ? "" : s;
        return retVAl;
    }

    [System.Serializable]
    public class ELEMENTS
    {
        ///contains all dialogue related elements on the UI
        public GameObject speechPanel;
        public Text speakerNameText;
        public Text speechText;
    }

    public GameObject speechPanel {get{return elements.speechPanel;}}
    public Text speakerNameText {get{return elements.speakerNameText;}}
    public Text speechText {get{return elements.speechText;}}
}