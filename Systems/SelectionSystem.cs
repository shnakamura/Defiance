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
        MonoModHooks.Modify(typeof(AWorldListItem).GetMethod("GetDifficulty", Flags), GetDifficulty_Patch);        
    }

    private static void GetDifficulty_Patch(ILContext il) {
        var c = new ILCursor(il);
        
        if (!c.TryGotoNext(i => i.MatchSwitch(out _)) || !c.TryGotoNext(i => i.MatchSwitch(out _))) {
            return;
        }

        c.Index++;
        
        c.Emit(OpCodes.Ldarg, 1);
        c.Emit(OpCodes.Ldarg, 2);
        
        c.Emit(OpCodes.Ldarg, 0);
        c.Emit(OpCodes.Ldfld, typeof(AWorldListItem).GetField("_data", Flags));


        
        c.EmitDelegate(delegate(ref string expertText, ref Color gameModeColor, WorldFileData _data) {
            ModContent.GetInstance<Defiance>().Logger.Debug("ILLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL");
            
            if (_data.GameMode < ModDifficultyLoader.VanillaDifficultyCount || _data.ForTheWorthy) {
                return;
            }
            
            expertText = ModDifficultyLoader.Get(_data.GameMode).DisplayName.Value;
            gameModeColor = ModDifficultyLoader.Get(_data.GameMode).TextColor;
        });
    }
}
