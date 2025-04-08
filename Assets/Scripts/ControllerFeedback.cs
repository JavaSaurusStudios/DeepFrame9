using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class ControllerFeedback : MonoBehaviour
{
    public static ControllerFeedback INSTANCE;

    [SerializeField]
    private ParticleSystem fireSystem;
    [SerializeField]
    private ParticleSystem launchSystem;
    [SerializeField]
    private ParticleSystem chargingSystem;
    [SerializeField]
    private HitSoundEffect hitSound;
    [SerializeField]
    private PowerupSoundEffect chargeSound;
    private float chargeTime;
    private float chargeMaxed = .75f;
    private int chargedMultiplier = 2;
    private bool isChargedShot = false;

    private LineRenderer lr;
    [SerializeField]
    private float maxDragDistance = 3f;
    [SerializeField]
    private float maxLaunchPower = 15f;

    [SerializeField]
    private float minCrashSpeed = 5f;

    private Rigidbody2D rb;
    private Vector3 currentRbPos;


    private List<Brick> attachedBricks;

    public Controller controller;

    private bool holding = false;

    [SerializeField]
    private float radius = .25f;
    private float currentAngularVelocity;

    [SerializeField]
    private SpriteRenderer bodyRenderer;
    private Color initColor;

    private AudioSource audioSource;
    public AudioClip attachSound;
    public AudioClip releaseSound;

    public Transform faceTransform;

    private int baseDamageMultiplier = 1;
    // Start is called before the first frame update
    void Awake()
    {
        INSTANCE = this;

        audioSource = GetComponent<AudioSource>();
        initColor = bodyRenderer.color;
        ;
        attachedBricks = new List<Brick>();

        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        controller.onDragUpdate += (isDragging, holdDuration, startMousePosition, currentMousePosition, dragDistance, dragDirection) =>
        {
            if (GameManager.INSTANCE.state != GameManager.GameState.PLAYING) return;
            if (isDragging && TurnTracker.INSTANCE.isTurn)
            {
                if (!holding && isDragging)
                {
                    //dragging start!
                    chargeTime = 0;
                }
                chargeTime += 3 * Time.deltaTime;
                bodyRenderer.color = Color.Lerp(bodyRenderer.color, Color.white, 3 * Time.deltaTime);
                chargeSound.Play();
                holding = true;
                //  if (dragDirection.y > 0) return;
                rb.angularVelocity = 0;
                rb.position = currentRbPos;
                lr.enabled = true;
                lr.SetPosition(0, transform.position);
                Vector3 point = transform.position + dragDirection * Mathf.Min(maxDragDistance, dragDistance);
                lr.SetPosition(1, point);
                launchSystem.transform.position = transform.position;
                //rb.MovePosition(Vector2.MoveTowards(rb.position, currentMousePosition, Time.deltaTime * loadingSpeed));
                rb.gravityScale = 0;
            }
            else
            {
                if (holding && !isDragging)
                {
                    //dragging END!

                }
                bodyRenderer.color = Color.Lerp(bodyRenderer.color, initColor, Time.deltaTime * 3);
                chargeSound.Stop();
                holding = false;
                currentAngularVelocity = rb.angularVelocity;
                currentRbPos = rb.position;
                rb.gravityScale = 1;
                lr.enabled = false;
            }
        };

        controller.onLaunch += (distance, direction) =>
        {
            if (GameManager.INSTANCE.state != GameManager.GameState.PLAYING) return;
            if (!TurnTracker.INSTANCE.isTurn) return;
            isChargedShot = chargeTime >= chargeMaxed;

            TurnTracker.INSTANCE.TakeTurn();
            launchSystem.Play();
            holding = false;
            //if (direction.y > 0) return;
            rb.angularVelocity = currentAngularVelocity;
            rb.velocity = (-direction * Mathf.Lerp(0, maxLaunchPower, distance / maxDragDistance));
        };

    }

    void LateUpdate()
    {

        var fireEmission = fireSystem.emission;
        if (GameManager.INSTANCE.state == GameManager.GameState.WAITING || GameManager.INSTANCE.state == GameManager.GameState.STARTING)
        {
            fireEmission.rateOverTime = 35;
        }
        else
        {
            fireEmission.rateOverTime = holding ? 0 : Mathf.Lerp(0, 300, rb.velocity.magnitude / 20f);
        }

        if (GameManager.INSTANCE.state != GameManager.GameState.PLAYING)
        {
            chargeSound.Stop();
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
        }
        else
        {
            rb.isKinematic = false;
        }



        if (GameManager.INSTANCE.state != GameManager.GameState.PLAYING) return;

        //TODO fix the holding
        float y = faceTransform.localScale.y;
        if (Input.GetMouseButtonDown(0))
        {
            chargingSystem.Play();
        }


        faceTransform.localScale = new Vector3(
            faceTransform.localScale.x,
            Mathf.Lerp(faceTransform.localScale.y, Input.GetMouseButton(0) ? .1f : 1f, Time.deltaTime * 5f),
            faceTransform.localScale.z
        );


        if (Input.GetMouseButtonUp(0))
        {
            chargingSystem.Stop();
        }



    }

    [SerializeField]
    private bool crashThroughEnabled = false;
    private bool reapplyVelocity;
    private Vector3 velocityBuffer;

    void OnCollisionExit2D(Collision2D collision)
    {
        if (reapplyVelocity && crashThroughEnabled)
        {
            reapplyVelocity = false;
            rb.isKinematic = false;
            rb.velocity = velocityBuffer;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        reapplyVelocity = false;

        if (GameManager.INSTANCE.state != GameManager.GameState.PLAYING) return;
        Brick brick = collision.collider.GetComponent<Brick>();
        if (!brick) brick = collision.collider.GetComponentInParent<Brick>();

        int dmg = baseDamageMultiplier * (isChargedShot ? chargedMultiplier : 1) * Mathf.FloorToInt(rb.velocity.magnitude / minCrashSpeed);
        hitSound.Play();

        if (brick && (rb.velocity.magnitude > minCrashSpeed || brick.isPickup))
        {
            brick.DoSmoke();
            DoDamage(brick, brick.isPickup ? 5 : dmg, isChargedShot, collision);
            if (!brick.isPickup && isChargedShot)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, Mathf.Min(1.5f, (isChargedShot ? .5f : .25f) * (1 + attachedBricks.Count)));
                if (colliders.Length > 0)
                {
                    foreach (Collider2D collider in colliders)
                    {
                        Brick brick2 = collider.GetComponent<Brick>();
                        if (brick2 == null) brick2 = collider.GetComponentInParent<Brick>();
                        if (brick2 != null && brick2 != brick && !brick2.isPickup)
                        {
                            int newDmg = dmg * Mathf.CeilToInt(1f / Vector3.Distance(transform.position, collider.transform.position));
                            DoDamage(brick2, newDmg, false, collision);
                        }
                    }
                }
            }
        }
        isChargedShot = false;
    }


    private void DoDamage(Brick brick, int dmg, bool isCharged, Collision2D collision)
    {

        brick.hp -= dmg;
        if (brick.hp <= 0)
        {
            bool attached = brick.isPickup && (brick.onAttach == null ? false : brick.onAttach.Invoke(collision));
            if (attached)
            {
                attachedBricks.Add(brick);
            }
            else
            {
                if (isChargedShot && crashThroughEnabled)
                {
                    reapplyVelocity = true;
                    velocityBuffer = rb.velocity;
                    rb.velocity = Vector2.zero;
                    rb.isKinematic = true;
                }
                //this is the destroying
                foreach (SpriteRenderer spriteRenderer in brick.spriteRenderers) spriteRenderer.enabled = false;
                brick.spriteCollider.enabled = false;
                if (isChargedShot) brick.onEffect?.Invoke(isCharged);
                brick.Recycle(3f);
            }
        }
        else
        {
            brick.Boop(.25f);
        }
    }


    public void AttachBrick(Collision2D collision)
    {
        Vector3 dir = (collision.collider.transform.position - transform.position).normalized;
        collision.collider.enabled = false;
        collision.collider.transform.position = transform.position + (dir * .25f);
        collision.collider.transform.parent = transform;
        audioSource.pitch = Random.Range(.95f, 1.05f);
        audioSource.PlayOneShot(attachSound);
    }

    public void ReleaseBricks()
    {
        foreach (Brick brick in attachedBricks)
        {
            Vector3 dir = (transform.position - brick.transform.position).normalized;
            brick.transform.parent = null;
            brick.Release(dir.normalized);
            audioSource.pitch = Random.Range(.95f, 1.05f);
            audioSource.PlayOneShot(releaseSound);
        }
        attachedBricks.Clear();
    }


}