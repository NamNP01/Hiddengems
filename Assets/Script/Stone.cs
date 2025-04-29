using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public GemControler gemControler; // Tham chiếu đến GemControler
    public float spawnChance = 0.3f;

    public PlayerProgressData gameData;
    private void OnMouseDown()
    {
        if (gemControler.pickaxeCount <= 0)
        {
            return;
        }

        Vector3 stonePosition = transform.position;

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

        // Snap vị trí theo grid
        stonePosition = gemControler.SnapToGrid(stonePosition);

        Destroy(gameObject, 0.1f);

        gemControler.RemoveStonePosition(stonePosition);
        gemControler.UpdatePickaxe();
    }

}
