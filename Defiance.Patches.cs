using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Defiance;

public sealed partial class Defiance : Mod {
    public override void Load() {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;

        MonoModHooks.Modify(typeof(UIWorldCreation).GetMethod("AddWorldDifficultyOptions", flags), Add_Patch);
        MonoModHooks.Modify(typeof(UIWorldCreation).GetMethod("MakeInfoMenu", flags), Make_Patch);

        On_UIWorldCreation.SetupGamepadPoints += (orig, self, batch) => {  };

        On_UIWorldSelect.NewWorldClick += (orig, self, evt, element) => { orig(self, evt, element); };
    }

    protected void Make_Patch(ILContext il) {
        var c = new ILCursor(il);

        c.GotoNext(i => i.MatchNewobj(typeof(UIElement).GetConstructor(Type.EmptyTypes)));
        c.Index++;

        c.Emit(OpCodes.Pop).Emit(OpCodes.Newobj, typeof(UIGrid).GetConstructor(Type.EmptyTypes));
        c.Emit(OpCodes.Dup);
        
        c.EmitDelegate(delegate(UIGrid self) { self.ListPadding = 0f; });
    }

    protected void Add_Patch(ILContext il) {
        var c = new ILCursor(il);

        c.GotoNext(i => i.MatchStloc(0));
        c.Index++;

        c.Emit(OpCodes.Ldloca, 0);
        c.EmitDelegate(delegate(ref Array array) {
            var enumType = array.GetType().GetElementType();
            var newArray = Array.CreateInstance(enumType, array.Length + DifficultyCount);

            for (var i = 0; i < array.Length + DifficultyCount; i++) {
                newArray.SetValue(Enum.ToObject(enumType, i), i);
            }

            array = newArray;
        });

        c.GotoNext(i => i.MatchStloc(1));
        c.Index++;

        c.Emit(OpCodes.Ldloca, 1);
        c.EmitDelegate(delegate(ref LocalizedText[] array) {
            var list = new List<LocalizedText>(array);

            foreach (var modDifficulty in ModContent.GetContent<ModDifficulty>()) {
                list.Add(Language.GetText(GetLocalizationKey($"Difficulties.{modDifficulty.GetType().Name}.DisplayName")));
            }

            array = list.ToArray();
        });

        c.GotoNext(i => i.MatchStloc(2));
        c.Index++;

        c.Emit(OpCodes.Ldloca, 2);
        c.EmitDelegate(delegate(ref LocalizedText[] array) {
            var list = new List<LocalizedText>(array);

            foreach (var modDifficulty in ModContent.GetContent<ModDifficulty>()) {
                list.Add(Language.GetText(GetLocalizationKey($"Difficulties.{modDifficulty.GetType().Name}.Description")));
            }

            array = list.ToArray();
        });

        c.GotoNext(i => i.MatchStloc(3));
        c.Index++;

        c.Emit(OpCodes.Ldloca, 3);
        c.EmitDelegate(delegate(ref Color[] array) {
            var list = new List<Color>(array);

            foreach (var modDifficulty in ModContent.GetContent<ModDifficulty>()) {
                list.Add(modDifficulty.TextColor);
            }

            array = list.ToArray();
        });

        c.GotoNext(i => i.MatchStloc(4));
        c.Index++;

        c.Emit(OpCodes.Ldloca, 4);
        c.EmitDelegate(delegate(ref string[] array) {
            var list = new List<string>(array);

            foreach (var modDifficulty in ModContent.GetContent<ModDifficulty>()) {
                list.Add(modDifficulty.IconTexture);
            }

            array = list.ToArray();
        });

        c.GotoNext(i => i.MatchCallvirt(typeof(UIElement).GetMethod("Append", BindingFlags.Public | BindingFlags.Instance)));
        c.Remove();

        c.EmitDelegate(delegate(UIElement self, UIElement element) {
            if (self is UIGrid grid) {
                grid.Add(element);

                element.Width = StyleDimension.FromPercent(0.25f);

                element.HAlign = 0f;
                element.VAlign = 0.5f;
            }
        });
    }
}
