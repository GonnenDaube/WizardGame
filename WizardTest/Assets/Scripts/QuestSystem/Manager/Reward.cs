using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reward : MonoBehaviour
{
    private Quest manager;

    public void Load(Quest manager, RewardInfo info)
    {
        this.manager = manager;
        transform.parent = manager.transform;
    }
    public void Destroy()
    {
        Object.Destroy(this.gameObject);
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
