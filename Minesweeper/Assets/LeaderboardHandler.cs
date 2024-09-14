using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/* This work is adapted from "Steam integration with Unity – Achievements, Leaderboards, Building" by ArmanDoesStuff, used under CC BY 4.0. 
 * https://www.armandoesstuff.com/posts/unity-to-steam
 * https://creativecommons.org/licenses/by/4.0/deed.en
 */
public class LeaderboardHandler : MonoBehaviour
{
    [SerializeField] Transform holder;
    [SerializeField] GameObject highscorePrefab;
    List<GameObject> highscorePrefabs = new List<GameObject>();
    public void BtnBeginFillLeaderboardLocal()
    {
        FindObjectOfType<LeaderboardManager>().GetLeaderBoardData(Steamworks.ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser, 14);
    }
    public void BtnBeginFillLeaderboardGlobal()
    {
        FindObjectOfType<LeaderboardManager>().GetLeaderBoardData(Steamworks.ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 14);
    }
    public void BtnBeginFillLeaderboardFriends()
    {
        FindObjectOfType<LeaderboardManager>().GetLeaderBoardData(Steamworks.ELeaderboardDataRequest.k_ELeaderboardDataRequestFriends, 14);
    }
    public void FillLeaderboard(List<LeaderboardManager.LeaderboardData> lDataset)
    {
        Debug.Log("filling leaderboard");
        foreach (GameObject g in highscorePrefabs)
        {
            Destroy(g);
        }
        foreach (LeaderboardManager.LeaderboardData lD in lDataset)
        {
            GameObject g = Instantiate(highscorePrefab, holder);
            highscorePrefabs.Add(g);
            FillHighscorePrefab(g, lD);
        }
    }
    void FillHighscorePrefab(GameObject _prefab, LeaderboardManager.LeaderboardData _lData)
    {
        _prefab.transform.Find("username").GetComponent<Text>().text = _lData.username;
        _prefab.transform.Find("score").GetComponent<Text>().text = _lData.score.ToString();
        _prefab.transform.Find("rank").GetComponent<Text>().text = _lData.rank.ToString();
    }
}
