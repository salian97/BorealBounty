using UnityEngine;
using UnityEngine.SceneManagement;

public class SquirrelPositionRestore : MonoBehaviour
{
    void Start()
    {
        if (BattleReturnData.lastScene == SceneManager.GetActiveScene().name)
        {
            transform.position = BattleReturnData.lastPosition;
        }
    }
}
