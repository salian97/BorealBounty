using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutsceneToWorld : MonoBehaviour
{
    public PlayableDirector cutsceneTimeline;  // assign in Inspector
    public string worldSceneName = "Spring";

    void Start()
    {
        if (cutsceneTimeline != null)
        {
            cutsceneTimeline.stopped += OnCutsceneEnded;
        }
        else
        {
            Debug.LogWarning("PlayableDirector not assigned!");
        }
    }

    void OnCutsceneEnded(PlayableDirector director)
    {
        SceneManager.LoadScene(worldSceneName);
    }
}
