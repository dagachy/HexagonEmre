using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager instance;
    [SerializeField]
    TextMeshProUGUI scoreText;
    [SerializeField]
    TextMeshProUGUI highscoreText;
    [SerializeField]
    TextMeshProUGUI moveText;

    int score;
    int highscore;
    int move;

    void Awake () {
        instance = this;
    }

    void Start () {
        score = 0;
        highscore = PlayerPrefs.GetInt ("highScore");
        move = 0;
        UpdateScore ();
    }

    public void AddScore (int score) {
        this.score += score;
        if (this.score > PlayerPrefs.GetInt ("highScore")) {
            PlayerPrefs.SetInt ("highScore", this.score);
        }
        UpdateScore ();
    }

    public void AddMove () {
        this.move++;
        UpdateScore ();
    }

    public int GetScore () {
        return score;
    }

    void UpdateScore () {
        scoreText.text = score.ToString ();
        highscoreText.text = "Highscore: " + PlayerPrefs.GetInt ("highScore").ToString ();
        moveText.text = move.ToString ();
    }

}