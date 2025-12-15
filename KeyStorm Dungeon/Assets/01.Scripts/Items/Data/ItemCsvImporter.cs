#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class ItemCsvImporter
{
    private const string SettingsAssetName = "ItemCsvImportSettings";

    [MenuItem("Tools/Items/Import ItemData from CSV")]
    public static void Import()
    {
        var settings = FindSettings();
        if (settings == null)
        {
            Debug.LogError($"[ItemCsvImporter] {SettingsAssetName}.asset 을 찾지 못했어. " +
                           $"Create > Data > Item CSV Import Settings 로 먼저 만들어줘.");
            return;
        }

        if (settings.csvFile == null)
        {
            Debug.LogError("[ItemCsvImporter] Settings에 csvFile(TextAsset) 연결이 안 돼 있어.");
            return;
        }

        // 출력 폴더 준비
        string outFolder = $"Assets/Resources/{settings.resourcesFolder}";
        if (!AssetDatabase.IsValidFolder(outFolder))
        {
            Directory.CreateDirectory(outFolder);
            AssetDatabase.Refresh();
        }

        // CSV 파싱
        var rows = ParseTable(settings.csvFile.text);
        if (rows.Count == 0)
        {
            Debug.LogError("[ItemCsvImporter] CSV에서 유효한 데이터 행을 못 찾았어.");
            return;
        }

        // 헤더 맵
        var header = rows[0];
        var headerMap = BuildHeaderMap(header);

        // 필수 컬럼 체크 (네 표 기준)
        // Name, ID, AttackChange, DropRoom ... 나머지는 없어도 기본값으로 처리 가능
        RequireColumn(headerMap, "Name");
        RequireColumn(headerMap, "ID");
        RequireColumn(headerMap, "AttackChange");
        RequireColumn(headerMap, "DropRoom");

        // 생성/업데이트
        int created = 0, updated = 0, skipped = 0;
        var createdIds = new HashSet<string>();

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

            // 에셋 경로(이름은 안전하게 ID 기반)
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

            // 값 채우기
            ApplyRowToItemData(asset, r, headerMap, settings);

            EditorUtility.SetDirty(asset);
            if (isNew) created++; else updated++;
        }

        // CSV에 없는 에셋 삭제(옵션)
        if (settings.clearMissingAssets)
        {
            var allAssets = AssetDatabase.FindAssets("t:ItemData", new[] { outFolder })
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .ToList();

            foreach (var path in allAssets)
            {
                var a = AssetDatabase.LoadAssetAtPath<ItemData>(path);
                if (a == null) continue;
                if (!createdIds.Contains(a.itemId))
                {
                    AssetDatabase.DeleteAsset(path);
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[ItemCsvImporter] 완료! created:{created}, updated:{updated}, skipped:{skipped} / out:{outFolder}");
    }

    // -------------------------
    // Row -> ItemData
    // -------------------------
    private static void ApplyRowToItemData(ItemData data, List<string> r, Dictionary<string, int> m, ItemCsvImportSettings settings)
    {
        data.itemId = Get(r, m, "ID");
        data.itemName = Get(r, m, "Name");

        // (옵션) 설명 컬럼이 있으면 사용 (없으면 빈값)
        data.description = GetOrDefault(r, m, "Description", "");

        // 패시브 CSV라면 기본 false 강제(엑셀 수정 못 하니까 여기서 제어)
        data.isActiveItem = GetBoolOrDefault(r, m, "IsActiveItem", settings.defaultIsActiveItem);

        // AttackChange
        data.attackChange = GetBoolOrDefault(r, m, "AttackChange", false);

        // Tier (없으면 defaultTier)
        data.tier = GetEnumOrDefault<ItemTier>(r, m, "Tier", settings.defaultTier);

        // DropRoom (Flags 파싱 + Store->Shop 매핑)
        string dropRoomRaw = Get(r, m, "DropRoom");
        data.dropRoom = ParseDropRooms(dropRoomRaw);

        // 스탯들(컬럼 없으면 0으로)
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

        // 액티브 전용 쿨타임 (패시브 CSV면 보통 없음 → 기본값)
        data.cooldownType = GetEnumOrDefault<ActiveCooldownType>(r, m, "CooldownType", ActiveCooldownType.None);
        data.cooldownMax = GetFloatOrDefault(r, m, "CooldownMax", 0f);

        // (Value 컬럼은 ItemData에 필드가 없다면 여기서 무시)
        // 필요하면 ItemData에 public float value; 추가 후 아래 한 줄로 매핑:
        // data.value = GetFloatOrDefault(r, m, "Value", 0f);
    }

    // -------------------------
    // DropRoom Flags parse
    // -------------------------
    static ItemDropRoom ParseDropRooms(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return ItemDropRoom.None;

        ItemDropRoom result = ItemDropRoom.None;

        var parts = s.Split(new[] { ',', '|', ';' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var raw in parts)
        {
            var token = raw.Trim();

            if (Enum.TryParse<ItemDropRoom>(token, true, out var flag))
                result |= flag;
            else
                Debug.LogWarning($"DropRoom 파싱 실패: '{token}'");
        }

        return result;
    }



    // -------------------------
    // CSV parsing (tabs or commas)
    // -------------------------
    private static List<List<string>> ParseTable(string text)
    {
        var lines = text.Replace("\r\n", "\n").Replace("\r", "\n")
            .Split('\n')
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToList();

        if (lines.Count == 0) return new List<List<string>>();

        // 구분자 자동 추정: 헤더 라인에 탭이 많으면 탭, 아니면 쉼표
        char delimiter = lines[0].Count(c => c == '\t') >= 2 ? '\t' : ',';

        var table = new List<List<string>>();
        foreach (var line in lines)
        {
            // 단순 split (따옴표 포함 복잡 CSV는 여기선 생략. 너 표는 단순형이라 보통 OK)
            var cells = line.Split(delimiter).Select(c => c.Trim()).ToList();
            table.Add(cells);
        }
        return table;
    }

    static List<string> SplitCsvLine(string line)
    {
        var result = new List<string>();
        if (string.IsNullOrEmpty(line)) return result;

        bool inQuotes = false;
        var sb = new System.Text.StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    sb.Append('"'); // "" -> "
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
                continue;
            }

            if (c == ',' && !inQuotes)
            {
                result.Add(sb.ToString());
                sb.Clear();
                continue;
            }

            sb.Append(c);
        }

        result.Add(sb.ToString());
        return result;
    }

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

        // 1/0도 지원
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

        // "3.0" 같은 경우도 있을 수 있어 float로 파싱 후 캐스팅
        if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var f))
            return Mathf.RoundToInt(f);

        return def;
    }

    private static float GetFloatOrDefault(List<string> r, Dictionary<string, int> m, string col, float def)
    {
        var s = Get(r, m, col);
        if (string.IsNullOrWhiteSpace(s)) return def;

        // 한국 로케일에서 소수점이 , 로 들어오는 경우 보정
        s = s.Replace(",", ".");

        if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            return v;

        return def;
    }

    private static TEnum GetEnumOrDefault<TEnum>(List<string> r, Dictionary<string, int> m, string col, TEnum def) where TEnum : struct
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

    private static ItemCsvImportSettings FindSettings()
    {
        // 프로젝트에서 Settings SO 찾기
        var guids = AssetDatabase.FindAssets($"t:ItemCsvImportSettings {SettingsAssetName}");
        if (guids == null || guids.Length == 0)
        {
            // 이름이 달라도 타입만 있으면 찾게 보완
            guids = AssetDatabase.FindAssets("t:ItemCsvImportSettings");
        }
        if (guids == null || guids.Length == 0) return null;

        var path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<ItemCsvImportSettings>(path);
    }

}
#endif
