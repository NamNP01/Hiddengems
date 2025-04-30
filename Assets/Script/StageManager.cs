using System.Collections.Generic;
using UnityEngine;

// Yêu cầu mỗi loại gem (prefab)
[System.Serializable]
public class GemRequirement
{
    public GameObject gemPrefab;
    public Vector2Int size;
}

[System.Serializable]
public class StageRequirement
{
    public List<GemRequirement> requirements;
}

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    public List<StageRequirement> stages;

    private int currentStage = 0;
    private HashSet<string> remaining = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        LoadStage(0);
    }

    public void LoadStage(int index)
    {
        if (index < 0 || index >= stages.Count)
        {
            Debug.LogWarning("Invalid stage index.");
            return;
        }

        remaining.Clear();
        foreach (var req in stages[index].requirements)
        {
            string key = GetKeyName(req.gemPrefab.name);
            remaining.Add(key);
        }

        Debug.Log($"Stage {index} loaded with {remaining.Count} gem(s).");
    }

    public void OnGemCollected(string rawName)
    {
        string key = GetKeyName(rawName);
        if (!remaining.Contains(key))
        {
            Debug.LogWarning($"Gem '{key}' not in remaining list.");
            return;
        }

        remaining.Remove(key);
        Debug.Log($"Collected gem '{key}'. Remaining: {remaining.Count}");

        if (remaining.Count == 0)
        {
            AdvanceStage();
        }
    }

    private void AdvanceStage()
    {
        Debug.Log($"Stage {currentStage} complete!");
        currentStage++;

        if (currentStage < stages.Count)
        {
            LoadStage(currentStage);
        }
        else
        {
            Debug.Log("All stages completed!");
        }
    }

    private string GetKeyName(string rawName)
    {
        return rawName.Replace("(Clone)", "").Trim();
    }
}
