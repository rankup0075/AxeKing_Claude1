using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Data")]
    public int gold = 0;
    public int currentTerritory = 1;
    public int currentStage = 1;
    public int currentRound = 1;

    [Header("Game Settings")]
    public bool isPaused = false;

    // === ��Ż ���� �ý��� ===
    // [NEW] ���������� ����� ��Ż�� ���� ID (Town���� ���� ID�� ��Ż ��ġ�� ����)
    public string lastPortalID = null;

    // [NEW] ��� �ε�� ��ġ�� �� �� ������Ʈ ���� �������� ������ ���� ����� ������
    [SerializeField] private float spawnPlaceDelay = 0.02f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // [NEW] �� �ε� �̺�Ʈ ����
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }



    void OnDestroy()
    {
        // [NEW] �� �ε� �̺�Ʈ ����
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public int Gold => gold;

    public void AddGold(int amount)
    {
        gold += amount;
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateGoldDisplay(gold);
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            if (UIManager.Instance != null)
                UIManager.Instance.UpdateGoldDisplay(gold);
            return true;
        }
        return false;
    }

    public void UnlockStage(int territoryId, int stageId, int roundId)
    {
        // ���� �������� ��� ���� ����
        if (roundId > 3)
        {
            roundId = 1;
            stageId++;
            if (stageId > 5)
            {
                stageId = 1;
                territoryId++;
            }
        }

        // ���൵ ����
        if (territoryId > currentTerritory ||
            (territoryId == currentTerritory && stageId > currentStage) ||
            (territoryId == currentTerritory && stageId == currentStage && roundId > currentRound))
        {
            currentTerritory = territoryId;
            currentStage = stageId;
            currentRound = roundId;
            SavePlayerData();
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        if (UIManager.Instance != null)
            UIManager.Instance.ShowGameOverUI();
        else
            Debug.LogWarning("[GameManager] UIManager�� ���� GameOverUI ǥ�� �Ұ�");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Town");
    }

    public void SavePlayerData()
    {
        PlayerPrefs.SetInt("Gold", gold);
        PlayerPrefs.SetInt("CurrentTerritory", currentTerritory);
        PlayerPrefs.SetInt("CurrentStage", currentStage);
        PlayerPrefs.SetInt("CurrentRound", currentRound);

        // [NEW] ������ ��Ż ID ����
        PlayerPrefs.SetString("LastPortalID", lastPortalID ?? "");

        PlayerPrefs.Save();
    }

    public void LoadPlayerData()
    {
        gold = PlayerPrefs.GetInt("Gold", 0);
        currentTerritory = PlayerPrefs.GetInt("CurrentTerritory", 1);
        currentStage = PlayerPrefs.GetInt("CurrentStage", 1);
        currentRound = PlayerPrefs.GetInt("CurrentRound", 1);

        // [NEW] ������ ��Ż ID �ε�
        lastPortalID = PlayerPrefs.GetString("LastPortalID", "");

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateGoldDisplay(gold);
    }

    // [NEW] ��Ż���� ����: ������ ��� ��Ż�� ���� ID
    public void SetLastPortalID(string portalId)
    {
        lastPortalID = portalId;
        Debug.Log($"[GameManager] ������ ��Ż ID ����: {lastPortalID}");
    }

    // [NEW] �� �ε� �� Town�̸� ���� ID�� ��Ż�� ã�� �� ��ġ�� �÷��̾� ��ġ
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (string.IsNullOrEmpty(lastPortalID))
        {
            Debug.Log("[GameManager] lastPortalID ������� �� ������ ���� ����");
            return;
        }
        StartCoroutine(PlacePlayerAtLastPortalAfterDelay());
    }


    // [NEW] ���� ��ġ �ڷ�ƾ
    private System.Collections.IEnumerator PlacePlayerAtLastPortalAfterDelay()
    {
        yield return null;
        yield return new WaitForSeconds(spawnPlaceDelay);

        Portal[] portals = Object.FindObjectsByType<Portal>(FindObjectsSortMode.None);
        if (portals == null || portals.Length == 0)
        {
            Debug.LogWarning("[GameManager] �� ������ Portal�� ã�� ����");
            yield break;
        }

        Portal target = null;
        foreach (var p in portals)
        {
            if (p != null && p.portalID == lastPortalID)
            {
                target = p;
                break;
            }
        }

        if (target == null)
        {
            Debug.LogWarning($"[GameManager] ���� ID ��Ż({lastPortalID}) ����");
            yield break;
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[GameManager] Player �±� ������Ʈ�� ã�� ����");
            yield break;
        }

        // X, Y�� �̵�, Z�� ����
        Vector3 currentPos = player.transform.position;
        Vector3 spawnPos;

        if (target.spawnPoint != null)
        {
            spawnPos = new Vector3(
                target.spawnPoint.position.x,
                target.spawnPoint.position.y,
                currentPos.z                // �� ���� Z�� ����
            );
        }
        else
        {
            spawnPos = new Vector3(
                target.transform.position.x,
                target.transform.position.y,
                currentPos.z                // �� ���� Z�� ����
            );
        }

        // ȸ�� ����ȭ (����)
        if (target.spawnPoint != null)
        {
            Vector3 euler = player.transform.eulerAngles;
            euler.y = target.spawnPoint.eulerAngles.y;
            player.transform.eulerAngles = euler;
        }

        player.transform.position = spawnPos;

        // Rigidbody �ӵ� ����
        var rb = player.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = Vector3.zero;

        Debug.Log($"[GameManager] '{lastPortalID}' ��������Ʈ ���� (Z ����): {player.transform.position}");
    }


}
