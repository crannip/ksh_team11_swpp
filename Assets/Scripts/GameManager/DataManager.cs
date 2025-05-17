using System;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private LeaderBoardContent[] _leaderboard;

    public LeaderBoardContent[] GetLeaderboard()
    {
        return _leaderboard;
    }

    public void SetLeaderboard(LeaderBoardContent[] leaderboard)
    {
        _leaderboard = leaderboard;
    }
}
