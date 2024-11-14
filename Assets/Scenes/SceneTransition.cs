using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    [Header("Transition Settings")]
    [SerializeField] private Image transitionPanel;
    [SerializeField] private float transitionTime = 1f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = transitionCurve.Evaluate(elapsedTime / transitionTime);
            transitionPanel.color = new Color(0f, 0f, 0f, progress);
            yield return null;
        }
    }

    public IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = 1f - transitionCurve.Evaluate(elapsedTime / transitionTime);
            transitionPanel.color = new Color(0f, 0f, 0f, progress);
            yield return null;
        }
    }
}