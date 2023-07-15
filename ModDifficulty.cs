using Microsoft.Xna.Framework;
using MonoMod.Utils;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Defiance;

public abstract class ModDifficulty : ModType {
    public abstract Color TextColor { get; protected set; }

    public abstract string IconTexture { get; protected set; }
    public abstract string BunnyTexture { get; protected set; }
    public abstract string BackgroundTexture { get; protected set; }
    
    public bool Enabled { get; internal set; }

    protected sealed override void Register() {
        ModTypeLookup<ModDifficulty>.Register(this);

        var name = GetType().Name;

        Language.GetOrRegister(Mod.GetLocalizationKey($"Difficulties.{name}.DisplayName"), () => name.SpacedPascalCase());
        Language.GetOrRegister(Mod.GetLocalizationKey($"Difficulties.{name}.Description"), () => name.SpacedPascalCase());
    }
}

public sealed class TestDifficulty : ModDifficulty {
    public override Color TextColor { get; protected set; } = Color.Pink;

    public override string IconTexture { get; protected set; } = "Images/UI/WorldCreation/IconDifficultyExpert";
    public override string BunnyTexture { get; protected set; } = "Images/UI/WorldCreation/PreviewDifficultyExpert2";
    public override string BackgroundTexture { get; protected set; } = "Images/UI/WorldCreation/PreviewDifficultyExpert1";
}