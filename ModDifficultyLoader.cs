using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Defiance;

/// <summary>
///     Provides centralized difficulty loading and handling.
/// </summary>
public static class ModDifficultyLoader {
    /// <summary>
    ///     The current amount of vanilla difficulties.
    /// </summary>
    public const int VanillaDifficultyCount = 4;

    /// <summary>
    ///     The current amount of vanilla and modded difficulties.
    /// </summary>
    public static int DifficultyCount { get; private set; } = VanillaDifficultyCount;

    /// <summary>
    ///     The current amount of modded difficulties;
    /// </summary>
    public static int ModdedDifficultyCount => DifficultyCount - VanillaDifficultyCount;

    internal static List<ModDifficulty> Difficulties { get; set; } = new List<ModDifficulty>();

    internal static int Register<T>(T difficulty) where T : ModDifficulty {
        ModTypeLookup<ModDifficulty>.Register(difficulty);

        difficulty.Id = DifficultyCount;
        Difficulties.Add(difficulty);
        DifficultyCount++;

        return difficulty.Id;
    }

    /// <summary>
    ///     Attempts to retrieve a modded difficulty from the given index.
    /// </summary>
    public static ModDifficulty Get(int index) {
        return Difficulties[index - VanillaDifficultyCount];
    }

    /// <summary>
    ///     Returns whether a difficulty is enabled or not in the current active world.
    /// </summary>
    public static bool IsEnabled<T>() where T : ModDifficulty {
        return Main.GameMode == ModContent.GetInstance<T>().Id;
    }
}
