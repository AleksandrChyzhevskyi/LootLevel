using _Development.Scripts.Extensions;
using _Development.Scripts.Roulette;
using BLINK.RPGBuilder.Characters;
using BLINK.RPGBuilder.Data;
using BLINK.RPGBuilder.LogicMono;
using BLINK.RPGBuilder.Managers;
using BLINK.RPGBuilder.World;
using JetBrains.Annotations;
using UnityEngine;

namespace _Development.Scripts.LootLevel
{
    public class RewardCalculator : IRewardCalculator
    {
        private readonly CoroutineRunnerElements _coroutineRunner;
        private Coroutine _coroutine;

        public RewardCalculator() =>
            _coroutineRunner = RPGBuilderEssentials.Instance.RunnerElements;

        public void PutInWorld(RPGLootTable lootItems, Transform transform) =>
            Reward(lootItems, transform);

        public void PutInWorldWithMultiplier(RPGLootTable lootItems, Transform transform, int quantity) =>
            Reward(lootItems, transform, quantity);

        public void PutInInventory(RPGLootTable.LOOT_ITEMS lootItem, int winnerPrize = default)
        {
            if (winnerPrize == default)
                winnerPrize = GetCountSpawnItems(lootItem);

            RPGItem itemType = GameDatabase.Instance.GetItems()[lootItem.itemID];

            if (itemType.ItemType.ItemTypeFunction == EconomyData.ItemTypeFunction.Currency)
            {
                RPGCurrency currencyReward = GetCurrencyReward(itemType.entryDisplayName);

                int countCurrency = Character.Instance.getCurrencyAmount(currencyReward) + winnerPrize;

                if (currencyReward.ID == (int)CurrencyID.Crystal)
                {
                    EconomyUtilities.setCurrencyAmount(currencyReward, countCurrency);
                    GeneralEvents.Instance.OnPlayerCurrencyChanged(currencyReward);
                    return;
                }

                if (_coroutineRunner.IsStartCoroutine) 
                    _coroutineRunner.StopCoroutineValue(_coroutine);

                _coroutine = _coroutineRunner.StartUpdateCurrency(currencyReward, countCurrency);
                EconomyUtilities.setCurrencyAmount(currencyReward, countCurrency);
            }
            else
                HandleItemLooting(winnerPrize, itemType);
        }

        public int GetCountWithMultiplier(RouletteSettings rouletteSettings, RPGLootTable lootItems)
        {
            int levelPlayer = Character.Instance.CharacterData.Level;
            int playerRewardMultiplier = 1;

            foreach (var PlayerRewardMultiplier in rouletteSettings.PlayerRewardMultipliers)
            {
                if ((int)PlayerRewardMultiplier.Level == levelPlayer)
                    playerRewardMultiplier = PlayerRewardMultiplier.Multiplier;
            }

            return GetCountSpawnItems(lootItems.GetLootItem()) * playerRewardMultiplier;
        }

        private void GetItemInInventory(int itemID, RPGLootTable.LOOT_ITEMS lootItem, Transform transform,
            int count = 0)
        {
            if (count == 0)
                count = GetCountSpawnItems(lootItem);

            RPGItem itemREF = GameDatabase.Instance.GetItems()[itemID];
            if (itemREF.dropInWorld && itemREF.itemWorldModel != null)
                GetItemInWorld(transform, count, itemREF);
            else
                HandleItemLooting(count, itemREF);
        }

        private static void GetItemInWorld(Transform transform, int count, RPGItem itemREF)
        {
            EconomyData.WorldDroppedItemEntry newLoot = new EconomyData.WorldDroppedItemEntry
                { item = itemREF, count = count };
            GameObject newLootGO = GameEvents.Instance.InstantiateGameobject(
                itemREF.itemWorldModel, new Vector3(
                    transform.position.x,
                    transform.position.y + 1, transform.position.z), Quaternion.identity);
            newLootGO.layer = itemREF.worldInteractableLayer;
            newLoot.worldDroppedItemREF = newLootGO.AddComponent<WorldDroppedItem>();
            newLoot.worldDroppedItemREF.curLifetime = 0;
            newLoot.worldDroppedItemREF.maxDuration = itemREF.durationInWorld;
            newLoot.worldDroppedItemREF.item = itemREF;

            newLoot.itemDataID =
                RPGBuilderUtilities.HandleNewItemDATA(itemREF.ID,
                    CharacterEntries.ItemEntryState.InWorld);

            newLoot.worldDroppedItemREF.InitPhysics();
            GameState.allWorldDroppedItems.Add(newLoot);
        }

        private void HandleItemLooting(int count, RPGItem itemREF)
        {
            RPGBuilderUtilities.HandleItemLooting(itemREF.ID,
                RPGBuilderUtilities.HandleNewItemDATA(itemREF.ID,
                    CharacterEntries.ItemEntryState.InWorld), count, false,
                true);
        }

        private void Reward(RPGLootTable lootItems, Transform transform, int count = 0)
        {
            if (count == 0)
                count = GetCountSpawnItems(lootItems.GetLootItem());

            RPGLootTable.LOOT_ITEMS lootItem = lootItems.GetLootItem();
            GetItemInInventory(lootItem.itemID, lootItem, transform, count);
        }

        private RPGCurrency GetCurrencyReward(string entryName)
        {
            RPGCurrency currencyReward = new RPGCurrency();

            foreach (RPGCurrency rpgCurrency in GameDatabase.Instance.GetCurrencies().Values)
                if (rpgCurrency.entryDisplayName == entryName)
                    currencyReward = rpgCurrency;
            return currencyReward;
        }

        private int GetCountSpawnItems(RPGLootTable.LOOT_ITEMS lootItems) =>
            lootItems.min == lootItems.max
                ? lootItems.min
                : RandomElements.RandomInt(lootItems.min, lootItems.max);
    }
}