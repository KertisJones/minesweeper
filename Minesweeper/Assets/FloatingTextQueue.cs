using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static FloatingTextQueue;

public class FloatingTextQueue : MonoBehaviour
{
    public int maxLines = 5;
    public float marginDistance = 5f;
    public Vector3 singularFloaterPositionOffset = Vector3.zero;
    public List<Floater> textQueue = new List<Floater>();
    public Floater singularFoater = new Floater();
    public GameObject floatingTextPrefab;
    public GameObject floatingTextSingularPrefab;

    private int updatePositionsOnUpdate = 0;

    [System.Serializable]
    public struct Floater
    {
        public Floater(float newScoreValue, string newTranslationKey, string newTranslationKeyPrefix1, string newTranslationKeyPrefix2, string newTranslationKeySuffix, int newComboCount, GameObject newFloatingText)
        {
            scoreValue = newScoreValue;
            translationKey = newTranslationKey;
            translationKeyPrefix1 = newTranslationKeyPrefix1;
            translationKeyPrefix2 = newTranslationKeyPrefix2;
            translationKeySuffix = newTranslationKeySuffix;
            comboCount = newComboCount;
            floatingText = newFloatingText;
        }

        public float scoreValue;
        public string translationKey;
        public string translationKeyPrefix1;
        public string translationKeyPrefix2;
        public string translationKeySuffix;
        public int comboCount;
        public GameObject floatingText;


        public void UpdateScore(float newScoreValue, int newComboCount)
        {
            scoreValue = newScoreValue;
            comboCount = newComboCount;
            floatingText.GetComponent<FloatingText>().UpdateTextScore(scoreValue, translationKey, comboCount, translationKeyPrefix1, translationKeyPrefix2, translationKeySuffix);
        }

        public void SetTranslationKey(string newTranslationKey)
        {
            translationKey = newTranslationKey;
        }

        public void SetPrefixes(string newTranslationKeyPrefix1, string newTranslationKeyPrefix2, string newTranslationKeySuffix)
        {
            translationKeyPrefix1 = newTranslationKeyPrefix1;
            translationKeyPrefix2 = newTranslationKeyPrefix2;
            translationKeySuffix = newTranslationKeySuffix;
        }
    }

    private void Update()
    {
        if (updatePositionsOnUpdate > 0)
        {
            updatePositionsOnUpdate--;
            PositionFloaters();
        }
    }

    public void SpawnText(float scoreValue, string translationKey, bool combineExistingScores, string translationKeyPrefix1, string translationKeyPrefix2, string translationKeySuffix, int comboCount = 1)
    {
        // If this score description exists already, increment it instead of spawning a new text
        if (combineExistingScores)
        {
            foreach (Floater floater in textQueue)
            {
                if (floater.translationKey == translationKey)
                {
                    if (comboCount  == 1)
                        IncrementFloater(textQueue.IndexOf(floater), scoreValue);
                    else
                        IncrementFloater(textQueue.IndexOf(floater), scoreValue, comboCount);
                    return;
                }
            }
        }

        GameObject newFloatingText = Instantiate(floatingTextPrefab, this.transform.position, Quaternion.identity, this.transform);
        newFloatingText.GetComponent<FloatingText>().UpdateTextScore(scoreValue, translationKey, comboCount, translationKeyPrefix1, translationKeyPrefix2, translationKeySuffix);
               
        Floater newFloater = new Floater(scoreValue, translationKey, translationKeyPrefix1, translationKeyPrefix2, translationKeySuffix, 1, newFloatingText);
        textQueue.Insert(0, newFloater);

        if (textQueue.Count > maxLines)
            Destroy(textQueue[textQueue.Count - 1].floatingText);

        // Force the canvas and text mesh to update
        Canvas.ForceUpdateCanvases();
        newFloatingText.GetComponent<TextMeshProUGUI>().ForceMeshUpdate();

        updatePositionsOnUpdate = 3;
        PositionFloaters();
    }

    private void IncrementFloater(int currentIndex, float scoreValueAdd)
    {
        IncrementFloater(currentIndex, scoreValueAdd, textQueue[currentIndex].comboCount + 1);
    }
    private void IncrementFloater(int currentIndex, float scoreValueAdd, int newComboCount)
    {
        Floater floaterToUpdate = textQueue[currentIndex];


        floaterToUpdate.UpdateScore(floaterToUpdate.scoreValue + scoreValueAdd, newComboCount);
        
        // Reset its position in the queue
        textQueue.RemoveAt(currentIndex);
        textQueue.Insert(0, floaterToUpdate);

        // Force the canvas and text mesh to update
        Canvas.ForceUpdateCanvases();
        floaterToUpdate.floatingText.GetComponent<TextMeshProUGUI>().ForceMeshUpdate();

        updatePositionsOnUpdate = 3;
        PositionFloaters();
    }
    public void RemoveFloater(GameObject floatingTextToRemove)
    {
        int indexToDestroy = -1;
        foreach (Floater floater in textQueue)
        {
            if (floater.floatingText.Equals(floatingTextToRemove))
            {
                indexToDestroy = textQueue.IndexOf(floater);
            }                
        }

        if (indexToDestroy != -1)
            textQueue.RemoveAt(indexToDestroy);
    }

    void PositionFloaters()
    {       
        float totalDistance = 0;

        foreach (Floater floater in textQueue)
        {
            floater.floatingText.GetComponent<TextMeshProUGUI>().ForceMeshUpdate();

            //SetNewStartingValues()
            floater.floatingText.GetComponent<IdleJiggle>().ForceResetPosition();
            floater.floatingText.transform.localPosition = new Vector3(floater.floatingText.transform.localPosition.x, totalDistance, floater.floatingText.transform.localPosition.z);
            floater.floatingText.GetComponent<IdleJiggle>().SetNewStartingPosition();

            totalDistance -= floater.floatingText.GetComponent<TextMeshProUGUI>().mesh.bounds.size.y + marginDistance;
        }
    }

    public void UpdateSingularFloater(float scoreValue, string translationKey, string translationKeyPrefix1, string translationKeyPrefix2, string translationKeySuffix, int comboCount = 1)
    {
        // Singular Floater is empty, spawn new...
        if (singularFoater.floatingText == null)
        {
            GameObject newFloatingText = Instantiate(floatingTextSingularPrefab, this.transform.position + singularFloaterPositionOffset, Quaternion.identity, this.transform);
            newFloatingText.GetComponent<FloatingText>().UpdateTextScore(scoreValue, translationKey, comboCount, translationKeyPrefix1, translationKeyPrefix2, translationKeySuffix);

            singularFoater = new Floater(scoreValue, translationKey, translationKeyPrefix1, translationKeyPrefix2, translationKeySuffix, 1, newFloatingText);
        }
        // Otherwise, update floater
        else
        {
            int combo = comboCount;
            if (combo == 1)
                combo += singularFoater.comboCount;

            singularFoater.SetPrefixes(translationKeyPrefix1, translationKeyPrefix2, translationKeySuffix);
            singularFoater.UpdateScore(singularFoater.scoreValue + scoreValue, combo);
        }
    }

    public void ResetSingularFloater()
    {
        Destroy(singularFoater.floatingText);
        singularFoater = new Floater();
    }

    public void RefreshSingularFloater()
    {
        if (singularFoater.floatingText == null)
            return;
        singularFoater.floatingText.GetComponent<FloatingText>().RefreshFade();
    }

}
