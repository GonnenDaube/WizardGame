using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    public GameObject _reward;
    public GameObject _task;
    private QuestSystemManager manager;
    private TaskInfo[] tasks;
    private Task task;
    private Reward reward;
    public string description;
    private int taskIndex;
    public void Load(QuestSystemManager manager, QuestInfo info)
    {
        this.manager = manager;
        transform.parent = manager.transform;
        tasks = info.taskInfos;
        taskIndex = 0;
        reward = ((GameObject)Instantiate(_reward)).GetComponent<Reward>();
        reward.Load(this, info.rewardInfo);
        description = info.description;
        LoadNextTask();
    }
    public void LoadNextTask()
    {
        task = ((GameObject)Instantiate(_task)).GetComponent<Task>();
        task.Load(this, tasks[taskIndex]);
    }
    public void Destroy()
    {
        task.Destroy();
        reward.Destroy();
        Object.Destroy(this.gameObject);
    }
    public void OnTaskComplete()
    {
        task.Destroy();
        taskIndex++;
        if (taskIndex < tasks.Length)
        {
            LoadNextTask();
            task.ReloadScriptables();
        }
        else
            manager.OnQuestComplete();
    }
    public void SaveProgress()
    {

    }
    public void LoadProgress()
    {

    }
}
