using UnityEngine;

public class BattleField : MonoBehaviour
{
    [SerializeField] AnnoyBear bear;
    // int playerLayer = 1 <<8; : Physics에서는 이 방식 사용
    // Physics는 비트 플래그로 작동하지만 Gameobject의 레이어는 단일 레이어값으로 작동한다.
    int playerLayer = (int) LAYERS.PLAYER;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == playerLayer)
        {
            bear.IsInBattleField = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            bear.IsInBattleField = false;
        }
    }
}
