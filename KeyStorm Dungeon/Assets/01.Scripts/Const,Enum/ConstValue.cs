using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConstValue
{
    #region 애니메이션
    public const string MoveAnim = "IsMove";
    public const string AxisX = "MoveX";
    public const string AxisY = "MoveY";
    public const string DieAnim = "Die";
    public const string HurtAnim = "Hurt";
    public const string AttackAnim = "IsAttack";

    public const string DoorAnim = "OpenDoor";
    #endregion

    #region 공격관련
    public const float ShootOffset = 1.5f;
    public const int DefaultBulletCount = 1;
    public const int SpecialBulletConsume = 2;
    #endregion

    #region 방생성 관련(지금은 사용안함)
    public const float SplitNodeHeight = 0.5f;
    public const int WallLayer = 1 << 7;
    #endregion

    #region 씬 관련
    public const string LoadingScene = "LoadingScene";
    public const string StartScene = "StartScene";
    public const string GameScene = "MainScene";
    #endregion

    #region BGM 관련
    public const float DefaultBgmVolume = 0.5f;
    public const string EasyBgm = "Forest";
    public const string NormalBgm = "Forest";
    public const string HardBgm = "Forest";
    public const string StartBgm = "StartBgm";
    public const string ClearBgm = "GameClear";
    public const string GameOverBgm = "GameOver";
    public const string BossBgm = "BossBgm";
    #endregion

    #region SFX 관련
    public const float DefaultSfxVolume = 0.5f;
    public const string ShootSfx = "Shoot";
    public const string GoldSfx = "Gold";
    public const string ButtonSfx = "Button";
    public const string HitSfx = "Hit";
    public const string BombSfx = "Bomb";
    public const string PlayerMoveSfx = "PlayerMove";
    public const string PlayerHurtSfx = "PlayerHurt";
    public const string DashSfx = "Dash";
    public const string GetItemSfx = "GetItem";
    public const string DoorSfx = "Door";
    #endregion

    #region 게임 종료 관련
    public const string GameOverText = "Game Over...";
    public const string GameClearText = "Game Clear!!!";
    #endregion

    #region Effect 관련
    public const float DefaultEffectSize = 1f;
    #endregion

    #region DoTween 관련
    public const int DefaultJumpCount = 1;
    #endregion

    #region UI 관련
    public const float DefaultScaleValue = 1f;
    #endregion

    #region InventoryUI 관련
    public const float CenterValue = 0.5f;
    public const int DefalueMoveIndex = 1;
    #endregion
}
