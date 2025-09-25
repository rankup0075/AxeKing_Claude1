using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject shopPanel;
    public Transform itemContainer;
    public GameObject itemPrefab;
    public TextMeshProUGUI goldText;

    private ShopItem[] shopItems;

    [System.Serializable]
    public class ShopItem
    {
        public string itemName;
        public int price;
        public string description;
        public ItemType type;
        public int statBonus;
    }

    public enum ItemType { Weapon, Armor }

    void Start()
    {
        InitializeShop();
    }

    void InitializeShop()
    {
        shopItems = new ShopItem[]
        {
            // ����
            new ShopItem { itemName = "�Ϲ� ����", price = 1000, description = "���ݷ� +1", type = ItemType.Weapon, statBonus = 1 },
            new ShopItem { itemName = "��¦ ��ī�ο� ����", price = 3000, description = "���ݷ� +2", type = ItemType.Weapon, statBonus = 2 },
            // ... �� ���� �����۵�
            
            // ��  
            new ShopItem { itemName = "õ����", price = 1000, description = "ü�� +30", type = ItemType.Armor, statBonus = 30 },
            new ShopItem { itemName = "���� ����", price = 3000, description = "ü�� +50", type = ItemType.Armor, statBonus = 50 },
            // ... �� ���� ����
        };

        CreateShopItems();
    }

    void CreateShopItems()
    {
        foreach (var item in shopItems)
        {
            GameObject itemUI = Instantiate(itemPrefab, itemContainer);

            // UI ����
            TextMeshProUGUI nameText = itemUI.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI priceText = itemUI.transform.Find("Price").GetComponent<TextMeshProUGUI>();
            Button buyButton = itemUI.transform.Find("BuyButton").GetComponent<Button>();

            nameText.text = item.itemName;
            priceText.text = $"{item.price}��";

            // ���� ��ư �̺�Ʈ
            buyButton.onClick.AddListener(() => BuyItem(item));
        }
    }

    public void BuyItem(ShopItem item)
    {
        if (GameManager.Instance.Gold >= item.price)
        {
            GameManager.Instance.SpendGold(item.price);

            // �÷��̾� �κ��丮�� ������ �߰�
            PlayerInventory inventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
            inventory.AddEquipment(item.itemName, item.type, item.statBonus);

            UpdateGoldDisplay();
        }
    }

    void UpdateGoldDisplay()
    {
        goldText.text = $"���: {GameManager.Instance.Gold}��";
    }

    public void OpenShop()
    {
        shopPanel.SetActive(true);
        UpdateGoldDisplay();
        Time.timeScale = 0f; // ���� �Ͻ�����
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        Time.timeScale = 1f; // ���� �����
    }
}