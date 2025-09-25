//using System.Collections.Generic;
//using UnityEngine;

//public class GameQuestManager : MonoBehaviour
//{
//    public static GameQuestManager Instance;

//    [System.Serializable]
//    public class Quest
//    {
//        public int questId;
//        public string questName;
//        public string requiredItem;
//        public int requiredAmount;
//        public int goldReward;
//        public QuestStatus status;
//    }

//    public enum QuestStatus
//    {
//        Available,
//        Accepted,
//        Completed,
//        Claimed
//    }

//    public List<Quest> quests = new List<Quest>();

//    void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//            InitializeQuests();
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    void InitializeQuests()
//    {
//        quests = new List<Quest>
//        {
//            new Quest { questId = 1, questName = "��� ��� 1", requiredItem = "����� ����", requiredAmount = 30, goldReward = 400, status = QuestStatus.Available },
//            new Quest { questId = 2, questName = "��� ��� 2", requiredItem = "����� ����", requiredAmount = 70, goldReward = 700, status = QuestStatus.Available },
//            new Quest { questId = 3, questName = "�� óġ 1", requiredItem = "���� ����", requiredAmount = 50, goldReward = 1400, status = QuestStatus.Available },
//            new Quest { questId = 4, questName = "�� óġ 2", requiredItem = "���� ����", requiredAmount = 70, goldReward = 2000, status = QuestStatus.Available },
//            new Quest { questId = 5, questName = "ȭ������ ��� 1", requiredItem = "ȭ������", requiredAmount = 50, goldReward = 2500, status = QuestStatus.Available },
//            new Quest { questId = 6, questName = "ȭ������ ��� 2", requiredItem = "ȭ������", requiredAmount = 80, goldReward = 3200, status = QuestStatus.Available },
//            new Quest { questId = 7, questName = "�������� ��� 1", requiredItem = "��������", requiredAmount = 50, goldReward = 4000, status = QuestStatus.Available },
//            new Quest { questId = 8, questName = "�������� ��� 2", requiredItem = "��������", requiredAmount = 50, goldReward = 4500, status = QuestStatus.Available },
//            new Quest { questId = 9, questName = "���渶���� ��� 1", requiredItem = "������ ��", requiredAmount = 50, goldReward = 5000, status = QuestStatus.Available },
//            new Quest { questId = 10, questName = "���渶���� ��� 2", requiredItem = "������ ��", requiredAmount = 100, goldReward = 5500, status = QuestStatus.Available }
//        };
//    }

//    public void AcceptQuest(int questId)
//    {
//        Quest quest = quests.Find(q => q.questId == questId);
//        if (quest != null && quest.status == QuestStatus.Available)
//        {
//            quest.status = QuestStatus.Accepted;
//        }
//    }

//    public void CompleteQuest(int questId)
//    {
//        Quest quest = quests.Find(q => q.questId == questId);
//        if (quest != null && quest.status == QuestStatus.Accepted)
//        {
//            PlayerInventory inventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();

//            if (inventory.HasItem(quest.requiredItem, quest.requiredAmount))
//            {
//                inventory.RemoveItem(quest.requiredItem, quest.requiredAmount);
//                GameManager.Instance.AddGold(quest.goldReward);
//                quest.status = QuestStatus.Completed;

//                Debug.Log($"����Ʈ �Ϸ�: {quest.questName}, ����: {quest.goldReward}���");
//            }
//        }
//    }

//    public bool CanCompleteQuest(int questId)
//    {
//        Quest quest = quests.Find(q => q.questId == questId);
//        if (quest == null || quest.status != QuestStatus.Accepted) return false;

//        PlayerInventory inventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
//        return inventory.HasItem(quest.requiredItem, quest.requiredAmount);
//    }
//}