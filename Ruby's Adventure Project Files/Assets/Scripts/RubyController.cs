using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    //Determines walk speed.
    public float speed = 3.0f;

    //Determines maximum amount of health and how long invincibility lasts after getting hit.
    public int maxHealth = 5;
    public float timeInvincible = 2.0f;

    //Determines current health and allows other scripts to access it.
    public int health { get { return currentHealth; } }
    int currentHealth;

    //Determines whether you are invincible and, if so, for how much longer.
    bool isInvincible;
    float invincibleTimer;

    //Determines collision.
    Rigidbody2D rigidbody2d;

    //Determines horizontal and vertical character movement.
    float horizontal;
    float vertical;

    //Determines animations based on cardinal directions.
    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    public GameObject projectilePrefab;

    //
    public GameObject stunProjectile;

    public AudioClip throwSound;
    public AudioClip hitSound;

    //
    public AudioClip chargeSound;
    public AudioClip stunThrowSound;

    public ParticleSystem damageEffect;

    AudioSource audioSource;

    public static int fixedRobotsAmount; //The current amount of fixed robots.

    public int maxRobots = 1; //The max amount of robots. I have it at 1 for testing purposes, but can be changed to a different value.

    public GameObject victoryText; //Text that is set active when all robots have been fixed.

    //
    public bool stunnerOn = false;

    //
    float holdDownStartTime;
    float holdDownEndTime;

    //
    float stunCogsLeft = 3;
    bool noCogs;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();

        fixedRobotsAmount = 0; //Sets the amount of fixed robots to 0 when the game begins.
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        //
        if (Input.GetKeyDown(KeyCode.C))
        {
            holdDownStartTime = Time.time;

            if (stunCogsLeft == 0)
            {
                noCogs = true;
            }

            if (stunnerOn == true && noCogs != true)
            {
                PlaySound(chargeSound);
            }
        }

        //
        if (Input.GetKeyUp(KeyCode.C))
        {
            holdDownEndTime = Time.time;

            if(holdDownEndTime - holdDownStartTime > 1f)
            {
                if (stunnerOn == true && noCogs != true)
                {
                    Launch(stunProjectile);
                    holdDownStartTime = holdDownStartTime - Time.time;
                    ChangeStunBar(-1);
                }
            }
            else if (holdDownEndTime - holdDownStartTime <= 1f)
            {
                Launch(projectilePrefab);
                holdDownStartTime = holdDownStartTime - Time.time;
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }

    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;

            damageEffect.Play();

            PlaySound(hitSound);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    //
    void Launch(GameObject bulletType)
    {
        GameObject projectileObject = Instantiate(bulletType, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        if(bulletType == stunProjectile)
        {
            PlaySound(stunThrowSound);
        }
        else
        {
            PlaySound(throwSound);
        }
    }

    //
    public void ChangeStunBar(float amount)
    {
        if(stunCogsLeft == 0)
        {
            stunnerOn = false;
        }
        stunCogsLeft = Mathf.Clamp(stunCogsLeft + amount, 0, 3);
        UIPowerupBar.instance.SetValue(stunCogsLeft / (float)3f);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void VictoryScreen() //Function that activates victoryText. Referenced in EnemyController.
    {
        victoryText.SetActive(true);
    }
}
