using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Security.Cryptography;

public class PlayfabAuthenticator : DoNotDestroySingleton<PlayfabAuthenticator>
{
    private string playfabPlayerID;

    // byte in failFunc will be error type
    // 0: playfab
    // 1: Other

    public void Register(string playerName, string password, string email, System.Action<string> successFunc, System.Action<string, byte> failFunc)
    {
        string playerNameHash = getHashString(playerName);
        string passwordHash = getHashString(password);

        // Login to playfab using playerName as ID without creating account to check if account exists
        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            CreateAccount = false,
            CustomId = playerNameHash
        }, result =>
        {
            // User exists
            failFunc("User already exists", 1);
        }, error =>
        {
            // User does not exist
            PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
            {
                CreateAccount = true,
                CustomId = playerNameHash
            }, result2 =>
            {
                // Created account, time to set account credentials
                setData(new Dictionary<string, string>
                {
                    { "password", passwordHash },
                    { "email", email }
                }, () =>
                {
                    // Success setting data, now to return
                    successFunc(playerName);
                }, () =>
                {
                    failFunc("Unknown error: 104", 0);
                });
            }, error2 =>
            {
                logError(error2);
                failFunc("Unknown error: 105", 0);
            });
        });
    }

    public void Login(string playerName, string password, System.Action<string> successFunc, System.Action<string, byte> failFunc)
    {
        // Login to playfab using playerName as ID
        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            CreateAccount = false,
            CustomId = getHashString(playerName)
        }, result =>
        {
            // Get playfab player ID
            getPlayerID(() =>
            {
                // Get user data to check password
                checkPlayerLoginCredentials(password, () => successFunc(playerName), failFunc);
            }, failFunc);
        }, error =>
        {
            logError(error);
            failFunc("Unknown error: 103", 0);
        });
    }

    public void setData(Dictionary<string, string> valuesToSet, System.Action successFunc = null, System.Action failFunc = null)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = valuesToSet
        }, result =>
        {
            Debug.Log("Successfully updated user data");
            successFunc?.Invoke();
        }, error =>
        {
            Debug.LogWarning("Got error setting user data");
            logError(error);
            failFunc?.Invoke();
        });
    }
    private void getPlayerID(System.Action successFunc, System.Action<string, byte> failFunc)
    {
        GetAccountInfoRequest request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request
            , result =>
            {
                Debug.Log("Retrieved playfabID");
                playfabPlayerID = result.AccountInfo.PlayFabId;
                successFunc();
            }
            , error =>
            {
                logError(error);
                failFunc("Unknown error: 101", 0);
            });
    }

    private void checkPlayerLoginCredentials(string password, System.Action successFunc, System.Action<string, byte> failFunc)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = playfabPlayerID,
            Keys = null
        }, result =>
        {
            if (result.Data["password"].Value == getHashString(password))
                successFunc();
            else
                failFunc("Password incorrect", 1);
        }, error =>
        {
            logError(error);
            failFunc("Unknown error: 102", 0);
        });
    }

    private void logError(PlayFabError err)
    {
        Debug.LogWarning(err.GenerateErrorReport());
    }

    private static byte[] getHash(string inputString)
    {
        using (HashAlgorithm algorithm = SHA256.Create())
            return algorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(inputString));
    }
    private static string getHashString(string inputString)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (byte b in getHash(inputString))
            sb.Append(b.ToString("X2"));

        return sb.ToString();
    }
}
