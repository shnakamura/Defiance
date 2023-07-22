using Microsoft.Xna.Framework;

namespace Defiance.Tests;

public class TestDifficulty : ModDifficulty {
    public override string BackgroundTexture => "Defiance/Tests/TestBackground";
    public override string IconTexture => "Defiance/Tests/TestIcon";
    public override string BunnyTexture => "Defiance/Tests/TestBunny";

    public override bool IsJourneyMode => true;
    
    public override float EnemyMaxLifeMultiplier => 50f;

    public override Color TextColor => Color.Green;
}
