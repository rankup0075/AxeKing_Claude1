using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject loadingPanel;
    public GameObject confirmQuitPanel;

    [Header("Main Menu Buttons")]
    public Button newGameButton;
    public Button continueButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Settings UI")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    //public Dropdown qualityDropdown;
    //public Toggle fullscreenToggle;

    [Header("Loading UI")]
    public Slider loadingProgressBar;
    public TextMeshProUGUI loadingText;
    public TextMeshProUGUI tipText;

    //[Header("Audio")]
    //public AudioSource backgroundMusic;
    //public AudioClip buttonClickSound;
    //public AudioClip buttonHoverSound;

    [Header("Game Settings")]
    public string firstSceneName = "Town";

    private string[] loadingTips = {
        "Tip: ������ Ȱ���Ͽ� ü���� ��� ȸ���ϼ���!",
        "Tip: Shift�� ���� �޸�����!",
        "Tip: ������ �°� �ȴٸ� ��� ������ �Ǵ� �����ϼ���!",
        "Tip: ���� ���� ���� �ʹٰ��? ����Ʈ�� �ϼ���!",
        "Tip: ���� ���� ������ ������ ���� ��������!"
    };

    void Start()
    {
        InitializeMenu();
        LoadPlayerPrefs();

        // ��ư �̺�Ʈ ����
        SetupButtonEvents();

        // ���� ���� ��ư Ȱ��ȭ ���� Ȯ��
        continueButton.interactable = HasSaveData();

        //// ��� ���� ���
        //if (backgroundMusic != null)
        //    backgroundMusic.Play();
    }

    void InitializeMenu()
    {
        // ��� �г��� ��Ȱ��ȭ�ϰ� ���� �޴��� Ȱ��ȭ
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        loadingPanel.SetActive(false);
        confirmQuitPanel.SetActive(false);
    }

    void SetupButtonEvents()
    {
        // ���� �޴� ��ư��
        newGameButton.onClick.AddListener(StartNewGame);
        continueButton.onClick.AddListener(ContinueGame);
        settingsButton.onClick.AddListener(OpenSettings);
        quitButton.onClick.AddListener(ShowQuitConfirm);

        // ��ư ���� ȿ�� �߰�
        AddButtonSounds(newGameButton);
        AddButtonSounds(continueButton);
        AddButtonSounds(settingsButton);
        AddButtonSounds(quitButton);

        // ���� �г� ��ư��
        GameObject backButton = settingsPanel.transform.Find("BackButton")?.gameObject;
        if (backButton != null)
        {
            backButton.GetComponent<Button>().onClick.AddListener(CloseSettings);
        }

        // ���� Ȯ�� �г� ��ư��
        GameObject confirmButton = confirmQuitPanel.transform.Find("ConfirmButton")?.gameObject;
        GameObject cancelButton = confirmQuitPanel.transform.Find("CancelButton")?.gameObject;

        if (confirmButton != null)
            confirmButton.GetComponent<Button>().onClick.AddListener(QuitGame);
        if (cancelButton != null)
            cancelButton.GetComponent<Button>().onClick.AddListener(CancelQuit);
    }

    void AddButtonSounds(Button button)
    {
        // ��ư Ŭ�� ����
        //button.onClick.AddListener(() => PlaySound(buttonClickSound));

        // ��ư ȣ�� ���� (EventTrigger ���)
        UnityEngine.EventSystems.EventTrigger trigger = button.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

        UnityEngine.EventSystems.EventTrigger.Entry entry = new UnityEngine.EventSystems.EventTrigger.Entry();
        entry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        //entry.callback.AddListener((data) => PlaySound(buttonHoverSound));
        trigger.triggers.Add(entry);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }

    public void StartNewGame()
    {
        // �� ���� ������ �ʱ�ȭ
        PlayerPrefs.DeleteKey("Gold");
        PlayerPrefs.DeleteKey("CurrentTerritory");
        PlayerPrefs.DeleteKey("CurrentStage");
        PlayerPrefs.DeleteKey("CurrentRound");
        PlayerPrefs.Save();

        StartCoroutine(LoadSceneAsync(firstSceneName));
    }

    public void ContinueGame()
    {
        if (HasSaveData())
        {
            StartCoroutine(LoadSceneAsync(firstSceneName));
        }
    }

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        LoadCurrentSettings();
    }

    public void CloseSettings()
    {
        SaveSettings();
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void ShowQuitConfirm()
    {
        confirmQuitPanel.SetActive(true);
    }

    public void CancelQuit()
    {
        confirmQuitPanel.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        // �ε� ȭ�� ǥ��
        mainMenuPanel.SetActive(false);
        loadingPanel.SetActive(true);

        // ���� �� ǥ��
        if (tipText != null)
            tipText.text = loadingTips[Random.Range(0, loadingTips.Length)];

        // �񵿱� �� �ε�
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        float fakeLoadingTime = 0f;
        float totalFakeTime = 2f; // �ּ� 2�ʰ� �ε� ȭ�� ǥ��

        while (!asyncLoad.isDone)
        {
            // ���� �ε� ���൵�� ��¥ �ε� �ð��� ����
            float realProgress = asyncLoad.progress;
            fakeLoadingTime += Time.deltaTime;
            float fakeProgress = fakeLoadingTime / totalFakeTime;

            float displayProgress = Mathf.Min(realProgress, fakeProgress);

            // UI ������Ʈ
            if (loadingProgressBar != null)
                loadingProgressBar.value = displayProgress;

            if (loadingText != null)
                loadingText.text = $"�ε���... {(displayProgress * 100):F0}%";

            // �ε� �Ϸ� ����
            if (realProgress >= 0.9f && fakeLoadingTime >= totalFakeTime)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    void LoadPlayerPrefs()
    {
        // ����� ����
        //if (masterVolumeSlider != null)
        //{
        //    float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        //    masterVolumeSlider.value = masterVolume;
        //    AudioListener.volume = masterVolume;
        //}

        //if (musicVolumeSlider != null && backgroundMusic != null)
        //{
        //    float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        //    musicVolumeSlider.value = musicVolume;
        //    backgroundMusic.volume = musicVolume;
        //}

        //// �׷��� ����
        //if (qualityDropdown != null)
        //{
        //    int quality = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        //    qualityDropdown.value = quality;
        //    QualitySettings.SetQualityLevel(quality);
        //}

        //if (fullscreenToggle != null)
        //{
        //    bool fullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        //    fullscreenToggle.isOn = fullscreen;
        //    Screen.fullScreen = fullscreen;
        //}
    }

    void LoadCurrentSettings()
    {
        // ���� ���������� UI�� �ݿ�
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = AudioListener.volume;

        //if (musicVolumeSlider != null && backgroundMusic != null)
        //    musicVolumeSlider.value = backgroundMusic.volume;

        //if (qualityDropdown != null)
        //    qualityDropdown.value = QualitySettings.GetQualityLevel();

        //if (fullscreenToggle != null)
        //    fullscreenToggle.isOn = Screen.fullScreen;

        //// �����̴� �̺�Ʈ ����
        //if (masterVolumeSlider != null)
        //    masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

        //if (musicVolumeSlider != null)
        //    musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        //if (qualityDropdown != null)
        //    qualityDropdown.onValueChanged.AddListener(OnQualityChanged);

        //if (fullscreenToggle != null)
        //    fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
    }

    void SaveSettings()
    {
        // ������ ����
        if (masterVolumeSlider != null)
            PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);

        if (musicVolumeSlider != null)
            PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);

        //if (qualityDropdown != null)
        //    PlayerPrefs.SetInt("QualityLevel", qualityDropdown.value);

        //if (fullscreenToggle != null)
        //    PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);

        PlayerPrefs.Save();
    }

    // ���� ���� �̺�Ʈ��
    void OnMasterVolumeChanged(float value)
    {
        AudioListener.volume = value;
    }

    void OnMusicVolumeChanged(float value)
    {
        //if (backgroundMusic != null)
        //    backgroundMusic.volume = value;
    }

    void OnQualityChanged(int value)
    {
        QualitySettings.SetQualityLevel(value);
    }

    void OnFullscreenChanged(bool value)
    {
        Screen.fullScreen = value;
    }

    bool HasSaveData()
    {
        return PlayerPrefs.HasKey("Gold") ||
               PlayerPrefs.HasKey("CurrentTerritory") ||
               PlayerPrefs.HasKey("CurrentStage");
    }

    void Update()
    {
        // ESC Ű�� ���� �г� �ݱ�
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel.activeInHierarchy)
            {
                CloseSettings();
            }
            else if (confirmQuitPanel.activeInHierarchy)
            {
                CancelQuit();
            }
        }
    }

    public void CloseSettingPanel()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
            Time.timeScale = 1f; // ���� �簳
        }

        // �޽��� �гε� �ݱ�
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
    }
}