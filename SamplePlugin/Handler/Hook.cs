using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using SamplePlugin.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Delegates;

namespace SamplePlugin.Handler
{
    public static unsafe class Hook
    {
        private static uint ID = 0;
        public static Handler.Action* Manager;
        public static MainWindow Thing;
        private delegate byte UseActionDelegate(nint actionManager, uint actionType, uint actionID, ulong targetedActorID, uint param, uint useType, int pvp, nint a8);
        public delegate void UseActionEventDelegate(nint actionManager, uint actionType, uint actionID, ulong targetedActorID, uint param, uint useType, int pvp, nint a8, byte ret);
        public static event UseActionEventDelegate OnUseAction;
        private static Hook<UseActionDelegate> UseActionHook;

        public delegate void ReceiveActionEffectEventDelegate(int sourceActorID, nint sourceActor, nint vectorPosition, nint effectHeader, nint effectArray, nint effectTrail, float oldLock, float newLock);
        public static event ReceiveActionEffectEventDelegate OnReceiveActionEffect;
        private delegate void ReceiveActionEffectDelegate(int sourceActorID, nint sourceActor, nint vectorPosition, nint effectHeader, nint effectArray, nint effectTrail);
        private static Hook<ReceiveActionEffectDelegate> ReceiveActionEffectHook;
        private static void ReceiveActionEffectDetour(int sourceActorID, nint sourceActor, nint vectorPosition, nint effectHeader, nint effectArray, nint effectTrail)
        {
            if (ID != 0)
            {
                Thing.Ready = true;
                Thing.ID = ID;
                ID = 0;
            }
            ReceiveActionEffectHook.Original(sourceActorID, sourceActor, vectorPosition, effectHeader, effectArray, effectTrail);
            OnReceiveActionEffect?.Invoke(sourceActorID, sourceActor, vectorPosition, effectHeader, effectArray, effectTrail, Manager->animationLock, Manager->animationLock);
        }
        private static byte UseActionDetour(nint actionManager, uint actionType, uint actionID, ulong targetedActorID, uint param, uint useType, int pvp, nint a8)
        {
            ID = actionID;
            var ret = UseActionHook.Original(actionManager, actionType, actionID, targetedActorID, param, useType, pvp, a8);
            OnUseAction?.Invoke(actionManager, actionType, actionID, targetedActorID, param, useType, pvp, a8, ret);
            return ret;
        }
        public static void Initialize() {
            Manager = (Handler.Action*) FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance();
            var A = (nint)FFXIVClientStructs.FFXIV.Client.Game.ActionManager.MemberFunctionPointers.UseAction;
            UseActionHook = DalamudApi.GameInteropProvider.HookFromAddress<UseActionDelegate>(A, UseActionDetour);
            ReceiveActionEffectHook = DalamudApi.GameInteropProvider.HookFromAddress<ReceiveActionEffectDelegate>(DalamudApi.SigScanner.ScanModule("40 55 56 57 41 54 41 55 41 56 41 57 48 8D AC 24"), ReceiveActionEffectDetour);
            UseActionHook.Enable();
            ReceiveActionEffectHook.Enable();
        }

        public static void Dispose()
        {
            UseActionHook?.Dispose();
        }
    }

}
