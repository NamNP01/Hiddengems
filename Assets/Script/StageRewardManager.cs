using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StageRewardManager : MonoBehaviour
{
    [Header("Singleton")]
    public static StageRewardManager Instance { get; private set; }

    [Header("UI")]
    public Slider progressSlider;
    public GameObject rewardObject;
    public TextMeshProUGUI rewardText;

    [Header("Data")]
    public PlayerProgressData data;
    public GemControler gemControler;

    public int rewardPickaxe = 5;

    void Start()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        rewardObject.SetActive(false);

        progressSlider.value = data.currentStage;
    }

    public void OnStageCompleted()
    {
        if (progressSlider != null)
        {
            data.currentStage++;
            progressSlider.value = data.currentStage;
        }

        ShowReward();
    }

    private void ShowReward()
    {
        if (!rewardObject.activeSelf)
            rewardObject.SetActive(true);
        if (rewardText != null)
        {
            if (data.currentStage == 4)
            {
                rewardText.text = $"X{rewardPickaxe} Pickaxe!\nCongratulations! \r\nYou completed all the gates! \r\nClaim your rewards!";
            }
            else
            {

                rewardText.text = $"X{rewardPickaxe} Pickaxe!";
            }
        }
        data.pickaxeCount += rewardPickaxe;
        gemControler.UpdatePickaxeCountUI();
    }


    public void HideReward()
    {
        rewardObject.SetActive(false);
    }
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

}
