using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class SpriteData
{
    public byte[] image;
    public bool scriptable;
    public string name;

    public SpriteData(byte[] image, bool scriptable, string name)
    {
        this.image = image;
        this.scriptable = scriptable;
        this.name = name;
    }
}
