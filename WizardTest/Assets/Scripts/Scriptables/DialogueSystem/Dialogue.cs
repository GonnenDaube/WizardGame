using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    [SerializeField]
    private List<Sentence> sentences;
    [SerializeField]
    private bool endTask;
    private int cur;
    private GameObject dialogueBox;
    private Animator dialogue;
    private Text text;
    private Text speaker;
    private bool inAnimation;
    private bool skip;

    void Start()
    {
        inAnimation = false;
        skip = false;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (!inAnimation && !skip)
                LoadNextSentence();
            if (skip)
                skip = false;
        }
    }

    public void StartDialogue()
    {
        dialogueBox = GameObject.Find("DialoguePanel");
        dialogue = dialogueBox.GetComponent<Animator>();
        dialogue.SetBool("DialogueShown", true);
        speaker = GameObject.Find("DialogueSpeaker").GetComponent<Text>();
        text = GameObject.Find("DialogueMessage").GetComponent<Text>();
        cur = 0;
        StartCoroutine(StartDialogueAnimation());
    }

    private IEnumerator StartDialogueAnimation()
    {
        yield return new WaitForSeconds(0.25f);
        LoadNextSentence();
    }

    private void LoadNextSentence()
    {
        if (cur < sentences.Count)
        {
            ResetDialogue();
            inAnimation = true;
            StartCoroutine(AnimateDialogue(sentences[cur], 1));
            cur++;
        }
        else
            EndDialogue();
    }

    private void ResetDialogue()
    {
        speaker.text = "";
        text.text = "";
    }

    private IEnumerator AnimateDialogue(Sentence sentence, int length)
    {
        yield return new WaitForSeconds(0.05f);

        if (length != sentence.GetSpeaker().Length + sentence.GetText().Length + 1)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                speaker.text = sentence.GetSpeaker();
                text.text = sentence.GetText();//skip animation
                inAnimation = false;
                skip = true;
            }
            else
            {
                if (sentence.GetSpeaker().Length >= length)
                    speaker.text = sentence.GetSpeaker().Substring(0, length);
                else
                    text.text = sentence.GetText().Substring(0, length - sentence.GetSpeaker().Length);
                StartCoroutine(AnimateDialogue(sentence, ++length));
            }
        }
        else
        {
            inAnimation = false;
        }
    }

    private void EndDialogue()
    {
        ResetDialogue();
        dialogue.SetBool("DialogueShown", false);
        if (endTask)
            GetComponentInParent<Scriptable>().OnTaskComplete();
    }
}
