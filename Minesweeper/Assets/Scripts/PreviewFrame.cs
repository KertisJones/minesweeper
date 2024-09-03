using UnityEngine;

public class PreviewFrame : MonoBehaviour
{
    public TetrominoSpawner spawner;
    public SpriteRenderer wallLeft;
    public SpriteRenderer wallRight;
    public SpriteRenderer wallBottom;

    public SpriteRenderer backgroundLines;

    public GameObject gameModeName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (spawner.previewCount == 0)
            wallLeft.size = new Vector2(5, 4);
        else if (spawner.previewCount > 1)
        {
            float wallHeight = 4;
            wallHeight += (spawner.previewCount - 1) * 3;
            wallLeft.size = new Vector2(1, wallHeight);
            wallRight.size = new Vector2(1, wallHeight);
            backgroundLines.size = new Vector2(4, wallHeight - 2);

            wallLeft.transform.localPosition -= new Vector3(0, (wallHeight - 4), 1);
            wallRight.transform.localPosition -= new Vector3(0, (wallHeight - 4), 1);
            wallBottom.transform.localPosition -= new Vector3(0, (wallHeight - 4), 1);
            backgroundLines.transform.localPosition -= new Vector3(0, (wallHeight - 4), 1);
            gameModeName.transform.localPosition -= new Vector3(0, (wallHeight - 4) * 10, 0);
        }

    }
}
