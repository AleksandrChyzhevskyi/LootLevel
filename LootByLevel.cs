using System.Collections.Generic;
using UnityEngine;

namespace _Development.Scripts.LootLevel
{
    [CreateAssetMenu(fileName = "LootByLevel", menuName = "Loot/LootByLevel")]
    public class LootByLevel : ScriptableObject
    {
        public List<LootTableByLevel> LootByLevels;
    }
}