//using UnityEngine;
//using UnityEngine.Events;

//public class EnemyHealth : MonoBehaviour
//{
//    [Header("Health Settings")]
//    public int maxHealth = 5;
//    private int currentHealth;

//    [Header("UI")]
//    public GameObject healthBarPrefab;
//    private GameObject healthBarInstance;
//    private UnityEngine.UI.Slider healthSlider;

//    [Header("Events")]
//    public UnityEvent OnDeath;
//    public UnityEvent<int, int> OnHealthChanged;

//    public int CurrentHealth => currentHealth;

//    public int goldDrop = 50;
//    void Start()
//    {
//        currentHealth = maxHealth;
//        CreateHealthBar();
//    }

//    void CreateHealthBar()
//    {
//        if (healthBarPrefab != null)
//        {
//            // �� �Ӹ� ���� ü�¹� ����
//            healthBarInstance = Instantiate(healthBarPrefab, transform);
//            healthBarInstance.transform.localPosition = Vector3.up * 2.5f;

//            // Canvas�� ī�޶� ���ϵ��� ����
//            Canvas canvas = healthBarInstance.GetComponent<Canvas>();
//            canvas.worldCamera = Camera.main;
//            canvas.sortingLayerName = "UI";

//            healthSlider = healthBarInstance.GetComponentInChildren<UnityEngine.UI.Slider>();
//            if (healthSlider != null)
//            {
//                healthSlider.value = 1f;
//            }
//        }
//    }

//    public void TakeDamage(int damage)
//    {
//        if (currentHealth <= 0) return;

//        currentHealth = Mathf.Max(0, currentHealth - damage);
//        UpdateHealthBar();

//        // ���� ó��
//        EnemyController controller = GetComponent<EnemyController>();
//        if (controller != null)
//        {
//            controller.TakeHit();
//        }

//        OnHealthChanged?.Invoke(currentHealth, maxHealth);

//        if (currentHealth <= 0)
//        {
//            Die();
//        }
//    }

//    void UpdateHealthBar()
//    {
//        if (healthSlider != null)
//        {
//            healthSlider.value = (float)currentHealth / maxHealth;

//            // ü���� 0�� �Ǹ� ü�¹� �����
//            if (currentHealth <= 0)
//            {
//                healthBarInstance.SetActive(false);
//            }
//        }
//    }

//    void Die()
//    {
//        OnDeath?.Invoke();

//        EnemyController controller = GetComponent<EnemyController>();
//        if (controller != null)
//        {
//            controller.Die();
//        }

//        PlayerGold playerGold = GameObject.FindWithTag("Player").GetComponent<PlayerGold>();
//        if (playerGold != null)
//        {
//            playerGold.EarnGold(goldDrop);
//        }
//    }

//    void Update()
//    {
//        // ü�¹ٰ� �׻� ī�޶� ���ϵ���
//        if (healthBarInstance != null && Camera.main != null)
//        {
//            healthBarInstance.transform.LookAt(Camera.main.transform.position);
//            healthBarInstance.transform.Rotate(0, 180, 0);
//        }
//    }
//}