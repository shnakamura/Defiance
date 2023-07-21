using Terraria.ModLoader;

namespace Defiance.Tests;

public class TestDifficulty : ModDifficulty {
    public override string BackgroundTexture { get; } = "Defiance/Tests/TestBackground";

    public override string IconTexture { get; } = "Defiance/Tests/TestIcon";

    public override string BunnyTexture { get; } = "Defiance/Tests/TestBunny";
}
