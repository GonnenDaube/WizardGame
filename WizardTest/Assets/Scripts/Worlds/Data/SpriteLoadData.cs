using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLoadData
{
    public Sprite image;
    public bool scriptable;
    public string name;

    public SpriteLoadData(Sprite image, bool scriptable, string name)
    {
        this.image = image;
        this.scriptable = scriptable;
        this.name = name;
    }
}