using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scriptable : MonoBehaviour
{
    private Task manager;
    private const float ratio = 16.0f / 9.0f;

    public void Load(Task manager, ScriptableInfo info)
    {
        GameObject layer = GameObject.Find("Layer" + info.layerIndex);
        this.manager = manager;
        transform.position = new Vector3(info.position[0] + layer.transform.position.x, info.position[1], info.position[2]);//offset to layer origin
        transform.rotation = Quaternion.Euler(0, 0, info.rotation);
        transform.localScale *= info.scale;
        transform.parent = layer.transform;
        UpdateLayer(this.gameObject, layer.layer);
    }

    private void UpdateLayer(GameObject obj, int layer)
    {
        obj.layer = layer;
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            UpdateLayer(obj.transform.GetChild(i).gameObject, layer);
        }
    }

    public void OnTaskComplete()
    {
        manager.CompleteTask();
    }
}
