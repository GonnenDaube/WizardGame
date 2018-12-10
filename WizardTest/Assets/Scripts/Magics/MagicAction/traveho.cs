using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class traveho : MagicAction
{
    private Vector3 velocity;
    private GameObject effected;

    // Use this for initialization
    void Start()
    {
        Debug.Log("traveho activated");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override bool RequiresMouseTrail()
    {
        return true;
    }

    public override void SetMouseTrail(List<Vector3> pos, Vector3 rawStart)
    {
        Vector3 vel;
        Vector3 avg = Vector3.zero;
        for (int i = 0; i < pos.Count - 1; i++)
        {
            vel = pos[i + 1] - pos[i];
            avg += vel;
        }
        velocity = avg.normalized;
        velocity *= 2.0f * Vector3.Distance(pos[0], pos[pos.Count - 1]);
        Vector2 vel2 = new Vector2(velocity.x, velocity.y);

        RaycastHit2D hit = Physics2D.Raycast(pos[0], Vector2.up, 0.1f);
        hit.transform.gameObject.GetComponent<Rigidbody2D>().velocity += vel2;
    }
}
