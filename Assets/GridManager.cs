using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    //public float cellSize = 1.0f; // Kích thước mỗi ô trên grid
    //public int gridWidth = 10;   // Số ô theo chiều ngang
    //public int gridHeight = 10;  // Số ô theo chiều dọc

    //private Dictionary<Vector3, bool> stonePositions = new Dictionary<Vector3, bool>();

    //// Thêm vị trí stone vào grid
    //public void AddStonePosition(Vector3 position)
    //{
    //    position = SnapToGrid(position);
    //    if (!stonePositions.ContainsKey(position))
    //    {
    //        stonePositions[position] = true;
    //        Debug.Log("Stone added at: " + position);
    //    }
    //}

    //// Xóa vị trí stone khỏi grid
    //public void RemoveStonePosition(Vector3 position)
    //{
    //    position = SnapToGrid(position);
    //    if (stonePositions.ContainsKey(position))
    //    {
    //        stonePositions.Remove(position);
    //        Debug.Log("Stone removed from: " + position);
    //    }
    //}

    //// Kiểm tra nếu có stone tại vị trí
    //public bool IsStoneAtPosition(Vector3 position)
    //{
    //    position = SnapToGrid(position);
    //    return stonePositions.ContainsKey(position);
    //}

    //// Lấy các vị trí trong phạm vi xung quanh một điểm
    //public List<Vector3> GetPositionsInRange(Vector3 center, int range)
    //{
    //    List<Vector3> positions = new List<Vector3>();
    //    center = SnapToGrid(center);

    //    for (int x = -range; x <= range; x++)
    //    {
    //        for (int z = -range; z <= range; z++)
    //        {
    //            // Loại bỏ các ô nằm ngoài phạm vi hình tròn
    //            if (Mathf.Abs(x) + Mathf.Abs(z) > range) continue;

    //            Vector3 position = center + new Vector3(x * cellSize, 0, z * cellSize);
    //            if (IsStoneAtPosition(position))
    //            {
    //                positions.Add(position);
    //            }
    //        }
    //    }

    //    return positions;
    //}

    //// Chuyển vị trí sang tọa độ grid
    //public Vector3 SnapToGrid(Vector3 position, Vector2Int size)
    //{
    //    float x, y;

    //    // Kiểm tra nếu size.x là chẵn
    //    if (size.x % 2 == 0)
    //    {
    //        x = Mathf.Round(position.x / cellSize) * cellSize; // Làm tròn về grid
    //    }
    //    else
    //    {
    //        x = position.x; // Giữ nguyên vị trí
    //    }

    //    // Kiểm tra nếu size.y là chẵn
    //    if (size.y % 2 == 0)
    //    {
    //        y = Mathf.Round(position.y / cellSize) * cellSize; // Làm tròn về grid
    //    }
    //    else
    //    {
    //        y = position.z; // Giữ nguyên vị trí
    //    }

    //    // Trục Y giữ nguyên vì không cần xử lý grid (trường hợp 2D top-down)
    //    return new Vector3(x, y, position.z);
    //}
}

