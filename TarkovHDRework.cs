using SPTarkov.DI.Annotations;
using SPTarkov.Common.Semver.Implementations;
using SPTarkov.Server.Core;
using SPTarkov.Server.Core.Controllers;
using SPTarkov.Server.Core.Utils.Json;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Dialog;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Services.Mod;
using SPTarkov.Server.Core.Utils;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace tarkovhdrework
{
    public record ModMetadata : AbstractModMetadata
    {
        public override string ModGuid { get; init; } = "tarkov.hd.rework";
        public override string Name { get; init; } = "TarkovHDRework";
        public override string Author { get; init; } = "PulledP0rk";
        public override List<string>? Contributors { get; init; }
        public override SemanticVersioning.Version Version { get; init; } = new("0.4.4");
        public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0");


        public override List<string>? Incompatibilities { get; init; }
        public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
        public override string? Url { get; init; }
        public override bool? IsBundleMod { get; init; } = true;
        public override string? License { get; init; } = "CC BY-NC-ND 4.0"; 

    }

    // Check `OnLoadOrder` for list of possible choices
    [Injectable(TypePriority = OnLoadOrder.PostDBModLoader)]
    public class TarkovHDRework(
        ISptLogger<TarkovHDRework> logger,
        ModHelper modHelper,
        DatabaseService databaseService,
        LocaleService localeService,
        ServerLocalisationService serverLocalisationService)
        : IOnLoad
    {
        public Task OnLoad()
        {
            var databases = databaseService.GetTables();
            var pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());

            var assetReplacements = modHelper.GetJsonDataFromFile<Dictionary<string, string>>(System.IO.Path.Combine(pathToMod, "db"), "assetReplacements.json");
            var itemMappings = modHelper.GetJsonDataFromFile<Dictionary<string, string>>(System.IO.Path.Combine(pathToMod, "db"), "items.json");

            logger.Success($"Mod loaded after database!");

            // Create an instance of NewLocales and call EditLocales
            var newLocales = new NewLocales(logger, databaseService, localeService, serverLocalisationService);
            newLocales.EditLocales();


            var items = databaseService.GetItems();
            int updatedCount = 0;

            foreach (var (itemName, itemId) in itemMappings)
            {
                if (!items.TryGetValue(itemId, out var item)) continue;
                if (!assetReplacements.TryGetValue(itemName, out var newBundlePath)) continue;
                var properties = item.Properties;
                if (properties == null) continue;
                var itemPrefab = properties.Prefab;
                if (itemPrefab == null) continue;
                var prefabPath = itemPrefab.Path;
                if (prefabPath == null) continue;
                itemPrefab.Path = newBundlePath;
                updatedCount++;

                logger.Debug($"Updated {itemName} ({itemId}): {itemPrefab.Path} -> {newBundlePath}");
            }

            logger.Success($"Asset replacement complete! Updated {updatedCount} item bundle paths.");

            return Task.CompletedTask;

        }
    }

}



//var propsProperty = item.GetType().GetProperty("Props");
