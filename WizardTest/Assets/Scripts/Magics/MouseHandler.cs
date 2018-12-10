using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MachineLearning;

public class MouseHandler : MonoBehaviour
{

    [SerializeField]
    private Camera cam;
    [SerializeField]
    private GameObject effect;
    [SerializeField]
    private MagicIdentifier magicIdentifier;
    [SerializeField]
    private GameObject[] magics;
    private List<Vector3> pos;
    private Vector3 rawStart;
    private bool record;
    private MagicAction magic;

    void Start()
    {
        Cursor.visible = false;
        record = false;
        pos = new List<Vector3>();
        magic = null;
    }

    void Update()
    {
        Vector3 cur = GetMousePos();
        if (Input.GetMouseButtonDown(1))
        {
            record = true;
            rawStart = Input.mousePosition;
            pos = new List<Vector3>();
            pos.Add(cur);
        }
        if (Input.GetMouseButtonUp(1))
        {
            record = false;
            if (magic != null && magic.RequiresMouseTrail())
            {
                magic.SetMouseTrail(pos, rawStart);
                magic = null;
            }
            else
            {
                IdentifyMagic();
            }
        }

        if (record)
        {
            pos.Add(cur);
            UpdateTrail(cur);
        }

        transform.position = cur;
    }

    private Vector3 GetMousePos()
    {
        Vector3 cur = cam.ScreenToWorldPoint(Input.mousePosition);
        cur.z = -5.0f;
        return cur;
    }

    private void UpdateTrail(Vector3 cur)
    {
        GameObject particle = (GameObject)Instantiate(effect);
        particle.transform.position = cur;
        Destroy(particle, 1.0f);
    }

    private void IdentifyMagic()
    {
        List<Vector2> data = Normalization.NormalizeData(pos, 30);
        List<double> input = new List<double>();
        foreach (Vector2 vec in data)
        {
            input.Add(vec.x);
            input.Add(vec.y);
        }
        //instantiate drawn magic:
        //expects magic to destroy self when done
        int index = magicIdentifier.IdentifyMagic(input.ToArray());
        if (index != -1)
            magic = Instantiate(magics[index]).GetComponent<MagicAction>();
    }
}
