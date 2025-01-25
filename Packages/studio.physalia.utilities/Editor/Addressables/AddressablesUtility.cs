using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;

namespace Physalia
{
    public static class AddressablesUtility
    {
        private static readonly Logger.Label Label = Logger.Label.CreateFromCurrentClassEditor();

        public static bool BuildAddressables()
        {
            AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);

            bool success = string.IsNullOrEmpty(result.Error);
            if (!success)
            {
                Logger.Error(Label, $"Addressables build error encountered: {result.Error}");
            }
            return success;
        }

        public static void CleanAddressables()
        {
            AddressableAssetSettings.CleanPlayerContent();
        }

        public static AddressableAssetGroupTemplate GetGroupTemplate(int index)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (index < 0 || index >= settings.GroupTemplateObjects.Count)
            {
                Logger.Error(Label, $"Cannot find the template with index: {index}");
                return null;
            }

            var template = settings.GetGroupTemplateObject(index) as AddressableAssetGroupTemplate;
            return template;
        }

        public static AddressableAssetGroup GetDefaultGroup()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup group = settings.DefaultGroup;
            if (group == null)
            {
                Logger.Error(Label, "Cannot find the default group!");
                return null;
            }

            return group;
        }

        public static AddressableAssetGroup GetGroup(string groupName)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup group = settings.FindGroup(groupName);
            if (group == null)
            {
                Logger.Error(Label, $"Cannot find the group with name: {groupName}");
                return null;
            }

            return group;
        }

        public static AddressableAssetGroup FindOrCreateGroup(string groupName, AddressableAssetGroupTemplate template = null, bool postEvent = true)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup group = settings.FindGroup(groupName);
            if (group == null)
            {
                if (template != null)
                {
                    group = settings.CreateGroup(groupName, false, false, postEvent, template.SchemaObjects);
                }
                else
                {
                    group = settings.CreateGroup(groupName, false, false, postEvent, null);
                }
            }

            return group;
        }

        public static void RemoveGroup(string groupName)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup group = settings.FindGroup(groupName);
            if (group == null)
            {
                Logger.Error(Label, $"Remove group failed! Cannot find the group with name: {groupName}");
                return;
            }

            settings.RemoveGroup(group);
        }

        public static AddressableAssetEntry CreateOrMoveEntry(this AddressableAssetGroup group, string assetPath, string address, bool postEvent = true)
        {
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            AddressableAssetEntry entry = group.Settings.CreateOrMoveEntry(guid, group, false, false);
            if (entry == null)
            {
                Logger.Error(Label, $"Create entry failed! Path: {assetPath}");
                return null;
            }

            entry.SetAddress(address, postEvent);
            return entry;
        }

        public static void RemoveEntry(this AddressableAssetGroup group, string assetPath, bool postEvent = true)
        {
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            bool success = group.Settings.RemoveAssetEntry(guid, postEvent);
            if (!success)
            {
                Logger.Error(Label, $"Remove entry failed! AssetPath: {assetPath}");
                return;
            }
        }

        public static void AddLabel(this AddressableAssetEntry entry, string label)
        {
            entry.SetLabel(label, true, true);
        }

        public static void RemoveLabel(this AddressableAssetEntry entry, string label)
        {
            entry.SetLabel(label, false);
        }

        public static void SortGroup(List<string> rule)
        {
            List<string> names = GetGroupNames();
            Dictionary<string, List<string>> map = new();
            List<string> other = new();
            List<string> filter = new(rule);
            filter.Sort(delegate (string x, string y)
            {
                return x.Length.CompareTo(y.Length);
            });

            foreach (var item in rule)
            {
                map.Add(item, new List<string>());
            }

            foreach (var name in names)
            {
                if (TryGetSortKey(filter, name, out string key))
                {
                    map[key].Add(name);
                }
                else
                {
                    other.Add(name);
                }
            }

            List<string> sol = new();
            foreach (var key in rule)
            {
                map[key].Sort();
                foreach (var name in map[key])
                {
                    sol.Add(name);
                }
            }

            other.Sort();
            foreach (var name in other)
            {
                sol.Add(name);
            }

            AssignGroupsByNameOrder(sol);
        }

        private static List<string> GetGroupNames()
        {
            var groups = AddressableAssetSettingsDefaultObject.Settings.groups;

            List<string> names = new();
            foreach (var group in groups)
            {
                names.Add(group.name);
            }

            return names;
        }

        private static void AssignGroupsByNameOrder(List<string> names)
        {
            var groups = AddressableAssetSettingsDefaultObject.Settings.groups;

            if (names.Count != groups.Count)
            {
                Logger.Error(Label, $"names.Count : {names.Count} != groups.Count : {groups.Count}");
                return;
            }

            Dictionary<string, AddressableAssetGroup> map = new();
            for (int i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                map.Add(group.name, group);
            }

            int index = 0;
            foreach (var name in names)
            {
                groups[index] = map[name];
                ++index;
            }

            EditorUtility.SetDirty(AddressableAssetSettingsDefaultObject.Settings);
        }

        private static bool TryGetSortKey(List<string> keys, string name, out string result)
        {
            result = "";
            foreach (var key in keys)
            {
                if (name.StartsWith(key))
                {
                    result = key;
                    return true;
                }
            }
            return false;
        }
    }
}
