using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Collections;
//using Steamworks;

public class InitializeOperation : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;

        /*if (SteamManager.Initialized)
        {
            //SteamManager.
            string name = SteamFriends.GetPersonaName();
            Debug.Log(name);
        }*/

        GetComponent<LoadNewScene>().OpenNewScene("Title");
    }

}
