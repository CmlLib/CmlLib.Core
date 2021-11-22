using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace CmlLib.Core.MojangLauncher
{
    public class MojangLauncherAccounts
    {
        [JsonProperty("accounts")]
        public Dictionary<string, MojangAccount?>? Accounts { get; set; }

        [JsonProperty("ActiveAccountLocalId")]
        public string? ActiveAccountLocalId { get; set; }

        [JsonProperty("mojangClientToken")]
        public string? MojangClientToken { get; set; }

        public MojangAccount? GetActiveAccount()
        {
            if (string.IsNullOrEmpty(ActiveAccountLocalId))
                return null;

            MojangAccount? value = null;
            var result = Accounts?.TryGetValue(ActiveAccountLocalId, out value);
            if (result == null || result == false)
                return null;

            return value;
        }

        public void SaveTo(string path)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            File.WriteAllText(path, json);
        }

        public static MojangLauncherAccounts? FromDefaultPath()
        {
            var path = Path.Combine(MinecraftPath.GetOSDefaultPath(), "launcher_accounts.json");
            return FromFile(path);
        }

        public static MojangLauncherAccounts? FromFile(string path)
        {
            var content = File.ReadAllText(path);
            return FromJson(content);
        }

        public static MojangLauncherAccounts? FromJson(string json)
        {
            return JsonConvert.DeserializeObject<MojangLauncherAccounts>(json);
        }
    }
}
