using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MagicAction : MonoBehaviour
{
    public abstract bool RequiresMouseTrail();
    public abstract void SetMouseTrail(List<Vector3> pos);
}
