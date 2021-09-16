using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Game Master")]
    [SerializeField] GameMaster gM;

    [Header("UI Manager Children")]
    [SerializeField] GameObject logReg;
    [SerializeField] GameObject mainStage;

    [Header("Login Objects")]
    [SerializeField] GameObject LoginUI;
    [SerializeField] TMP_InputField log_Email;
    [SerializeField] TMP_InputField log_Password;

    [Header("Register Objects")]
    [SerializeField] GameObject RegisterUI;
    [SerializeField] TMP_InputField reg_Username;
    [SerializeField] TMP_InputField reg_Email;
    [SerializeField] TMP_InputField reg_Password;
    [SerializeField] TMP_InputField reg_Verify_Password;

    [Header("Main Stage Objects")]
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject NavigationTab;
    [SerializeField] GameObject UserSettings;
    [SerializeField] GameObject NotePad;
    [SerializeField] GameObject BadgeDisplay;

    [Header("UserSettings Objects")]
    [SerializeField] TMP_InputField user_Username;
    [SerializeField] TMP_InputField user_Adress;
    [SerializeField] TMP_InputField user_TelNum;
    [SerializeField] TMP_InputField user_Email;
    [SerializeField] TMP_InputField user_DOB;
    [SerializeField] TMP_InputField user_ScoutGroup;

    [Header("NotePad Objects")]
    [SerializeField] TMP_InputField noteInputfield;

    // *** Badges Bool ***
    // Was public, turned private since it's no longer gonna be in use out side of this script.
    bool SpiderBadge = false;
    bool RockBadge = false;
    bool KnifeBadge = false;
    bool MycologyBadge = false;
    bool AutisticEmblem = false;

    #region Get & Set Values
    //  UI Manager Properties
    [HideInInspector] public GameObject LogReg
    {
        get { return logReg; }
    }    
    [HideInInspector] public GameObject MainStage
    {
        get { return mainStage; }
    }

    //  Login Properties 
    [HideInInspector] public TMP_InputField Log_Email
    {
        get { return log_Email; }
    }

    [HideInInspector] public TMP_InputField Log_Password
    {
        get { return log_Password; }
    }

    //  Register Propertie
    [HideInInspector] public TMP_InputField Reg_Username
    {
        get { return reg_Username; }
    }
    
    [HideInInspector] public TMP_InputField Reg_Email
    {
        get { return reg_Email; }
    }
    
    [HideInInspector] public TMP_InputField Reg_Password
    {
        get { return reg_Password; }
    }
    
    [HideInInspector] public TMP_InputField Reg_Verify_Password
    {
        get { return reg_Verify_Password; }
    }

    //  User Settings Properties
    [HideInInspector] public TMP_InputField User_Username
    {
        get { return user_Username; }
        set { user_Username = value; }
    }   
    
    [HideInInspector] public TMP_InputField User_Adress
    {
        get { return user_Adress; }
        set { user_Adress = value; }
    }
        
    [HideInInspector] public TMP_InputField User_TelNum
    {
        get { return user_TelNum; }
        set { user_TelNum = value; }
    }
    
    [HideInInspector] public TMP_InputField User_Email
    {
        get { return user_Email; }
        set { user_Email = value; }
    }    
    
    [HideInInspector] public TMP_InputField User_DBO
    {
        get { return user_DOB; }
        set { user_DOB = value; }
    }    
    
    [HideInInspector] public TMP_InputField User_ScoutGroup
    {
        get { return user_ScoutGroup; }
        set { user_ScoutGroup = value; }
    }      

    #endregion

    enum states
    {
        login,
        register,
        mainMenu,
        navigationTab,
        userSettings,
        notePad,
        badgeDisplay,
    }

    states priorState;
    states currentState = 0;

    void Start()
    {
        GoToState();
    }
    
    #region State Logic
    /// <summary>
    /// Gets you given state destination.
    /// Also deactivates prior state.
    /// </summary>
    void GoToState()
    {
        switch (priorState) //  Deactivates priorState
        {
            case (states)0:
                LoginUI.SetActive(false);
                break;

            case (states)1:
                RegisterUI.SetActive(false);
                break;

            case (states)2:
                MainMenu.SetActive(false);
                break;

            case (states)3:
                NavigationTab.SetActive(false);
                break;

            case (states)4:
                UserSettings.SetActive(false);
                break;
            
            case (states)5:
                NotePad.SetActive(false);
                break;
                            
            case (states)6:
                BadgeDisplay.SetActive(false);
                break;            

            default:
                break;
        }

        switch (currentState) //    Activates currentState
        {
            case (states)0:
                LoginUI.SetActive(true);
                break;    
                
            case (states)1:
                RegisterUI.SetActive(true);
                break;        
                
            case (states)2:
                MainMenu.SetActive(true);
                break;        
                
            case (states)3:
                NavigationTab.SetActive(true);
                break;

            case (states)4:
                UserSettings.SetActive(true);
                break;

            case (states)5:
                NotePad.SetActive(true);
                break;

            case (states)6:
                BadgeDisplay.SetActive(true);
                break;
        }
    }
    #endregion

    #region Buttons

    #region Action Buttons
    public void LoginButton()
    {
        gM.asembly.LoginAsembly();

    }

    public void RegisterButton()
    {
        gM.asembly.RegisterAsembly();
    }

    public void LogoutButton()
    {
        gM.firebaseManager.SignOutButton();
        mainStage.SetActive(false);        
        logReg.SetActive(true);
        currentState = (states)2;
        GoToState();
    }

    public void SaveUserData()
    {
        gM.asembly.UpdateUserSettingsAsembly();
    }

    public void SaveBadgeData()
    {
        UpdateSpiderPPrefs();
        UpdateRockPPrefs();
        UpdateKnifePPrefs();
        UpdateMycologyPPrefs();
        UpdateAutisticPPrefs();
    }

    public void DeleteUserButton()
    {
        gM.firebaseManager.DeleteUser();
    }
    #endregion

    #region Navigation Buttons
    public void ToLogin()
    {
        priorState = currentState;
        currentState = (states)0;
        GoToState();
    }

    public void ToRegister()
    {
        priorState = currentState;
        currentState = (states)1;
        GoToState();
    }

    public void ToMain()
    {
        priorState = currentState;
        currentState = (states)2;
        GoToState();
    }

    public void ToNavigation()
    {
        priorState = currentState;
        currentState = (states)3;
        GoToState();
    }

    public void ToUserSettings()
    {
        priorState = currentState;
        currentState = (states)4;
        gM.asembly.LoadUserSettings();
        GoToState();
    }    
    
    public void ToNotePad()
    {
        priorState = currentState;
        currentState = (states)5;
        LoadUserNotes();
        GoToState();
    }

    public void ToBadgeDisplay()
    {
        priorState = currentState;
        currentState = (states)6;
        LoadBadges();
        GoToState();
    }    
    #endregion

    #endregion

    #region Notepad
    /// <summary>
    /// If theres any data saved on device display it
    /// </summary>
    void LoadUserNotes()
    {
        if (PlayerPrefs.GetString("notes") != null && PlayerPrefs.GetString("notes") != "")
        {
            noteInputfield.text = PlayerPrefs.GetString("notes");
        }
    }

    public void SaveUserNotes()
    {
        PlayerPrefs.SetString("notes", noteInputfield.text);
    }
    #endregion

    #region Badges (PlayerPrefs)

    #region Conversion Region @_@
    /// <summary>
    /// Checks if _value has a value.
    /// If that value is true return true.
    /// Else return false
    /// </summary>
    /// <param name="_value"></param>
    /// <returns></returns>
    bool BoolConvertion(string _value)
    {
        if (_value == "true" && _value == "True")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region Load Badges
    /// <summary>
    /// Loads all badge data
    /// </summary>
    void LoadBadges()
    {
        LoadSpiderPPrefs();
        LoadRockPPrefs();
        LoadKnifePPrefs();
        LoadMycologyPPrefs();
        LoadAutisticPPrefs();
    }

    //----------------------------------------
    //
    //  All code bellow check if theres data.
    //  If there is print it.
    //  Else do nothing.
    //
    //----------------------------------------


    void LoadSpiderPPrefs()
    {
        if (PlayerPrefs.GetString("spider-badge") != null && PlayerPrefs.GetString("spider-badge") != "")
        {
            SpiderBadge = BoolConvertion(PlayerPrefs.GetString("spider-badge"));
            Debug.Log("Spider Value Loaded: " + BoolConvertion(PlayerPrefs.GetString("spider-badge")));
        }
    }

    void LoadRockPPrefs()
    {
        if (PlayerPrefs.GetString("rock-badge") != null && PlayerPrefs.GetString("rock-badge") != "")
        {
            KnifeBadge = BoolConvertion(PlayerPrefs.GetString("rock-badge"));
        }
    }      
    
    void LoadKnifePPrefs()
    {
        if (PlayerPrefs.GetString("knife-badge") != null && PlayerPrefs.GetString("knife-badge") != "")
        {
            KnifeBadge = BoolConvertion(PlayerPrefs.GetString("knife-badge"));
        }
    }    
    
    void LoadMycologyPPrefs()
    {
        if (PlayerPrefs.GetString("mycology-badge") != null && PlayerPrefs.GetString("mycology-badge") != "")
        {
            KnifeBadge = BoolConvertion(PlayerPrefs.GetString("mycology-badge"));
        }
    }    
    
    void LoadAutisticPPrefs()
    {
        if (PlayerPrefs.GetString("autistic-emblem") != null && PlayerPrefs.GetString("autistic-emblem") != "")
        {
            KnifeBadge = BoolConvertion(PlayerPrefs.GetString("autistic-emblem"));
        }
    }

    #endregion

    #region Update Badges

    //----------------------------------------------
    //  All code below updates given PlayPref value
    //----------------------------------------------

    void UpdateSpiderPPrefs()
    {
        PlayerPrefs.SetString("spider-badge", SpiderBadge.ToString());
        Debug.Log("Converted spider-badge to: " + PlayerPrefs.GetString("spider-badge"));
    }       
    
    void UpdateRockPPrefs()
    {
        PlayerPrefs.SetString("rock-badge", RockBadge.ToString());
    }    
    
    void UpdateKnifePPrefs()
    {
        PlayerPrefs.SetString("knife-badge", KnifeBadge.ToString());
    }    
    
    void UpdateMycologyPPrefs()
    {
        PlayerPrefs.SetString("mycology-badge", MycologyBadge.ToString());
    }    
    
    void UpdateAutisticPPrefs()
    {
        PlayerPrefs.SetString("autistic-emblem", AutisticEmblem.ToString());
    }

    #endregion

    #endregion

    #region CheckBox Logic
    public void hasSpiderBadge()
    {
        if (SpiderBadge)
        {
            SpiderBadge = false;
            Debug.Log("Spider Badge UI is now: " + SpiderBadge.ToString());
        }
        else
        {
            SpiderBadge = true;
            Debug.Log("Spider Badge UI is now: " + SpiderBadge.ToString());
        }
    }    
    
    public void hasRockClimbingBadge()
    {
        if (RockBadge)
        {
            RockBadge = false;
        }
        else
        {
            RockBadge = true;
        }
    }    
    
    public void hasKnifeChraftsmanBadge()
    {
        if (KnifeBadge)
        {
            KnifeBadge = false;
        }
        else
        {
            KnifeBadge = true;
        }
    }    
    
    public void hasMycologyBadge()
    {
        if (MycologyBadge)
        {
            MycologyBadge = false;
        }
        else
        {
            MycologyBadge = true;
        }
    }

    public void hasAutisticEmblem()
    {
        if (AutisticEmblem)
        {
            AutisticEmblem = false;
        }
        else
        {
            AutisticEmblem = true;
        }
    }
    #endregion
}
