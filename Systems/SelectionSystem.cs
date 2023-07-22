using System.Reflection;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.GameContent.UI.Elements;
using Terraria.IO;
using Terraria.ModLoader;

namespace Defiance.Systems;

internal sealed class SelectionSystem : ModSystem {
    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

    public override void OnModLoad() {
        On_AWorldListItem.GetDifficulty += GetDifficulty_Hook;
    }

    private static void GetDifficulty_Hook(On_AWorldListItem.orig_GetDifficulty orig, AWorldListItem self, out string expertText, out Color gameModeColor) {
        var type = typeof(AWorldListItem).GetField("_data", Flags);
        var value = (WorldFileData)type.GetValue(self);
        
        if (value.GameMode < ModDifficultyLoader.VanillaDifficultyCount || value.ForTheWorthy) {
            orig(self, out expertText, out gameModeColor);
            return;
        }

        expertText = ModDifficultyLoader.Get(value.GameMode).DisplayName.Value;
        gameModeColor = ModDifficultyLoader.Get(value.GameMode).TextColor;
    }
}
