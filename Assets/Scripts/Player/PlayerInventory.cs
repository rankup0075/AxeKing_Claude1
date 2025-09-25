
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Potions")]
    public int smallPotions = 0;
    public int mediumPotions = 0;
    public int largePotions = 0;

    [Header("Equipment")]
    public ItemEquipment currentWeapon;
    public ItemEquipment currentArmor;

    [Header("Items")]
    public Dictionary<string, int> items = new Dictionary<string, int>();

    public List<ItemEquipment> weaponStorage = new List<ItemEquipment>();
    public List<ItemEquipment> armorStorage = new List<ItemEquipment>();

    private PlayerHealth playerHealth;
    private PlayerController playerController;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerController = GetComponent<PlayerController>();

        InitializeItems();
        UpdateUI();
    }
    //void Awake()
    //{
    //    DontDestroyOnLoad(gameObject);
    //}


    void InitializeItems()
    {
        items["����� ����"] = 0;
        items["���� ����"] = 0;
        items["ȭ������"] = 0;
        items["��������"] = 0;
        items["������ ��"] = 0;
    }

    public void AddEquipment(string itemName, ShopUI.ItemType type, int statBonus, Sprite icon = null)
    {
        ItemEquipment newEquipment = new ItemEquipment
        {
            EquipmentitemName = itemName,
            Equipmenttype = type,
            EquipmentstatBonus = statBonus,
            icon = icon
        };

        if (type == ShopUI.ItemType.Weapon)
        {
            weaponStorage.Add(newEquipment);
            Debug.Log($"[Inventory] ���� {itemName} â�� �߰��� (�� {weaponStorage.Count}�� ����)");
        }
        else if (type == ShopUI.ItemType.Armor)
        {
            armorStorage.Add(newEquipment);
            Debug.Log($"[Inventory] �� {itemName} â�� �߰��� (�� {armorStorage.Count}�� ����)");
        }
    }

    public void EquipItem(ItemEquipment equipment, ShopUI.ItemType type)
    {
        if (type == ShopUI.ItemType.Weapon)
        {
            if (currentWeapon != null)
                currentWeapon.RemoveStats(playerController, playerHealth);

            currentWeapon = equipment;

            if (equipment != null)
                equipment.ApplyStats(playerController, playerHealth);
        }
        else if (type == ShopUI.ItemType.Armor)
        {
            if (currentArmor != null)
                currentArmor.RemoveStats(playerController, playerHealth);

            currentArmor = equipment;

            if (equipment != null)
                equipment.ApplyStats(playerController, playerHealth);
        }
    }

    public void UsePotion(int potionType)
    {
        if (playerHealth.CurrentHealth >= playerHealth.MaxHealth) return;

        switch (potionType)
        {
            case 0: if (smallPotions > 0) { smallPotions--; playerHealth.Heal(30); } break;
            case 1: if (mediumPotions > 0) { mediumPotions--; playerHealth.Heal(50); } break;
            case 2: if (largePotions > 0) { largePotions--; playerHealth.Heal(playerHealth.MaxHealth); } break;
        }

        UpdateUI();
    }

    public void AddPotion(int potionType, int amount)
    {
        switch (potionType)
        {
            case 0: smallPotions += amount; break;
            case 1: mediumPotions += amount; break;
            case 2: largePotions += amount; break;
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        UIManager.Instance.UpdatePotionCount(smallPotions, mediumPotions, largePotions);
    }
}
