using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerWizard : MonoBehaviour
{
    [Header("Player參數")]
    private Rigidbody2D Rb;
    private Animator Anim;
    public Collider2D GroundSensor;
    public Collider2D RightSlopeSensor;
    public LayerMask Ground;
    private float Speed = 250, JumpForce = 750;
    private float m_timeSinceAttack = 0, m_timeCollect = 0;
    public float JumpCount;
    private bool jumpPressed, NoGetDamage = false;
    static public float PlayerDamage = 1, PlayerHealth = 10;
    static public bool isColliding;
    private Renderer myRender;
    public static float facedirection, horizontalmove, JumpButton;
    public int Blinks;
    public float times, deadzonecount;
    public Button A, B, X, Y;
    public GameObject EnterEndZone;
    [Header("Material")]
    public PhysicsMaterial2D hard;
    public PhysicsMaterial2D soft;
    [Header("AttackCool")]
    public float atkcool;
    public Image atkbar;
    static public bool atkhit=false,atkuse=true,abuttondown=false;
    [Header("Fireboll")]
    private bool boll;
    public GameObject firebollobj;
    public Image firebollbar;
    public float firebollcool;

    void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        myRender = GetComponent<Renderer>();
    }
    void Update()//檢查是否輸入
    {
        Anim.SetFloat("running", Mathf.Abs(facedirection));
        if (GroundSensor.IsTouchingLayers(Ground))
        {
            JumpCount = 2;
        }
        //horizontalmove = Input.GetAxis("Horizontal");
        //facedirection = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump") && JumpCount > 0)
        {
            jumpPressed = true;
        }
        B.onClick.AddListener(() => {
            if (JumpCount > 0)
            {
                jumpPressed = true;
            }
        });
        if (Input.GetButton("Fire1") && atkuse)
        {
            Anim.SetBool("Attack", true);
            Anim.SetBool("NAttack", false);
            atkhit = true;
        }
        if (abuttondown && atkuse)
        {
            Anim.SetBool("Attack", true);
            Anim.SetBool("NAttack", false);
            atkhit = true;
        }
        if (atkhit == false)
        {
            Anim.SetBool("NAttack", true);
            Anim.SetBool("Attack", false);
        }
        if (Input.GetButton("Fire2") && firebollbar.fillAmount >= 0.99)
        {
            boll = true;
            firebollbar.fillAmount = 0;
        }
        X.onClick.AddListener(() => {
            if (firebollbar.fillAmount >= 0.99)
            {
                boll = true;
                firebollbar.fillAmount = 0;
            }
        });
    }
    void FixedUpdate()
    {
        m_timeSinceAttack += Time.deltaTime;
        m_timeCollect += Time.deltaTime;
        deadzonecount += Time.deltaTime;
        Movement();
        if (StaticCharactor.health <= 0)
        {
            StaticCharactor.lastheart -= 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        atkcheck();
        if (boll)
        {
            fireboll();
            boll = false;
        }else firebollbar.fillAmount += 1.0f / firebollcool * Time.deltaTime;
    }
    void Movement()
    {
        //腳色移動
        if (horizontalmove != 0f)
        {
            Rb.velocity = new Vector2(horizontalmove * Speed * Time.deltaTime, Rb.velocity.y);//* Time.deltaTime
            //Anim.SetFloat("running", Mathf.Abs(facedirection));
        }

        //面相方向
        if (facedirection > 0f)
        {
            transform.localScale = new Vector3(0.7f, 0.7f, 1);
        }
        if (facedirection < 0f)
        {
            transform.localScale = new Vector3(-0.7f, 0.7f, 1);
        }

        //設定Material 在空中時不卡牆壁
        if (GroundSensor.IsTouchingLayers(Ground))
            Rb.sharedMaterial = hard;
        else
            Rb.sharedMaterial = soft;

        //設定上斜坡
        if (RightSlopeSensor.IsTouchingLayers(Ground) && horizontalmove != 0f)
        {
            Rb.sharedMaterial = soft;
            //Rb.velocity = new Vector2(Rb.velocity.x, 4);
        }

        //腳色跳躍 多段跳躍 if(Input.GetButtonDown("Jump"))
        if (jumpPressed && GroundSensor.IsTouchingLayers(Ground))
        {
            Rb.velocity = new Vector2(Rb.velocity.x, JumpForce * Time.deltaTime); //*Time.deltaTime
            JumpCount--;
            jumpPressed = false;
        }
        else if (jumpPressed && JumpCount > 0 && !GroundSensor.IsTouchingLayers(Ground))
        {
            Rb.velocity = new Vector2(Rb.velocity.x, JumpForce * Time.deltaTime); //*Time.deltaTime
            JumpCount--;
            jumpPressed = false;
        }
    }
    public void PlayerGetDamage(float damage)
    {
        print("yes");
        if (!NoGetDamage)
        {
            StaticCharactor.health -= damage;
            Setting.Instance.shakecamera(3f, .2f);
            BlinkPlayer(Blinks, times);
        }
    }
    void BlinkPlayer(int numblinks, float seconds)
    {
        StartCoroutine(DoBlinks(numblinks, seconds));
    }
    IEnumerator DoBlinks(int numBlinks, float seconds)
    {
        NoGetDamage = true;
        for (int i = 0; i < numBlinks * 2; i++)
        {
            myRender.enabled = !myRender.enabled;
            yield return new WaitForSeconds(seconds);
        }
        myRender.enabled = true;
        NoGetDamage = false;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        //撿取藥水
        if (collision.gameObject.tag == "Posion")
        {
            if (m_timeCollect >= 0.05)
            {
                if (StaticCharactor.health <= 8)
                {
                    Destroy(collision.gameObject);
                    StaticCharactor.health += 2;
                }
                else if (StaticCharactor.health == 9)
                {
                    Destroy(collision.gameObject);
                    StaticCharactor.health += 2;
                }
            }
            m_timeCollect = 0;
        }
        //撿到金幣
        if (collision.gameObject.tag == "Coin")
        {
            if (m_timeCollect >= 0.02)
            {
                Destroy(collision.gameObject);
                CoinCount.Coin += 1;
                CoinCount.coinsoundplay();
            }
            m_timeCollect = 0;
        }
        //結束區域
        if (collision.gameObject.tag == "EndZone")
        {
            EnterEndZone.SetActive(true);
        }
        //死亡區域
        if (collision.gameObject.tag == "DeadZone")
        {
            if (deadzonecount > 0.3f)
            {
                StaticCharactor.lastheart -= 1;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                deadzonecount = 0;
            }
        }
    }
    void atkcheck()
    {
        if(atkhit == true)
        {
            atkbar.fillAmount -= 1.5f / atkcool * Time.deltaTime;
        }
        else
        {
            atkbar.fillAmount += 1.0f / atkcool * Time.deltaTime;
        }
        if (atkbar.fillAmount <= 0.01)
        {
            atkuse = false;
        }
        if (atkbar.fillAmount >= 0.99)
        {
            atkuse = true;
        }
        atkhit = false;
    }
    void fireboll()
    {
        if (transform.localScale.x >0)
        {
            Instantiate(firebollobj, new Vector3(transform.position.x + 1.8f, transform.position.y, transform.position.z), transform.rotation);
        }
        if (transform.localScale.x <0)
        {
            Instantiate(firebollobj, new Vector3(transform.position.x - 1.8f, transform.position.y, transform.position.z), transform.rotation);
        }
    }
}