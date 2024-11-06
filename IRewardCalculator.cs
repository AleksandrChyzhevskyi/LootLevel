using _Development.Scripts.Roulette;
using UnityEngine;

namespace _Development.Scripts.LootLevel
{
    public interface IRewardCalculator
    {
        void PutInWorld(RPGLootTable lootItems, Transform transform);
        void PutInWorldWithMultiplier(RPGLootTable lootItems, Transform transform, int count = 0);
        int GetCountWithMultiplier(RouletteSettings rouletteSettings, RPGLootTable lootItems);
        void PutInInventory(RPGLootTable.LOOT_ITEMS lootItem, int winnerPrize = default);
    }
}