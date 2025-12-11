using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBombController : MonoBehaviour
{
    public PlayerInventory inventory;
    public GameObject thrownBombPrefab;

    [Header("설정")]
    public float bombFuseTime = 3f;   // 들기 시작한 시점부터 얼마나 뒤에 터질지
    public float throwPower = 5f;     // 던질 때 힘의 크기

    ThrownBomb currentBomb;           // 현재 들고 있는 폭탄 참조
    Vector2 lastLookDir = Vector2.right; // 플레이어가 "바라보는" 방향 (기본 오른쪽)

    void Update()
    {
        // 플레이어가 바라보는 방향 업데이트 (이동 방향 기준, 필요하면 네 이동 코드랑 맞추면 됨)
        UpdateLookDirection();

        // Shift 누르는 순간  폭탄 들기
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            TryHoldBomb();
        }

        // Shift를 떼는 순간  현재 바라보는 방향으로 던지기
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            TryThrowHeldBomb();
        }
    }

    void UpdateLookDirection()
    {
        // 여기서는 간단히 WASD/화살표 기준 이동 방향으로 봄
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 move = new Vector2(x, y);

        if (move.sqrMagnitude > 0.01f)
        {
            lastLookDir = move.normalized;
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

    void TryThrowHeldBomb()
    {
        if (currentBomb == null) return;

        // 혹시 바라보는 방향 정보가 0,0이면 기본값 하나 정해두기 (오른쪽)
        if (lastLookDir.sqrMagnitude < 0.01f)
        {
            lastLookDir = Vector2.right;
        }

        currentBomb.Throw(lastLookDir, throwPower);
        currentBomb = null;
    }
}
