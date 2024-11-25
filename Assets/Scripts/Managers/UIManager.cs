using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    [Header("[Stamina]")]
    [SerializeField] private Slider staminaBar;

    private Coroutine hideStaminaCoroutine;
    private bool isStaminaBarVisible = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (staminaBar != null)
        {
            staminaBar.value = 1f;
            staminaBar.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("스태미너 바 없음");
        }
    }

    public void UpdateStaminaBar(float value)
    {
        if (staminaBar != null)
        {
            staminaBar.value = value;
            ShowStaminaBar();

            if (value >= 1f)
            {
                if (hideStaminaCoroutine != null)
                {
                    StopCoroutine(hideStaminaCoroutine);
                }
                hideStaminaCoroutine = StartCoroutine(HideStaminaBarWithDelay(1f));
            }
            else
            {
                if (hideStaminaCoroutine != null)
                {
                    StopCoroutine(hideStaminaCoroutine);
                    hideStaminaCoroutine = null;
                }
            }
        }
        else
        {
            Debug.LogError("스태미나 바 없음");
        }
    }

    private void ShowStaminaBar()
    {
        if (!isStaminaBarVisible && staminaBar != null)
        {
            staminaBar.gameObject.SetActive(true);
            isStaminaBarVisible = true;
        }
    }

    private IEnumerator HideStaminaBarWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (staminaBar != null)
        {
            staminaBar.gameObject.SetActive(false);
            isStaminaBarVisible = false;
        }
    }
}
