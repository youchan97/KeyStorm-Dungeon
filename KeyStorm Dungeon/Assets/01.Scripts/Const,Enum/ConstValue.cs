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

    public const string DoorAnim = "OpenDoor";
    #endregion

    #region 공격관련
    public const float ShootOffset = 1.5f;
    public const int DefaultBulletCount = 1;
    #endregion

    #region 방생성 관련(지금은 사용안함)
    public const float SplitNodeHeight = 0.5f;
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
    #endregion

    #region SFX 관련
    public const float DefaultSfxVolume = 0.5f;
    public const string ShootSfx = "Shoot";
    public const string GoldSfx = "Gold";
    #endregion

    #region 게임 종료 관련
    public const string GameOverText = "Game Over...";
    #endregion
}
