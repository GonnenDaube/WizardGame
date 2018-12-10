using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldData;
using UnityEngine.UI;

public class Portal : MonoBehaviour
{
    public string name;
    public float x;
    public float y;
    public Link link;

    private Text text;
    private Animator animator;
    public GameObject title;
    private bool inside;

    void Start()
    {
        text = title.GetComponent<Text>();
        animator = title.GetComponent<Animator>();
        inside = false;
    }

    void Update()
    {
        if (inside && Input.GetKeyUp(KeyCode.Space))
            GameObject.Find("PortalManager").GetComponent<PortalManager>().Travel();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //show portal name if linked
        if (link != null)
        {
            text.text = name;
            animator.SetBool("isHidden", false);
            GameObject.Find("PortalManager").GetComponent<PortalManager>().SetLink(link.world_target, link.portal_target);
            inside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //hide title
        animator.SetBool("isHidden", true);
        inside = false;
    }
}
