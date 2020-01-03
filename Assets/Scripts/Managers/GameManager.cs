using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{


    public enum GameState
    {
        Start,
        Ready,
        Busy,
        GameOver
    }
    public static GameManager instance;
    [SerializeField]
    GameState currentState;
    public GameObject gameOverPanel;
    string gameOverText;
    int roundCount = 0;
    bool roundStarted;
    PlayerController playerController;

    public int RoundCount { get => roundCount; set => roundCount = value; }
    public GameState CurrentState { get => currentState; set => currentState = value; }
    public string GameOverText { get => gameOverText; set => gameOverText = value; }
    public bool RoundStarted { get => roundStarted; set => roundStarted = value; }

    void Awake()
    {
        CurrentState = GameState.Start;
        instance = this;
    }

    void Start()
    {
        
        playerController = PlayerController.instance;

    }

    void Update()
    {
        if (currentState != GameState.Start && currentState != GameState.GameOver)
            UpdateGameState();
        else if (currentState == GameState.GameOver)
            GameOver();
    }

    void UpdateGameState()
    {
        if (PlayerController.instance.isRotating || PlayerController.instance.isExploding)
            currentState = GameState.Busy;
        else
        {
            currentState = GameState.Ready;

        }
    }

    public void changeGameState(GameState state){
        currentState = state;
    }

    public void EndRoundIfRoundStarted()
    {
        if (roundStarted)
        {
            roundStarted = false;
            StartCoroutine(EndRound());

        }
    }

    // Bir sonraki game state ready olduğunda round count arttırılır.
    IEnumerator EndRound()
    {
        yield return new WaitUntil(() => currentState == GameState.Ready);

        roundCount++;

    }

    void GameOver()
    {
        if (gameOverPanel.activeSelf == false)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
