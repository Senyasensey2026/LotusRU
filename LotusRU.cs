using System.Reflection;
using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers.Server;
using SPTarkov.Server.Core.Models.Spt.Tables;

namespace LotusRU;

/// <summary>
/// Runs after Lotus and WTT-CommonLib have registered their texts
/// and replaces the Russian ones with this translation.
/// </summary>
[Injectable(TypePriority = OnLoadOrder.PostLoad + 10)]
public class LotusRussianLocale(
    ISptLogger<LotusRussianLocale> logger,
    ModHelper modHelper,
    LocaleTable localeTable,
    TemplateTable templateTable
) : IOnLoad
{
    private const string Lang = "ru";

    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        var modPath = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        var localesPath = Path.Combine(modPath, "locales");

        var texts = modHelper.GetJsonDataFromFile<Dictionary<string, string>>(localesPath, "ru.json");
        var lines = modHelper.GetJsonDataFromFile<Dictionary<string, string>>(localesPath, "dialogues_ru.json");

        // Quests, trader name/description and keycard
        if (localeTable.Global.TryGetValue(Lang, out var ruLocale))
        {
            ruLocale.AddTransformer(data =>
            {
                if (data is null)
                {
                    return data;
                }

                foreach (var (key, value) in texts)
                {
                    data[key] = value;
                }

                return data;
            });
        }
        else
        {
            logger.Warning("[Lotus RU] Russian locale not found, quest texts were not applied");
        }

        // BTR driver dialogues added by Lotus
        var dialoguesTranslated = 0;
        foreach (var element in templateTable.Dialogue.Elements)
        {
            if (!element.LocalizationDictionary.TryGetValue("en", out var en))
            {
                continue;
            }

            var ru = new Dictionary<string, string>();
            foreach (var id in en.Keys)
            {
                if (lines.TryGetValue(id, out var line))
                {
                    ru[id] = line;
                }
            }

            if (ru.Count == 0)
            {
                continue;
            }

            if (element.LocalizationDictionary.TryGetValue(Lang, out var existing))
            {
                foreach (var (id, line) in ru)
                {
                    existing[id] = line;
                }
            }
            else
            {
                element.LocalizationDictionary[Lang] = ru;
            }

            dialoguesTranslated++;
        }

        logger.Success($"[Lotus RU] Loaded {texts.Count} texts and {dialoguesTranslated} dialogues");
        return Task.CompletedTask;
    }
}
