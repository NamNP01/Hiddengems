using System.Collections.Generic;
using UnityEngine;

public class GemManager : MonoBehaviour
{
    public float cellSize = 1.0f; // Kích thước mỗi ô trên grid
    public int gridWidth = 10;   // Số ô theo chiều ngang
    public int gridHeight = 10;  // Số ô theo chiều dọc

    private Dictionary<Vector3, bool> stonePositions = new Dictionary<Vector3, bool>();

    [System.Serializable]
    public class GemConfig
    {
        public GameObject gemPrefab; // Prefab của gem
        public Vector2Int size;      // Kích thước gem trên grid
        public int count;            // Số lượng gem còn lại
    }

    public List<GemConfig> gemConfigs; // Danh sách các gem có thể tạo

    // Thêm vị trí stone vào grid
    public void AddStonePosition(Vector3 position)
    {
        position = SnapToGrid(position);
        if (!stonePositions.ContainsKey(position))
        {
            stonePositions[position] = true;
            Debug.Log("Stone added at: " + position);
        }
    }

    // Xóa vị trí stone khỏi grid
    public void RemoveStonePosition(Vector3 position)
    {
        position = SnapToGrid(position);
        if (stonePositions.ContainsKey(position))
        {
            stonePositions.Remove(position);
            Debug.Log("Stone removed from: " + position);
        }
    }

    // Kiểm tra nếu có stone tại vị trí
    public bool IsStoneAtPosition(Vector3 position)
    {
        position = SnapToGrid(position);
        return stonePositions.ContainsKey(position);
    }

    // Lấy các vị trí trong phạm vi xung quanh một điểm
    public List<Vector3> GetPositionsInRange(Vector3 center, int range)
    {
        List<Vector3> positions = new List<Vector3>();
        center = SnapToGrid(center);

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                // Loại bỏ các ô nằm ngoài phạm vi hình tròn
                if (Mathf.Abs(x) + Mathf.Abs(z) > range) continue;

                Vector3 position = center + new Vector3(x * cellSize, 0, z * cellSize);
                if (IsStoneAtPosition(position))
                {
                    positions.Add(position);
                }
            }
        }

        return positions;
    }

    // Chuyển vị trí sang tọa độ grid (snap theo kích thước gem)
    public Vector3 SnapToGrid(Vector3 position, Vector2Int size)
    {
        float x, z;

        // Kiểm tra nếu size.x là chẵn
        if (size.x % 2 == 0)
        {
            x = Mathf.Round(position.x / cellSize) * cellSize;
        }
        else
        {
            x = Mathf.Floor(position.x / cellSize) * cellSize;
        }

        // Kiểm tra nếu size.y là chẵn
        if (size.y % 2 == 0)
        {
            z = Mathf.Round(position.z / cellSize) * cellSize;
        }
        else
        {
            z = Mathf.Floor(position.z / cellSize) * cellSize;
        }

        return new Vector3(x, position.y, z);
    }

    // Chuyển vị trí sang tọa độ grid (snap mặc định nếu không có kích thước gem)
    public Vector3 SnapToGrid(Vector3 position)
    {
        return new Vector3(
            Mathf.Round(position.x / cellSize) * cellSize,
            position.y,
            Mathf.Round(position.z / cellSize) * cellSize
        );
    }

    // Tạo gem ngẫu nhiên tại vị trí stone
    public void SpawnRandomGem(Vector3 stonePosition)
    {
        // Đảm bảo danh sách gemConfigs không rỗng
        if (gemConfigs.Count == 0)
        {
            Debug.LogWarning("No gems available to spawn.");
            return;
        }

        // Chọn ngẫu nhiên một gem từ danh sách
        int randomIndex = Random.Range(0, gemConfigs.Count);
        GemConfig selectedGem = gemConfigs[randomIndex];

        // Snap vị trí theo kích thước gem
        stonePosition = SnapToGrid(stonePosition, selectedGem.size);

        // Tạo gem tại vị trí đã làm tròn
        Instantiate(selectedGem.gemPrefab, stonePosition, Quaternion.identity);
        Debug.Log($"Spawned gem: {selectedGem.gemPrefab.name} at {stonePosition}");

        // Giảm số lượng gem
        selectedGem.count--;

        // Nếu số lượng gem giảm về 0, xóa khỏi danh sách
        if (selectedGem.count <= 0)
        {
            gemConfigs.RemoveAt(randomIndex);
            Debug.Log($"Gem {selectedGem.gemPrefab.name} removed from the list.");
        }

        // Loại bỏ vị trí stone
        RemoveStonePosition(stonePosition);
    }
}
