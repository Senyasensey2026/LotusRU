using SPTarkov.Server.Core.Models.Spt.Mod;
using Range = SemanticVersioning.Range;
using Version = SemanticVersioning.Version;

namespace LotusRU;

public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.senyasensey2026.lotusru";
    public string Name { get; init; } = "Lotus RU";
    public string Author { get; init; } = "Senyasensey2026";
    public List<string>? Contributors { get; init; } = ["Praxideke"];
    public Version Version { get; init; } = new("1.8.1");
    public Range SptVersion { get; init; } = new("~4.1.6");
    public List<string>? Incompatibilities { get; init; } = null;
    public Dictionary<string, Range>? ModDependencies { get; init; } = new()
    {
        { "com.Luna.LunnayalunaLotus", new Range("~1.8.1") }
    };
    public string? Url { get; init; } = "https://github.com/Senyasensey2026/LotusRU";
    public string License { get; init; } = "MIT";
    public bool HasPrepatcher { get; init; } = false;
}
