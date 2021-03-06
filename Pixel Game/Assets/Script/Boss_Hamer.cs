using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Boss_Hamer : Enemy
{
    private bool isDead;
    public bool isFlipped = false;
    public Rigidbody2D Rb;
    public AudioSource hamer1,sword1;
    public void Start()
    {
        isDead = false;
        EnemyBar.max = health;
        base.Start();
        Rb = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        EnemyBar.num = health;
        if (health <= 0)
        {
            EnemyDead();
        }
        else if (health <= 100)
        {
            GetComponent<Animator>().SetTrigger("IsEnrage");
        }
    }

    public void LookAtPlayer()
    {
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if (transform.position.x > Player.transform.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (transform.position.x < Player.transform.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }
    public void EnemyDead()
    {
        if(isDead == false)
        {
            GetComponent<Animator>().SetTrigger("Dead");
        }
        isDead = true;
        

    }
    public void BossDeadScore()
    {
        PlayerPrefs.SetInt("2-2", 3);
        SceneManager.LoadScene("Class");
    }
    public void JumpAtcChangePosition()
    {
        if (isFlipped)
        {
            gameObject.transform.position = new Vector2(gameObject.transform.position.x  +8.1f, gameObject.transform.position.y);

        }
        else
        {
            gameObject.transform.position = new Vector2(gameObject.transform.position.x - 8.1f, gameObject.transform.position.y);
        }
    }
    public void SpinAtcChangePosition()
    {
        if (isFlipped)
        {
            gameObject.transform.position = new Vector2(gameObject.transform.position.x + 2.6f, gameObject.transform.position.y);

        }
        else
        {
            gameObject.transform.position = new Vector2(gameObject.transform.position.x - 2.6f, gameObject.transform.position.y);
        }
    }
    public void DashChangePosition()
    {
        if (!isFlipped)
        {
            gameObject.transform.position = new Vector2(gameObject.transform.position.x -4f, gameObject.transform.position.y);

        }
        else
        {
            gameObject.transform.position = new Vector2(gameObject.transform.position.x +4f, gameObject.transform.position.y);
        }
    }
    public void hamer1play()
    {
        hamer1.Play();
    }
    public void sword1play()
    {
        sword1.Play();
    }
}
