using UnityEngine;

namespace _Development.Scripts.LootLevel
{
    [CreateAssetMenu(fileName = "LootTableByLevel", menuName = "Loot/LootTableByLevel")]
    public class LootTableByLevel : ScriptableObject
    {
        [Range(0, 10)] public int MinRewardLevel;
        [Range(0, 10)] public int MaxRewardLevel;
        public RPGLootTable LootForLevel;
    }
}