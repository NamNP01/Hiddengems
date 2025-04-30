using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Unity.Android.Gradle.Manifest;

public class GemControler : MonoBehaviour
{
    [Header("")]
    public float cellSize = 1.0f;

    [Header("Pickaxe Text")]
    public TextMeshProUGUI pickaxeText;
    public GameObject pickaxePopup;
    public Dictionary<Vector3, bool> stonePositions = new Dictionary<Vector3, bool>();
    [Header("Check place possible")]
    public bool isPlacementPossible = true;
    [Header("Dynamite ")]
    public int dynamiteCount = 0;
    public List<(GemConfig gem, Vector3 position)> placedGems = new List<(GemConfig, Vector3)>();
    [Header("Data")]
    public PlayerProgressData gameData;
    [System.Serializable]
    public class GemConfig
    {
        public GameObject gemPrefab;
        public Vector2Int size;
    }
    void Start()
    {
        FindAllStones();
        UpdatePickaxeCountUI();
        AssignDynamiteToRandomStones();
    }

    [Header("List Gems")]
    public List<GemConfig> gemConfigs;


    [Header("List stone positions")]
    public List<Vector3> allStonePositions = new List<Vector3>();


    public void UpdatePickaxeCountUI()
    {

        pickaxeText.text = "pickaxe: " + gameData.pickaxeCount.ToString();
    }
    public void UpdatePickaxe()
    {
        gameData.pickaxeCount--;
        if (gameData.pickaxeCount == 0)
        {
            pickaxePopup.SetActive(true);
        }
        UpdatePickaxeCountUI();
    }
    public void OnYesButtonClicked()
    {
        gameData.pickaxeCount += 100;
        UpdatePickaxeCountUI();
        pickaxePopup.SetActive(false);
    }

    public void OnNoButtonClicked()
    {
        pickaxePopup.SetActive(false);
    }
    public void FindAllStones()
    {
        GameObject[] stoneObjects = GameObject.FindGameObjectsWithTag("Stone");

        allStonePositions.Clear();  
        foreach (GameObject stone in stoneObjects)
        {
            if (stone != null)
            {
                allStonePositions.Add(stone.transform.position);
                //Debug.Log("Stone found at: " + stone.transform.position);
            }
        }
    }

