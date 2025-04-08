using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickPool : MonoBehaviour
{
    public static BrickPool INSTANCE;

    public Brick brickPickupPrefab;
    public Brick brickPrefab;

    private List<Brick> brickPool;
    private List<Brick> pickupPool;

    void Awake()
    {
        INSTANCE = this;

        pickupPool = new List<Brick>();
        for (int i = 0; i < 20; i++)
        {
            Brick brick = Instantiate(brickPickupPrefab);
            brick.isPickup = true;
            brick.gameObject.SetActive(false);
            pickupPool.Add(brick);
        }

        brickPool = new List<Brick>();
        for (int i = 0; i < 100; i++)
        {
            Brick brick = Instantiate(brickPrefab);
            brick.gameObject.SetActive(false);
            brickPool.Add(brick);
        }

    }

    public Brick GetBrick(Vector3 position, Quaternion rotation, int hp, int type)
    {

        Brick prefab;
        List<Brick> pool;

        switch (type)
        {
            case 1:
                prefab = brickPickupPrefab;
                pool = pickupPool;
                break;
            case 2:
                prefab = brickPickupPrefab;
                pool = pickupPool;
                break;
            case 3:
                prefab = brickPickupPrefab;
                pool = pickupPool;
                break;
            default:
                prefab = brickPrefab;
                pool = brickPool;
                break;
        }
        Brick targetBrick = null;
        foreach (Brick brick in pool)
        {
            if (!brick.gameObject.activeInHierarchy)
            {
                brick.transform.position = position;
                brick.transform.rotation = rotation;
                brick.hp = hp;
                brick.gameObject.SetActive(true);
                targetBrick = brick;
                break;
            }
        }

        // Optional: Expand pool if none are available


        targetBrick = (targetBrick == null) ? Instantiate(prefab, position, rotation) as Brick : targetBrick;
        targetBrick.hp = hp;
        targetBrick.gameObject.SetActive(true);
        pool.Add(targetBrick);


        switch (type)
        {
            case 1:
                targetBrick.gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
                targetBrick.onAttach = (collision) =>
                {
                    ControllerFeedback.INSTANCE.AttachBrick(collision);
                    return true;
                };
                break;

            case 2:
                targetBrick.gameObject.GetComponent<SpriteRenderer>().color = Color.magenta;
                targetBrick.onAttach = (collision) =>
                {
                    ControllerFeedback.INSTANCE.AttachBrick(collision);
                    StartCoroutine(Inline());
                    IEnumerator Inline() { yield return new WaitForSeconds(3); ControllerFeedback.INSTANCE.ReleaseBricks(); }
                    return true;
                };
                break;

            case 3:
                targetBrick.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                targetBrick.onAttach = (collision) =>
                {
                    return false;
                };
                break;

            default:
                targetBrick.onAttach = (collision) => { return false; };
                targetBrick.onEffect = (x) =>
                {
                    if (x)
                    {
                        targetBrick.LargeEffect?.SetActive(true);
                    }
                    else
                    {
                        targetBrick.LargeEffect?.SetActive(true);
                    }
                };
                break;
        }




        return targetBrick;
    }

    public void ReturnBrick(Brick brick)
    {
        brick.gameObject.SetActive(false);
    }
}
