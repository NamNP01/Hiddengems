using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.tvOS;
using UnityEngine.UI;

public class GemCountUI : MonoBehaviour
{
    public static GemCountUI Instance { get; private set; }

    [Header("Dependencies")]
    public GemControler gemController;
    public TextMeshProUGUI gemListText;

    // Lưu trạng thái số lượng gem còn lại
    private Dictionary<string, int> gemCounts = new Dictionary<string, int>();

    private void Awake()
    {
        // Thiết lập singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (gemController == null || gemListText == null)
        {
            Debug.LogError("GemCountUI: Missing references in Inspector!");
            return;
        }

        InitializeGemCounts();
        RefreshUI();
    }

    private void InitializeGemCounts()
    {
        gemCounts.Clear();
        foreach (var gem in gemController.gemConfigs)
        {
            string name = gem.gemPrefab.name;
            if (gemCounts.ContainsKey(name))
                gemCounts[name]++;
            else
                gemCounts[name] = 1;
        }
    }

    public void OnGemDestroyed(string gemName)
    {
        if (gemCounts.ContainsKey(gemName) && gemCounts[gemName] > 0)
        {
            gemCounts[gemName]--;
            if (gemCounts[gemName] <= 0)
                gemCounts.Remove(gemName);
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (gemListText == null) return;

        if (gemCounts.Count == 0)
        {
            gemListText.text = "Completed!";
            StageRewardManager.Instance.OnStageCompleted();

            return;
        }

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Gem Missing:");
        foreach (var kvp in gemCounts)
        {
            sb.AppendLine($" {kvp.Key}: {kvp.Value}");
        }

        gemListText.text = sb.ToString();
    }

    public void SetRequirements(Dictionary<string, int> reqs)
    {
        gemCounts = new Dictionary<string, int>(reqs);
        RefreshUI();
    }
}
