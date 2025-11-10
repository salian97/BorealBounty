using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DestroyNPC : MonoBehaviour
{
   void Start()
    {
        if (BattleReturnData.lastScene == SceneManager.GetActiveScene().name)
        {
            Destroy(gameObject);
        }
    }
}
