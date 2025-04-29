using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProgressData", menuName = "Game Data/Player Progress")]
public class PlayerProgressData : ScriptableObject
{
    [Header("Default Values")]
    public int defaultStage = 0;
    public int defaultPickaxe = 10;

    [Header("Runtime Values")]
    public int currentStage;
    public int pickaxeCount;

    public void ResetToDefaults()
    {
        currentStage = defaultStage;
        pickaxeCount = defaultPickaxe;
    }
}
