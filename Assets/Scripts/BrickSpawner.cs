using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
    public TMPro.TextMeshProUGUI depthIndicatorText; // Assign in Inspector

    private float yInterval = 10f;
    private float xInterval = .4f;
    private float xLimit = 2.4f;
    private int cols = 13;

    private int lv = 0;

    private System.Random generator;
    // Start is called before the first frame update
    void Start()
    {
        // CreateLevel();
        int seed = 1231283; // Could be based on a shared value (e.g., server timestamp, player ID)
        generator = new System.Random(seed);
    }

    private void CreateLevel()
    {
        depthIndicatorText.text = (lv * 100).ToString("D4");
        lv++;
        //Create the bottom of the level
        float baseLine = lv * yInterval;

        transform.position = Vector3.down * baseLine;

        Vector3 pos = Vector3.down * baseLine;
        for (int j = 0; j < cols; j++)
        {
            pos.x = -xLimit + (xInterval * j);
            Brick brick = BrickPool.INSTANCE.GetBrick(pos, Quaternion.identity, 3, 0);
        }

        //Spawn random bricks to pick up above 
        int amount = 1 + (lv * 2);
        for (int i = 0; i < amount; i++)
        {
            int spawnColumn = generator.Next(0, cols);
            int spawnRow = generator.Next(1, 9);
            Vector3 tmp = new Vector3(
                -xLimit + (xInterval * spawnColumn),
                -(baseLine + spawnRow),
                0);


            int rand = generator.Next(0, 100);
            int type = 1;

            if (rand > 60)
            {
                type = 2;
            }

            if (rand > 80)
            {
                type = 3;
                type = 0;
            }

            Brick brick = BrickPool.INSTANCE.GetBrick(tmp, Quaternion.identity, 1, (lv <= 1) ? 1 : type);

        }

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponent<ControllerFeedback>())
        {
            CreateLevel();
        }
    }

}


