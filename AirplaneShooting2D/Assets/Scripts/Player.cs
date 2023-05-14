using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float hAxis;
    public float vAxis;

    public int life;
    public int score;
    public float speed;
    public int maxPower;
    public int power;
    public int maxBoom;
    public int boom;
    public float maxShotDelay;
    public float curShotDelay;

    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject boomEffect;

    public GameManager gameManager;
    public ObjectManager objectManager;
    public bool isHit;
    public bool isBoomTime;

    public GameObject[] followers;
    public bool isRespawnTime;


    public bool[] joyControl;
    public bool isControl;
    public bool isButtonA;
    public bool isButtonB;

    Animator anim;
    SpriteRenderer spriter;

    void Awake()
    {
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        UnHitable();
        Invoke("UnHitable", 3);
    }

    void UnHitable()
    {
        isRespawnTime = !isRespawnTime;

        if (isRespawnTime)
        {
            spriter.color = new Color(1, 1, 1, 0.5f);

            for (int i = 0; i < followers.Length; i++)
                followers[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        }

        else
        {
            spriter.color = new Color(1, 1, 1, 1);

            for (int i = 0; i < followers.Length; i++)
                followers[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        }
    }

    void Update()
    {
        UpdateMove();
        UpdateFire();
        UpdateBoom();
        UpdateReload();
    }

    public void JoyPanel(int type)
    {
        for(int i = 0; i < 9; i++)
        {
            joyControl[i] = i == type;
        }
    }

    public void JoyDown()
    {
        isControl = true;
    }

    public void JoyUp()
    {
        isControl = false;
    }

    void UpdateMove()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        if (joyControl[0]) { hAxis = -1; vAxis = 1; }
        if (joyControl[1]) { hAxis = 0; vAxis = 1; }
        if (joyControl[2]) { hAxis = 1; vAxis = 1; }
        if (joyControl[3]) { hAxis = -1; vAxis = 0; }
        if (joyControl[4]) { hAxis = 0; vAxis = 0; }
        if (joyControl[5]) { hAxis = 1; vAxis = 0; }
        if (joyControl[6]) { hAxis = -1; vAxis = -1; }
        if (joyControl[7]) { hAxis = 0; vAxis = -1; }
        if (joyControl[8]) { hAxis = 1; vAxis = -1; }

        if (isTouchRight && hAxis == 1 || isTouchLeft && hAxis == -1 || !isControl)
            hAxis = 0;
        if (isTouchTop && vAxis == 1 || isTouchBottom && vAxis == -1 || !isControl)
            vAxis = 0;

        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(hAxis, vAxis, 0) * speed * Time.deltaTime;
        transform.position = curPos + nextPos;

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
            anim.SetInteger("Input", (int)hAxis);
    }

    public void ButtonADown()
    {
        isButtonA = true;
    }

    public void ButtonAUp()
    {
        isButtonA = false;
    }

    public void ButtonBDown()
    {
        isButtonB = true;
    }

    void UpdateFire()
    {
        //if (!Input.GetButton("Fire1"))
        //    return;

        if (!isButtonA)
            return;

        if (curShotDelay < maxShotDelay)
            return;

        switch (power)
        {
            case 1:
                GameObject bullet = objectManager.MakeObj("BulletPlayerA");
                bullet.transform.position = transform.position;

                Rigidbody2D rigidBullet = bullet.GetComponent<Rigidbody2D>();
                rigidBullet.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 2:
                GameObject bulletR = objectManager.MakeObj("BulletPlayerA");
                bulletR.transform.position = transform.position + Vector3.right * 0.1f;
                GameObject bulletL = objectManager.MakeObj("BulletPlayerA");
                bulletL.transform.position = transform.position + Vector3.left * 0.1f;

                Rigidbody2D rigidBulletR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidBulletL = bulletL.GetComponent<Rigidbody2D>();
                rigidBulletR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidBulletL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            default:
                GameObject bulletRR = objectManager.MakeObj("BulletPlayerA");
                bulletRR.transform.position = transform.position + Vector3.right * 0.25f;
                GameObject bulletCC = objectManager.MakeObj("BulletPlayerB");
                bulletCC.transform.position = transform.position;
                GameObject bulletLL = objectManager.MakeObj("BulletPlayerA");
                bulletLL.transform.position = transform.position + Vector3.left * 0.25f;

                Rigidbody2D rigidBulletRR = bulletRR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidBulletCC = bulletCC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidBulletLL = bulletLL.GetComponent<Rigidbody2D>();
                rigidBulletRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidBulletCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidBulletLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;

        }
        curShotDelay = 0;

    }

    void UpdateReload()
    {
        curShotDelay += Time.deltaTime;

    }

    void UpdateBoom()
    {
        //if (!Input.GetButtonDown("Fire2"))
        //    return;
        if (!isButtonB)
            return;
        isButtonB = false;
        if (isBoomTime)
            return;

        if (boom == 0)
            return;

        boom--;
        isBoomTime = true;
        gameManager.UpdateBoomIcon(boom);

        boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 3f);
        GameObject[] enemiesL = objectManager.GetPool("EnemyL");
        GameObject[] enemiesM = objectManager.GetPool("EnemyM");
        GameObject[] enemiesS = objectManager.GetPool("EnemyS");
        for (int index = 0; index < enemiesL.Length; index++)
        {
            if (enemiesL[index].activeSelf)
            {
                Enemy enemyLogic = enemiesL[index].GetComponent<Enemy>();
                enemyLogic.OnHit(100);
            }
        }
        for (int index = 0; index < enemiesM.Length; index++)
        {
            if (enemiesM[index].activeSelf)
            {
                Enemy enemyLogic = enemiesM[index].GetComponent<Enemy>();
                enemyLogic.OnHit(100);
            }
        }
        for (int index = 0; index < enemiesS.Length; index++)
        {
            if (enemiesS[index].activeSelf)
            {
                Enemy enemyLogic = enemiesS[index].GetComponent<Enemy>();
                enemyLogic.OnHit(100);
            }
        }

        GameObject[] bulletsA = objectManager.GetPool("BulletEnemyA");
        GameObject[] bulletsB = objectManager.GetPool("BulletEnemyA");
        for (int index = 0; index < bulletsA.Length; index++)
        {
            if (bulletsA[index].activeSelf)
                bulletsA[index].SetActive(false);
        }
        for (int index = 0; index < bulletsA.Length; index++)
        {
            if (bulletsB[index].activeSelf)
                bulletsB[index].SetActive(false);
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
            }
        }
        else if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            if (isRespawnTime)
                return;

            if (isHit)
                return;

            isHit = true;
            life--;
            gameManager.UpdateLifeIcon(life);
            gameManager.CallExplosion("P", transform.position);

            if(life <= 0)
            {
                gameManager.GameOver();
            }
            else
            {
                gameManager.RespawnPlayer();
            }
            
            gameObject.SetActive(false);
            if(collision.gameObject.tag == "EnemyBullet")
                collision.gameObject.SetActive(false);
            
        }
        else if(collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch(item.type)
            {
                case "Coin":
                    score += 1000;
                    break;
                case "Power":
                    if (maxPower == power)
                        score += 500;
                    else
                    {
                        power++;
                        AddFollower();
                    }
                        
                    break;
                case "Boom":
                    if (maxBoom == boom)
                        score += 500;
                    else
                    {
                        boom++;
                        gameManager.UpdateBoomIcon(boom);
                    }
                        
                    break;
            }
            collision.gameObject.SetActive(false);
        }
    } 

    void OffBoomEffect()
    {
        boomEffect.SetActive(false);
        isBoomTime = false;
    }

    void AddFollower()
    {
        if (power == 4)
            followers[0].SetActive(true);
        else if (power == 5)
            followers[1].SetActive(true);
        else if (power == 6)
            followers[2].SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
            }
        }
    }
}
