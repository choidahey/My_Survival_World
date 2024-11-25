using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    // Todo
    // 설정에 마우스 감도 추가
    private string Header = "[ UIManager ]";
    private GameManager gameManager;

    [Header("[Settings]")]
    public GameObject setting_panel;

    [Header("[Quit Setting]")]
    public GameObject quit_panel;
    public Button quit_yes_button;
    public Button quit_no_button;
    public Button quit_button;

    private GameObject modal_background;
    private Dictionary<Button, System.Action> buttonActions;

    private bool isActivePanel = false;

    [Header("[Stamina]")]
    [SerializeField] private Slider staminaBar;

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
        InitializeGameManager();
        CreateModalBackground();
        RegisterQuitButtons();
        OnControlQuitPanel(false);

        if (staminaBar != null)
            staminaBar.value = 100f;
        else
            Debug.Log("스태미너 바 없음");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && quit_panel.activeSelf)
        {
            OnControlQuitPanel(false);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnControlQuitPanel(true);
        }
    }


    private void InitializeGameManager()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.Log(Header + " GameManager를 찾을 수 없습니다!");
        }
    }

    private void CreateModalBackground()
    {
        // Canvas 아래에 모달 배경 생성
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError(Header + " Canvas를 찾을 수 없습니다!");
            return;
        }

        // GameObject 생성
        modal_background = new GameObject("ModalBackground");
        modal_background.transform.SetParent(canvas.transform, false);

        // Image 컴포넌트 추가
        Image backgroundImage = modal_background.AddComponent<Image>();
        backgroundImage.color = new Color(0, 0, 0, 0.9f); // 반투명 검은색 (투명도를 낮춤)

        // RectTransform 설정
        RectTransform rectTransform = modal_background.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero; // 화면의 왼쪽 아래
        rectTransform.anchorMax = Vector2.one;  // 화면의 오른쪽 위
        rectTransform.offsetMin = Vector2.zero; // 패딩 없음
        rectTransform.offsetMax = Vector2.zero;

        // Raycast Block 설정 (UI 입력 차단)
        backgroundImage.raycastTarget = true;

        // ModalBackground를 Quit Panel 바로 뒤로 이동
        if (quit_panel != null)
        {
            modal_background.transform.SetSiblingIndex(quit_panel.transform.GetSiblingIndex());
        }

        // 초기 상태 비활성화
        modal_background.SetActive(false);
    }

    public void OnControlQuitPanel(bool flag)
    {
        if (quit_panel != null && modal_background != null)
        {
            quit_panel.SetActive(flag);
            modal_background.SetActive(flag);
        }
        else
        {
            Debug.LogError(Header + " Quit Panel 또는 Modal Background가 없습니다!");
        }
    }

    private void RegisterQuitButtons()
    {
        // 버튼과 이벤트 매핑
        buttonActions = new Dictionary<Button, System.Action>
        {
            { quit_yes_button, () => gameManager.Quit() },
            { quit_no_button, () => OnControlQuitPanel(false) },
            { quit_button, () => OnControlQuitPanel(true) }
        };

        // 버튼 이벤트 등록
        foreach (var entry in buttonActions)
        {
            if (entry.Key != null)
            {
                entry.Key.onClick.RemoveAllListeners();
                entry.Key.onClick.AddListener(() => entry.Value.Invoke());
            }
            else
            {
                Debug.LogError($"{Header} 버튼이 연결되지 않았습니다!");
            }
        }
    }

    public void UpdateStaminaBar(float value)
    {
        if (staminaBar != null)
        {
            staminaBar.value = value;
        }
        else
            Debug.Log("스테미나 바 없음");
    }
}
