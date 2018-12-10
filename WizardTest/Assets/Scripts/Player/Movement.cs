using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldData;

public class Movement : MonoBehaviour
{
    public float velocity;
    public float dir;
    public float pos;
    public float worldSize;
    public GameObject player;
    public float jmpForce;
    private float jmpVelocity;
    private bool left;
    private bool right;
    private bool sprint;
    private bool jmp;
    private bool fall;
    private bool inAir;
    private const float ratio = (16.0f / 9.0f);
    private GameObject layer;
    private Animator animator;
    private Vector3 target;

    void Start()
    {
        dir = 0;
        right = left = false;
        jmp = false;
        fall = false;
        animator = player.GetComponent<Animator>();
    }

    public void SetPlayerPosition()
    {
        GameObject layerObject = GameObject.Find("Layer4");
        float pos = CalcCurrentPosition(layerObject);
        float y = FindHeightPerPosition(pos, layerObject);
        target = new Vector3(0.0f, y, 1.5f);
        player.transform.position = new Vector3(0.0f, y, 1.5f);
    }

    void Update()
    {
        UpdateInput();
        Move();
    }

    private void UpdateInput()
    {
        //update input
        if (Input.GetKeyDown(KeyCode.D))
        {
            right = true;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            left = true;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            right = false;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            left = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
            sprint = true;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            sprint = false;

        if (Input.GetKeyDown(KeyCode.W))
            jmp = true;
        if (Input.GetKeyUp(KeyCode.W))
            jmp = false;

        //calc direction from input
        if (left && right)
        {
            dir = 0;
            animator.SetBool("IsRun", false);
        }
        else if (left)
        {
            dir = -1;
            animator.SetBool("IsRun", true);
            player.transform.localScale = new Vector3(-0.2f, 0.2f, 1.0f);
        }
        else if (right)
        {
            dir = 1;
            animator.SetBool("IsRun", true);
            player.transform.localScale = new Vector3(0.2f, 0.2f, 1.0f);
        }
        else
        {
            dir = 0;
            animator.SetBool("IsRun", false);
        }

        //Jump
        if (inAir && player.transform.position.y < target.y)
        {
            animator.SetBool("IsJump", false);
            inAir = false;
            jmpVelocity = 0.0f;
        }
        if (jmp && !inAir)
        {
            jmpVelocity = jmpForce;
            animator.SetBool("IsFall", false);
            animator.SetBool("IsJump", true);
            inAir = true;
        }
        if (inAir && jmpVelocity < 0.0f)
        {
            fall = true;
            animator.SetBool("IsJump", false);
            animator.SetBool("IsFall", true);
        }
        if (!inAir && fall)
        {
            fall = false;
            animator.SetBool("IsFall", false);
        }
    }

    private void Move()
    {
        float step;
        if (dir != 0)
        {
            step = dir * velocity * Time.deltaTime * 100.0f / Mathf.Pow(worldSize, 1.5f);
            step *= sprint ? 2 : 1;
            pos += step;
            pos = Mathf.Clamp(pos, 0.0f, 100.0f);
            UpdateLayers();
            UpdatePlayerPosition();
        }
        if (inAir)
        {
            Vector3 playerPos = player.transform.position;
            playerPos.y += jmpVelocity * Time.deltaTime - 10 / 2.0f * Time.deltaTime * Time.deltaTime;
            player.transform.position = playerPos;
            jmpVelocity -= 10 * Time.deltaTime;
        }
        else
        {
            step = velocity / 100.0f * Time.deltaTime;
            step *= sprint ? 2 : 1;
            player.transform.position = Vector3.MoveTowards(player.transform.position, target, step);
        }
    }

    public void UpdateLayers()
    {
        GameObject layerObject;
        for (int i = 0; i < 6; i++)
        {
            layerObject = GameObject.Find("Layer" + i);
            layerObject.transform.position = new Vector3(-CalcCurrentPosition(layerObject), 0.0f, 6.0f - i);
        }
    }

    private void UpdatePlayerPosition()
    {
        GameObject layerObject = GameObject.Find("Layer4");
        float pos = CalcCurrentPosition(layerObject);
        float y = FindHeightPerPosition(pos, layerObject);
        target = new Vector3(0.0f, y, 1.5f);
    }

    private float CalcCurrentPosition(GameObject layerObject)
    {
        Layer layer = layerObject.GetComponent<Layer>();
        float size = ratio * layer.size / 10.0f;
        float offset = (size - ratio * 10.0f) * pos / 100;
        return ratio * 5.0f + offset;
    }

    private float FindHeightPerPosition(float pos, GameObject layerObject)
    {
        Mesh mesh = layerObject.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        int bot = 0, top = vertices.Length - 1, mid = (bot + top) / 2;
        bool found = false;
        float height;
        float distance;
        float weight;
        while (!found && top > bot)
        {
            mid = (bot + top) / 2;
            if (vertices[mid].x == pos)
                found = true;
            else if (vertices[mid].x < pos)
                bot = mid + 1;
            else
                top = mid - 1;
        }
        if (!found)
        {
            distance = vertices[mid + 1].x - vertices[mid].x;
            weight = (pos - vertices[mid].x) / distance;
            height = weight * vertices[mid].y + (1 - weight) * vertices[mid + 1].y;
        }
        else
            height = vertices[mid].y;
        return height;
    }
}
