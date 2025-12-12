using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item CSV Import Settings", fileName = "ItemCsvImportSettings")]
public class ItemCsvImportSettings : ScriptableObject
{
    [Header("입력 CSV")]
    public TextAsset csvFile;

    [Header("출력 폴더 (Resources 하위 경로)")]
    [Tooltip("예: Items 라고 쓰면 Assets/Resources/Items 에 생성됨")]
    public string resourcesFolder = "Items";

    [Header("생성/업데이트 옵션")]
    public bool overwriteExisting = true;
    public bool clearMissingAssets = false; // CSV에 없는 기존 에셋 삭제 여부

    [Header("기본값 (CSV에 없거나 파싱 실패 시)")]
    public bool defaultIsActiveItem = false; // 패시브 CSV면 false 고정
    public ItemTier defaultTier = ItemTier.Tier1;
}
