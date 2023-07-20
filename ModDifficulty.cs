using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Defiance;

/// <summary>
///     Provides a custom difficulty mode for the world creation menu.
/// </summary>
public abstract class ModDifficulty : ModType, ILocalizedModType {
    /// <summary>
    ///     The internal identity of this difficulty mode.
    /// </summary>
    public int Id { get; internal set; }

    /// <summary>
    ///     The translations for the display name of this difficulty mode.
    /// </summary>
    public virtual LocalizedText DisplayName => this.GetLocalization(nameof(DisplayName), () => "Difficulty");

    /// <summary>
    ///     The translations for the description of this difficulty mode.
    /// </summary>
    public virtual LocalizedText Description => this.GetLocalization(nameof(Description), () => "Difficulty's Description");

    /// <summary>
    ///     The color of the display name text.
    /// </summary>
    public virtual Color TextColor { get; protected set; } = Color.White;

    /// <summary>
    ///     The path to the icon texture that will appear for this difficulty in the button selection menu.
    /// </summary>
    public virtual string IconTexture { get; protected set; } = "Images/UI/WorldCreation/IconDifficultyExpert";

    /// <summary>
    ///     The path to the bunny texture that will appear for this difficulty in the world creation preview frame.
    /// </summary>
    public virtual string BunnyTexture { get; protected set; } = "Images/UI/WorldCreation/PreviewDifficultyExpert2";

    /// <summary>
    ///     The path to the background texture that will appear for this difficulty in the world creation preview frame.
    /// </summary>
    public virtual string BackgroundTexture { get; protected set; } = "Images/UI/WorldCreation/PreviewDifficultyExpert1";

    public string LocalizationCategory { get; } = "Difficulties";

    protected sealed override void Register() {
        DifficultyLoader.Register(this);
    }

    public bool IsEnabled() {
        return Main.GameMode == Id;
    }
}
