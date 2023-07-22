using System;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Defiance.Systems;

internal sealed class CreationSystem : ModSystem {
    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

    public override void OnModLoad() {
        MonoModHooks.Modify(typeof(UIWorldCreation).GetMethod("AddWorldDifficultyOptions", Flags), AddWorldDifficultyOptions_Patch);
        MonoModHooks.Modify(typeof(UIWorldCreation).GetMethod("MakeInfoMenu", Flags), MakeInfoMenu_Patch);
        MonoModHooks.Modify(typeof(UIWorldCreation).GetMethod("FinishCreatingWorld", Flags), FinishCreatingWorld_Patch);

        MonoModHooks.Modify(typeof(UIWorldCreationPreview).GetMethod("DrawSelf", Flags), SelfDraw_Patch);
        
        On_UIWorldSelect.NewWorldClick += (orig, self, evt, element) => { orig(self, evt, element); };

        // TODO: Fix unknown issue with UI snap points.
        On_UIWorldCreation.SetupGamepadPoints += (orig, self, batch) => { };
    }

    private static void FinishCreatingWorld_Patch(ILContext il) {
        var c = new ILCursor(il);

        // Set Main.GameMode
        if (!c.TryGotoNext(i => i.MatchSwitch(out _)) || !c.TryGotoNext(i => i.MatchSwitch(out _))) {
            return;
        }

        c.Index++;
        
        c.Emit(OpCodes.Ldarg, 0);
        c.Emit(OpCodes.Ldfld, typeof(UIWorldCreation).GetField("_optionDifficulty", Flags));

        c.EmitDelegate(delegate(byte _optionDifficulty) {
            if (_optionDifficulty < ModDifficultyLoader.VanillaDifficultyCount) {
                return;
            }

            Main.GameMode = _optionDifficulty;
        });
    }

    private static void SelfDraw_Patch(ILContext il) {
        var c = new ILCursor(il);

        // Custom background
        if (!c.TryGotoNext(i => i.MatchSwitch(out _))) {
            return;
        }

        c.Index++;

        c.Emit(OpCodes.Ldloc, 1);
        c.Emit(OpCodes.Ldloc, 2);
        
        c.Emit(OpCodes.Ldarg, 0);
        c.Emit(OpCodes.Ldfld, typeof(UIWorldCreationPreview).GetField("_difficulty", Flags));
        
        c.Emit(OpCodes.Ldarg, 1);
        
        c.EmitDelegate(delegate(Vector2 position, Color color, byte difficulty, SpriteBatch spriteBatch) {
            spriteBatch.Draw(ModContent.Request<Texture2D>(ModDifficultyLoader.Get(difficulty).BackgroundTexture).Value, position, color);
        });
        
        // Custom bunny
        if (!c.TryGotoNext(i => i.MatchSwitch(out _)) || !c.TryGotoNext(i => i.MatchSwitch(out _)) || !c.TryGotoNext(i => i.MatchSwitch(out _))) {
            return;
        }

        c.Index++;

        c.Emit(OpCodes.Ldloc, 1);
        c.Emit(OpCodes.Ldloc, 2);
        
        c.Emit(OpCodes.Ldarg, 0);
        c.Emit(OpCodes.Ldfld, typeof(UIWorldCreationPreview).GetField("_difficulty", Flags));
        
        c.Emit(OpCodes.Ldarg, 1);
        
        c.EmitDelegate(delegate(Vector2 position, Color color, byte difficulty, SpriteBatch spriteBatch) {
            spriteBatch.Draw(ModContent.Request<Texture2D>(ModDifficultyLoader.Get(difficulty).BunnyTexture).Value, position, color);
        });
    }

    private static void MakeInfoMenu_Patch(ILContext il) {
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

    private static void AddWorldDifficultyOptions_Patch(ILContext il) {
        var c = new ILCursor(il);

        // Options
        if (!c.TryGotoNext(i => i.MatchStloc(0))) {
            return;
        }

        c.Index++;

        c.Emit(OpCodes.Ldloca, 0);
        c.EmitDelegate(delegate(ref Array array) {
            var enumType = array.GetType().GetElementType();
            var newArray = Array.CreateInstance(enumType, array.Length + ModDifficultyLoader.ModdedDifficultyCount);

            newArray.SetValue(Enum.ToObject(enumType, 3), 0);
            newArray.SetValue(Enum.ToObject(enumType, 0), 1);
            newArray.SetValue(Enum.ToObject(enumType, 1), 2);
            newArray.SetValue(Enum.ToObject(enumType, 2), 3);

            for (var i = ModDifficultyLoader.VanillaDifficultyCount; i < array.Length + ModDifficultyLoader.ModdedDifficultyCount; i++) {
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
            array = array.Concat(Enumerable.Repeat("Images/UI/WorldCreation/IconDifficultyNormal", range.Count())).ToArray();
        });

        if (!c.TryGotoNext(i => i.MatchStloc(7))) {
            return;
        }

        c.Index++;
        
        c.Emit(OpCodes.Ldloc, 7);
        
        c.Emit(OpCodes.Ldflda, typeof(GroupOptionButton<>).MakeGenericType(typeof(UIWorldCreation).GetNestedType("WorldDifficultyId", Flags)).GetField("_iconTexture", Flags));
        
        c.Emit(OpCodes.Ldloc, 6);

        c.EmitDelegate(delegate(ref Asset<Texture2D> iconTexture, int id) {
            if (id < ModDifficultyLoader.VanillaDifficultyCount) {
                return;
            }

            iconTexture = ModContent.Request<Texture2D>(ModDifficultyLoader.Get(id).IconTexture);
        });

        // Replaces .Append() calls for .Add()
        if (!c.TryGotoNext(i => i.MatchCallOrCallvirt(typeof(UIElement).GetMethod("Append")))) {
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
