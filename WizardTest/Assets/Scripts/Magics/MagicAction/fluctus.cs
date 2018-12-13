using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fluctus : MagicAction
{

    // Use this for initialization
    void Start()
    {
        Debug.Log("fluctus");
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override bool RequiresMouseTrail()
    {
        return false;
    }

    public override void SetMouseTrail(List<Vector3> pos)
    {
        Debug.Log("Proccessing");
    }
}
