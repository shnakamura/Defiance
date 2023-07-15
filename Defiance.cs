using System.Linq;
using Terraria.ModLoader;

namespace Defiance;

public sealed partial class Defiance : Mod {
    internal static int DifficultyCount => ModContent.GetContent<ModDifficulty>().Count();

    public static bool IsEnabled<T>() where T : ModDifficulty {
        return ModContent.GetInstance<T>().Enabled;
    }

    public static bool IsDisabled<T>() where T : ModDifficulty {
        return !ModContent.GetInstance<T>().Enabled;
    }
}
