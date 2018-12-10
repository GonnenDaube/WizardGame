using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clipeum : MagicAction
{

    // Use this for initialization
    void Start()
    {
        Debug.Log("clipeum");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override bool RequiresMouseTrail()
    {
        return false;
    }

    public override void SetMouseTrail(List<Vector3> pos, Vector3 rawStart)
    {

    }
}