    public void RemoveStonePosition(Vector3 position)
    {
        position = SnapToGrid(position);

        if (allStonePositions.Contains(position))
        {
            allStonePositions.Remove(position);  
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
            position.x,
            position.y,                     
            Mathf.Floor(position.z / cellSize) * cellSize
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

        stonePosition = SnapToGrid(stonePosition);

        List<Vector3> occupiedPositions = GetOccupiedPositionsForGem(selectedGem, stonePosition);

        if (occupiedPositions == null || occupiedPositions.Count != selectedGem.size.x * selectedGem.size.y)
        {
            Debug.Log("Cannot spawn gem: Invalid occupied positions.");
            return;
        }

        Vector3 centerPosition = CalculateCenterPosition(occupiedPositions);
        centerPosition = SnapToGrid(centerPosition);
        centerPosition.z = selectedGem.gemPrefab.transform.position.z;

        Instantiate(selectedGem.gemPrefab, centerPosition, Quaternion.identity);
        Debug.Log($"Spawned gem: {selectedGem.gemPrefab.name} at {centerPosition}");


        gemConfigs.RemoveAt(randomIndex);

        foreach (Vector3 pos in occupiedPositions)
        {
            RemoveStonePosition(pos);
        }
    }
    public void PlaceLargestGemAndCheckRemaining()
    {
        placedGems.Clear();
        List<GemConfig> sortedGemConfigs = gemConfigs.OrderByDescending(gem => gem.size.x * gem.size.y).ToList();
        List<Vector3> remainingPositions = new List<Vector3>(allStonePositions);

        int totalGems = sortedGemConfigs.Count;
        Debug.Log($"Total gems to place: {totalGems}");

        bool allGemsPlaced = BacktrackPlaceGems(sortedGemConfigs, remainingPositions, 0);

        if (allGemsPlaced)
        {
            Debug.Log("All gems were successfully placed!");

            if (!isPlacementPossible)
            {
                foreach (var placedGem in placedGems)
                {
                    Instantiate(placedGem.gem.gemPrefab, placedGem.position, Quaternion.identity);
                    //Debug.Log($"Gem {placedGem.gem.gemPrefab.name} placed at {placedGem.position}");
                    gemConfigs.Remove(placedGem.gem);
                    Debug.Log($"remove Gem ");
                }
            }
        }
        else
        {
            Debug.Log("Some gems could not be placed.");
            isPlacementPossible = false;
        }
    }

    private bool BacktrackPlaceGems(List<GemConfig> sortedGemConfigs, List<Vector3> remainingPositions, int gemIndex)
    {
        if (gemIndex >= sortedGemConfigs.Count)
        {
            return true;
        }

        GemConfig currentGem = sortedGemConfigs[gemIndex];

        foreach (Vector3 position in remainingPositions.ToList())
        {
            if (CanPlaceGemAtPosition(currentGem, position, remainingPositions))
            {
                List<Vector3> occupiedPositions = GetOccupiedPositionsForGem(currentGem, position);
                Vector3 centerPosition = CalculateCenterPosition(occupiedPositions);

                //Debug.Log($"Gem {currentGem.gemPrefab.name} placed at {centerPosition}");

                // Lưu lại vị trí của gem đã đặt
                placedGems.Add((currentGem, centerPosition));

                if(!isPlacementPossible)
                {
                    Debug.Log($"Gem {currentGem.gemPrefab.name} placed at {centerPosition} occupying positions: {string.Join(", ", occupiedPositions.Select(pos => pos.ToString()))}");

                }


                foreach (Vector3 occupiedPosition in occupiedPositions)
                {
                    remainingPositions.Remove(occupiedPosition);
                }

                if (BacktrackPlaceGems(sortedGemConfigs, remainingPositions, gemIndex + 1))
                {
                    return true;
                }

                foreach (Vector3 occupiedPosition in occupiedPositions)
                {
                    remainingPositions.Add(occupiedPosition);
                }

                placedGems.RemoveAt(placedGems.Count - 1);
            }
        }

        return false;
    }


    private Vector3 CalculateCenterPosition(List<Vector3> occupiedPositions)
    {
        Vector3 sum = Vector3.zero;
        foreach (Vector3 pos in occupiedPositions)
        {
            sum += pos;
        }

        return sum / occupiedPositions.Count;
    }

        private bool CanPlaceGemAtPosition(GemConfig gem, Vector3 position, List<Vector3> remainingPositions)
        {
            List<Vector3> occupiedPositions = GetOccupiedPositionsForGem(gem, position);

            if (occupiedPositions == null || remainingPositions == null)
            {
                //Debug.LogWarning("Occupied or remaining positions are null.");
                return false;
            }
            return occupiedPositions.All(pos => remainingPositions.Contains(pos));

        }
    public List<Vector3> GetOccupiedPositionsForGem(GemConfig gem, Vector3 basePosition)
    {
        List<Vector3> occupiedPositions = new List<Vector3>();
        basePosition = SnapToGrid(basePosition);

        // Bước 1: Kiểm tra theo trục X
        List<Vector3> horizontalPositions = new List<Vector3>();
        for (int offsetX = 0; offsetX < gem.size.x; offsetX++)
        {
            Vector3 posX = basePosition + new Vector3(offsetX * cellSize, 0, 0);
            if (allStonePositions.Contains(posX))
            {
                horizontalPositions.Add(posX);
            }
            else
            {
                return null; // Nếu bất kỳ ô X nào không có, trả về null
            }
        }

        // Nếu gem chỉ 1 dòng (size.y == 1) thì không cần kiểm tra Y
        if (gem.size.y == 1)
        {
            return horizontalPositions;
        }

        // Thử hướng lên
        bool canMoveUp = true;
        List<Vector3> upPositions = new List<Vector3>(horizontalPositions);
        for (int offsetY = 1; offsetY < gem.size.y; offsetY++)
        {
            foreach (var pos in horizontalPositions)
            {
                Vector3 upPos = pos + new Vector3(0, offsetY * cellSize, 0);
                if (allStonePositions.Contains(upPos))
                {
                    upPositions.Add(upPos);
                }
                else
                {
                    canMoveUp = false;
                    break;
                }
            }
            if (!canMoveUp) break;
        }

        if (canMoveUp)
        {
            return upPositions;
        }

        // Nếu không thể lên, thử hướng xuống
        bool canMoveDown = true;
        List<Vector3> downPositions = new List<Vector3>(horizontalPositions);
        for (int offsetY = 1; offsetY < gem.size.y; offsetY++)
        {
            foreach (var pos in horizontalPositions)
            {
                Vector3 downPos = pos + new Vector3(0, -offsetY * cellSize, 0);
                if (allStonePositions.Contains(downPos))
                {
                    downPositions.Add(downPos);
                }
                else
                {
                    canMoveDown = false;
                    break;
                }
            }
            if (!canMoveDown) break;
        }

        if (canMoveDown)
        {
            return downPositions;
        }

        // Không thể lên hoặc xuống đúng quy tắc
        return null;
    }
    public void AssignDynamiteToRandomStones()
    {
        if (dynamiteCount <= 0 || allStonePositions.Count == 0) return;

        GameObject[] stoneObjects = GameObject.FindGameObjectsWithTag("Stone");

        List<GameObject> shuffledStones = stoneObjects.OrderBy(x => Random.value).ToList();

        int count = Mathf.Min(dynamiteCount, shuffledStones.Count);
        for (int i = 0; i < count; i++)
        {
            Stone stoneScript = shuffledStones[i].GetComponent<Stone>();
            if (stoneScript != null)
            {
                stoneScript.isDynamite = true;
                Debug.Log($"Assigned dynamite to stone at: {shuffledStones[i].transform.position}");
            }
        }
    }

}
