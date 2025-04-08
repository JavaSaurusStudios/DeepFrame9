using System.Collections;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public int type = 0;
    public SpriteRenderer[] spriteRenderers;
    public Collider2D spriteCollider;
    private int _hp = 1;

    public GameObject LargeEffect;
    public GameObject smallEffect;
    public ParticleSystem smokeEffect;


    private Vector3 targetLocalScale;

    public int hp
    {
        get
        {
            return _hp;
        }

        set
        {
            _hp = value;
            targetLocalScale = isPickup ? new Vector3(.2f, .2f, 1) : new Vector3(.33f, Mathf.Max(1, value) * .125f, 1);
        }
    }
    public bool isPickup;

    public delegate bool OnAttach(Collision2D collision);
    public OnAttach onAttach;

    public delegate void OnEffect(bool largeEffect);
    public OnEffect onEffect;

    public void DoSmoke()
    {
        if (smokeEffect != null)
        {
            smokeEffect?.Play();
        }
    }

    public void Boop(float y)
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y - y, transform.localScale.z);
    }

    public void Release(Vector3 direction, float speed = 5, float lifeTime = 3)
    {
        StartCoroutine(Launch(direction, speed, lifeTime));
    }

    private IEnumerator Launch(Vector3 direction, float speed, float lifeTime)
    {
        float t = 0;
        while (t < lifeTime)
        {
            t += Time.deltaTime;
            transform.position += direction * speed * Time.deltaTime;
            yield return null;
        }
        Recycle();
    }

    public void Recycle(float after = 0)
    {
        StartCoroutine(RecycleASync(after));
    }

    private IEnumerator RecycleASync(float after)
    {
        yield return new WaitForSeconds(after);
        hp = 1;
        BrickPool.INSTANCE.ReturnBrick(this);
        foreach (SpriteRenderer spriteRenderer in spriteRenderers) spriteRenderer.enabled = true;
        spriteCollider.enabled = true;
        if (LargeEffect != null) LargeEffect.SetActive(false);
    }

    void Update()
    {
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetLocalScale, Time.deltaTime * 3);
    }
}
