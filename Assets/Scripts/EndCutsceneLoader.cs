using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class EndCutsceneLoader : MonoBehaviour
{
    public PlayableDirector TimeLineES;      
    public string nextSceneName = "EndHome"; 
    void Start()
    {
        if (TimeLineES != null)
        {
            TimeLineES.stopped += OnTimelineStopped;
        }
        else
        {
            Debug.LogWarning("PlayableDirector not assigned in EndCutsceneLoader.");
        }
    }

    void OnTimelineStopped(PlayableDirector director)
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
