using UnityEngine;

namespace _Development.Scripts.LootLevel
{
    public static class RandomElements
    {
        public static int RandomInt(int min, int max) =>
            Random.Range(min, max);

        public static int RandomInt(int max) =>
            Random.Range(0, max);
    }
}