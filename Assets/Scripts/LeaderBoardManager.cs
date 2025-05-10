using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
class LeaderBoardEntry
{
    public string name;
    public float timeInSeconds;

    public LeaderBoardEntry(string name, float timeInSeconds)
    {
        this.name = name;
        this.timeInSeconds = timeInSeconds;
    }

    public string GetName()
    {
        return name;
    }

    public string GetTime()
    {
        int minutes = (int)(timeInSeconds / 60);
        int seconds = (int)(timeInSeconds % 60);
        int milliseconds = (int)((timeInSeconds - (minutes * 60 + seconds)) * 1000);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
}

[System.Serializable]
class LeaderBoardContent
{
    public List<LeaderBoardEntry> entries;
    public LeaderBoardContent(List<LeaderBoardEntry> entries)
    {
        this.entries = entries;
    }

    public string GetNameStr(int maxLen)
    {
        string str = string.Empty;
        int i = 0;
        foreach (LeaderBoardEntry entry in entries)
        {
            str += entry.GetName() + "\n";
            i++;
            if (i == maxLen) break;
        }
        while (i < maxLen)
        {
            str += "--------\n";
            i++;
        }
        return str;
    }

    public string GetTimeStr(int maxLen)
    {
        string str = string.Empty;
        int i = 0;
        foreach (LeaderBoardEntry entry in entries)
        {
            str += entry.GetTime() + "\n";
            i++;
            if (i == maxLen) break;
        }
        while (i < maxLen)
        {
            str += "--:--.---\n";
            i++;
        }
        return str;
    }

    public void AddRecord(string name, float time, int maxLen)
    {
        int i = entries.Count;
        while (i > 0)
        {
            if (entries[i - 1].timeInSeconds <= time) break;
            i--;
        }
        LeaderBoardEntry newEntry = new LeaderBoardEntry(name, time);
        entries.Insert(i, newEntry);
        while (entries.Count > maxLen)
        {
            entries.RemoveAt(entries.Count - 1);
        }
    }

    public void ClearRecords()
    {
        entries.Clear();
    }

    public static LeaderBoardContent FromJson(string jsonString)
    {
        return JsonUtility.FromJson<LeaderBoardContent>(jsonString);
    }
}

public class LeaderBoardManager : MonoBehaviour
{
    public const string KEY = "leaderboard";

    public int stageNo = 3;
    public int length = 3;

    private LeaderBoardContent[] leaderboard;

    void Start()
    {
        leaderboard = new LeaderBoardContent[stageNo];
        for(int i = 1; i <= stageNo; i++)
        {
            string record = PlayerPrefs.GetString(KEY + i, "{\"entries\":[]}");
            leaderboard[i - 1] = LeaderBoardContent.FromJson(record);
        }
    }

    public string GetNameStr(int stage)
    {
        return leaderboard[stage - 1].GetNameStr(length);
    }

    public string GetTimeStr(int stage)
    {
        return leaderboard[stage - 1].GetTimeStr(length);
    }

    public void AddRecord(int stage, string name,float time)
    {
        int i = stage - 1;
        leaderboard[i].AddRecord(name, time, length);
        PlayerPrefs.SetString(KEY + stage, JsonUtility.ToJson(leaderboard[i]));
    }

    public void ClearRecords()
    {
        for (int stage = 1; stage <= stageNo; stage++) 
        {
            PlayerPrefs.DeleteKey(KEY + stage);
            leaderboard[stage - 1].ClearRecords();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
