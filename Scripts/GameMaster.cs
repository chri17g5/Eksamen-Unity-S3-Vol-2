using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    //------------------------------------------------------------------
    //
    //  This scipt is the bridge to all other scripts in the project!
    //
    //------------------------------------------------------------------


    [SerializeField] internal UIManager uIManager;
    [SerializeField] internal Asembly asembly;
    [SerializeField] internal FirebaseManager firebaseManager;
}
