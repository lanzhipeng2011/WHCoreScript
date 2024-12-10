//This code create by CodeEngine

using System;
 using Module.Log;
 using Google.ProtocolBuffers;
 using System.Collections;
using GCGame.Table;
using GCGame;

namespace SPacket.SocketInstance
 {
 public class GC_PLAY_SOUNDSHandler : Ipacket
 {
 public uint Execute(PacketDistributed ipacket)
 {
     GC_PLAY_SOUNDS packet = (GC_PLAY_SOUNDS )ipacket;
     if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
     //enter your logic


     Tab_Sounds soundsTab = TableManager.GetSoundsByID(packet.SoundID, 0);
     if (soundsTab == null)
     {
         //LogModule.DebugLog("GC_PLAY_SOUNDSHandler sound name " + packet.SoundName + " is null");
         return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
     }

     if (packet.SoundType == (int)GC_PLAY_SOUNDS.SOUNDTYPE.SOUND_EFFECT)
     {
         GameManager.gameManager.SoundManager.PlaySoundEffect(packet.SoundID);
     }
     else if (packet.SoundType == (int)GC_PLAY_SOUNDS.SOUNDTYPE.SOUND_SCENE)
     {
         GameManager.gameManager.SoundManager.PlayBGMusic(packet.SoundID, soundsTab.FadeOutTime, soundsTab.FadeInTime);
     }

     return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
 }
 }
 }
