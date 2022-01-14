﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hazel;
using UnityEngine;

namespace Nebula.Roles.Crewmate
{
    public class Bait : Role
    {
        static public Color Color = new Color(0f / 255f, 247f / 255f, 255f / 255f);

        public class BaitEvent : Events.LocalEvent
        {
            byte murderId;
            public BaitEvent(byte murderId) : base(0.1f + (float)NebulaPlugin.rnd.NextDouble() * 0.2f)
            {
                this.murderId = murderId;
            }

            public override void OnTerminal()
            {
                RPCEventInvoker.UncheckedCmdReportDeadBody(murderId, PlayerControl.LocalPlayer.PlayerId);
            }
        }
    
        public override void OnMurdered(byte murderId)
        {
            UnityEngine.Debug.Log("Bait murdered.");
            //少しの時差の後レポート
            Events.LocalEvent.Activate(new BaitEvent(murderId));
        }

        public Bait()
            : base("Bait", "bait", Color, RoleCategory.Crewmate, Side.Crewmate, Side.Crewmate,
                 Crewmate.crewmateSideSet, Crewmate.crewmateSideSet, Crewmate.crewmateEndSet,
                 false, false, false, false, false)
        {
        }
    }
}