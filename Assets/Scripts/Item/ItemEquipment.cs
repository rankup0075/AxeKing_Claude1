//using Unity.VisualScripting;
//using UnityEngine;

//[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
//public class ItemEquipment 
//{
//    [Header("Equipment Stats")]
//    public EquipmentSlot equipmentSlot = EquipmentSlot.Weapon;
//    public int attackPower = 0;
//    public int healthBonus = 0;

//    [Header("Requirements")]
//    public int levelRequired = 1;




//    public string EquipmentitemName;
//    public ShopUI.ItemType Equipmenttype;
//    public int EquipmentstatBonus;
//    public Sprite icon;

//    public enum EquipmentSlot
//    {
//        Weapon,      // ����
//        Armor       // ��
//    }

//    void Awake()
//    {


//    }

//    // ��� ��� (����)
//    //public override bool UseItem(GameObject user)
//    //{
//    //    PlayerInventory inventory = user.GetComponent<PlayerInventory>();
//    //    if (inventory != null)
//    //    {
//    //        return inventory.EquipItem(this);
//    //    }
//    //    return false;
//    //}

//    // ��� ���� �ؽ�Ʈ (�������̵�)

//    //string GetSlotString()
//    //{
//    //    switch (equipmentSlot)
//    //    {
//    //        case EquipmentSlot.Weapon: return "����";
//    //        case EquipmentSlot.Armor: return "��";
//    //        default: return "�� �� ����";
//    //    }
//    //}

//    //// ��� �ļ� ó��
//    //void OnEquipmentBroken()
//    //{
//    //    Debug.Log($"{itemName}��(��) �ļյǾ����ϴ�!");
//    //    // �ļ� �� ȿ�� ���ҳ� �ٸ� ó�� ����
//    //}


//    // ��� �ɷ�ġ ����
//    public void ApplyStats(PlayerController player, PlayerHealth health)
//    {
//        if (Equipmenttype == ShopUI.ItemType.Weapon)
//        {
//            player.attackDamage += EquipmentstatBonus;
//        }
//        else if (Equipmenttype == ShopUI.ItemType.Armor)
//        {
//            health.IncreaseMaxHealth(EquipmentstatBonus);
//        }
//    }

//    // ���� �� �ɷ�ġ ����
//    public void RemoveStats(PlayerController player, PlayerHealth health)
//    {
//        if (Equipmenttype == ShopUI.ItemType.Weapon)
//        {
//            player.attackDamage -= EquipmentstatBonus;
//        }
//        else if (Equipmenttype == ShopUI.ItemType.Armor)
//        {
//            health.IncreaseMaxHealth(-EquipmentstatBonus);
//        }
//    }

//    //void ApplySpecialEffects(PlayerController player)
//    //{
//    //    if (specialEffects == null) return;

//    //    foreach (var effect in specialEffects)
//    //    {
//    //        // Ư�� ȿ�� ���� ���� (���� Ȯ��)
//    //        switch (effect.effectType)
//    //        {
//    //            case EquipmentEffect.EffectType.AttackSpeedBonus:
//    //                // ���ݼӵ� ���� ����
//    //                break;
//    //            case EquipmentEffect.EffectType.MovementSpeedBonus:
//    //                // �̵��ӵ� ���� ����
//    //                break;
//    //                // �ٸ� ȿ���鵵 �߰� ����
//    //        }
//    //    }
//    //}

//    //void RemoveSpecialEffects(PlayerController player)
//    //{
//    //    // ApplySpecialEffects�� �������� ȿ�� ����
//    //}
//}

using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class ItemEquipment
{
    public string EquipmentitemName;
    public ShopUI.ItemType Equipmenttype;
    public int EquipmentstatBonus;
    public Sprite icon;

    public enum EquipmentSlot { Weapon, Armor }

    public void ApplyStats(PlayerController player, PlayerHealth health)
    {
        if (Equipmenttype == ShopUI.ItemType.Weapon)
        {
            player.attackDamage += EquipmentstatBonus;
        }
        else if (Equipmenttype == ShopUI.ItemType.Armor)
        {
            health.IncreaseMaxHealth(EquipmentstatBonus);
        }
    }

    public void RemoveStats(PlayerController player, PlayerHealth health)
    {
        if (Equipmenttype == ShopUI.ItemType.Weapon)
        {
            player.attackDamage -= EquipmentstatBonus;
        }
        else if (Equipmenttype == ShopUI.ItemType.Armor)
        {
            health.IncreaseMaxHealth(-EquipmentstatBonus);
        }
    }
}

