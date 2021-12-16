using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DialogueSystem : MonoBehaviour
{
    //used to located dialogue in other systems
    public static DialogueSystem instance;

    void Awake()
    {
        //instance is only dialogue system in this scene
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Say(string speech, string speaker)
    {
        if (isSpeaking)
        {
            StopSpeaking();
            speaking = StartCoroutine(Speaking(speech, speaker));
        }
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
    Coroutine speaking = null;
    IEnumerator Speaking(string targetSpeech, string speaker)
    {
        speechPanel.SetActive(true);
        speechText.text = "";
        speakerNameText.text = speaker; //temporary

        while(speechText.text != targetSpeech)
        {
            speechText.text += targetSpeech[speechText.text.Length];
            yield return new WaitForEndOfFrame();
        }

        isWaitingForUserInput = true;
        while(isWaitingForUserInput)
            yield return new WaitForEndOfFrame();

        StopSpeaking();
    }

    [System.Serializable]
    public class ELEMENTS
    {
        ///contains all dialogue related elements on the UI
        [SerializeField] private GameObject speechPanel;
        [SerializeField] private TextMeshProGUI speakerNameText;
        [SerializeField] private TextMeshProGUI speechText;
    }

    public GameObject speechPanel {get{return ELEMENTS.speechPanel;}}
    public TextMeshProGUI speakerNameText {get{return ELEMENTS.speakerNameText;}}
    public TextMeshProGUI speechText {get{return ELEMENTS.speechText;}}
}