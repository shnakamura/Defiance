using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Defiance;

public abstract class ModDifficulty : ModType, ILocalizedModType {
    public GameModeData Data { get; private set; }

    public virtual LocalizedText DisplayName => this.GetLocalization(nameof(DisplayName), () => "Difficulty");
    public virtual LocalizedText Description => this.GetLocalization(nameof(Description), () => "Difficulty's Description");

    public virtual Color TextColor => Color.White;

    public virtual string IconTexture => "Terraria/Images/UI/WorldCreation/IconDifficultyExpert";
    public virtual string BunnyTexture => "Terraria/Images/UI/WorldCreation/PreviewDifficultyExpert2";
    public virtual string BackgroundTexture => "Terraria/Images/UI/WorldCreation/PreviewDifficultyExpert1";

    public virtual bool IsExpertMode => false;
    public virtual bool IsMasterMode => false;
    public virtual bool IsJourneyMode => false;

    public virtual float EnemyMaxLifeMultiplier => 1f;
    public virtual float EnemyDamageMultiplier => 1f;
    public virtual float DebuffTimeMultiplier => 1f;
    public virtual float KnockbackToEnemiesMultiplier => 1f;
    public virtual float TownNPCDamageMultiplier => 1f;
    public virtual float EnemyDefenseMultiplier => 1f;
    public virtual float EnemyMoneyDropMultiplier => 1f;

    public string LocalizationCategory => "Difficulties";

    protected sealed override void Register() {
        var id = ModDifficultyLoader.Add(this);

        Data = new GameModeData {
            Id = id,
            IsExpertMode = IsExpertMode,
            IsMasterMode = IsMasterMode,
            IsJourneyMode = IsJourneyMode,
            EnemyMaxLifeMultiplier = EnemyMaxLifeMultiplier,
            EnemyDamageMultiplier = EnemyDamageMultiplier,
            DebuffTimeMultiplier = DebuffTimeMultiplier,
            KnockbackToEnemiesMultiplier = KnockbackToEnemiesMultiplier,
            TownNPCDamageMultiplier = TownNPCDamageMultiplier,
            EnemyDefenseMultiplier = EnemyDefenseMultiplier,
            EnemyMoneyDropMultiplier = EnemyMoneyDropMultiplier
        };

        Main.RegisteredGameModes.TryAdd(id, Data);
    }
}
