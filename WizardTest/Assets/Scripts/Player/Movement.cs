using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldData;

public class Movement : MonoBehaviour
{
    [SerializeField]
    private float velocity;
    [SerializeField]
    private float jumpForce;

    private Animator animator;
    private Rigidbody2D rbody;

    [SerializeField]
    private bool walk;
    private bool right;
    private bool left;
    private bool sprint;
    private bool jump;
    private bool fall;
    private int dir;
    private Vector2 pos;
    private float momentum;

    private List<GameObject> layers;
    private List<float> sizes;
    private Transform cam;

    private const float ratio = 16.0f / 9.0f;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody2D>();
        cam = Camera.main.transform;
        momentum = 0.0f;
        dir = 0;
        walk = false;
        sizes = new List<float>();
        layers = new List<GameObject>();
    }

    public void SetPlayerPosition()
    {
        SetPlayerPosition(new Vector2(0.0f, 0.0f));
    }

    public void SetPlayerPosition(Vector2 pos)
    {
        layers.Clear();
        for (int i = 0; i < 6; i++)
        {
            layers.Add(GameObject.Find("Layer" + i));
            sizes.Add(layers[i].GetComponent<MeshRenderer>().bounds.size.x);
        }
        transform.position = new Vector3(pos.x, pos.y, 1.5f);
        UpdateCamera();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInput();
        Move();
    }

    private void UpdateCamera()
    {
        if (transform.position.x < sizes[4] - ratio * 10.0f && transform.position.x > 0.0f)
        {
            cam.Translate(transform.position.x - cam.position.x, 0.0f, 0.0f);
            UpdateLayers();
        }
    }

    private void UpdateLayers()
    {
        float per = CalcPercentage();
        for (int i = 0; i < 6; i++)
        {
            if (i != 4)//no need to translate layer 4, calculation would translate by zero
            {
                layers[i].transform.Translate(CalcLayerPosition(per, sizes[i]) - layers[i].transform.position.x, 0.0f, 0.0f);
            }
        }
    }

    private float CalcPercentage()
    {
        return (cam.position.x) / (sizes[4] - 10.0f * ratio);
    }

    private float CalcLayerPosition(float per, float size)
    {
        float offset = per * (size - 10.0f * ratio) + 5.0f * ratio;
        return cam.transform.position.x - offset;
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
            jump = true;
        if (Input.GetKeyUp(KeyCode.W))
            jump = false;

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
            transform.localScale = new Vector3(-0.2f, 0.2f, 1.0f);
        }
        else if (right)
        {
            dir = 1;
            animator.SetBool("IsRun", true);
            transform.localScale = new Vector3(0.2f, 0.2f, 1.0f);
        }
        else
        {
            dir = 0;
            animator.SetBool("IsRun", false);
        }

        if (walk && jump)
        {
            rbody.isKinematic = false;
            rbody.velocity += Vector2.up * jumpForce;
            animator.SetBool("IsFall", false);
            animator.SetBool("IsJump", true);
            walk = false;
        }
        if (rbody.velocity.y < 0.0f)
        {
            fall = true;
            animator.SetBool("IsJump", false);
            animator.SetBool("IsFall", true);
        }
        if (walk && fall)
        {
            fall = false;
            animator.SetBool("IsFall", false);
        }
    }

    private void Move()
    {
        if (walk)
        {
            if (dir != 0)
            {
                Vector2 vel = Vector2.right * dir * velocity;
                if (sprint)
                    vel *= 2;
                pos += vel * Time.deltaTime;
                RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down, 1.2f, (1 << LayerMask.NameToLayer("layer4")));
                if (hit.collider != null && hit.collider.name.Contains("Layer"))
                {
                    pos += Vector2.down * (hit.distance - 1.0f);
                }
                rbody.MovePosition(pos);
            }
        }
        else
        {
            rbody.velocity = rbody.velocity * Vector2.up + Vector2.right * dir * velocity * (sprint ? 2 : 1) + Vector2.right * momentum;
        }

        if (dir != 0 || momentum != 0.0f)
            UpdateCamera();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.name.Contains("Layer"))
        {
            walk = true;
            rbody.isKinematic = true;
            pos = new Vector2(transform.position.x, transform.position.y);
            momentum = 0.0f;
            rbody.velocity = Vector2.zero;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.name.Contains("Layer"))
        {
            walk = false;
            rbody.isKinematic = false;
        }
    }

    public void Push(Vector2 vel)
    {
        if (walk)
        {
            if (vel.y > 0.0f)
            {
                rbody.isKinematic = false;
                walk = false;
                rbody.velocity = vel.y * Vector2.up;
                momentum = vel.x;
            }
        }
        else
        {
            rbody.velocity += vel;
        }
    }
}
