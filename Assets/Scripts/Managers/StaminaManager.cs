using UnityEngine;
using System.Collections;

public class StaminaManager : MonoBehaviour
{
    public static StaminaManager instance { get; private set; }

    private float maxStamina = 100f;
    private float staminaRegenRate = 5f;
    private float currentStamina;
    public bool isExhausted { get; private set; }
    private bool isRegenActive = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        currentStamina = maxStamina;
        UIManager.instance.UpdateStaminaBar(currentStamina / maxStamina);
    }

    private void Update()
    {
        if (isExhausted) return;

        if (isRegenActive && currentStamina < maxStamina)
            RegenerateStamina();
    }

    // 스태미너 사용
    public bool ConsumeStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            UIManager.instance.UpdateStaminaBar(currentStamina / maxStamina);

            if (currentStamina <= 0.5f)
            {
                StartCoroutine(HandleExhaustion());
            }

            return true;
        }
        return false;
    }

    private IEnumerator HandleExhaustion()
    {
        isExhausted = true;
        isRegenActive = false;
        Debug.Log("탈진 상태: 2초 대기");

        yield return new WaitForSeconds(2f);

        isExhausted = false;
        isRegenActive = true;
        Debug.Log("탈진 해제");
    }


    // 스태미너 회복
    private void RegenerateStamina()
    {
        currentStamina += staminaRegenRate * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UIManager.instance.UpdateStaminaBar(currentStamina / maxStamina);
    }

    public bool HasEnoughStamina(float amount)
    {
        return currentStamina >= amount;
    }

    public void EnableStaminaRegen()
    {
        if (!isExhausted) isRegenActive = true; // 탈진 상태가 아닐 때만 회복 활성화
    }

    public void DisableStaminaRegen()
    {
        isRegenActive = false;
    }
}
