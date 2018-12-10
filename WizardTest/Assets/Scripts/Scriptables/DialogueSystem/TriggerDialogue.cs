using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialogue : MonoBehaviour
{
    [SerializeField]
    private string dialogueName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject.Find(dialogueName).GetComponent<Dialogue>().StartDialogue();
    }
}
