using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int indexPosition = 2;

    private Vector3 depotPos = new Vector3(-6.41f, -0.82f, 0f);
    private Vector3 leftPos = new Vector3(-2.19f, -0.82f, 0f);
    private Vector3 middlePos = new Vector3(1.46f, -0.82f, 0f);
    private Vector3 rightPos = new Vector3(5.55f, -0.82f, 0f);

    [SerializeField] private Datas myData;
    public bool haveGift = false;

    private Animator thisAnimator;
    [SerializeField] private Animator chestAnimator;
    private GameObject level;
    
    public GameObject playerGiftSprite;
    private float speed = 21f;

    [SerializeField] private GameObject pauseGo;
    private int pauseIndex = 0;
    [SerializeField] private AudioSource catchSound;


    void Start()
    {
        //Désactive la Pause, assigne les variables, désactivele gift d'affiche pour le joueur et place le joueur si le mode de déplacement G&W est sélectionner

        pauseGo.SetActive(false);
        level = GameObject.FindGameObjectWithTag("LevelManager");
        thisAnimator = GetComponent<Animator>();

        if(level.GetComponent<LevelManager>().movementIndex == 1)
        {
            indexPosition = 2;
            transform.position = middlePos;
        }

        playerGiftSprite.SetActive(false);
    }

    void Update()
    {
        IsInput();      //Verifie les inputs

        if(level.GetComponent<LevelManager>().movementIndex == 1)
        {
            VerifyPosGift(); //place le cadeau dans les mains du sprite selon ou il est dans le niveau

            if (indexPosition == 0 && haveGift == true)     //si le joueur ce trouve sur la case coffre, il ajoute du score etc...
            {
                myData.score++;
                haveGift = false;
                chestAnimator.SetTrigger("Open");
                chestAnimator.GetComponent<AudioSource>().Play();
            }
        }
        
        if (haveGift == true)
        {
            if(level.GetComponent<LevelManager>().movementIndex == 1)
            {       //Attend la fin de l'animation avant de faire spawn le cadeau sur le joueur
                if (thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && thisAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                {
                    playerGiftSprite.SetActive(true);
                }
            }
            else if(level.GetComponent<LevelManager>().movementIndex == 0)
            {
                playerGiftSprite.SetActive(true);
            }   
        }
        else
        {
            playerGiftSprite.SetActive(false);
        }    
    }

    private void IsInput()              //Gere les Inputs
    {
        if (level.GetComponent<LevelManager>().movementIndex == 1)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = true;

                if (indexPosition > 0)
                {
                    indexPosition--;
                    VerifyPos();
                }
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = false;

                if (indexPosition < 3)
                {
                    indexPosition++;
                    VerifyPos();
                }
            }
        }

        else if (level.GetComponent<LevelManager>().movementIndex == 0)
        {

            if (Input.GetKey(KeyCode.Q))
            {
                thisAnimator.SetFloat("speed", 1);
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
                
                if (gameObject.transform.position.x > -8.16f)
                {
                    gameObject.transform.Translate(Vector2.left * speed * Time.deltaTime);
                }
            }
            else if (Input.GetKey(KeyCode.D))
            {
                thisAnimator.SetFloat("speed", 1);
                gameObject.GetComponent<SpriteRenderer>().flipX = false;

                if (gameObject.transform.position.x < 7.81f)
                {
                    gameObject.transform.Translate(Vector2.right * speed * Time.deltaTime);
                }
            }
            else  
                thisAnimator.SetFloat("speed", 0);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseIndex == 0)
            {
                pauseIndex++;
                pauseGo.SetActive(true);
                level.GetComponent<LevelManager>().Pause();
            }
            else
            {
                pauseGo.SetActive(false);
                pauseIndex = 0;
                level.GetComponent<LevelManager>().Play();
            }
        }
    }

    private void VerifyPos()    //Place le joueur au bon endroit selon l'index de position
    {
        if (indexPosition == 0)
        {
            transform.position = depotPos;
        }

        if (indexPosition == 1)
        {
            transform.position = leftPos;
        }

        if (indexPosition == 2)
        {
            transform.position = middlePos;
        }

        if (indexPosition == 3)
        {
            transform.position = rightPos;
        }
    }

    private void VerifyPosGift()        //place le cadeau dans les mains du sprite selon ou il est dans le niveau
    {
        if (indexPosition == 1)
        {

            if (this.gameObject.GetComponent<SpriteRenderer>().flipX == true && playerGiftSprite.active == true)
            {
                playerGiftSprite.transform.position = new Vector3(-1.38f, -1.74f, 0f);
            }
            else if (this.gameObject.GetComponent<SpriteRenderer>().flipX == false && playerGiftSprite.active == true)
            {
                playerGiftSprite.transform.position = new Vector3(-3.03f, -1.74f, 0f);
            }
        }

        if (indexPosition == 2)
        {

            if (this.gameObject.GetComponent<SpriteRenderer>().flipX == true && playerGiftSprite.active == true)
            {
                playerGiftSprite.transform.position = new Vector3(2.26f, -1.75f, 0f);
            }
            else if (this.gameObject.GetComponent<SpriteRenderer>().flipX == false && playerGiftSprite.active == true)
            {
                playerGiftSprite.transform.position = new Vector3(0.61f, -1.77f, 0f);
            }
        }

        if (indexPosition == 3)
        {

            if (this.gameObject.GetComponent<SpriteRenderer>().flipX == false && playerGiftSprite.active == true)
            {
                playerGiftSprite.transform.position = new Vector3(4.69f, -1.73f, 0f);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Target") && haveGift == false)       //Si le joueur attrape le cadeau il l'attrape et ne peut pas en avoir un autre
        {
            thisAnimator.SetTrigger("catch");
            catchSound.Play();
            Destroy(col.gameObject);
            level.GetComponent<LevelManager>().maxGift--;
            haveGift = true;
        }

        if(level.GetComponent<LevelManager>().movementIndex == 0)
        {
            if (col.gameObject.CompareTag("Chest") &&  haveGift == true)
            {
                myData.score++;
                haveGift = false;
                chestAnimator.SetTrigger("Open");
                chestAnimator.GetComponent<AudioSource>().Play();
            }
        }
    }
}
