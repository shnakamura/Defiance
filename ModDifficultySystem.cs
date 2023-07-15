using System.IO;
using Terraria.ModLoader;

namespace Defiance; 

internal sealed class ModDifficultySystem : ModSystem {
    public override void NetSend(BinaryWriter writer) {
        foreach (var difficulty in ModContent.GetContent<ModDifficulty>()) {
            writer.Write(difficulty.Enabled);
        }
    }

    public override void NetReceive(BinaryReader reader) {
        foreach (var difficulty in ModContent.GetContent<ModDifficulty>()) {
            difficulty.Enabled = reader.ReadBoolean();
        }
    }
}
