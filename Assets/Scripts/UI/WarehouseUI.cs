using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class WarehouseUI : MonoBehaviour
{
    public GameObject warehousePanel;
    public Button weaponTabButton;
    public Button armorTabButton;
    public Button closeButton;
    public Button equipButton;
    public Button unequipButton;

    public Transform itemListContainer;
    public GameObject itemPrefab;
    public GameObject messagePanel;

    private PlayerInventory playerInventory;
    private ShopUI.ItemType currentTab = ShopUI.ItemType.Weapon;
    private ItemEquipment selectedItem;

    [Header("Camera")]
    public Transform chestTransform;
    private CameraFollow camFollow;

    private GameManager gameManager;

    [Header("Stats UI")]
    public TextMeshProUGUI attackStatText;
    public TextMeshProUGUI healthStatText;

    [Header("Equipped Panel")]
    public Transform equippedWeaponPanel;
    public Transform equippedArmorPanel;
    private GameObject equippedWeaponUI;
    private GameObject equippedArmorUI;

    void Start()
    {
        camFollow = Camera.main.GetComponent<CameraFollow>();
        playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
        gameManager = GameManager.Instance;

        closeButton.onClick.AddListener(CloseWarehouse);
        weaponTabButton.onClick.AddListener(() => ShowItems(ShopUI.ItemType.Weapon));
        armorTabButton.onClick.AddListener(() => ShowItems(ShopUI.ItemType.Armor));
        equipButton.onClick.AddListener(EquipSelectedItem);
        unequipButton.onClick.AddListener(UnequipCurrentItem);

        warehousePanel.SetActive(false);
        messagePanel.SetActive(false);
        equipButton.interactable = false;
        unequipButton.interactable = false;
    }

    void RefreshStats()
    {
        var pc = playerInventory.GetComponent<PlayerController>();
        var ph = playerInventory.GetComponent<PlayerHealth>();

        attackStatText.text = $"���ݷ�: {pc.attackDamage}";
        healthStatText.text = $"ü��: {ph.CurrentHealth}/{ph.MaxHealth}";
    }

    public void OpenWarehouse()
    {
        warehousePanel.SetActive(true);
        Debug.Log("OpenWareHouse ����");

        if (camFollow != null && chestTransform != null)
            camFollow.SetTarget(chestTransform, false, true);

        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.canMove = false;
                controller.StopImmediately();
            }
        }

        selectedItem = null;
        equipButton.interactable = false;
        unequipButton.interactable = false;

        // ���� �������� ������ �г� ���� ����
        RefreshEquippedPanel();

        // �κ��丮 �� ����
        ShowItems(currentTab);
    }

    void CloseWarehouse()
    {
        warehousePanel.SetActive(false);

        if (camFollow != null) camFollow.ResetTarget();

        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var controller = player.GetComponent<PlayerController>();
            if (controller != null) controller.canMove = true;
        }
    }

    void ShowItems(ShopUI.ItemType type)
    {
        currentTab = type;

        foreach (Transform child in itemListContainer)
            Destroy(child.gameObject);

        List<ItemEquipment> itemsToShow = (type == ShopUI.ItemType.Weapon) ?
            playerInventory.weaponStorage : playerInventory.armorStorage;

        Debug.Log($"[Warehouse] {type} ����Ʈ �ҷ����� - �� {itemsToShow.Count}��");

        foreach (var item in itemsToShow)
        {
            GameObject obj = Instantiate(itemPrefab, itemListContainer);

            obj.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = item.EquipmentitemName;
            obj.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>().text =
                type == ShopUI.ItemType.Weapon ? $"���ݷ� +{item.EquipmentstatBonus}" : $"ü�� +{item.EquipmentstatBonus}";
            obj.transform.Find("ItemImage").GetComponent<Image>().sprite = item.icon;

            var statusText = obj.transform.Find("StatusText").GetComponent<TextMeshProUGUI>();
            bool isEquipped = (type == ShopUI.ItemType.Weapon && playerInventory.currentWeapon == item) ||
                              (type == ShopUI.ItemType.Armor && playerInventory.currentArmor == item);

            statusText.text = isEquipped ? "������" : "";
            statusText.gameObject.SetActive(true);

            obj.GetComponent<Button>().onClick.AddListener(() =>
            {
                selectedItem = item;
                HighlightSelection(obj);

                // ��ư ���� ����
                equipButton.interactable = !isEquipped;   // �������̸� ��Ȱ��ȭ
                unequipButton.interactable = isEquipped; // �������̸� ���� ����

                Debug.Log(isEquipped
                    ? $"[Warehouse] {item.EquipmentitemName} ���õ� (������ �� ���� ����)"
                    : $"[Warehouse] {item.EquipmentitemName} ���õ� (���� ����)");
            });
        }

        RefreshStats();
    }

    void HighlightSelection(GameObject selected)
    {
        foreach (Transform child in itemListContainer)
        {
            var image = child.GetComponent<Image>();
            if (image != null)
                image.color = Color.white;
        }
        selected.GetComponent<Image>().color = new Color(1f, 0.7f, 0.7f);
    }

    void EquipSelectedItem()
    {
        if (selectedItem == null) return;

        playerInventory.EquipItem(selectedItem, selectedItem.Equipmenttype);
        Debug.Log($"[Warehouse] {selectedItem.EquipmentitemName} ���� �Ϸ�");

        // ���� �г� ����
        RefreshEquippedPanel();
        ShowItems(currentTab);

        equipButton.interactable = false;
        unequipButton.interactable = true;
    }

    void UnequipCurrentItem()
    {
        if (selectedItem == null) return;

        if (currentTab == ShopUI.ItemType.Weapon && playerInventory.currentWeapon == selectedItem)
        {
            playerInventory.EquipItem(null, ShopUI.ItemType.Weapon);
            Debug.Log($"[Warehouse] {selectedItem.EquipmentitemName} ���� ���� �Ϸ�");
        }
        else if (currentTab == ShopUI.ItemType.Armor && playerInventory.currentArmor == selectedItem)
        {
            playerInventory.EquipItem(null, ShopUI.ItemType.Armor);
            Debug.Log($"[Warehouse] {selectedItem.EquipmentitemName} �� ���� �Ϸ�");
        }

        selectedItem = null;
        RefreshEquippedPanel();
        ShowItems(currentTab);

        equipButton.interactable = false;
        unequipButton.interactable = false;
    }

    void RefreshEquippedPanel()
    {
        // ���� ���� ����
        if (equippedWeaponUI != null) Destroy(equippedWeaponUI);
        if (playerInventory.currentWeapon != null)
            equippedWeaponUI = CreateEquippedUI(playerInventory.currentWeapon, equippedWeaponPanel);

        // �� ���� ����
        if (equippedArmorUI != null) Destroy(equippedArmorUI);
        if (playerInventory.currentArmor != null)
            equippedArmorUI = CreateEquippedUI(playerInventory.currentArmor, equippedArmorPanel);
    }

    GameObject CreateEquippedUI(ItemEquipment item, Transform parent)
    {
        GameObject obj = Instantiate(itemPrefab, parent);

        obj.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = item.EquipmentitemName;
        obj.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>().text =
            item.Equipmenttype == ShopUI.ItemType.Weapon ?
            $"���ݷ� +{item.EquipmentstatBonus}" : $"ü�� +{item.EquipmentstatBonus}";
        obj.transform.Find("ItemImage").GetComponent<Image>().sprite = item.icon;

        // ���� �гο����� StatusText ����
        obj.transform.Find("StatusText").gameObject.SetActive(false);

        // Ŭ�� �� ���� ��ư Ȱ��ȭ
        obj.GetComponent<Button>().onClick.AddListener(() =>
        {
            selectedItem = item;
            equipButton.interactable = false;
            unequipButton.interactable = true;
            Debug.Log($"[Warehouse] ���� �г� ������ {item.EquipmentitemName} ���õ� (���� ����)");
        });

        return obj;
    }
}
