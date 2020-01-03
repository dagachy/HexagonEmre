using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardChecker : MonoBehaviour
{
    public static BoardChecker instance;

    bool matchFound = false;

    int notMatchHexCount = 0;

    public bool MatchFound { get => matchFound; set => matchFound = value; }

    void Awake()
    {
        instance = this;
    }

    void GetHexCount()
    {
        notMatchHexCount = HexagonManager.instance.GetHexCount();
    }

    void SwapColors(int item1_X, int item1_Y, int item2_X, int item2_Y)
    {
        Color temp = HexagonManager.instance.GetFromArray(item1_X, item1_Y).GetComponent<BaseTile>().HexColor;
        HexagonManager.instance.GetFromArray(item1_X, item1_Y).GetComponent<BaseTile>().HexColor = HexagonManager.instance.GetFromArray(item2_X, item2_Y).GetComponent<BaseTile>().HexColor;
        HexagonManager.instance.GetFromArray(item2_X, item2_Y).GetComponent<BaseTile>().HexColor = temp;
    }

    /*
        Her round sona erdiğinde olası hareketler kontrol edilir. İlk bulduğu move'dan sonra bu metot'tan return yapılarak çıkılır.
        Daha sonra eklenebilecek olan move önerme için de bu fonksiyon kolaylıkla kullanılabilinir.
    */
    public void CheckPossibleMatches()
    {
        if (GameManager.instance.CurrentState == GameManager.GameState.Ready)
        {
            for (int x = 0; x < GridHandler.instance.gridWidth; x++)
            {
                for (int y = 0; y < GridHandler.instance.gridHeight; y++)
                {
                    for (int area = 1; area <= 6; area++)
                    {
                        Tuple<SelectedHexagon, SelectedHexagon> result = HexagonManager.instance.GetFromArray(x, y).GetComponent<BaseTile>().GetClosestItems(area);

                        if (result != null)
                        {
                            SwapColors(x, y, result.Item1.PosX, result.Item1.PosY);

                            HexagonManager.instance.GetFromArray(x, y).GetComponent<BaseTile>().CheckNearbyHexagons(false);

                            SwapColors(x, y, result.Item1.PosX, result.Item1.PosY);

                            if (matchFound)
                            {
                                matchFound = false;
                                //Debug.Log("Match Found " + x + "," + y);
                                return;
                            }

                        }

                    }
                }
            }
            GameManager.instance.GameOverText = "There is no possible matches";
            GameManager.instance.CurrentState = GameManager.GameState.GameOver;
        }

    }

    /*  
    Oyun başladığında çağırılan metotdur.
    Oyun başladığında haksız puan kazanmayı önlemek için, oyunda bulunan bütün 
    matchlerdeki hexagonlara tekrardan yeni color atar.
    */
    public void CheckInitialMatches()
    {
        GetHexCount();

        while (notMatchHexCount > 0)
        {
            GetHexCount();
            for (int x = 0; x < GridHandler.instance.gridWidth; x++)
            {
                for (int y = 0; y < GridHandler.instance.gridHeight; y++)
                {
                    HexagonManager.instance.GetFromArray(x, y).GetComponent<BaseTile>().CheckNearbyHexagons(false);


                    if (matchFound)
                    {
                        HexagonManager.instance.GetFromArray(x, y).GetComponent<BaseTile>().GetRandomColor();
                        matchFound = false;
                    }
                    else
                    {
                        notMatchHexCount--;
                    }
                }
            }
        }
        GameManager.instance.changeGameState(GameManager.GameState.Ready);


    }

}
