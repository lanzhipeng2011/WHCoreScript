//This code create by CodeEngine

using System;
using Module.Log;
using Google.ProtocolBuffers;
using System.Collections;
using GCGame.Table;
using Clojure;
using UnityEngine;
namespace SPacket.SocketInstance
 {
 public class GC_DO_CLIENT_SCRIPTHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
   GC_DO_CLIENT_SCRIPT packet = (GC_DO_CLIENT_SCRIPT )ipacket;
   if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

   Tab_ClientScript script_line = TableManager.GetClientScriptByID(packet.Scriptid)[0];

   if (script_line != null) 
   {
       if (RT.MainContext == null) 
       {
           RT.InitRuntime();
           GameTrigger.Instance.Load();
       }

       //1 直接执行
       switch (script_line.ScriptType) 
       {
           //触发器
           case 0:
             RT.LoadASTTree(script_line.ScriptName, ((TextAsset)Resources.Load("Tables/Triggers/" + script_line.ScriptName)).text);              
             RT.EvaluateTree(RT.GetASTTree(script_line.ScriptName));
             GameTrigger.Instance.CallEvent("GameLoad");
           break;
           //直接执行
           case 1: 
             string script_str = ((TextAsset)Resources.Load("Tables/Triggers/" + script_line.ScriptName)).text;
             RT.EvaluateTree(RT.GetASTTree(script_str));

           break;
       }

   }
   //enter your logic
   return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
