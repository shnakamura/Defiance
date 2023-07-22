using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Defiance;

public abstract class ModDifficulty : ModType, ILocalizedModType {
    public virtual LocalizedText DisplayName => this.GetLocalization(nameof(DisplayName), () => "Difficulty");
    public virtual LocalizedText Description => this.GetLocalization(nameof(Description), () => "Difficulty's Description");

    public virtual Color TextColor { get; } = Color.White;
    
    public string LocalizationCategory { get; } = "Difficulties";
    
    public virtual string IconTexture { get; } = "Terraria/Images/UI/WorldCreation/IconDifficultyExpert";
    public virtual string BunnyTexture { get; } = "Terraria/Images/UI/WorldCreation/PreviewDifficultyExpert2";
    public virtual string BackgroundTexture { get; } = "Terraria/Images/UI/WorldCreation/PreviewDifficultyExpert1";
    
    public GameModeData Data { get; private set; }

    public virtual bool IsExpertMode { get;} = false;
    public virtual bool IsMasterMode { get; } = false;
    public virtual bool IsJourneyMode { get; } = false;

    public virtual float EnemyMaxLifeMultiplier { get; } = 1f;
    public virtual float EnemyDamageMultiplier { get; } = 1f;
    public virtual float DebuffTimeMultiplier { get; } = 1f;
    public virtual float KnockbackToEnemiesMultiplier { get; } = 1f;
    public virtual float TownNPCDamageMultiplier { get; } = 1f;
    public virtual float EnemyDefenseMultiplier { get; } = 1f;
    public virtual float EnemyMoneyDropMultiplier { get; } = 1f;

    protected sealed override void Register() {
        var id = ModDifficultyLoader.Register(this);

        Data = new GameModeData {
            Id = id,
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
