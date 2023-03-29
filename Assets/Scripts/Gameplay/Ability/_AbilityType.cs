namespace Gameplay.Ability
{
    public enum _AbilityType
    {
        Unset = 0,
        
        SpeedUp = 1,
        Slowness = 2,
        Rocket = 3,
    }

    public static class AbilityTypeExtension
    {
        public static string SpawnKey(this _AbilityType type)
        {
            return $"ability_{type}".ToLower();
        }
    }
}