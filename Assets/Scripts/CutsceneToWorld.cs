using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneToWorld : MonoBehaviour
{
    [Header("Cutscene Settings")]
    public PlayableDirector timeline;

    [Header("UI Settings")]
    public Button nextButton;

    [Header("Next Scene")]
    public string nextSceneName = "NextScene"; // Change in Inspector

    void Start()
    {
        // Hide the button at the start
        if (nextButton != null)
            nextButton.gameObject.SetActive(false);
        else
            Debug.LogWarning("Next Button not assigned in Inspector.");

        // Subscribe to timeline end event
        if (timeline != null)
            timeline.stopped += OnCutsceneEnd;
        else
            Debug.LogWarning("Timeline not assigned in Inspector.");
    }

    void OnCutsceneEnd(PlayableDirector director)
    {
        // Show button when cutscene finishes
        if (nextButton != null)
            nextButton.gameObject.SetActive(true);
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
