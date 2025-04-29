using UnityEngine;

public class Setup : MonoBehaviour
{
    public PlayerProgressData gameData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameData.ResetToDefaults();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
