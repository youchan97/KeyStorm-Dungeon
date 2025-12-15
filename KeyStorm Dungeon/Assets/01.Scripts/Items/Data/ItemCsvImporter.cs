#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class ItemCsvImporter
{
    // Settings 에셋 이름을 "정확히" 이걸로 만들 것
    private const string PassiveSettingsName = "ItemCsvImportSettings_Passive";
    private const string ActiveSettingsName = "ItemCsvImportSettings_Active";

    // -------------------------
    // 메뉴 2개 (패시브 / 액티브)
    // -------------------------
    [MenuItem("Tools/Items/Import Passive ItemData from CSV")]
    public static void ImportPassive()
    {
        ImportInternal(PassiveSettingsName);
    }

    [MenuItem("Tools/Items/Import Active ItemData from CSV")]
    public static void ImportActive()
    {
        ImportInternal(ActiveSettingsName);
    }

    // -------------------------
    // 공통 Import 로직
    // -------------------------
    private static void ImportInternal(string settingsAssetName)
    {
        var settings = FindSettings(settingsAssetName);
        if (settings == null)
        {
            Debug.LogError($"[ItemCsvImporter] {settingsAssetName}.asset 을 찾지 못했어. " +
                           $"Create > Data > Item CSV Import Settings 로 만들고, 이름을 정확히 '{settingsAssetName}'로 바꿔줘.");
            return;
        }

        if (settings.csvFile == null)
        {
            Debug.LogError($"[ItemCsvImporter] '{settingsAssetName}' Settings에 csvFile(TextAsset) 연결이 안 돼 있어.");
            return;
        }

        // 출력 폴더 준비
        string outFolder = $"Assets/Resources/{settings.resourcesFolder}";
        if (!AssetDatabase.IsValidFolder(outFolder))
        {
            Directory.CreateDirectory(outFolder);
            AssetDatabase.Refresh();
        }

        // CSV/TSV 파싱 (따옴표 CSV 지원)
        var rows = ParseTable(settings.csvFile.text);
        if (rows.Count <= 1)
        {
            Debug.LogError("[ItemCsvImporter] CSV에서 유효한 데이터 행을 못 찾았어(헤더만 있거나 비어있음).");
            return;
        }

        // 헤더 맵
        var header = rows[0];
        var headerMap = BuildHeaderMap(header);

        // 최소 필수: Name, ID, DropRoom, Tier 정도면 충분
        RequireColumn(headerMap, "Name");
        RequireColumn(headerMap, "ID");
        RequireColumn(headerMap, "Tier");
        RequireColumn(headerMap, "DropRoom");

        int created = 0, updated = 0, skipped = 0;
        var createdIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (int i = 1; i < rows.Count; i++)
        {
            var r = rows[i];
            if (r.All(string.IsNullOrWhiteSpace)) continue;

            string itemName = Get(r, headerMap, "Name");
            string itemId = Get(r, headerMap, "ID");

            if (string.IsNullOrWhiteSpace(itemId))
            {
                Debug.LogWarning($"[ItemCsvImporter] {i + 1}행: ID가 비어있어 스킵");
                skipped++;
                continue;
            }

            createdIds.Add(itemId);

            string safeName = MakeSafeFileName($"{itemId}_{itemName}");
            string assetPath = $"{outFolder}/{safeName}.asset";

            ItemData asset = AssetDatabase.LoadAssetAtPath<ItemData>(assetPath);
            bool isNew = false;

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<ItemData>();
                AssetDatabase.CreateAsset(asset, assetPath);
                isNew = true;
            }
            else if (!settings.overwriteExisting)
            {
                skipped++;
                continue;
            }

            ApplyRowToItemData(asset, r, headerMap, settings);

            EditorUtility.SetDirty(asset);
            if (isNew) created++; else updated++;
        }

        // CSV에 없는 에셋 삭제 옵션
        if (settings.clearMissingAssets)
        {
            var allAssets = AssetDatabase.FindAssets("t:ItemData", new[] { outFolder })
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .ToList();

            foreach (var path in allAssets)
            {
                var a = AssetDatabase.LoadAssetAtPath<ItemData>(path);
                if (a == null) continue;
                if (string.IsNullOrEmpty(a.itemId)) continue;

                if (!createdIds.Contains(a.itemId))
                {
                    AssetDatabase.DeleteAsset(path);
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[ItemCsvImporter] '{settingsAssetName}' 완료! created:{created}, updated:{updated}, skipped:{skipped} / out:{outFolder}");
    }

    // -------------------------
    // Row -> ItemData 매핑
    // -------------------------
    private static void ApplyRowToItemData(ItemData data, List<string> r, Dictionary<string, int> m, ItemCsvImportSettings settings)
    {
        data.itemId = Get(r, m, "ID");
        data.itemName = Get(r, m, "Name");

        // (옵션) 없으면 빈값
        data.description = GetOrDefault(r, m, "Description", "");

        // 액티브/패시브는 Settings로 강제 가능
        //  - 액티브 CSV에 IsActiveItem 컬럼이 없으면 settings.defaultIsActiveItem 사용
        data.isActiveItem = settings.forceIsActiveItem
            ? settings.defaultIsActiveItem
            : GetBoolOrDefault(r, m, "IsActiveItem", settings.defaultIsActiveItem);

        // AttackChange (없으면 false)
        data.attackChange = GetBoolOrDefault(r, m, "AttackChange", false);

        // Tier (없으면 Settings 기본값)
        data.tier = GetEnumOrDefault<ItemTier>(r, m, "Tier", settings.defaultTier);

        // DropRoom (Flags 파싱)
        // 이제 CSV에서 "Treasure, Boss" 처럼 쓰려면 셀을 따옴표로 감싸서 "Treasure, Boss" 로 넣으면 됨
        string dropRoomRaw = Get(r, m, "DropRoom");
        data.dropRoom = ParseDropRooms(dropRoomRaw);

        // 스탯들(패시브 CSV에 주로 존재, 액티브 CSV엔 없을 수 있음)
        data.maxHp = GetIntOrDefault(r, m, "MaxHP", 0);
        data.moveSpeed = GetFloatOrDefault(r, m, "MoveSpeed", 0f);
        data.damage = GetFloatOrDefault(r, m, "Damage", 0f);
        data.specialDamageMultiple = GetFloatOrDefault(r, m, "SpecialDamageMultiple", 0f);
        data.damageMultiple = GetFloatOrDefault(r, m, "DamageMultiple", 0f);
        data.attackSpeed = GetFloatOrDefault(r, m, "AttackSpeed", 0f);
        data.attackSpeedMultiple = GetFloatOrDefault(r, m, "AttackSpeedMultiple", 0f);
        data.range = GetFloatOrDefault(r, m, "Range", 0f);
        data.shotSpeed = GetFloatOrDefault(r, m, "ShotSpeed", 0f);
        data.maxAmmo = GetIntOrDefault(r, m, "MaxAmmo", 0);
        data.useAmmo = GetIntOrDefault(r, m, "UseAmmo", 0);
        data.scale = GetFloatOrDefault(r, m, "Scale", 0f);

        // 액티브 쿨다운: 네 표는 "CoolDown" 컬럼
        // 혹시 다른 표는 "CooldownMax"일 수도 있어서 둘 다 지원
        float cd = GetFloatOrDefault(r, m, "CoolDown", float.NaN);
        if (float.IsNaN(cd))
            cd = GetFloatOrDefault(r, m, "CooldownMax", 0f);

        data.cooldownMax = cd;

        // cooldownType은 CSV에 없으면 기본 None (현재 동작 유지)
        // 필요하면 아래 한 줄을 취향대로 바꿔서 액티브 기본값을 줄 수도 있음:
        // data.cooldownType = data.isActiveItem ? ActiveCooldownType.PerRoom : ActiveCooldownType.None;
        data.cooldownType = GetEnumOrDefault<ActiveCooldownType>(r, m, "CooldownType", ActiveCooldownType.None);
    }

    // -------------------------
    // DropRoom Flags parse (Treasure, Store, Boss 등)
    // -------------------------
    private static ItemDropRoom ParseDropRooms(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return ItemDropRoom.None;

        ItemDropRoom result = ItemDropRoom.None;

        // "Treasure, Store, Boss" / "Treasure|Shop" / "Treasure;Boss" 모두 지원
        // ★ CSV에서 콤마를 쓸 땐 반드시 셀을 따옴표로 감싸기: "Treasure, Boss"
        var parts = s.Split(new[] { ',', '|', ';' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var raw in parts)
        {
            var token = raw.Trim();

            // Store/Shop 토큰 보정(둘 중 뭐든 받아줌)
            if (token.Equals("Store", StringComparison.OrdinalIgnoreCase))
            {
                if (!Enum.TryParse<ItemDropRoom>("Store", true, out _))
                    token = "Shop";
            }
            else if (token.Equals("Shop", StringComparison.OrdinalIgnoreCase))
            {
                if (!Enum.TryParse<ItemDropRoom>("Shop", true, out _))
                    token = "Store";
            }

            if (Enum.TryParse<ItemDropRoom>(token, true, out var flag))
                result |= flag;
            else
                Debug.LogWarning($"[ItemCsvImporter] DropRoom 파싱 실패: '{token}' (원본:'{s}'). CSV라면 여러 값은 \"Treasure, Boss\" 처럼 따옴표로 감싸거나 Treasure|Boss 사용 추천.");
        }

        return result;
    }

    // -------------------------
    // CSV/TSV parsing (따옴표 CSV 지원)
    // -------------------------
    private static List<List<string>> ParseTable(string text)
    {
        var lines = text.Replace("\r\n", "\n").Replace("\r", "\n")
            .Split('\n')
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToList();

        if (lines.Count == 0) return new List<List<string>>();

        char delimiter = DetectDelimiter(lines);

        var table = new List<List<string>>();
        foreach (var line in lines)
        {
            // ★ 핵심: 따옴표(") 안의 delimiter는 무시
            var cells = SplitLineRespectQuotes(line, delimiter);
            table.Add(cells);
        }
        return table;
    }

    private static char DetectDelimiter(List<string> lines)
    {
        // 탭이 많으면 TSV, 아니면 CSV
        int tabCount = lines[0].Count(c => c == '\t');
        return tabCount >= 2 ? '\t' : ',';
    }

    private static List<string> SplitLineRespectQuotes(string line, char delimiter)
    {
        var result = new List<string>();
        if (line == null) return result;

        var sb = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                // "" -> " (CSV escape)
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    sb.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
                continue;
            }

            if (c == delimiter && !inQuotes)
            {
                result.Add(sb.ToString().Trim());
                sb.Clear();
                continue;
            }

            sb.Append(c);
        }

        result.Add(sb.ToString().Trim());
        return result;
    }

    // -------------------------
    // Header / Get helpers
    // -------------------------
    private static Dictionary<string, int> BuildHeaderMap(List<string> header)
    {
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < header.Count; i++)
        {
            var key = header[i].Trim();
            if (string.IsNullOrWhiteSpace(key)) continue;
            if (!map.ContainsKey(key))
                map.Add(key, i);
        }
        return map;
    }

    private static void RequireColumn(Dictionary<string, int> map, string name)
    {
        if (!map.ContainsKey(name))
            Debug.LogWarning($"[ItemCsvImporter] 컬럼 '{name}' 이(가) 없음. (없어도 동작은 하지만 해당 값은 기본값 처리됨)");
    }

    private static string Get(List<string> r, Dictionary<string, int> m, string col)
    {
        if (!m.TryGetValue(col, out int idx)) return "";
        if (idx < 0 || idx >= r.Count) return "";
        return r[idx];
    }

    private static string GetOrDefault(List<string> r, Dictionary<string, int> m, string col, string def)
    {
        var v = Get(r, m, col);
        return string.IsNullOrWhiteSpace(v) ? def : v;
    }

    private static bool GetBoolOrDefault(List<string> r, Dictionary<string, int> m, string col, bool def)
    {
        var s = Get(r, m, col);
        if (string.IsNullOrWhiteSpace(s)) return def;

        s = s.Trim();
        if (s.Equals("TRUE", StringComparison.OrdinalIgnoreCase)) return true;
        if (s.Equals("FALSE", StringComparison.OrdinalIgnoreCase)) return false;
        if (s == "1") return true;
        if (s == "0") return false;

        return def;
    }

    private static int GetIntOrDefault(List<string> r, Dictionary<string, int> m, string col, int def)
    {
        var s = Get(r, m, col);
        if (string.IsNullOrWhiteSpace(s)) return def;

        if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v))
            return v;

        if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var f))
            return Mathf.RoundToInt(f);

        return def;
    }

    private static float GetFloatOrDefault(List<string> r, Dictionary<string, int> m, string col, float def)
    {
        var s = Get(r, m, col);
        if (string.IsNullOrWhiteSpace(s)) return def;

        // "0,2" 같은 한국식 소수점 보정
        s = s.Replace(",", ".");

        if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            return v;

        return def;
    }

    private static TEnum GetEnumOrDefault<TEnum>(List<string> r, Dictionary<string, int> m, string col, TEnum def)
        where TEnum : struct
    {
        var s = Get(r, m, col);
        if (string.IsNullOrWhiteSpace(s)) return def;

        if (Enum.TryParse<TEnum>(s.Trim(), true, out var v))
            return v;

        return def;
    }

    private static string MakeSafeFileName(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name.Trim();
    }

    private static ItemCsvImportSettings FindSettings(string settingsAssetName)
    {
        // 이름까지 포함해서 정확히 찾기
        var guids = AssetDatabase.FindAssets($"t:ItemCsvImportSettings {settingsAssetName}");
        if (guids == null || guids.Length == 0) return null;

        var path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<ItemCsvImportSettings>(path);
    }
}
#endif