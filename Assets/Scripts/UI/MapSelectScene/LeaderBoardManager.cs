using UnityEngine;


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

    public void AddRecord(int stage, string name, float time)
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
