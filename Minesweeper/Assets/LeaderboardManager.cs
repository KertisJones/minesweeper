using UnityEngine;
using Steamworks;
public class LeaderboardManager : MonoBehaviour
{
    // Steamworks
    //protected Callback<leaderboard> m_GameOverlayActivated;
    SteamLeaderboard_t m_CurrentLeaderboard;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SteamManager.Initialized)
        {
            //SteamManager.Instance
            
        }
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    /*public void FindLeaderboard(string leaderboardName)
    {
        leaderboard
    }*/
}
