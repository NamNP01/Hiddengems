using UnityEngine;

public class Stone : MonoBehaviour
{
    public GemControler gemControler; // Tham chiếu đến GemControler

    private void OnMouseDown()
    {
        if (gemControler.pickaxeCount <= 0)
        {
            Debug.Log("Không đủ cuốc để phá đá.");
            return;
        }

        Vector3 stonePosition = transform.position;

        // Tạm thời xóa vị trí viên đá khỏi danh sách
        bool wasStoneInList = gemControler.allStonePositions.Contains(stonePosition);
        if (wasStoneInList)
        {
            gemControler.allStonePositions.Remove(stonePosition);
        }

        // Gọi hàm kiểm tra khả năng đặt gem
        gemControler.PlaceLargestGemAndCheckRemaining();

        // Đưa lại vị trí stone vào danh sách nếu cần
        if (wasStoneInList)
        {
            gemControler.allStonePositions.Add(stonePosition);
        }

        // ⚡️ CHỈ nếu KHÔNG thể đặt gem nữa thì mới gọi PlaceGemAfterCheckRemaining
        if (!gemControler.isPlacementPossible)
        {
            //gemControler.PlaceGemAfterCheckRemaining();
        }

        // Snap vị trí theo grid
        stonePosition = gemControler.SnapToGrid(stonePosition);

        // Tính xác suất spawn gem
        float spawnChance = 0.3f; // Xác suất 30%

        if (gemControler.isPlacementPossible && Random.Range(0f, 1f) <= spawnChance)
        {
            gemControler.SpawnRandomGem(stonePosition);
        }

        Destroy(gameObject, 0.1f);

        gemControler.RemoveStonePosition(stonePosition);
        gemControler.UpdatePickaxe();
    }

}
