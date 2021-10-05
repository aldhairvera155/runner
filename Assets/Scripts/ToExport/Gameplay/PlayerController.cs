using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [Header("Horizontal Movement")]
    [SerializeField] private Rigidbody2D rgb;

    [FormerlySerializedAs("speed")] [SerializeField]
    private float moveSpeed;

    [Header("Vertical Movement")]
    [SerializeField] private float jumpForce;

    public bool grounded;
    public LayerMask whatIsGround;
    [SerializeField] private float checkRadius;
    [SerializeField] private Transform groundCheck;
    
    private Collider2D col;
    
    [Space]
    [Space]

    [SerializeField] private int extraJumps;

    private bool started = false;

    [SerializeField] private Vector2 powerAmount;

    private Coroutine currentCoroutine = null;
    private Coroutine endCoroutine = null;

    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem good_Particle;
    [SerializeField] private ParticleSystem bad_Particle;

    [Header("Player Gender")]
    [SerializeField] private GameObject maleGameobject;
    [SerializeField] private GameObject femaleGameobject;

    [SerializeField] private Collider2D maleCollider;
    [SerializeField] private Collider2D femaleCollider;
    [SerializeField] private List<SpriteRenderer> maleSprites = new List<SpriteRenderer>();
    [SerializeField] private List<SpriteRenderer> femaleSprites = new List<SpriteRenderer>();
    private List<SpriteRenderer> tempSprites = new List<SpriteRenderer>();

    [Header("End Game Properties")]
    [SerializeField] private float waitTime;

    [SerializeField] private AnimationCurve animationCurve;
    private float _timer = 0;

    private Vector2 gravity;

    private bool isEnding = false;
    private bool endStuff = false;
    private bool runAgain = false;

    private void Start()
    {
        if (PlayerPrefs.GetInt("Gender") == 0 || !PlayerPrefs.HasKey("Gender"))
            // if(SaveDataManager._instance.gameData.gender==0)
        {
            femaleGameobject.SetActive(false);
            anim = maleGameobject.GetComponent<Animator>();
            tempSprites = maleSprites;
            col = maleCollider;
        }
        else
        {
            maleGameobject.SetActive(false);
            anim = femaleGameobject.GetComponent<Animator>();
            tempSprites = femaleSprites;
            col = femaleCollider;
        }

        Physics2D.gravity = new Vector2(0f, -9.81f);

        SetExtraJumps();

        good_Particle.Stop();
        bad_Particle.Stop();
        gravity = Physics2D.gravity;
    }


    private void FixedUpdate()
    {
        if (GameStates._instance.IsGamePlaying())
        {
            //Has a little delay
            // grounded = Physics2D.IsTouchingLayers(col, whatIsGround);

            grounded = Physics2D.OverlapCircle(groundCheck.position,checkRadius, whatIsGround);
            
            if (!started)
            {
                anim.SetTrigger("Run");
                started = true;
                Physics2D.gravity = gravity;
            }

            if (grounded)
                Movement();
        }
        else if (GameStates._instance.IsGamePre_End())
        {
            _timer += Time.deltaTime;

            rgb.velocity = new Vector2(animationCurve.Evaluate(_timer) * Time.fixedDeltaTime * moveSpeed,
                rgb.velocity.y);
            if (_timer < .9f)
            {
                grounded = Physics2D.IsTouchingLayers(col, whatIsGround);

                anim.SetBool("JumpB", false);
            }

            if (_timer >= .9f)
            {
                rgb.velocity = new Vector2(animationCurve.Evaluate(1) * Time.fixedDeltaTime * moveSpeed,
                    rgb.velocity.y);
                GameStates._instance.Set_GameState_End();
            }
        }
        else if (GameStates._instance.IsGameEnd())
        {
            if (!isEnding)
            {
                var currentStars = PlayerPrefs.GetInt(EndMessage._level + "_localStarNumber");
                if (currentStars > 0)
                {
                    anim.SetTrigger("Celebrate");
                    AudioMixerManager._instance.PlayBackgroundSource2(true);
                }
                else
                {
                    anim.SetTrigger("Bad");
                    AudioMixerManager._instance.PlayBackgroundSource2(false);
                }

                isEnding = true;
            }
        }
        else
        {
            if (started)
            {
                rgb.velocity = Vector2.zero;
                anim.SetTrigger("Stop");
                started = false;
                Physics2D.gravity = Vector2.zero;
            }
        }
    }


    private void Update()
    {
        if (GameStates._instance.IsGamePlaying())
        {
            if (grounded)
            {
                if (!runAgain)
                {
                    // anim.SetTrigger("Run");
                    anim.SetBool("JumpB", false);
                    runAgain = true;
                }

                SetExtraJumps();
            }

            if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && extraJumps > 0)
            {
                runAgain = false;
                AudioMixerManager._instance.CallSFX(AudioMixerManager.SFXType.Jump);
                anim.SetBool("JumpB", true);
                rgb.velocity = new Vector2(rgb.velocity.x, Time.fixedDeltaTime * jumpForce);
                extraJumps--;
            }
            else if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && extraJumps <= 0 &&
                     grounded)
            {
                AudioMixerManager._instance.CallSFX(AudioMixerManager.SFXType.Jump);
                anim.SetBool("JumpB", true);
                rgb.velocity = new Vector2(rgb.velocity.x, Time.fixedDeltaTime * jumpForce);
            }
        }
    }

    private void SetExtraJumps()
    {
        extraJumps = 1;
    }

    private void Movement()
    {
        rgb.velocity = new Vector2(Time.fixedDeltaTime * moveSpeed, rgb.velocity.y);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("End"))
        {
            other.gameObject.GetComponent<Collider2D>().enabled = false;
            GameStates._instance.Set_GameState_Pre_End();

            EndMessage._instance.EndRain();

            if (endCoroutine == null)
                endCoroutine = StartCoroutine(EndLevelCondition());
        }
    }

    private IEnumerator EndLevelCondition()
    {
        print("Call end level condition");
        yield return new WaitForSeconds(waitTime);
        rgb.velocity = Vector2.zero;
        EndMessage._instance.AnimateAndSet();
        GameStates._instance.Set_GameState_End();
    }

    public void BlinkEffect()
    {
        if (currentCoroutine == null)
            currentCoroutine = StartCoroutine(BlinkEffect_Coroutine());
    }

    public void RunParticleEffect(CollectableObject.CollectableType _collectableType)
    {
        switch (_collectableType)
        {
            case CollectableObject.CollectableType.Collectable:
                good_Particle.Play();
                break;
            case CollectableObject.CollectableType.Obstacle:
                bad_Particle.Play();
                break;
        }
    }

    private IEnumerator BlinkEffect_Coroutine()
    {
        var tempColor = Color.white;
        var count = 0;

        while (count <= 2)
        {
            for (var i = 0; i < tempSprites.Count; i++)
                tempSprites[i].color = new Color(tempColor.r, tempColor.g, tempColor.b, 0);
            yield return new WaitForSeconds(.2f);

            for (var i = 0; i < tempSprites.Count; i++)
                tempSprites[i].color = new Color(tempColor.r, tempColor.g, tempColor.b, 1);
            yield return new WaitForSeconds(.2f);
            count++;
        }

        currentCoroutine = null;
    }
}