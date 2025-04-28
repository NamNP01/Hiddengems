using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class GemControler : MonoBehaviour
{
    public float cellSize = 1.0f; // Kích thước mỗi ô trên grid
    public int gridWidth = 10;   // Số ô theo chiều ngang
    public int gridHeight = 10;  // Số ô theo chiều dọc
    public int pickaxeCount = 100;
    public TextMeshProUGUI pickaxeText;
    public GameObject pickaxePopup;

    public Dictionary<Vector3, bool> stonePositions = new Dictionary<Vector3, bool>();

    public bool isPlacementPossible = true; // Biến để theo dõi việc đặt gem có khả thi không

    private List<(GemConfig gem, Vector3 position)> placedGems = new List<(GemConfig, Vector3)>();

    [System.Serializable]
    public class GemConfig
    {
        public GameObject gemPrefab; // Prefab của gem
        public Vector2Int size;      // Kích thước gem trên grid
        //public int count;            // Số lượng gem còn lại
    }
    void Start()
    {
        // Gọi FindAllStones để lưu tất cả các vị trí stone khi game bắt đầu
        FindAllStones();
        UpdatePickaxeCountUI();
        PlaceLargestGemAndCheckRemaining();
    }
    public List<GemConfig> gemConfigs; // Danh sách các gem có thể tạo

    // Danh sách các vị trí stone được public để có thể truy cập từ bên ngoài
    public List<Vector3> allStonePositions = new List<Vector3>();

    // Hàm để tìm tất cả các GameObject có tag "Stone" và lưu vị trí của chúng
    public void UpdatePickaxeCountUI()
    {
        pickaxeText.text = "pickaxe: " + pickaxeCount.ToString();
    }
    public void UpdatePickaxe()
    {
        pickaxeCount--;
        if (pickaxeCount == 0)
        {
            pickaxePopup.SetActive(true);
        }
        UpdatePickaxeCountUI();
    }
    // Xử lý khi nhấn nút "Yes"
    public void OnYesButtonClicked()
    {
        // Thêm 100 cuốc và đóng popup
        pickaxeCount += 100;
        UpdatePickaxeCountUI();
        pickaxePopup.SetActive(false);
    }

    // Xử lý khi nhấn nút "No"
    public void OnNoButtonClicked()
    {
        // Đóng popup mà không thêm cuốc
        pickaxePopup.SetActive(false);
    }
    public void FindAllStones()
    {
        // Lấy tất cả các GameObject có tag "Stone"
        GameObject[] stoneObjects = GameObject.FindGameObjectsWithTag("Stone");

        // Lưu vị trí của từng stone vào danh sách allStonePositions
        allStonePositions.Clear();  // Xóa danh sách cũ trước khi thêm mới
        foreach (GameObject stone in stoneObjects)
        {
            if (stone != null)
            {
                allStonePositions.Add(stone.transform.position);
                //Debug.Log("Stone found at: " + stone.transform.position);
            }
        }
    }

    // Xóa vị trí stone khỏi grid
    public void RemoveStonePosition(Vector3 position)
    {
        // Làm tròn vị trí nếu cần
        position = SnapToGrid(position);

        // Loại bỏ vị trí khỏi allStonePositions
        if (allStonePositions.Contains(position))
        {
            allStonePositions.Remove(position);  // Xóa vị trí stone khỏi danh sách
            Debug.Log("Stone removed from allStonePositions: " + position);
        }
        else
        {
            Debug.Log("Position not found in allStonePositions: " + position);
        }
    }

    public Vector3 SnapToGrid(Vector3 position)
    {
        return new Vector3(
            position.x, // Làm tròn xuống x
            position.y,                                   // Giữ nguyên y
            Mathf.Floor(position.z / cellSize) * cellSize // Làm tròn xuống z
        );
    }

    public void SpawnRandomGem(Vector3 stonePosition)
    {
        if (gemConfigs.Count == 0)
        {
            Debug.Log("No gems available to spawn.");
            return;
        }

        int randomIndex = Random.Range(0, gemConfigs.Count);
        GemConfig selectedGem = gemConfigs[randomIndex];
        Debug.Log($"Selected Gem: {selectedGem.gemPrefab.name}");

        // Snap vị trí đá vào lưới
        stonePosition = SnapToGrid(stonePosition);

        // Lấy danh sách các ô bị chiếm nếu có thể đặt được gem tại vị trí này
        List<Vector3> occupiedPositions = GetOccupiedPositionsForGem(selectedGem, stonePosition);

        if (occupiedPositions == null || occupiedPositions.Count != selectedGem.size.x * selectedGem.size.y)
        {
            Debug.Log("Cannot spawn gem: Invalid occupied positions.");
            return;
        }

        // Tính trung tâm và spawn gem
        Vector3 centerPosition = CalculateCenterPosition(occupiedPositions);
        centerPosition = SnapToGrid(centerPosition);
        centerPosition.z = selectedGem.gemPrefab.transform.position.z;

        Instantiate(selectedGem.gemPrefab, centerPosition, Quaternion.identity);
        Debug.Log($"Spawned gem: {selectedGem.gemPrefab.name} at {centerPosition}");

        // Giảm số lượng gem và loại bỏ nếu hết

        gemConfigs.RemoveAt(randomIndex);
        //selectedGem.count--;
        //if (selectedGem.count <= 0 && randomIndex >= 0 && randomIndex < gemConfigs.Count)
        //{
        //    gemConfigs.RemoveAt(randomIndex);
        //    Debug.Log($"Gem {selectedGem.gemPrefab.name} removed from the list.");
        //}

        PlaceLargestGemAndCheckRemaining();

        // Xóa tất cả các ô đã sử dụng
        foreach (Vector3 pos in occupiedPositions)
        {
            RemoveStonePosition(pos);
        }
    }

    //public void PlaceLargestGemAndCheckRemaining()
    //{
    //    // Sắp xếp danh sách các gem theo diện tích giảm dần
    //    List<GemConfig> sortedGemConfigs = gemConfigs.OrderByDescending(gem => gem.size.x * gem.size.y).ToList();
    //    List<Vector3> remainingPositions = new List<Vector3>(allStonePositions);

    //    int totalGems = sortedGemConfigs.Count;
    //    Debug.Log($"Total gems to place: {totalGems}");

    //    bool allGemsPlaced = true;
    //    foreach (GemConfig gem in sortedGemConfigs)
    //    {
    //        for (int i = 0; i < gem.count; i++)
    //        {
    //            bool isPlaced = false;
    //            foreach (Vector3 position in remainingPositions.ToList())
    //            {
    //                if (CanPlaceGemAtPosition(gem, position, remainingPositions))
    //                {
    //                    // Tính toán trung tâm của các ô mà gem sẽ chiếm dụng
    //                    List<Vector3> occupiedPositions = GetOccupiedPositionsForGem(gem, position);
    //                    Vector3 centerPosition = CalculateCenterPosition(occupiedPositions);

    //                    //// Tạo gem tại vị trí trung tâm
    //                    //Instantiate(gem.gemPrefab, centerPosition, Quaternion.identity);
    //                    Debug.Log($"Gem {gem.gemPrefab.name} placed at {centerPosition}");

    //                    // Cập nhật danh sách các vị trí đã sử dụng
    //                    foreach (Vector3 occupiedPosition in occupiedPositions)
    //                    {
    //                        remainingPositions.Remove(occupiedPosition);
    //                    }

    //                    isPlaced = true;
    //                    break;
    //                }
    //            }

    //            if (!isPlaced)
    //            {
    //                allGemsPlaced = false;
    //                Debug.Log($"Failed to place gem: {gem.gemPrefab.name}.");
    //                isPlacementPossible = false;
    //                break;
    //            }
    //        }
    //    }

    //    if (allGemsPlaced)
    //        Debug.Log("All gems were successfully placed!");
    //    else
    //        Debug.Log("Some gems could not be placed.");
    //}
    public void PlaceLargestGemAndCheckRemaining()
    {
        // Sắp xếp danh sách các gem theo diện tích giảm dần
        List<GemConfig> sortedGemConfigs = gemConfigs.OrderByDescending(gem => gem.size.x * gem.size.y).ToList();
        List<Vector3> remainingPositions = new List<Vector3>(allStonePositions);

        int totalGems = sortedGemConfigs.Count;
        Debug.Log($"Total gems to place: {totalGems}");

        bool allGemsPlaced = BacktrackPlaceGems(sortedGemConfigs, remainingPositions, 0);

        if (allGemsPlaced)
        {
            Debug.Log("All gems were successfully placed!");

            // Hiển thị vị trí của mỗi viên gem nếu tất cả được đặt thành công
            foreach (var placedGem in placedGems)
            {
                // Sinh gem tại vị trí
                Instantiate(placedGem.gem.gemPrefab, placedGem.position, Quaternion.identity);
                Debug.Log($"Gem {placedGem.gem.gemPrefab.name} placed at {placedGem.position}");
                //RemoveStonePosition(placedGem.position);
            }
        }
        else
        {
            Debug.Log("Some gems could not be placed.");
        }
    }

    private bool BacktrackPlaceGems(List<GemConfig> sortedGemConfigs, List<Vector3> remainingPositions, int gemIndex)
    {
        if (gemIndex >= sortedGemConfigs.Count)
        {
            // Đã đặt tất cả các gem
            return true;
        }

        GemConfig currentGem = sortedGemConfigs[gemIndex];

        // Kiểm tra vị trí cho gem này
        foreach (Vector3 position in remainingPositions.ToList())
        {
            if (CanPlaceGemAtPosition(currentGem, position, remainingPositions))
            {
                // Tính toán các ô mà gem sẽ chiếm dụng
                List<Vector3> occupiedPositions = GetOccupiedPositionsForGem(currentGem, position);
                Vector3 centerPosition = CalculateCenterPosition(occupiedPositions);

                //Debug.Log($"Gem {currentGem.gemPrefab.name} placed at {centerPosition}");

                // Lưu lại vị trí của gem đã đặt
                placedGems.Add((currentGem, centerPosition));

                Debug.Log($"Gem {currentGem.gemPrefab.name} placed at {centerPosition} occupying positions: {string.Join(", ", occupiedPositions.Select(pos => pos.ToString()))}");


                // Cập nhật danh sách các vị trí đã sử dụng
                foreach (Vector3 occupiedPosition in occupiedPositions)
                {
                    remainingPositions.Remove(occupiedPosition);
                }

                // Đặt gem tiếp theo
                if (BacktrackPlaceGems(sortedGemConfigs, remainingPositions, gemIndex + 1))
                {
                    return true;
                }

                // Nếu không thể tiếp tục, hoàn tác lại
                foreach (Vector3 occupiedPosition in occupiedPositions)
                {
                    remainingPositions.Add(occupiedPosition);
                }

                // Nếu không thể đặt viên gem, hoàn tác lại
                placedGems.RemoveAt(placedGems.Count - 1);
            }
        }

        // Nếu không thể đặt gem này, quay lại thử viên gem tiếp theo
        return false;
    }



    //public void PlaceGemAfterCheckRemaining()
    //{
    //    List<GemConfig> sortedGemConfigs = gemConfigs.OrderByDescending(gem => gem.size.x * gem.size.y).ToList();
    //    List<Vector3> remainingPositions = new List<Vector3>(allStonePositions);

    //    // Dùng for thay vì foreach để có thể xóa phần tử
    //    for (int g = sortedGemConfigs.Count - 1; g >= 0; g--)
    //    {
    //        GemConfig gem = sortedGemConfigs[g];
    //        int placedCount = 0;

    //        for (int i = 0; i < gem.count; i++)
    //        {
    //            bool placed = false;

    //            foreach (Vector3 position in remainingPositions.ToList())
    //            {
    //                if (CanPlaceGemAtPosition(gem, position, remainingPositions))
    //                {
    //                    List<Vector3> occupiedPositions = GetOccupiedPositionsForGem(gem, position);
    //                    Vector3 centerPosition = CalculateCenterPosition(occupiedPositions);

    //                    Instantiate(gem.gemPrefab, centerPosition, Quaternion.identity);

    //                    foreach (Vector3 occupiedPosition in occupiedPositions)
    //                    {
    //                        remainingPositions.Remove(occupiedPosition);
    //                        allStonePositions.Remove(occupiedPosition);
    //                    }

    //                    placedCount++;
    //                    placed = true;
    //                    break;
    //                }
    //            }

    //            if (!placed)
    //            {
    //                break;
    //            }
    //        }

    //        // Giảm count của gem sau khi đặt thành công
    //        gem.count -= placedCount;

    //        // Nếu count <= 0 thì xóa gem ra khỏi danh sách
    //        if (gem.count <= 0)
    //        {
    //            gemConfigs.Remove(gem);
    //            Debug.Log($"Gem {gem.gemPrefab.name} removed from gemConfigs due to count being 0!");
    //        }
    //    }
    //}


    //Hàm tính toán trung tâm của các ô mà gem sẽ chiếm dụng



    private Vector3 CalculateCenterPosition(List<Vector3> occupiedPositions)
    {
        // Tính toán trung tâm bằng cách lấy trung bình các vị trí
        Vector3 sum = Vector3.zero;
        foreach (Vector3 pos in occupiedPositions)
        {
            sum += pos;
        }

        // Trung tâm là tổng các vị trí chia cho số lượng vị trí
        return sum / occupiedPositions.Count;
    }


    // Hàm kiểm tra xem gem có thể được đặt tại một vị trí cụ thể không
    private bool CanPlaceGemAtPosition(GemConfig gem, Vector3 position, List<Vector3> remainingPositions)
    {
        // Lấy tất cả các vị trí cần thiết để đặt gem tại vị trí này
        List<Vector3> occupiedPositions = GetOccupiedPositionsForGem(gem, position);

        // Kiểm tra xem tất cả các vị trí cần thiết có nằm trong danh sách vị trí còn lại không
        if (occupiedPositions == null || remainingPositions == null)
        {
            Debug.LogWarning("Occupied or remaining positions are null.");
            return false;
        }
        return occupiedPositions.All(pos => remainingPositions.Contains(pos));

    }

    // Hàm tính toán các vị trí mà gem sẽ chiếm
    private List<Vector3> GetOccupiedPositionsForGem(GemConfig gem, Vector3 basePosition)
    {
        List<Vector3> occupiedPositions = new List<Vector3>();
        basePosition = SnapToGrid(basePosition);

        // Duyệt qua tất cả các vị trí trong phạm vi gem (X và Y)
        for (int offsetX = 0; offsetX < gem.size.x; offsetX++)
        {
            for (int offsetY = 0; offsetY < gem.size.y; offsetY++)
            {
                // Tạo các vị trí cho các ô bị chiếm dụng
                Vector3 positionToCheck = basePosition + new Vector3(offsetX * cellSize, offsetY * cellSize, 0);

                // Kiểm tra xem vị trí này có bị chiếm dụng không
                if (allStonePositions.Contains(positionToCheck))
                {
                    occupiedPositions.Add(positionToCheck);
                }
                else
                {
                    return null; // Nếu có bất kỳ vị trí nào không hợp lệ, trả về null
                }
            }
        }

        // Nếu tất cả các ô hợp lệ, trả về danh sách các vị trí bị chiếm dụng
        return occupiedPositions;
    }

}
