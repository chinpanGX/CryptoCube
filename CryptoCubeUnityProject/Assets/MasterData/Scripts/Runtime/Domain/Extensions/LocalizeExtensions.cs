using UnityEngine;

namespace MasterData.Runtime.Domain
{
    public static class LocalizeExtensions
    {
        public static string GetText(this LocalizationMasterDataTable table, string key, Language language)
        {
            var data = table.GetByKey(key);
            return language switch
            {
                Language.Japanese => data.Jp,
                Language.English => data.En,
                _ => data.Jp
            };
        }
    }
}
