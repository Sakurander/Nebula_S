﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using Hazel;
using Nebula.Objects;

namespace Nebula.Roles.Crewmate
{
    public class Sheriff : Role
    {
        static public Color Color = new Color(240f/255f, 191f/255f, 0f);

        private CustomButton killButton;

        private Module.CustomOption killCooldownOption;

        public override void MyPlayerControlUpdate()
        {
            Game.MyPlayerData data = Game.GameData.data.myData;
            data.currentTarget = Patches.PlayerControlPatch.SetMyTarget();
            Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Color.yellow);
        }
        public override void ButtonInitialize(HudManager __instance)
        {
            if (killButton != null)
            {
                killButton.Destroy();
            }
            killButton = new CustomButton(
                () =>
                {
                    byte targetId = Game.GameData.data.myData.currentTarget.PlayerId;

                    MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UncheckedMurderPlayer, Hazel.SendOption.Reliable, -1);
                    killWriter.Write(PlayerControl.LocalPlayer.PlayerId);
                    killWriter.Write(targetId);
                    killWriter.Write(byte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                    RPCEvents.UncheckedMurderPlayer(PlayerControl.LocalPlayer.PlayerId, targetId, Byte.MaxValue);


                    killButton.Timer = killButton.MaxTimer;
                    Game.GameData.data.myData.currentTarget = null;
                },
                () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
                () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
                () => { killButton.Timer = killButton.MaxTimer; },
                __instance.KillButton.graphic.sprite,
                new Vector3(0f, 1f, 0),
                __instance,
                KeyCode.Q
            );
            killButton.MaxTimer = killCooldownOption.getFloat();
        }

        public override void OnMeetingEnd() {
            killButton.Timer = killButton.MaxTimer;
        }

        public override void CleanUp()
        {
            if (killButton != null)
            {
                killButton.Destroy();
                killButton = null;
            }
        }

        public override void LoadOptionData()
        {
            killCooldownOption = CreateOption(Color.white, "killCooldownOption", 30f, 10f, 60f, 2.5f);
        }

        public Sheriff()
            : base("Sheriff", "sheriff", Color, RoleCategory.Crewmate, Side.Crewmate, Side.Crewmate,
                 Crewmate.crewmateSideSet, Crewmate.crewmateSideSet, Crewmate.crewmateEndSet,
                 false, false, false, false, false)
        {
            killButton = null;
        }
    }
}