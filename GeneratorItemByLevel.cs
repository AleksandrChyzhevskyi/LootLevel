using BLINK.RPGBuilder.Characters;
using System.Collections;
using UnityEngine;

namespace _Development.Scripts.LootLevel
{
    public class GeneratorItemByLevel : MonoBehaviour
    {
        [SerializeField] private LootByLevel _lootByLevel;

        private RPGLootTable _lootItems;
        private IRewardCalculator _rewardCalculator;

        public void Reward()
        {
            if(_rewardCalculator == null)
            {
                _rewardCalculator = new RewardCalculator();
            }
            CheckLevel();
            _rewardCalculator.PutInWorld(_lootItems, transform);
        }

        private void CheckLevel()
        {
            int levelPlayer = Character.Instance.CharacterData.Level;

            foreach (var level in _lootByLevel.LootByLevels)
            {
                if (level.MinRewardLevel > levelPlayer || level.MaxRewardLevel < levelPlayer)
                    continue;

                _lootItems = level.LootForLevel;
            }
        }
    }
}