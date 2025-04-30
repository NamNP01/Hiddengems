using UnityEngine;

public class Setup : MonoBehaviour
{
    public PlayerProgressData gameData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        gameData.pickaxeCount = 10;
        gameData.currentStage = 0;
    }

}
