using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// T - Key Type
// U - Object to store Type
public class SuccFailWrapper<T, U>
{
    private class Holder<U>
    {
        public U success;
        public U fail;
    }
    private Dictionary<T, Holder<U>> holders = new Dictionary<T, Holder<U>>();

    public bool Add(T key, U success, U fail)
    {
        if (holders.ContainsKey(key))
            return false;

        holders.Add(key, new Holder<U>() { success = success, fail = fail });
        return true;
    }

    public bool onSuccess(T key, out U result)
    {
        Holder<U> value;
        if (holders.TryGetValue(key, out value))
        {
            result = value.success;
            Remove(key);
            return true;
        }
        result = default;
        return false;
    }
    public bool onFailure(T key, out U result)
    {
        Holder<U> value;
        if (holders.TryGetValue(key, out value))
        {
            result = value.fail;
            Remove(key);
            return true;
        }
        result = default;
        return false;
    }

    public bool Remove(T key)
    {
        return holders.Remove(key);
    }
}

// T - Dictionary key type
// U - Callback type
public class SuccFailCallbackWrapper<T, U>
{
    private SuccFailWrapper<T, System.Action<U>> succFail = new SuccFailWrapper<T, System.Action<U>>();

    public bool Add(T key, System.Action<U> successFunc, System.Action<U> failFunc)
    {
        return succFail.Add(key, successFunc, failFunc);
    }

    public bool invokeSuccess(T key, U obj)
    {
        System.Action<U> succFunc;
        bool keyFound = succFail.onSuccess(key, out succFunc);
        succFunc?.Invoke(obj);
        return keyFound;
    }
    public bool invokeFailure(T key, U obj)
    {
        System.Action<U> failFunc;
        bool keyFound = succFail.onFailure(key, out failFunc);
        failFunc?.Invoke(obj);
        return keyFound;
    }

    public bool Remove(T key)
    {
        return succFail.Remove(key);
    }
}
