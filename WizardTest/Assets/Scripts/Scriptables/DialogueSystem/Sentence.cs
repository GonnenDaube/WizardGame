using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Sentence
{
    [SerializeField]
    private string speaker;
    [TextArea]
    [SerializeField]
    private string text;

    public Sentence(string speaker, string text)
    {
        this.speaker = speaker;
        this.text = text;
    }

    public string GetSpeaker()
    {
        return this.speaker;
    }
    public string GetText()
    {
        return this.text;
    }
}
