namespace GUPS.AntiCheat.Protected.Prefs
{
    /// <summary>
    /// Enum over all save and load able PlayerPrefs types.
    /// </summary>
    public enum EPlayerPrefsType : byte
    {
        UNKNOWN = 0,
        BOOL = 1,
        INT = 2,
        FLOAT = 3,
        STRING = 4,
        VECTOR2 = 5,
        VECTOR3 = 6,
        VECTOR4 = 7,
        QUATERNION = 8,
    }
}
