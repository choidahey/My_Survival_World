using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroSceneLoader : MonoBehaviour
{
    [SerializeField] private Button start_button;

    private void Start()
    {
        if (start_button != null)
        {
            start_button.onClick.AddListener(LoadLoadingScene);
        }
        else
        {
            Debug.LogError("[IntroSceneLoader] Start 버튼이 할당되지 않았습니다!");
        }
    }

    public void LoadLoadingScene()
    {
        SceneManager.LoadScene("Loading");
    }
}
