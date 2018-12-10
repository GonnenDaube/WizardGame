using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class QuestSystemManager : MonoBehaviour
{
    public GameObject _quest;
    private int activeQuestIndex;
    private Quest activeQuest;
    private List<QuestInfo> quests;

    public void LoadNextQuest()
    {
        GameObject quest = (GameObject)Instantiate(_quest);
        activeQuest = quest.GetComponent<Quest>();
        activeQuest.Load(this, quests[activeQuestIndex]);
    }

    public void OnQuestComplete()
    {
        activeQuest.Destroy();
        activeQuestIndex++;
        if (activeQuestIndex < quests.Count)
            LoadNextQuest();
        else
            Debug.Log("Story Line Complete!");
    }

    public int GetProgress()
    {
        return 100 * activeQuestIndex / quests.Count;
    }

    public void SaveProgress()
    {

    }

    public void LoadProgress()
    {
        activeQuestIndex = 0;
        if (quests.Count != 0)
            LoadNextQuest();
    }

    public void LoadQuests()
    {
        string path = Directory.GetCurrentDirectory() + "/Assets/Resources/Quests/StoryLine1";
        string[] paths = Directory.GetFiles(path, "*.json");
        string json;
        quests = new List<QuestInfo>();
        foreach (string p in paths)
        {
            json = File.ReadAllText(p);
            quests.Add(JsonConvert.DeserializeObject<QuestInfo>(json));
        }
    }

    void Start()
    {
        //TODO: change to smart loading
        //ie: load only the next 3 quests, load quest count seperately
        LoadQuests();
        LoadProgress();
    }
}
