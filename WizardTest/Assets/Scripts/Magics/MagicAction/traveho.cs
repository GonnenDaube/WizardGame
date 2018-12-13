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
        Debug.Log("traveho");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override bool RequiresMouseTrail()
    {
        return true;
    }

    public override void SetMouseTrail(List<Vector3> pos)
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

        RaycastHit2D hit = Physics2D.Raycast(pos[0] + Camera.main.transform.position, Vector2.up, 0.01f);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("player"))
            {
                hit.collider.gameObject.GetComponent<Movement>().Push(vel2);
            }
            else
            {
                if (hit.collider.gameObject.tag.Equals("Movable"))
                    hit.collider.gameObject.GetComponent<Rigidbody2D>().velocity += vel2;
            }
        }

        Destroy(this.gameObject);
    }
}
