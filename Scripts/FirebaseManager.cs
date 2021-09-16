using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Linq;

public class FirebaseManager :  MonoBehaviour
{
    [SerializeField] GameMaster gM;

    internal DependencyStatus dependencyStatus;
    FirebaseAuth auth;
    FirebaseUser user;
    DatabaseReference dbRefrence;

    
    void Awake()
    {
        //  Checks that all of the necesary dependencies for Firebase are present
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //   If they are avaliable initlize Firebase
                InitilizeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitilizeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //  Setting the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        dbRefrence = FirebaseDatabase.DefaultInstance.RootReference;
    }

    #region Auth Logic
    /// <summary>
    /// Checks if email and pass word are in order.
    /// Then act acording to data.
    /// If user data is correct, user is now logged in.
    /// </summary>
    /// <param name="_email">User Login Email</param>
    /// <param name="_password">User Login Password</param>
    /// <returns></returns>
    public IEnumerator LoginLogic(string _email, string _password)
    {
        //  Calling the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //  Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        //  Checks for errors
        if (LoginTask.Exception != null)
        {
            //  If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;

                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            Debug.Log(message);
        }
        else
        {
            //  User is now logged in
            //  Now get result
            user = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.Email);
            gM.uIManager.LogReg.SetActive(false);
            gM.uIManager.MainStage.SetActive(true);
            gM.uIManager.ToMain();
        }
    }

    /// <summary>
    /// Check if all veribles are in order.
    /// If they are create a user then return them to Login Menu
    /// </summary>
    /// <param name="_username">Username Register text</param>
    /// <param name="_email">Email Register text</param>
    /// <param name="_password">Password Register text</param>
    /// <param name="_veryfiPassword">verify Register password text</param>
    /// <returns></returns>
    public IEnumerator RegisterLogic(string _username, string _email, string _password, string _veryfiPassword)
    {
        if (_username == null)
        {
            Debug.LogWarning("Username is null");
        }
        else if (_password != _veryfiPassword)
        {
            Debug.LogWarning("Passwords do not match");
        }
        else
        {
            //  Calling the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //  Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null) //  Checks if there's errors
            {
                //  If any errors occrue handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                Debug.LogWarning(message);
            }
            else
            {
                //  User has now been created
                //  Now get the result
                user = RegisterTask.Result;

                if (user != null)
                {
                    //  Createing a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username};

                    //  Call the firebase auth update user profile function passing the profile with the username
                    var ProfileTask = user.UpdateUserProfileAsync(profile);
                    //  Wait until the task completes

                    if (ProfileTask.Exception != null)
                    {
                        //  If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        Debug.LogWarning("Username set failed");
                    }
                    else
                    {
                        //  Username is now set
                        //  Now return to login screen
                        gM.uIManager.ToLogin();
                        UpdateUsernameDatabase(_username);
                        UpdateEmailDatabase(_email);
                    }
                }
            }
        }
    }

    public void SignOutButton()
    {
        auth.SignOut();

        gM.uIManager.MainStage.SetActive(false);
        gM.uIManager.LogReg.SetActive(true);
    }
    #endregion

    #region Mostly Database Logic

    #region Update/Create Logic

    #region UserSettings
    /// <summary>
    /// Updates user settings. (NOTE ALL PARAMETERS ARE STRINGS)
    /// </summary>
    /// <param name="username">Type username</param>
    /// <param name="adress">Type adress</param>
    /// <param name="telNum">Type Telephone number</param>
    /// <param name="email">Type Email</param>
    /// <param name="dOB">Type Date of Birth</param>
    /// <param name="scoutGroup">Type Scout Group</param>
    public void UpdateUserSettings(string username, string adress, string telNum, string email, string dOB, string scoutGroup)
    {
        StartCoroutine(UpdateUsernameDatabase(username));
        StartCoroutine(UpdateUsernameAuth(username));
        StartCoroutine(UpdateAdressDatabase(adress));
        StartCoroutine(UpdateTelNumDatabase(telNum));
        StartCoroutine(UpdateEmailAuth(email));
        StartCoroutine(UpdateEmailDatabase(email));
        StartCoroutine(UpdateDateOfBirthDatabase(dOB));
        StartCoroutine(UpdateScoutGroupDatabase(scoutGroup));
    }

    IEnumerator UpdateUsernameDatabase(string _username)
    {
        //  Path to username in db, then sets value
        var dbTask = dbRefrence.Child("users").Child(user.UserId).Child("username").SetValueAsync(_username);
        //  Wait until tasks completes
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);
        
        if (dbTask.Exception != null) //  If an error eccors display error message in conesole. Else everything works as intended
        {
            Debug.LogWarning(message: $"Failed to register task with {dbTask.Exception}");
        }
        else
        {
            Debug.Log("DB Username Updated");
        }
    }

    public IEnumerator UpdateUsernameAuth(string _username)
    {
        //  Setting user profiles new username
        UserProfile profile = new UserProfile { DisplayName = _username };

        //  Calling the Firebase auth update user profile function passing the profile with the username
        var ProfileTask = user.UpdateUserProfileAsync(profile);
        //  Wait until task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null) //  If an error eccors display error message in conesole. Else everything works as intended
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            Debug.Log("Auth Email Updated");
        }
    }

    IEnumerator UpdateAdressDatabase(string _adress)
    {
        //  Path to username in db, then sets value
        var dbTask = dbRefrence.Child("users").Child(user.UserId).Child("adress").SetValueAsync(_adress);
        //  Wait until tasks completes
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);
        
        if (dbTask.Exception != null) //  If an error eccors display error message in conesole. Else everything works as intended
        {
            Debug.LogWarning(message: $"Failed to register task with {dbTask.Exception}");
        }
        else
        {
            Debug.Log("DB adress Updated");
        }
    }

    IEnumerator UpdateTelNumDatabase(string _telNum)
    {
        //  Path to username in db, then sets value
        var dbTask = dbRefrence.Child("users").Child(user.UserId).Child("telephone-number").SetValueAsync(_telNum);
        //  Wait until tasks completes
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);
        
        if (dbTask.Exception != null) //  If an error eccors display error message in conesole. Else everything works as intended
        {
            Debug.LogWarning(message: $"Failed to register task with {dbTask.Exception}");
        }
        else
        {
            Debug.Log("DB telephone-number Updated");
        }
    }    

        IEnumerator UpdateEmailAuth(string _email)
    {
        //  Calling the Firebase auth update user profile function passing the profile with the username
        var ProfileTask = user.UpdateEmailAsync(_email);
        //  Wait until task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.IsFaulted) //  If an error eccors display error message in conesole. Else everything works as intended
        {
            Debug.LogWarning(message: $"Failed to UpdateEmailAsync Error: {ProfileTask.Exception}");
        }
        else
        {
            Debug.Log("Auth Email Updated");
        }
    }

    IEnumerator UpdateEmailDatabase(string _email)
    {
        //  Path to username in db, then sets value
        var dbTask = dbRefrence.Child("users").Child(user.UserId).Child("email").SetValueAsync(_email);
        //  Wait until tasks completes
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null) //  If an error eccors display error message in conesole. Else everything works as intended
        {
            Debug.LogWarning(message: $"Failed to register task with {dbTask.Exception}");
        }
        else
        {
            Debug.Log("DB email Updated");
        }
    }
    IEnumerator UpdateDateOfBirthDatabase(string _dOB)
    {
        //  Path to username in db, then sets value
        var dbTask = dbRefrence.Child("users").Child(user.UserId).Child("data-of-birth").SetValueAsync(_dOB);
        //  Wait until tasks completes
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);
        
        if (dbTask.Exception != null) //  If an error eccors display error message in conesole. Else everything works as intended
        {
            Debug.LogWarning(message: $"Failed to register task with {dbTask.Exception}");
        }
        else
        {
            Debug.Log("DB date-of-birth Updated");
        }
    }    
    
    IEnumerator UpdateScoutGroupDatabase(string _scoutGroup)
    {
        //  Path to username in db, then sets value
        var dbTask = dbRefrence.Child("users").Child(user.UserId).Child("scout-group").SetValueAsync(_scoutGroup);
        //  Wait until tasks completes
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);
        
        if (dbTask.Exception != null) //  If an error eccors display error message in conesole. Else everything works as intended
        {
            Debug.LogWarning(message: $"Failed to register task with {dbTask.Exception}");
        }
        else
        {
            Debug.Log("DB scout-group Updated");
        }
    }
    #endregion

    #region Badge Update

    /// <summary>
    /// Updates users badge data!
    /// Please submit data as a bool passed to string!
    /// </summary>s
    /// <param name="_sb">Spider Badge</param>
    /// <param name="_rcb">Rock Clibing Badge</param>
    /// <param name="_rcb">Rock Clibing Badge</param>
    /// <param name="_kcb">Knife Craftsmans Badge</param>
    /// <param name="_mb">Mycologist Badge (what a cool name for shroom collectors! :D)</param>
    /// <param name="_ae">Autistic emblem (patrick dubbed this one)</param>
    public void BadgeUpdate(string _sb, string _rcb, string _kcb, string _mb, string _ae)
    {
        StartCoroutine(UpdateSpiderBadge(_sb));
        StartCoroutine(UpdateSpiderBadge(_rcb));
        StartCoroutine(UpdateSpiderBadge(_kcb));
        StartCoroutine(UpdateSpiderBadge(_mb));
        StartCoroutine(UpdateSpiderBadge(_ae));
    }

    IEnumerator UpdateSpiderBadge(string _spiderBadge)
    {
        //  Path to username in db, then sets value
        var dbTask = dbRefrence.Child("users").Child(user.UserId).Child("badges").Child("spider-badge").SetValueAsync(_spiderBadge);
        //  Wait until tasks completes
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null) //  If an error eccors display error message in conesole. Else everything works as intended
        {
            Debug.LogWarning(message: $"Failed to register task with {dbTask.Exception}");
        }
        else
        {
            Debug.Log("DB Username Updated");
        }
    }    
    
    IEnumerator UpdateRockBadge(string _rockBadge)
    {
        //  Path to username in db, then sets value
        var dbTask = dbRefrence.Child("users").Child(user.UserId).Child("badges").Child("rock-climbing-badge").SetValueAsync(_rockBadge);
        //  Wait until tasks completes
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null) //  If an error eccors display error message in conesole. Else everything works as intended
        {
            Debug.LogWarning(message: $"Failed to register task with {dbTask.Exception}");
        }
        else
        {
            Debug.Log("DB Username Updated");
        }
    }

    IEnumerator UpdateKnifeBadge(string _knifeBadge)
    {
        //  Path to username in db, then sets value
        var dbTask = dbRefrence.Child("users").Child(user.UserId).Child("badges").Child("knife-craftsman-badge").SetValueAsync(_knifeBadge);
        //  Wait until tasks completes
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null) //  If an error eccors display error message in conesole. Else everything works as intended
        {
            Debug.LogWarning(message: $"Failed to register task with {dbTask.Exception}");
        }
        else
        {
            Debug.Log("DB Username Updated");
        }
    }
        
    IEnumerator UpdateMycologyBadge(string _mycologybadge )
    {
        //  Path to username in db, then sets value
        var dbTask = dbRefrence.Child("users").Child(user.UserId).Child("badges").Child("mycology-badge").SetValueAsync(_mycologybadge);
        //  Wait until tasks completes
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null) //  If an error eccors display error message in conesole. Else everything works as intended
        {
            Debug.LogWarning(message: $"Failed to register task with {dbTask.Exception}");
        }
        else
        {
            Debug.Log("DB Username Updated");
        }
    }    
    
    IEnumerator UpdateAutisticEmblem(string _autisticEmblem )
    {
        //  Path to username in db, then sets value
        var dbTask = dbRefrence.Child("users").Child(user.UserId).Child("badges").Child("austistic-emblem").SetValueAsync(_autisticEmblem);
        //  Wait until tasks completes
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null) //  If an error eccors display error message in conesole. Else everything works as intended
        {
            Debug.LogWarning(message: $"Failed to register task with {dbTask.Exception}");
        }
        else
        {
            Debug.Log("DB Username Updated");
        }
    }



    #endregion

    #endregion

    #region Load Data Logic

    #region User Settings Load
    /// <summary>
    /// Loads in user settings data
    /// </summary>
    /// <param name="_username">String in which username will be displayed</param>
    /// <param name="_adress">String in which Adress will be displayed</param>
    /// <param name="_telNum">String in which Telephone Number will be displayed</param>
    /// <param name="_email">String in which Email will be displayed</param>
    /// <param name="_dOB">String in which Date Of Birth will be displayed</param>
    /// <param name="_scoutGroup">String in which Scout Group  will be displayed</param>
    /// <returns></returns>
    public IEnumerator LoadUserSettingData(string _username, string _adress, string _telNum, string _email, string _dOB, string _scoutGroup)
    {
        //  Path to the specified user
        var dbTask = dbRefrence.Child("users").Child(user.UserId).GetValueAsync();
        //  Wait until task completes
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null) //    If theres an error display error message
        {
            Debug.LogWarning(message: $"Failed to register task with {dbTask.Exception}");
        }
        else if (dbTask.Result.Value == null)
        {
            Debug.LogWarning("NO DATA FOUND");
        }
        else
        {
            //  Data has been retrived
            DataSnapshot snapshot = dbTask.Result;

            //  Print data into load
            //  given input = snapshot.Child("inputname").Value.ToString();
            _username = snapshot.Child("username").Value.ToString();
            _adress = snapshot.Child("adress").Value.ToString();
            _telNum = snapshot.Child("telephone-number").Value.ToString();
            _email = snapshot.Child("email").Value.ToString();
            _dOB = snapshot.Child("date-of-birth").Value.ToString();
            _scoutGroup = snapshot.Child("scout-group").Value.ToString();
        }
    }    

    #endregion

    #region Badge Data load 

    //----------------------------------------------------
    //
    //  This region and it's children,
    //  have been handeled very caveman like.
    //
    //  NOTE: If possible clean up the bundles of code.
    //
    //----------------------------------------------------

    #region Data Connection and Value
    IEnumerator LoadSpiderData(string _spiderBadge)
    {
        //  Path to the specified user
        var dbTask = dbRefrence.Child("users").Child(user.UserId).Child("badges").GetValueAsync();
        //  Wait until task completes
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null) //    If theres an error display error message
        {
            Debug.LogWarning(message: $"Failed to register task with {dbTask.Exception}");
        }
        else
        {
            //  Data has been retrived
            DataSnapshot snapshot = dbTask.Result;
            //  Print data into load
            _spiderBadge = snapshot.Child("badges").Child("spider-badge").Value.ToString();
            Debug.Log("Spider Badge Data Loaded into string");
        }
    }    
    
    IEnumerator LoadRockData(string _rockBadge)
    {
        //  Path to the specified user
        var dbTask = dbRefrence.Child("users").Child(user.UserId).GetValueAsync();
        //  Wait until task completes
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null) //    If theres an error display error message
        {
            Debug.LogWarning(message: $"Failed to register task with {dbTask.Exception}");
        }
        else
        {
            //  Data has been retrived
            DataSnapshot snapshot = dbTask.Result;

            //  Print data into load
            _rockBadge = snapshot.Child("badges").Child("spider-badge").Value.ToString();
        }
    }    
    
    IEnumerator LoadKnifeData(string _knifeBadge)
    {
        //  Path to the specified user
        var dbTask = dbRefrence.Child("users").Child(user.UserId).GetValueAsync();
        //  Wait until task completes
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null) //    If theres an error display error message
        {
            Debug.LogWarning(message: $"Failed to register task with {dbTask.Exception}");
        }
        else
        {
            //  Data has been retrived
            DataSnapshot snapshot = dbTask.Result;

            //  Print data into load
            _knifeBadge = snapshot.Child("badges").Child("spider-badge").Value.ToString();
        }
    }    
    
    IEnumerator LoadMycologyData(string _mycologyBadge)
    {
        //  Path to the specified user
        var dbTask = dbRefrence.Child("users").Child(user.UserId).GetValueAsync();
        //  Wait until task completes
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null) //    If theres an error display error message
        {
            Debug.LogWarning(message: $"Failed to register task with {dbTask.Exception}");
        }
        else
        {
            //  Data has been retrived
            DataSnapshot snapshot = dbTask.Result;

            //  Print data into load
            _mycologyBadge = snapshot.Child("badges").Child("spider-badge").Value.ToString();
        }
    }    
    
    IEnumerator LoadAutisticData(string _autisticEmblem)
    {
        //  Path to the specified user
        var dbTask = dbRefrence.Child("users").Child(user.UserId).GetValueAsync();
        //  Wait until task completes
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.Exception != null) //    If theres an error display error message
        {
            Debug.LogWarning(message: $"Failed to register task with {dbTask.Exception}");
        }
        else
        {
            //  Data has been retrived
            DataSnapshot snapshot = dbTask.Result;

            //  Print data into load
            _autisticEmblem = snapshot.Child("badges").Child("spider-badge").Value.ToString();
        }
    }

    #endregion

    #region Convertion

    /// <summary>
    /// Converts database content to bool.
    /// </summary>
    /// <returns>true || false</returns>
    public bool ConvertSpiderToBool()
    {
        string value = "";
        StartCoroutine(LoadSpiderData(value));

        if (value == "true")
        {
            Debug.Log("Spider badge Convertet from string to bool: true");
            return true;
        }
        else
        {
            Debug.Log("Spider badge Convertet from string to bool: false");
            return false;
        }
    }
    
    /// <summary>
    /// Converts database content to bool.
    /// </summary>
    /// <returns>true || false</returns>
    public bool ConvertRockToBool()
    {
        string value = "";
        StartCoroutine(LoadRockData(value));

        if (value == "true")
        {
            return true;
        }
        else
        {
            return false;
        }
    }    
    
    /// <summary>
    /// Converts database content to bool.
    /// </summary>
    /// <returns>true || false</returns>
    public bool ConvertKnifeToBool()
    {
        string value = "";
        StartCoroutine(LoadKnifeData(value));

        if (value == "true")
        {
            return true;
        }
        else
        {
            return false;
        }
    }    
    
    /// <summary>
    /// Converts database content to bool.
    /// </summary>
    /// <returns>true || false</returns>
    public bool ConvertMycologyToBool()
    {
        string value = "";
        StartCoroutine(LoadMycologyData(value));

        if (value == "true")
        {
            return true;
        }
        else
        {
            return false;
        }
    }    
    
    /// <summary>
    /// Converts database content to bool.
    /// </summary>
    /// <returns>true || false</returns>
    public bool ConvertAutisticToBool()
    {
        string value = "";
        StartCoroutine(LoadAutisticData(value));

        if (value == "true")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #endregion

    #endregion

    #region Data Destruction
    /// <summary>
    /// Removes User from Firebase Auth 
    /// </summary>
    public void DeleteUser() //  Should Delete User and thier data
    {
        StartCoroutine(NullData("username"));
        StartCoroutine(NullData("adress"));
        StartCoroutine(NullData("telephone-number"));
        StartCoroutine(NullData("email"));
        StartCoroutine(NullData("data-of-birth"));
        StartCoroutine(NullData("scout-group"));


        //  Deletes User 
        user.DeleteAsync().ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("Delete Async was canceled.");
            }
            if (task.IsFaulted)
            {
                Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
            }
            if(task.IsCompleted)
            {
                Debug.Log("User deleted successfully.");

                gM.uIManager.MainStage.SetActive(false);
                gM.uIManager.LogReg.SetActive(true);
                gM.uIManager.ToLogin();
            }
        });
    }

    /// <summary>
    /// Nulls value from Firebase DB
    /// </summary>
    /// <param name="_value">name of value that needs to be null'ed</param>
    /// <returns></returns>
    IEnumerator NullData(string _value)
    {
        //  Gets user data and removes it
        var dbTask = dbRefrence.Child("users").Child(user.UserId).Child(_value).RemoveValueAsync();
        yield return new WaitUntil(predicate: () => dbTask.IsCompleted);

        if (dbTask.IsFaulted)
        {
            Debug.Log("ERROR: " + dbTask.Exception);
        }
        else if (dbTask.IsCompleted)
        {
            Debug.Log(_value + " has been null'ed.");
        }
    }

    #endregion

    #endregion
}
