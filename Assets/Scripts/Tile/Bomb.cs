using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : BaseTile
{
    [SerializeField]
    int countDown;
    int lifeCount;
    int initialRoundCount;
    int lastRoundCount;

    public TextMesh countText;
    void Start()
    {
        StartCoroutine(GetInitialCount());
        countDown = lifeCount = 6;
        UpdateText();
    }
    
    // Her seferinde round kontrolü yapılarak patlama kontrol ediliyor.
    void FixedUpdate()
    {
        if (GameManager.instance.CurrentState == GameManager.GameState.Ready)
        {
            lastRoundCount = GameManager.instance.RoundCount;
            countDown = lifeCount - (lastRoundCount - initialRoundCount);
            UpdateText();
            if (countDown < 1)
            {
                GameManager.instance.CurrentState = GameManager.GameState.GameOver;
                GameManager.instance.GameOverText = "Bomb exploded";
                Destroy(this);
            }
        }

    }

    // Round bitene kadar bekleyip daha sonra gamemanager'dan round sayısı çekilir.
    IEnumerator GetInitialCount()
    {
        yield return new WaitUntil(() => GameManager.instance.CurrentState == GameManager.GameState.Ready);

        initialRoundCount = GameManager.instance.RoundCount;
    }


    void UpdateText()
    {
        countText.text = countDown.ToString();
    }




}
