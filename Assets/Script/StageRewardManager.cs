using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageRewardManager : MonoBehaviour
{
    public static StageRewardManager Instance { get; private set; }

    public Slider progressSlider;
    public GameObject rewardObject;

    public PlayerProgressData data;

    private void Awake()
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
