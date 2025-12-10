using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBombController : MonoBehaviour
{
    public PlayerInventory inventory;
    public GameObject thrownBombPrefab;

    [Header("설정")]
    public float bombFuseTime = 3f;   // 들고 있기 시작한 시점부터 얼마나 뒤에 터질지
    public float throwPower = 5f;     // 던질 때 힘의 크기

    ThrownBomb currentBomb;           // 현재 들고 있는 폭탄 참조

    void Update()
    {
        // Shift로 폭탄 들기
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            TryHoldBomb();
        }

        // 방향키 입력으로 던지기 (임시 구현)
        if (currentBomb != null)
        {
            Vector2 dir = GetThrowDirectionFromInput();
            if (dir != Vector2.zero)
            {
                ThrowCurrentBomb(dir);
            }
        }
    }

    void TryHoldBomb()
    {
        // 이미 들고 있으면 또 들 수 없음
        if (currentBomb != null) return;

        // 폭탄 개수 없으면 사용 불가
        if (inventory.bombCount <= 0) return;

        inventory.bombCount--;
        HudUI.Instance.UpdateBomb(inventory.bombCount);

        // 폭탄 생성 후 들기 시작
        GameObject bombObj = Instantiate(
            thrownBombPrefab,
            transform.position,
            Quaternion.identity
        );

        currentBomb = bombObj.GetComponent<ThrownBomb>();
        if (currentBomb != null)
        {
            currentBomb.Hold(transform, bombFuseTime);
        }
    }

    void ThrowCurrentBomb(Vector2 dir)
    {
        if (currentBomb == null) return;

        currentBomb.Throw(dir, throwPower);
        currentBomb = null;
    }

    // 임시: 방향키로 던질 방향 결정 (나중에 키버튼 시스템(QWERASDF) 방향으로 교체 예정)
    Vector2 GetThrowDirectionFromInput()
    {
        float x = 0f;
        float y = 0f;

        if (Input.GetKeyDown(KeyCode.UpArrow)) y = 1f;
        if (Input.GetKeyDown(KeyCode.DownArrow)) y = -1f;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) x = -1f;
        if (Input.GetKeyDown(KeyCode.RightArrow)) x = 1f;

        Vector2 dir = new Vector2(x, y);
        if (dir.sqrMagnitude > 0f)
            return dir.normalized;

        return Vector2.zero;
    }
}
