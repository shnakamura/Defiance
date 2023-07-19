﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Defiance;

public class B : ModSystem{
    public override void PostUpdateEverything() {
        Main.NewText(Main.ActiveWorldFileData.GameMode);
    }
}

public sealed class Defiance : Mod {
    public static int DifficultyCount => ModContent.GetContent<ModDifficulty>().Count();

    public override void Load() {
        if (DifficultyCount <= 0) {
            Logger.Info("No custom difficulties were added.");
            return;
        }
        
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;

        MonoModHooks.Modify(typeof(UIWorldCreation).GetMethod("AddWorldDifficultyOptions", flags), Add_Patch);
        MonoModHooks.Modify(typeof(UIWorldCreation).GetMethod("MakeInfoMenu", flags), Make_Patch);
        
        On_UIWorldCreation.SetupGamepadPoints += (orig, self, batch) => {  };
        On_UIWorldCreation.FinishCreatingWorld += (orig, self) => {
            orig(self);

            var field = typeof(UIWorldCreation).GetField("_optionDifficulty", flags);
            var value = (int)field.GetValue(self);

            if (value > 3) {
                Main.GameMode = value;
            }
        };
        
        On_UIWorldSelect.NewWorldClick += (orig, self, evt, element) => { orig(self, evt, element); };
    }


    private static void Make_Patch(ILContext il) {
        var c = new ILCursor(il);

        c.GotoNext(i => i.MatchNewobj(typeof(UIElement).GetConstructor(Type.EmptyTypes)));
        c.Index++;

        c.Emit(OpCodes.Pop).Emit(OpCodes.Newobj, typeof(UIGrid).GetConstructor(Type.EmptyTypes));
        c.Emit(OpCodes.Dup);
        
        c.EmitDelegate(delegate(UIGrid self) {
            self.ListPadding = 0f;
        });
    }

    private static void Add_Patch(ILContext il) {
        var c = new ILCursor(il);

        c.GotoNext(i => i.MatchStloc(0));
        c.Index++;

        c.Emit(OpCodes.Ldloca, 0);
        c.EmitDelegate(delegate(ref Array array) {
            var enumType = array.GetType().GetElementType();
            var newArray = Array.CreateInstance(enumType, array.Length + DifficultyCount);

            newArray.SetValue(Enum.ToObject(enumType, 3), 0);
            newArray.SetValue(Enum.ToObject(enumType, 0), 1);
            newArray.SetValue(Enum.ToObject(enumType, 1), 2);
            newArray.SetValue(Enum.ToObject(enumType, 2), 3);

            for (var i = 4; i < array.Length + DifficultyCount; i++) {
                newArray.SetValue(Enum.ToObject(enumType, i), i);
            }

            array = newArray;
        });

        c.GotoNext(i => i.MatchStloc(1));
        c.Index++;

        c.Emit(OpCodes.Ldloca, 1);
        c.EmitDelegate(delegate(ref LocalizedText[] array) {
            var list = new List<LocalizedText>(array);

            foreach (var difficulty in ModContent.GetContent<ModDifficulty>()) {
                list.Add(Language.GetText(difficulty.Mod.GetLocalizationKey($"Difficulties.{difficulty.GetType().Name}.DisplayName")));
            }

            array = list.ToArray();
        });

        c.GotoNext(i => i.MatchStloc(2));
        c.Index++;

        c.Emit(OpCodes.Ldloca, 2);
        c.EmitDelegate(delegate(ref LocalizedText[] array) {
            var list = new List<LocalizedText>(array);

            foreach (var difficulty in ModContent.GetContent<ModDifficulty>()) {
                list.Add(Language.GetText(difficulty.Mod.GetLocalizationKey($"Difficulties.{difficulty.GetType().Name}.Description")));
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

            foreach (var difficulty in ModContent.GetContent<ModDifficulty>()) {
                list.Add(difficulty.IconTexture);
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