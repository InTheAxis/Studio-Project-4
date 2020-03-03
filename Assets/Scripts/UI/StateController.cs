using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StateController
{
    private static Dictionary<string, State> screenStates = new Dictionary<string, State>();
    private static Stack<State> history = new Stack<State>();

    private static State currentState = null;

    private static bool firstStart = true;
    private const string initialState = "Splashscreen";

    public static void Register(State state)
    {
        if (state == null || screenStates.ContainsKey(state.Name)) return;

        screenStates.Add(state.Name, state);
        if (!(firstStart && state.Name.ToLower().Equals(initialState.ToLower())))
        {
            state.gameObject.SetActive(false);
        }
        else
        {
            firstStart = false;

            currentState = state;
            showNext(state.Name, false, false);
        }
    }

    public static void Unregister(State state)
    {
        if (state == null || !screenStates.ContainsKey(state.Name)) return;
        screenStates.Remove(state.Name);
    }

    public static void showNext(string name, bool hideCurrent=true, bool addToHistory=true)
    {
        if (!screenStates.ContainsKey(name))
        {
            Debug.LogError(name + " does not exist! (ShowNext)");
            return;
        }

        if (currentState != null)
        {
            if(addToHistory)
                history.Push(currentState);
            if(hideCurrent)
                Hide(currentState);
        }

        currentState = screenStates[name];
        Show(currentState);
    }

    public static void showPrevious(bool hideCurrent=true)
    {
        if (history == null || history.Count == 0) return;

        if(hideCurrent)
            Hide(currentState);
        currentState = history.Pop();
        Show(currentState);
    }

    public static void Show(string name)
    {
        if (!screenStates.ContainsKey(name))
        {
            Debug.LogError(name + " does not exist! (Show)");
            return;
        }
        Show(screenStates[name]);
    }

    public static void Hide(string name)
    {
        if (!screenStates.ContainsKey(name))
            return;
        Hide(screenStates[name]);
    }

    private static void Show(State state)
    {
        if (state == null) return;
        state.onShow();
    }

    private static void Hide(State state)
    {
        if (state == null) return;
        state.onHide();
    }

    public static State getCurrentState()
    {
        return currentState;
    }

    public static State getState(string name)
    {
        if (!screenStates.ContainsKey(name))
            return null;
        else
            return screenStates[name];
    }

    public static IEnumerator fadeCanvasGroup(CanvasGroup canvas, bool fadeIn, float canvasFadeSpeed)
    {
        float targetAlpha = fadeIn ? 1.0f : 0.0f;
        canvas.alpha = 1.0f - targetAlpha;
        while (Mathf.Abs(targetAlpha - canvas.alpha) > 0.05f)
        {
            canvas.alpha = Mathf.Lerp(canvas.alpha, targetAlpha, Time.deltaTime * canvasFadeSpeed);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        canvas.alpha = targetAlpha;
    }

    public static IEnumerator fadeCanvasGroupAndHide(GameObject go, CanvasGroup canvas, bool fadeIn, float canvasFadeSpeed)
    {
        float targetAlpha = fadeIn ? 1.0f : 0.0f;
        canvas.alpha = 1.0f - targetAlpha;
        while (Mathf.Abs(targetAlpha - canvas.alpha) > 0.05f)
        {
            canvas.alpha = Mathf.Lerp(canvas.alpha, targetAlpha, Time.deltaTime * canvasFadeSpeed);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        canvas.alpha = targetAlpha;
        go.SetActive(false);

    }
}
