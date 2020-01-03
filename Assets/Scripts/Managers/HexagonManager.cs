using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonManager : MonoBehaviour {
    public static HexagonManager instance;

    GameObject[, ] hexArray;

    List<ArrayPos> explodeList = new List<ArrayPos> ();

    int hexWidth;
    int hexHeight;

    // Hexagon'ların tutulduğu array ile ilgili metotlar burada yer alır.
    // Ayrıca explode olacak hexagonlar da burada listede tutulur.

    void Awake () {
        instance = this;
    }

    public void InstantiateArray (int width, int height) {
        hexArray = new GameObject[width, height];
        hexWidth = width;
        hexHeight = height;
    }
    public void AddToArray (GameObject hex, int x, int y) {
        hexArray[x, y] = hex;
    }

    public void RemoveFromArray (int x, int y) {
        hexArray[x, y] = null;
    }

    public GameObject GetFromArray (int x, int y) {
        return hexArray[x, y];
    }

    public int GetHexCount () {
        return hexWidth * hexHeight;
    }

    public void RemoveAndDestroy (int x, int y) {
        Destroy (hexArray[x, y]);
        hexArray[x, y] = null;
    }

    // Her patlayan hexagonlar null değerine dönüşür
    // Bu metot her sütunda dönerek null sayısını buluyor ve üstteki hexagonları
    // null sayısı kadar aşağıya indiriyor.
    public void RelocateArray () {
        int nullCount = 0;
        for (int x = 0; x < hexWidth; x++) {
            for (int y = 0; y < hexHeight; y++) {
                if (hexArray[x, y] == null) {
                    nullCount++;
                } else if (nullCount > 0) {
                    hexArray[x, y - nullCount] = hexArray[x, y];
                    hexArray[x, y - nullCount].GetComponent<BaseTile> ().SetArrayPosition (x, y - nullCount);
                    hexArray[x, y].name = x + "," + (y - nullCount);
                    hexArray[x, y] = null;
                }
            }
            nullCount = 0;
        }

    }

    public void AddToExplodeArray (int posX, int posY) {
        explodeList.Add (new ArrayPos (posX, posY));
        // Debug.Log("Explode List Count: " + explodeList.Count);
    }

    public void EmptyList () {
        explodeList.Clear ();

    }

    public bool CheckItems (int firstSelectionPosX, int firstSelectionPosY, int secondSelectionPosX, int secondSelectionPosY) {
        bool firstItem = Utilities.inBounds2D (firstSelectionPosX, firstSelectionPosY, hexWidth, hexHeight);
        bool secondItem = Utilities.inBounds2D (secondSelectionPosX, secondSelectionPosY, hexWidth, hexHeight);
        return (firstItem && secondItem);
    }

    // Bütün array'i dönerek bütün hexagonlar için etrafındakiler ile match mi 
    // değil mi diye kontrol eden fonksiyon
    public void CheckForMatches () {
        for (int x = 0; x < hexWidth; x++) {
            for (int y = 0; y < hexHeight; y++) {
                if (hexArray[x, y].GetComponent<BaseTile> ().isMatched == false)
                    hexArray[x, y].GetComponent<BaseTile> ().CheckNearbyHexagons (true);
            }
        }

        if (explodeList.Count > 0) {
            PlayerController.instance.isExploding = true;
            StartCoroutine (Explode ());
        } else {
            PlayerController.instance.isExploding = false;
        }
    }

    // Explode listesine eklenen hexagonları patlatıp ayarlamaları yapar.
    public IEnumerator Explode () {
        yield return new WaitForSeconds (0.25f);
        foreach (ArrayPos hex in explodeList) {
            RemoveAndDestroy (hex.posX, hex.posY);

        }
        // Debug.Log("Explode List Count: " + explodeList.Count);
        ScoreManager.instance.AddScore (explodeList.Count * 5);
        EmptyList ();
        RelocateArray ();
        StartCoroutine (ArrayPositionOrganizer ());
        SpecialTileManager.instance.CheckForSpawns ();
        
    }

    // Patlamadan sonra array'de x ve y'si gerçekteki x ve y'sinden farklı olan
    // hexagonlar için tekrar pozisyon set edilir.
    public IEnumerator ArrayPositionOrganizer () {
        for (int x = 0; x < hexWidth; x++) {
            for (int y = 0; y < hexHeight; y++) {
                if (hexArray[x, y] != null) {
                    hexArray[x, y].name = x + "," + y;
                    hexArray[x, y].GetComponent<BaseTile> ().MoveTile (GridHandler.instance.CalcWordPos (x, y));
                    yield return new WaitForSeconds (.015f);
                }
            }
        }
        GridHandler.instance.RefillBoard ();
        BoardChecker.instance.CheckPossibleMatches ();
        GameManager.instance.EndRoundIfRoundStarted ();
        ScoreManager.instance.AddMove ();
    }
}

struct ArrayPos {
    public int posX;
    public int posY;

    public ArrayPos (int x, int y) {
        posX = x;
        posY = y;
    }

}