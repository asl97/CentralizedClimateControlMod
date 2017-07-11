﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace CentralizedClimateControl
{
    public class CompAirFlowConsumer : CompAirFlow
    {
        public const string AirFlowOutputKey = "CentralizedClimateControl.AirFlowOutput";
        public const string IntakeTempKey = "CentralizedClimateControl.Consumer.ConvertedTemperature";
        public const string FlowEfficiencyKey = "CentralizedClimateControl.Consumer.FlowEfficiencyKey";
        public const string ThermalEfficiencyKey = "CentralizedClimateControl.Consumer.ThermalEfficiencyKey";

        public float ConvertedTemperature = 0.0f;
        protected CompFlickable FlickableComp;

        public float ExhaustAirFlow
        {
            get
            {
                return this.Props.baseAirExhaust;
            }
        }

        public float FlowEfficiency
        {
            get
            {
                return AirFlowNet.FlowEfficiency;
            }
        }

        public string DebugString
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(this.parent.LabelCap + " CompAirFlowConsumer:");
                stringBuilder.AppendLine("   ConvertedTemperature: " + ConvertedTemperature);
                return stringBuilder.ToString();
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            CentralizedClimateControlUtility.GetNetManager(this.parent.Map).RegisterConsumer(this);
            this.FlickableComp = this.parent.GetComp<CompFlickable>();

            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void PostDeSpawn(Map map)
        {
            CentralizedClimateControlUtility.GetNetManager(map).DeregisterConsumer(this);
            ResetFlowVariables();
            base.PostDeSpawn(map);
        }

        public override string CompInspectStringExtra()
        {
            if (!IsOperating())
            {
                return base.CompInspectStringExtra();
            }

            var convertedTemp = ConvertedTemperature.ToStringTemperature("F0");
            string str = IntakeTempKey.Translate(new object[] { convertedTemp });

            var flowPercent = Mathf.FloorToInt(AirFlowNet.FlowEfficiency * 100) + "%";
            str += "\n";
            str += FlowEfficiencyKey.Translate(new object[] { flowPercent });

            var thermalPercent = Mathf.FloorToInt(AirFlowNet.ThermalEfficiency * 100) + "%";
            str += "\n";
            str += ThermalEfficiencyKey.Translate(new object[] { thermalPercent });

            return str + "\n" + base.CompInspectStringExtra();
        }

        public void TickRare()
        {
            if (!IsOperating())
            {
                return;
            }

            ConvertedTemperature = AirFlowNet.AverageConvertedTemperature;
        }

        public override void ResetFlowVariables()
        {
            ConvertedTemperature = 0.0f;
            base.ResetFlowVariables();
        }
    }
}