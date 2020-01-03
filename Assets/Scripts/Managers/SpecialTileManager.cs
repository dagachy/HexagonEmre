using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTileManager : MonoBehaviour
{

    public static SpecialTileManager instance;
    int previousBombCount;

    [SerializeField]
    int spawnBombInScore = 1000;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        previousBombCount = 0;
    }

    // Daha sonra eklenebilecek özel "Tile" için spawn kontrolü burada yapılacak.

    public void CheckForSpawns()
    {
        BombSpawn();
    }

    // Eklenecek yeni Tile'lar için spawn kuralları bu şekilde yapılabilinir.
    void BombSpawn()
    {
        if (ScoreManager.instance.GetScore() / spawnBombInScore > previousBombCount)
        {
            GridHandler.instance.isBombReady = true;
            previousBombCount++;
        }
    }
}
