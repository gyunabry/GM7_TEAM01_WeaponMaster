using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
    }

    public void LoadScene(SceneType type)
    {
        string sceneName = SceneNames.GetSceneName(type);
        SceneManager.LoadScene(sceneName);
    }
}
