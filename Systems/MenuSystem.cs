using System;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Defiance.Systems;

internal sealed class MenuSystem : ModSystem {
    public override void OnModLoad() {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var type = typeof(UIWorldCreation);

        MonoModHooks.Modify(type.GetMethod("AddWorldDifficultyOptions", flags), Add_Patch);
        MonoModHooks.Modify(type.GetMethod("MakeInfoMenu", flags), Menu_Patch);

        On_UIWorldSelect.NewWorldClick += (orig, self, evt, element) => { orig(self, evt, element); };

        // TODO: Fix unknown issue with UI snap points.
        On_UIWorldCreation.SetupGamepadPoints += (orig, self, batch) => { };
    }

    private static void Menu_Patch(ILContext il) {
        // TODO: Append the difficulty buttons to a new scrollable UIGrid instead of the base panel, then proceed to append that grid to the base panel instead.
        var c = new ILCursor(il);

        if (!c.TryGotoNext(i => i.MatchNewobj(typeof(UIElement).GetConstructor(Type.EmptyTypes)))) {
            return;
        }

        c.Index++;

        c.Emit(OpCodes.Pop);
        c.Emit(OpCodes.Newobj, typeof(UIGrid).GetConstructor(Type.EmptyTypes));
        c.Emit(OpCodes.Dup);

        c.EmitDelegate(delegate(UIGrid self) { self.ListPadding = 0f; });
    }

    private static void Add_Patch(ILContext il) {
        var c = new ILCursor(il);

        // Options
        if (!c.TryGotoNext(i => i.MatchStloc(0))) {
            return;
        }

        c.Index++;

        c.Emit(OpCodes.Ldloca, 0);
        c.EmitDelegate(delegate(ref Array array) {
            var enumType = array.GetType().GetElementType();
            var newArray = Array.CreateInstance(enumType, array.Length + DifficultyLoader.ModdedDifficultyCount);

            newArray.SetValue(Enum.ToObject(enumType, 3), 0);
            newArray.SetValue(Enum.ToObject(enumType, 0), 1);
            newArray.SetValue(Enum.ToObject(enumType, 1), 2);
            newArray.SetValue(Enum.ToObject(enumType, 2), 3);

            for (var i = 4; i < array.Length + DifficultyLoader.ModdedDifficultyCount; i++) {
                newArray.SetValue(Enum.ToObject(enumType, i), i);
            }

            array = newArray;
        });

        // Display Names
        if (!c.TryGotoNext(i => i.MatchStloc(1))) {
            return;
        }

        c.Index++;

        c.Emit(OpCodes.Ldloca, 1);
        c.EmitDelegate(delegate(ref LocalizedText[] array) {
            var range = ModContent.GetContent<ModDifficulty>().Select(x => x.DisplayName);
            array = array.Concat(range).ToArray();
        });

        // Descriptions
        if (!c.TryGotoNext(i => i.MatchStloc(2))) {
            return;
        }

        c.Index++;

        c.Emit(OpCodes.Ldloca, 2);
        c.EmitDelegate(delegate(ref LocalizedText[] array) {
            var range = ModContent.GetContent<ModDifficulty>().Select(x => x.Description);
            array = array.Concat(range).ToArray();
        });

        // Text Color
        if (!c.TryGotoNext(i => i.MatchStloc(3))) {
            return;
        }

        c.Index++;

        c.Emit(OpCodes.Ldloca, 3);
        c.EmitDelegate(delegate(ref Color[] array) {
            var range = ModContent.GetContent<ModDifficulty>().Select(x => x.TextColor);
            array = array.Concat(range).ToArray();
        });

        // Icons
        if (!c.TryGotoNext(i => i.MatchStloc(4))) {
            return;
        }

        c.Index++;

        c.Emit(OpCodes.Ldloca, 4);
        c.EmitDelegate(delegate(ref string[] array) {
            var range = ModContent.GetContent<ModDifficulty>().Select(x => x.IconTexture);
            array = array.Concat(range).ToArray();
        });

        // Replaces .Append() calls for .Add()
        if (!c.TryGotoNext(i => i.MatchCallvirt(typeof(UIElement).GetMethod("Append")))) {
            return;
        }

        c.Remove();
        c.EmitDelegate(delegate(UIElement self, UIElement element) {
            if (self is not UIGrid grid) {
                return;
            }

            element.Width = StyleDimension.FromPercent(0.25f);
            element.HAlign = 0f;
            element.VAlign = 0.5f;

            grid.Add(element);
        });
    }
}
