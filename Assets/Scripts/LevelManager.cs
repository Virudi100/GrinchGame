using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject giftPrefab;
    [SerializeField] private Datas myData;
 
    
    public int maxGift;
    private int whereSpawn;
    private GameObject newGift;


    private Vector3 giftLeftPos = new Vector3(-2.65f, 6.59f, 0f);
    private Vector3 giftMiddlePos = new Vector3(1.02f, 6.63f, 0f);
    private Vector3 giftRightPos = new Vector3(5.11f, 6.61f, 0);
    
    
    [SerializeField] private Text scoreText;
    [SerializeField] private Text highScore;
    private Rigidbody2D targetRb;
    private int difficultyIndex = 1;                        // 1 = Facile , 2 = Normal , 3 = Difficile

    [SerializeField] private GameObject Menu;
    private float gravityValue;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private Gifts giftScript;

    [SerializeField] private Camera MainCamera;
    [SerializeField] private GameObject UI;
    private bool isTravelling = false;

    private int start = 0;
    public int movementIndex = 1;         //Mouvement Standard = 0 , Mouvement Game & Watch = 1
    public Text movementButtonText;

    [SerializeField] private GameObject playerGW;
    [SerializeField] private GameObject playerStandard;

    [SerializeField] private Text timerText;
    private float speedTraveling = 4f;

    [SerializeField] private AudioSource easyMusic;
    [SerializeField] private AudioSource mediumMusic;
    [SerializeField] private AudioSource hardMusic;



    private float timer = 60f;

    private void Awake()
    {
        //Active le joueur

        playerStandard.SetActive(false);
    }

    void Start()
    {
        //Lance le travelling en début de jeu et désactive tout le jeu pendant 

        isTravelling = true;
        UI.SetActive(false);
        MainCamera.orthographicSize = 20f;
    }

    private void FixedUpdate()
    {
        Traveling();                                    //Fonction du traveling
        
        scoreText.text = ("Score = " + myData.score);   //Affiche le score

        if (isTravelling == false)                      //Si le traveling est terminé
        {
            if (Menu.active == false)
            {
                if (maxGift == 0)                       //Si le Menu est désactivé et que le nbr de cadeau est = 0 alors on fait spawn le cadeau
                {
                    RandomSpawn();                  
                }

                Timer();                                //Lance le timer de jeu

                timerText.text = (Mathf.Floor(timer) + " secondes left");
            }
        }

        if (maxGift < 0)                    //Prevention d'erreur où la valeur est négative
        {
            maxGift = 0;
        }

        if(giftScript.liveCount <= 0)       //GameOver quand les vies sont épuisé
        {
            GameOver();
        }
     
    }

    private void GameOver()                 //GameOver; Désactive les musiques, active l'affichage GameOver et mes le jeu en pause
    {
        gameOver.SetActive(true);
        easyMusic.Stop();
        mediumMusic.Stop();
        hardMusic.Stop();

        gameObject.GetComponent<AudioSource>().Play();
        Pause();
    }

    private void RandomSpawn()              //Instantie un cadeau a une valeur aleatoire entre de 1 a 3 et le place a la bonne position selon le chiffre tiré
    {
        maxGift++;
        gravityValue += 0.05f;
        whereSpawn = Random.Range(1, 4);

        if (whereSpawn == 1)
        {
            newGift = Instantiate(giftPrefab, giftLeftPos, Quaternion.identity);
        }
        else if (whereSpawn == 2)
        {
            newGift = Instantiate(giftPrefab, giftMiddlePos, Quaternion.identity);

        }
        else
            newGift = Instantiate(giftPrefab, giftRightPos, Quaternion.identity);

        targetRb = newGift.GetComponent<Rigidbody2D>();

        if (difficultyIndex == 1)        //Ajoute de la gravité pour le faire tomber plus vite selon la difficulté selectionner au debut, de la gravité est ajouté au fur et a mesure que le jeu avance
        {
            //Debug.Log("Spawn easy target");
            targetRb.gravityScale = 0.05f;
            targetRb.gravityScale += gravityValue;
        }
        else if (difficultyIndex == 2)
        {
            //Debug.Log("Spawn normal target");
            targetRb.gravityScale = 0.5f;
            targetRb.gravityScale += gravityValue;
        }
        else
        {
            //Debug.Log("Spawn hard target");
            targetRb.gravityScale = 1.2f;
            targetRb.gravityScale += gravityValue;
        }

    }

    private void Traveling()        //Lance le traveling de camera au debut
    {
        if (MainCamera.orthographicSize > 5.55f)
        {
            MainCamera.orthographicSize -=  Time.deltaTime * speedTraveling ;
        }
        else if (start == 0)
        {
            Pause();
            Menu.SetActive(true);
            gameObject.GetComponent<AudioSource>().Play();
            gameOver.SetActive(false);
            UI.SetActive(true);
            isTravelling = false;
            start = 1;
        }
    }

    private void Timer()        //Lance le timer de jeu et lance le GameOver quand il arrive à 0
    {
        timer -= Time.deltaTime;

        if(timer < 0)
        {
            GameOver();
        }
    }

    private void StartGame()    //Lance le jeu
    {
        Menu.SetActive(false);
        Play();
        RandomSpawn();

    }

    public void StartEasy()     //Selectionne le jeu en facile, lance le jeu et active la musique correspondante 
    {
        gameObject.GetComponent<AudioSource>().Stop();

        easyMusic.Play();
        mediumMusic.Stop();
        hardMusic.Stop();

        StartGame();
        difficultyIndex = 1;
        targetRb.gravityScale = 0.05f;

    }

    public void StartNormal()   //Selectionne le jeu en normal, lance le jeu et active la musique correspondante 
    {
        gameObject.GetComponent<AudioSource>().Stop();

        easyMusic.Stop();
        mediumMusic.Play();
        hardMusic.Stop();

        StartGame();
        difficultyIndex = 2;
        targetRb.gravityScale = 0.5f;
    }

    public void StartDifficile()    //Selectionne le jeu en difficile, lance le jeu et active la musique correspondante 
    {
        gameObject.GetComponent<AudioSource>().Stop();

        easyMusic.Stop();
        mediumMusic.Stop();
        hardMusic.Play();

        StartGame();
        difficultyIndex = 3;
        targetRb.gravityScale = 1.2f;
    }

    public void Pause()     //Mes le jeu en pause 
    {
        Time.timeScale = 0;
    }

    public void Play()      //Enleve la pause
    {
        Time.timeScale = 1;
    }

    public void Retry()     //Réinitialise tout les valeurs pour recommencer une nouvelle partie et detruit le cadeau existant
    {
        Menu.SetActive(true);

        easyMusic.Stop();
        mediumMusic.Stop();
        hardMusic.Stop();

        gameObject.GetComponent<AudioSource>().Play();

        if(myData.highScore < myData.score)
        {
            myData.highScore = myData.score;
        }

        highScore.text = ("Highscore: " + myData.highScore);
        myData.score = 0;
        gameOver.SetActive(false);
        giftScript.Restart();
        Destroy(newGift);
        gravityValue = 0;
        maxGift = 0;
        timer = 60;

        if(movementIndex == 1)
        {
            playerGW.GetComponent<Player>().haveGift = false;
            playerGW.GetComponent<Player>().playerGiftSprite.SetActive(false);
        }
        else if (movementIndex == 0)
        {
            playerStandard.GetComponent<Player>().haveGift = false;
            playerStandard.GetComponent<Player>().playerGiftSprite.SetActive(false);
        }
        
    }

    public void QuitGame()      //Lance le jeu
    {
        Application.Quit();
    }

    public void MovementMode()  //Active le bon joueur selon le mode de deplacement sélectionner
    {
        if(movementIndex == 0)
        {
            movementButtonText.text = ("Game & Watch Movement Selected");
            playerGW.SetActive(true);
            playerStandard.SetActive(false);
            movementIndex = 1;
        }
        else if(movementIndex == 1)
        {
            movementButtonText.text = ("Standard Movement Selected");
            playerGW.SetActive(false);
            playerStandard.SetActive(true);
            movementIndex = 0;
        }
    }
}
