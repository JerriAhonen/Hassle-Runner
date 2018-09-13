using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    private const int COIN_SCORE_AMOUNT = 5;
    
    public static GameManager Instance { set; get; }

    public bool IsDead { set; get; }
    private bool isGameStarted = false;
    private PlayerMovement playerMovement;

    // UI and the UI fields
    public Text scoreText, coinText, modifierText;
    private float score, coinScore, modifierScore;
    private int lastScore;

    // Death menu
    public GameObject deathMenu;
    public Animator deathMenuAnim;
    public Text deathMenuScoreText, deathMenuCoinText;


    private void Awake()
    {
        Instance = this;
        modifierScore = 1.0f;
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        scoreText.text = score.ToString("0");
        coinText.text = coinScore.ToString("0");
        modifierText.text = "x" + modifierScore.ToString("0.0");

        //deathMenu.SetActive(false);
    }

    private void Update()
    {
        if (MobileInput.Instance.Tap && !isGameStarted)
        {
            isGameStarted = true;
            playerMovement.StartRunning();
            FindObjectOfType<AsteroidSpawner>().IsScrolling = true;
        }

        if (isGameStarted && !IsDead)
        {
            // Bump the score up
            score += (Time.deltaTime * modifierScore);

            if (lastScore != (int)score)
            {
                scoreText.text = score.ToString("0");
                lastScore = (int)score;
            }   
        }
        /*
        if (IsDead && !deathMenu.activeSelf)
        {
            deathMenu.SetActive(true);
        }
        */
    }
    

    public void UpdateModifier(float modifierAmount)
    {
        modifierScore = 1.0f + modifierAmount;
        modifierText.text = "x" + modifierScore.ToString("0.0");
    }

    public void GetCoin()
    {
        coinScore++;
        coinText.text = coinScore.ToString("0");
        score += COIN_SCORE_AMOUNT;
    }

    public void OnDeath()
    {
        IsDead = true;
        deathMenuScoreText.text = "Score: " + score.ToString("0");
        deathMenuCoinText.text = "Coins: " + coinScore.ToString("0");
        deathMenuAnim.SetTrigger("Dead");
    }

    public void OnPlayButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}
