using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.tvOS;
using UnityEngine.UI;

public class GemCountUI : MonoBehaviour
{
    public Slider progressSlider;  // Kéo thả Slider trong Inspector
    public static GemCountUI Instance { get; private set; }

    [Header("Dependencies")]
    public GemControler gemController;   // Tham chiếu đến GemController
    public TextMeshProUGUI gemListText;  // Tham chiếu đến TextMeshProUGUI

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

    // Khởi tạo số lượng gem ban đầu
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

            // Di chuyển slider nếu có
            if (progressSlider != null)
            {
                progressSlider.value += 1f; // Tăng giá trị lên 1
            }

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
