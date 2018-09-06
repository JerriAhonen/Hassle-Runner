using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    private const int COIN_SCORE_AMOUNT = 5;


    public static GameManager Instance { set; get; }

    private bool isGameStarted = false;
    private PlayerMovement playerMovement;

    // UI and the UI fields
    public Text scoreText, coinText, modifierText;
    private float score, coinscore, modifierScore;
    private int lastScore;

    private void Awake()
    {
        Instance = this;
        modifierScore = 1.0f;
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        scoreText.text = score.ToString("0");
        coinText.text = coinscore.ToString("0");
        modifierText.text = "x" + modifierScore.ToString("0.0");
    }

    private void Update()
    {
        if (MobileInput.Instance.Tap && !isGameStarted)
        {
            isGameStarted = true;
            playerMovement.StartRunning();
        }

        if (isGameStarted)
        {
            // Bump the score up
            score += (Time.deltaTime * modifierScore);

            if (lastScore != (int)score)
            {
                scoreText.text = score.ToString("0");
                lastScore = (int)score;
            }
            
        }
    }
    

    public void UpdateModifier(float modifierAmount)
    {
        modifierScore = 1.0f + modifierAmount;
        modifierText.text = "x" + modifierScore.ToString("0.0");
    }

    public void GetCoin()
    {
        coinscore++;
        coinText.text = coinscore.ToString("0");
        score += COIN_SCORE_AMOUNT;
    }
}
