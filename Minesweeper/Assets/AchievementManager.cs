using Steamworks;
using UnityEngine;
/* This work is adapted from "Steam integration with Unity – Achievements, Leaderboards, Building" by ArmanDoesStuff, used under CC BY 4.0. 
 * https://www.armandoesstuff.com/posts/unity-to-steam
 * https://creativecommons.org/licenses/by/4.0/deed.en
 */
public class AchievementManager : MonoBehaviour
{
    [System.Serializable]
    public struct achievementID
    {
        // The achiID struct contains a section for steamID and androidID by default but simply add/remove any integrations you need.
        public string steamID;
        public string androidID;
    }
    [SerializeField] achievementID[] achiIDs;
    public int platform;
    bool isAchiUnlocked;

    // Set the keys in the editor via the achiIDs variable, and then just pass in an index to UnlockAchi when you want to unlock an achievement.
    public void UnlockAchievement(int _index)
    {
        // Prevent achievements from being unlocked in demo mode
        if (SteamManager.Initialized && !ScoreKeeper.versionIsDemo)
        {
            isAchiUnlocked = false;
            switch (platform)
            {
                case 0: //Steam
                    TestSteamAchievement(achiIDs[_index].steamID);
                    if (!isAchiUnlocked)
                    {
                        SteamUserStats.SetAchievement(achiIDs[_index].steamID);
                        SteamUserStats.StoreStats();
                    }
                    break;
                default:
                    break;
            }
        }
    }
    void TestSteamAchievement(string _id)
    {
        SteamUserStats.GetAchievement(_id, out isAchiUnlocked);
    }

    /*
    public void RelockAchi(int _index)
    {
        TestSteamAchi(achiIDs[_index].steamID);
        Debug.Log($"Achi with ID: {achiIDs[_index].steamID} unlocked = {isAchiUnlocked}");
        if (isAchiUnlocked)
        {
            SteamUserStats.ClearAchievement(achiIDs[_index].steamID);
            SteamUserStats.StoreStats();
        }
    }
    */
}
