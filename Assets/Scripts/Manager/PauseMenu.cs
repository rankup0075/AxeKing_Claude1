using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("Settings UI Panel")]
    public GameObject settingsPanel; // Inspector�� MainMenu�� Settings UI �г� ����
    public static PauseMenu Instance; // Ÿ���� UIManager �� PauseMenu�� ����

    private bool isOpen = false;

    void Awake()
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

    void Update()
    {
        // ESC Ű �Է� ����
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isOpen)
                CloseSettings();
            else
                OpenSettings();
        }
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            isOpen = true;
            Time.timeScale = 0f; // ���� ����
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            isOpen = false;
            Time.timeScale = 1f; // ���� �簳
        }
    }
}
