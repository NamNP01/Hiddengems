using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProgressData", menuName = "Game Data/Player Progress")]
public class PlayerProgressData : ScriptableObject
{

    [Header("Runtime Values")]
    public int currentStage;
    public int pickaxeCount;

}
