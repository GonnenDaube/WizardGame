using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour
{
    public string description;
    private Quest manager;
    Dictionary<string, ScriptableInfo[]> scriptablesInfo;
    private List<Scriptable> scriptables;
    public void Load(Quest manager, TaskInfo info)
    {
        this.manager = manager;
        transform.parent = manager.transform;
        scriptablesInfo = info.scriptables;
        scriptables = new List<Scriptable>();
        description = info.description;
    }
    public void ReloadScriptables()
    {
        foreach (Scriptable s in scriptables)
        {
            Object.Destroy(s);
        }
        scriptables.Clear();
        WorldGenerator worldGenerator = GameObject.Find("WorldGenerator").GetComponent<WorldGenerator>();
        scriptables = new List<Scriptable>();
        GameObject scriptable;
        if (scriptablesInfo.ContainsKey(worldGenerator.world_id))
        {
            foreach (ScriptableInfo s in scriptablesInfo[worldGenerator.world_id])
            {
                //all scriptables must implement "Scriptable"
                string pathname = "Scriptables/" + s.prefabName;
                scriptable = (GameObject)Instantiate((GameObject)Resources.Load(pathname));
                Scriptable script = scriptable.GetComponent<Scriptable>();
                script.Load(this, s);
                scriptables.Add(scriptable.GetComponent<Scriptable>());
            }
        }
    }
    public void Destroy()
    {
        foreach (Scriptable s in scriptables)
        {
            Object.Destroy(s.gameObject);
        }
        scriptables.Clear();
        Object.Destroy(this.gameObject);
    }
    public void CompleteTask()
    {
        manager.OnTaskComplete();
    }
}
