using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asembly : MonoBehaviour
{
    //-----------------------------------------------------------------------------------
    //
    //  This Script primaryly is used to gather parameters from UIManager.
    //  Then the method from FirebaseManager is turned into an Asembly Method.
    //  This Asembly Method is then cast to where is is needed! (normally in UIManager)
    //
    //-----------------------------------------------------------------------------------

    //----------------------------------------------------------------------------------------
    //
    //  Notes for tomorow, find out how to return variables from Firebasemanager to UImanager
    //
    //----------------------------------------------------------------------------------------



    [SerializeField] GameMaster gM;

    /// <summary>
    /// The login assembled.
    /// </summary>
    public void LoginAsembly()
    {
        StartCoroutine(gM.firebaseManager.LoginLogic(gM.uIManager.Log_Email.text,
                                                     gM.uIManager.Log_Password.text));
    }

    /// <summary>
    /// The register assembled
    /// </summary>
    public void RegisterAsembly()
    {
        StartCoroutine(gM.firebaseManager.RegisterLogic(gM.uIManager.Reg_Username.text, 
                                                        gM.uIManager.Reg_Email.text, 
                                                        gM.uIManager.Reg_Password.text, 
                                                        gM.uIManager.Reg_Verify_Password.text));
    }

    /// <summary>
    /// Update Settings assmenbled
    /// </summary>
    public void UpdateUserSettingsAsembly()
    {
        gM.firebaseManager.UpdateUserSettings(gM.uIManager.User_Username.text, 
                                              gM.uIManager.User_Adress.text, 
                                              gM.uIManager.User_TelNum.text, 
                                              gM.uIManager.User_Email.text, 
                                              gM.uIManager.User_DBO.text,
                                              gM.uIManager.User_ScoutGroup.text);
    }

    /// <summary>
    /// Load User Settings assmenbled
    /// </summary>
    public void LoadUserSettings()
    {
        StartCoroutine(gM.firebaseManager.LoadUserSettingData(gM.uIManager.User_Username.text,
                                               gM.uIManager.User_Adress.text,
                                               gM.uIManager.User_TelNum.text,
                                               gM.uIManager.User_Email.text,
                                               gM.uIManager.User_DBO.text,
                                               gM.uIManager.User_ScoutGroup.text));
    }

    //---------------------------------------------------------------------------------------------------------------
    //
    //  HEAVY NOTE: None of this code works due to firebase not allowing me to create new data for user.id children.
    //
    //---------------------------------------------------------------------------------------------------------------


    /*  /// <summary>
        /// Update Badge assmenbled
        /// </summary>
        public void UpdateBadgeAsembly()
        {
            gM.firebaseManager.BadgeUpdate(gM.uIManager.SpiderBadge.ToString(),
                                           gM.uIManager.RockBadge.ToString(),
                                           gM.uIManager.KnifeBadge.ToString(),
                                           gM.uIManager.MycologyBadge.ToString(),
                                           gM.uIManager.AutisticEmblem.ToString());
        }

        /// <summary>
        /// The asembly of all the badge loading features
        /// </summary>
        public void LoadBadgesAsembly() 
        {
            gM.uIManager.SpiderBadge = gM.firebaseManager.ConvertSpiderToBool();
            gM.uIManager.RockBadge = gM.firebaseManager.ConvertRockToBool();
            gM.uIManager.KnifeBadge = gM.firebaseManager.ConvertKnifeToBool();
            gM.uIManager.MycologyBadge = gM.firebaseManager.ConvertMycologyToBool();
            gM.uIManager.AutisticEmblem = gM.firebaseManager.ConvertAutisticToBool();
        }
     */
}
