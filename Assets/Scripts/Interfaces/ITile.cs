using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public interface ITile
{
    void GetRandomColor();
    void SetArrayPosition(int x, int y);

    void OnMouseDown();

    void OnMouseUp();

    void MoveTile(Vector2 targetPos);

    Tuple<SelectedHexagon, SelectedHexagon> GetClosestItems(int area);

    void FindAndSelectHexagonsInArea(float angle, int? area);

    void Select(GameObject selectionOne, GameObject selectionTwo);
    
    void UnSelect();

    bool isAvailable(SelectedHexagon h1, SelectedHexagon h2);

    void Relocate(int arrayPosX,int arrayPosY, Vector2 position);
    void CheckNearbyHexagons(bool explode);
    
    void CheckForMatch(GameObject firstHexagon, GameObject secondHexagon, bool explode);

}
