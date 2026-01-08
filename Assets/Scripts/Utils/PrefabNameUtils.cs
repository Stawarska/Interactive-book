#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    public static class PrefabNameUtils
    {
        public static List<string> GetPrefabNames(string folderPath)
        {
            var guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
            return guids.Select(guid => Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(guid))).ToList();
        }

        public static bool TryGetIndexFromName(string pageName, out int index)
        {
            var match = Regex.Match(pageName, "^" + Regex.Escape(Consts.PageNamePrefix) + @"(\d+)$");
            
            if (match.Success && int.TryParse(match.Groups[1].Value, out index)) 
                return true;
            
            Debug.LogWarning("Wrong page name my friend. Can't get index from " + pageName);
            index = -1;
            return false;
        }
    }
}
#endif