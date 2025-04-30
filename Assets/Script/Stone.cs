using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class Stone : MonoBehaviour
{
    private float spawnChance = 0.3f;

    [Header("Dynamite")]
    public bool isDynamite = false;

    [Header("Data")]
    public PlayerProgressData gameData;
    public GemControler gemControler;

    [Header("Click Cooldown")]
    private float clickCooldown = 2.5f;
    private float lastClickTime;
    private void OnMouseDown()
    {
        if (Time.time - lastClickTime < clickCooldown) return;

        lastClickTime = Time.time;
        if (gameData.pickaxeCount <= 0) return;

        Vector3 stonePosition = gemControler.SnapToGrid(transform.position);

        if (isDynamite)
        {
            Explode(stonePosition);
        }
        else
        {
            HandleNormalStone(stonePosition);
        }

        gemControler.UpdatePickaxe();
    }
    private void HandleNormalStone(Vector3 stonePosition)
    {
        bool wasStoneInList = gemControler.allStonePositions.Contains(stonePosition);
        if (wasStoneInList)
        {
            gemControler.allStonePositions.Remove(stonePosition);
        }

        gemControler.PlaceLargestGemAndCheckRemaining();

        if (wasStoneInList)
        {
            gemControler.allStonePositions.Add(stonePosition);
        }

        if (!gemControler.isPlacementPossible)
        {
            gemControler.PlaceLargestGemAndCheckRemaining();
        }
        else
        {
            if (spawnChance > 0f && Random.value <= spawnChance)
            {
                gemControler.SpawnRandomGem(stonePosition);
            }
        }

        Destroy(gameObject, 0.1f);
        gemControler.RemoveStonePosition(stonePosition);
    }

    private void Explode(Vector3 centerPosition)
    {
        List<Vector3> affectedPositions = new List<Vector3>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                Vector3 offset = new Vector3(dx * gemControler.cellSize, dy * gemControler.cellSize, 0);
                Vector3 targetPos = gemControler.SnapToGrid(centerPosition + offset);
                if (gemControler.allStonePositions.Contains(targetPos))
                {
                    affectedPositions.Add(targetPos);
                }
            }
        }

        foreach (var stonePosition in affectedPositions)
        {
            gemControler.allStonePositions.Remove(stonePosition);

            gemControler.PlaceLargestGemAndCheckRemaining();

            gemControler.allStonePositions.Add(stonePosition);

            if (!gemControler.isPlacementPossible)
            {
                gemControler.PlaceLargestGemAndCheckRemaining();
            }
            else
            {
                if (spawnChance > 0f && Random.value <= spawnChance)
                {
                    gemControler.SpawnRandomGem(stonePosition);
                }
            }
            GameObject[] allStones = GameObject.FindGameObjectsWithTag("Stone");
            foreach (var stone in allStones)
            {
                if (gemControler.SnapToGrid(stone.transform.position) == stonePosition)
                {
                    Destroy(stone, 0.1f);
                    break;
                }
            }

            gemControler.RemoveStonePosition(stonePosition);
        }

    }
}
