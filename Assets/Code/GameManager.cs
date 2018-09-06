using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager Instance { set; get; }

    private bool isGameStarted = false;
    private PlayerMovement playerMovement;

    // UI and the UI fields
    public Text scoreText, coinText, modifierText;
    private float score, coinscore, modifierScore;

    private void Awake()
    {
        Instance = this;
        UpdateScores(); // Start with updating all the scores.
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (MobileInput.Instance.Tap && !isGameStarted)
        {
            isGameStarted = true;
            playerMovement.StartRunning();
        }
    }

    public void UpdateScores()
    {
        scoreText.text = score.ToString();
        coinText.text = coinscore.ToString();
        modifierText.text = modifierScore.ToString();
    }
}
