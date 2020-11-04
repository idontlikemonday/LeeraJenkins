using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;

namespace LeeraJenkins.Common.Helpers
{
    public static class ResourceHelper
    {
        public static IDictionary<string, string> GetAllResourcesEntries(ResourceManager resourceManager)
        {
            var resourceSet = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            var dict = new Dictionary<string, string>();
            foreach (DictionaryEntry entry in resourceSet)
            {
                dict.Add(entry.Key.ToString(), entry.Value.ToString());
            }

            return dict
                .OrderBy(d => d.Key)
                .ToDictionary(d => d.Key, d => d.Value);
        }

        public static IEnumerable<string> GetAllResourcesValues(ResourceManager resourceManager)
        {
            var dict = GetAllResourcesEntries(resourceManager);
            return dict.Select(d => d.Value);
        }
    }
}
