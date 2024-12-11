using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;
    
    [Header("Loading Screen Components")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Slider progressBar;
    [SerializeField] private Text progressText;
    [SerializeField] private Text loadingTips;

    [Header("Loading Tips")]
    [SerializeField] private string[] loadingTipsArray = new string[]
    {
        "Tip: watch out for the zombies coming from the backside",
        "Tip: take your time to defeat the zombies",
        "Tip: keep your distance from the zombies",
        "Tip: don't panic if you are hit by the zombies",
    };

    private void Start()
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }

    private bool isLoading = false;

    public void LoadScene(string sceneName)
    {
        if (!isLoading)
        {
            isLoading = true;
            StartCoroutine(LoadSceneAsync(sceneName));
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        if (loadingPanel == null)
        {
            Debug.LogError("Loading panel is not set!");
            yield break;
        }

        loadingPanel.SetActive(true);
        loadingTips.text = GetRandomLoadingTip();

        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        float displayedProgress = 0f; // Progress value displayed on the progress bar
        while (!scene.isDone)
        {
            // Target progress value
            float targetProgress = Mathf.Clamp01(scene.progress / 0.9f);

            // Smoothly update the displayed progress
            displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, 3 * Time.deltaTime);
            progressBar.value = displayedProgress;
            progressText.text = "Loading...";

            // When loading is done (progress >= 1f)
            if (displayedProgress >= 1f)
            {
                yield return new WaitForSeconds(3f);
                scene.allowSceneActivation = true;
            }
            yield return null;
        }
        loadingPanel.SetActive(false);
    }

    private string firstLoadingTip;
    private string GetRandomLoadingTip()
    {
        if (loadingTipsArray.Length == 0)
            return "No loading tips available.";

        if (string.IsNullOrEmpty(firstLoadingTip))
        {
            firstLoadingTip = loadingTipsArray[Random.Range(0, loadingTipsArray.Length)];
        }

        return firstLoadingTip;
    }
}

