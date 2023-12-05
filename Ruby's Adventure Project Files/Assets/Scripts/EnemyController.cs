using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; //Allows you to reference TextMeshProUGUI.

public class EnemyController : MonoBehaviour
{
    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;

    Rigidbody2D rigidbody2D;
    float timer;
    int direction = 1;
    
    //
    float storedSpeed;

    Animator animator;

    bool broken = true;

    //
    bool stunned = false;

    public ParticleSystem smokeEffect;

    public TextMeshProUGUI fixedText; //A TextMeshPro GameObject that shows the number of robots fixed.

    public GameObject playerCharacter; //The Ruby playable character GameObject.

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();

        //
        storedSpeed = speed;

        if (RubyController.fixedRobotsAmount == 0) //Sets the number of fixed robots to 0 when starting. This code is optional; you can just set fixedText to "0" in the Inspector.
        {
            fixedText.text = RubyController.fixedRobotsAmount.ToString();
        }
    }

    void Update()
    {
        if (transform.position.magnitude > 1000.0f)
        {
            Destroy(gameObject);
        }

        if (!broken)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }

        //
        if(stunned == true)
        {
            speed = 0f;
        }
    }

    void FixedUpdate()
    {
        if (!broken)
        {
            return;
        }

        Vector2 position = rigidbody2D.position;

        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }

        rigidbody2D.MovePosition(position);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }

    //Public because we want to call it from elsewhere like the projectile script
    public void Fix()
    {
        broken = false;
        rigidbody2D.simulated = false;
        animator.SetTrigger("Fixed");
        smokeEffect.Stop();

        RubyController player = playerCharacter.GetComponent<RubyController>(); //Changes the on-screen text everytime a robot is fixed.
        RubyController.fixedRobotsAmount = RubyController.fixedRobotsAmount + 1;
        fixedText.text = RubyController.fixedRobotsAmount.ToString();
        if(RubyController.fixedRobotsAmount == player.maxRobots) //Brings up the win screen when fixing the max amount of robots.
        {
            player.VictoryScreen();
        }
    }

    //
    public void Stun()
    {
        stunned = true;
        StartCoroutine(StunTime());
    }

    //
    IEnumerator StunTime()
    {
        yield return new WaitForSeconds(3);
        stunned = false;
        speed = storedSpeed;
    }
}
