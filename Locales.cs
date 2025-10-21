using SPTarkov.DI.Annotations;
using SPTarkov.Common.Semver.Implementations;
using SPTarkov.Server.Core;
using SPTarkov.Server.Core.Utils.Json;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Services.Mod;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace tarkovhdrework
{

    public class NewLocales(
            ISptLogger<TarkovHDRework> logger,
            DatabaseService databaseService,
            LocaleService localeService,
            ServerLocalisationService serverLocalisationService)
    {
        public void EditLocales()
        {
            if (databaseService.GetLocales().Global.TryGetValue("en", out var lazyloadedValue))
            {
                // We have to add a transformer here, because locales are lazy loaded due to them taking up huge space in memory
                // The transformer will make sure that each time the locales are requested, the ones changed or added below are included
                lazyloadedValue.AddTransformer(lazyloadedLocaleData =>
                {
                    //lazyloadedLocaleData["Attention! This is a Beta version of Escape from Tarkov for testing purposes."] = "It's Porkin Time!";

                    //Graphics Card
                    //lazyloadedLocaleData["57347ca924597744596b4e71 Name"] = "RTX4090";


                    //lazyloadedLocaleData.Add("TestingLocales", "Testing Locales");


                    return lazyloadedLocaleData;
                });

                logger.Success("Added a custom locale to the database");
            }

            var _locales = localeService.GetLocaleDb("en");
            // Log this so we can see it in the console
            logger.Info(_locales["TestingLocales"]);

            // Log by the locale key and output the language the player has set
            // If the locale isn't found, it tries english
            // If english isn't found, it shows the key
            logger.Info(serverLocalisationService.GetText("TestingLocales"));

            logger.Info(_locales["Attention! This is a Beta version of Escape from Tarkov for testing purposes."]);
        }
    }
}