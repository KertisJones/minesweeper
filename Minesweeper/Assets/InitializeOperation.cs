using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Collections;
public class InitializeOperation : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;

        GetComponent<LoadNewScene>().OpenNewScene("Title");
    }

}
