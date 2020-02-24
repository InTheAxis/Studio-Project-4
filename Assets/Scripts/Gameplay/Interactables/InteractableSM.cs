using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class InteractableSM
{
    #region States
    private abstract class InteractableStateBase
    {
        public InteractableBase usingInteractable = null;
        public InteractableBase carryingInteractable = null;

        protected bool ensureOwnedByScene(InteractableBase interactable) { return ensureOwnedByScene(PhotonView.Get(interactable)); }
        protected bool ensureOwnedByScene(PhotonView interactable) { return interactable.Owner == null; }
        protected bool ensureOwnedByPlayer(InteractableBase interactable, Player player) { return ensureOwnedByPlayer(PhotonView.Get(interactable), player); }
        protected bool ensureOwnedByPlayer(PhotonView interactable, Player player) { return interactable.Owner?.ActorNumber == player.ActorNumber; }
        protected bool ensureNotOwnedByOther(InteractableBase interactable, Player requestingPlayer) { return ensureNotOwnedByOther(PhotonView.Get(interactable), requestingPlayer); }
        protected bool ensureNotOwnedByOther(PhotonView interactable, Player requestingPlayer) { return ensureOwnedByScene(interactable) || ensureOwnedByPlayer(interactable, requestingPlayer); }
        protected bool isCarrying(InteractableBase interactable = null)
        {
            if (interactable == null)
                return carryingInteractable != null;
            else
                return carryingInteractable == interactable;
        }
        protected bool isUsing(InteractableBase interactable = null)
        {
            if (interactable == null)
                return usingInteractable != null;
            else
                return usingInteractable == interactable;
        }

        public abstract STATE checkState();

        public enum STATE
        {
            NOTHING,
            CARRYING,
            CONSTANT_USING
        }
        public abstract STATE getState();

        public abstract bool useInteractable(InteractableBase interactable, Player requestingPlayer);
        public abstract bool constantUseInteractable(InteractableBase interactable, Player requestingPlayer);
        public abstract bool stopConstantUseInteractable(InteractableBase interactable, Player requestingPlayer);
        public abstract bool carryInteractable(InteractableBase interactable, Player requestingPlayer);
        public abstract bool dropInteractable(InteractableBase interactable, Player requestingPlayer);
    }
    private class InteractableStateNothing : InteractableStateBase
    {
        public override STATE getState() { return STATE.NOTHING; }
        public override STATE checkState()
        {
            return STATE.NOTHING;
        }
        public override bool useInteractable(InteractableBase interactable, Player requestingPlayer)
        {
            if (!ensureNotOwnedByOther(interactable, requestingPlayer))
                return false;
            if (isCarrying(interactable))
                return true;
            if (isCarrying() || isUsing())
                return false;
            if (!interactable.CanCarry)
                return true;
            return false;
        }
        public override bool constantUseInteractable(InteractableBase interactable, Player requestingPlayer)
        {
            if (!ensureNotOwnedByOther(interactable, requestingPlayer))
                return false;
            if (isCarrying() || isUsing())
                return false;
            if (!interactable.AllowConstantInteraction)
                return false;
            usingInteractable = interactable;
            return true;
        }
        public override bool stopConstantUseInteractable(InteractableBase interactable, Player requestingPlayer)
        {
            if (!ensureNotOwnedByOther(interactable, requestingPlayer))
                return false;
            if (isCarrying())
                return false;
            if (!isUsing())
                return false;
            usingInteractable = null;
            return true;
        }
        public override bool carryInteractable(InteractableBase interactable, Player requestingPlayer)
        {
            if (!ensureNotOwnedByOther(interactable, requestingPlayer))
                return false;
            if (isCarrying() || isUsing())
                return false;
            if (!interactable.CanCarry)
                return false;
            carryingInteractable = interactable;
            return true;
        }
        public override bool dropInteractable(InteractableBase interactable, Player requestingPlayer)
        {
            if (!ensureOwnedByPlayer(interactable, requestingPlayer))
                return false;
            if (isUsing())
                return false;
            if (isCarrying(interactable))
                return true;
            carryingInteractable = null;
            return false;
        }
    }
    private class InteractableStateCarrying : InteractableStateBase
    {
        public override STATE getState() { return STATE.CARRYING; }
        public override STATE checkState()
        {
            if (carryingInteractable == null)
                return STATE.NOTHING;
            else
                return STATE.CARRYING;
        }
        public override bool useInteractable(InteractableBase interactable, Player requestingPlayer)
        {
            if (isCarrying(interactable) && ensureOwnedByPlayer(interactable, requestingPlayer))
            {
                carryingInteractable = null;
                return true;
            }
            return false;
        }
        public override bool constantUseInteractable(InteractableBase interactable, Player requestingPlayer)
        {
            return false;
        }
        public override bool stopConstantUseInteractable(InteractableBase interactable, Player requestingPlayer)
        {
            return false;
        }
        public override bool carryInteractable(InteractableBase interactable, Player requestingPlayer)
        {
            return false;
        }
        public override bool dropInteractable(InteractableBase interactable, Player requestingPlayer)
        {
            if (isCarrying(interactable) && ensureOwnedByPlayer(interactable, requestingPlayer))
            {
                carryingInteractable = null;
                return true;
            }
            return false;
        }
    }
    private class InteractableStateConstantUsing : InteractableStateBase
    {
        public override STATE getState() { return STATE.CONSTANT_USING; }
        public override STATE checkState()
        {
            if (usingInteractable != null)
                return STATE.CONSTANT_USING;
            else
                return STATE.NOTHING;
        }
        public override bool useInteractable(InteractableBase interactable, Player requestingPlayer)
        {
            return false;
        }
        public override bool constantUseInteractable(InteractableBase interactable, Player requestingPlayer)
        {
            // This request should only be a one time reqeust, network-wise
            return false;
        }
        public override bool stopConstantUseInteractable(InteractableBase interactable, Player requestingPlayer)
        {
            if (isUsing() && ensureOwnedByPlayer(interactable, requestingPlayer))
            {
                usingInteractable = null;
                return true;
            }
            return false;
        }
        public override bool carryInteractable(InteractableBase interactable, Player requestingPlayer)
        {
            return false;
        }
        public override bool dropInteractable(InteractableBase interactable, Player requestingPlayer)
        {
            return false;
        }
    }
    #endregion

    private T changeState<T>(InteractableStateBase prevState) where T : InteractableStateBase, new()
    {
        return new T { usingInteractable = prevState.usingInteractable, carryingInteractable = prevState.carryingInteractable };
    }
    private InteractableStateBase changeState(InteractableStateBase prevState, InteractableStateBase.STATE newState)
    {
        switch (newState)
        {
            case InteractableStateBase.STATE.NOTHING:
                return changeState<InteractableStateNothing>(prevState);
            case InteractableStateBase.STATE.CARRYING:
                return changeState<InteractableStateCarrying>(prevState);
            case InteractableStateBase.STATE.CONSTANT_USING:
                return changeState<InteractableStateConstantUsing>(prevState);
            default:
                return prevState;
        }
    }

    private InteractableStateBase currentState = new InteractableStateNothing();

    public void checkState()
    {
        InteractableStateBase.STATE nextState = currentState.checkState();
        if (nextState != currentState.getState())
            currentState = changeState(currentState, nextState);
    }

    public bool useInteractable(InteractableBase interactable, Player requestingPlayer)
    {
        if (currentState.useInteractable(interactable, requestingPlayer))
        {
            if (currentState.getState() == InteractableStateBase.STATE.CARRYING)
                currentState = changeState<InteractableStateNothing>(currentState);
            return true;
        }
        return false;
    }

    public bool constantUseInteractable(InteractableBase interactable, Player requestingPlayer)
    {
        if (currentState.constantUseInteractable(interactable, requestingPlayer))
        {
            if (currentState.getState() == InteractableStateBase.STATE.NOTHING)
                currentState = changeState<InteractableStateConstantUsing>(currentState);
            return true;
        }
        return false;
    }

    public bool releaseConstantUseInteractable(InteractableBase interactable, Player requestingPlayer)
    {
        if (currentState.stopConstantUseInteractable(interactable, requestingPlayer))
        {
            if (currentState.getState() == InteractableStateBase.STATE.CONSTANT_USING)
                currentState = changeState<InteractableStateNothing>(currentState);
           return true;
        }
        return false;
    }

    public bool carryInteractable(InteractableBase interactable, Player requestingPlayer)
    {
        if (currentState.carryInteractable(interactable, requestingPlayer))
        {
            if (currentState.getState() == InteractableStateBase.STATE.NOTHING)
                currentState = changeState<InteractableStateCarrying>(currentState);
            return true;
        }
        return false;
    }

    public bool dropInteractable(InteractableBase interactable, Player requestingPlayer)
    {
        if (currentState.dropInteractable(interactable, requestingPlayer))
        {
            if (currentState.getState() == InteractableStateBase.STATE.CARRYING)
                currentState = changeState<InteractableStateNothing>(currentState);
            return true;
        }
        return false;
    }
}
