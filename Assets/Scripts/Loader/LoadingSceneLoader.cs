using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneLoader : MonoBehaviour
{
    [SerializeField] private Slider progress_bar;

    private void Start()
    {
        if (progress_bar == null)
        {
            Debug.LogError("[LoadingSceneLoader] Progress Bar가 할당되지 않았습니다!");
            return;
        }

        StartCoroutine(LoadMainSceneAsync());
    }

    private IEnumerator LoadMainSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Main");
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progress_bar.value = progress;

            Debug.Log($"Loading Progress: {progress}");

            if (operation.progress >= 0.9f)
            {
                progress_bar.value = 1f;
                Debug.Log("Loading complete. Activating Main scene.");

                yield return new WaitForSeconds(1f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
