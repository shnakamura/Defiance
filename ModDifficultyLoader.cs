using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Defiance;

public static class ModDifficultyLoader {
    public const int VanillaDifficultyCount = 4;

    internal static readonly List<ModDifficulty> Difficulties = new List<ModDifficulty>();

    public static int DifficultyCount { get; private set; } = VanillaDifficultyCount;

    public static int ModdedDifficultyCount => DifficultyCount - VanillaDifficultyCount;

    internal static int Add<T>(T difficulty) where T : ModDifficulty {
        ModTypeLookup<ModDifficulty>.Register(difficulty);

        Difficulties.Add(difficulty);
        DifficultyCount++;

        return DifficultyCount - 1;
    }

    public static ModDifficulty Get(int index) {
        return Difficulties[index - VanillaDifficultyCount];
    }

    public static bool IsEnabled<T>() where T : ModDifficulty {
        return Main.GameMode == ModContent.GetInstance<T>().Data.Id;
    }
}
