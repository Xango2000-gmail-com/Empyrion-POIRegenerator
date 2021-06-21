using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eleon.Modding;

namespace POIRegenerator
{
    class API
    {
        public static void Alert(int Target, string Message, string Alert, float Time)
        {
            byte prio = 2;
            if (Alert == "red")
            {
                prio = 0;
            }
            else if (Alert == "yellow")
            {
                prio = 1;
            }
            else
            {
                prio = 2;
            }

            if (Target == 0)
            {
                Storage.GameAPI.Game_Request(CmdId.Request_InGameMessage_AllPlayers, (ushort)Storage.CurrentSeqNr, new IdMsgPrio(Target, Message, prio, Time));
            }
            else if (Target < 999)
            {
                Storage.GameAPI.Game_Request(CmdId.Request_InGameMessage_Faction, (ushort)Storage.CurrentSeqNr, new IdMsgPrio(Target, Message, prio, Time));
            }
            else if (Target > 999)
            {
                Storage.GameAPI.Game_Request(CmdId.Request_InGameMessage_SinglePlayer, (ushort)Storage.CurrentSeqNr, new IdMsgPrio(Target, Message, prio, Time));
            }
        }

        public static int PlayerInfo(int playerID)
        {
            Storage.CurrentSeqNr = CommonFunctions.SeqNrGenerator(Storage.CurrentSeqNr);
            Storage.GameAPI.Game_Request(CmdId.Request_Player_Info, (ushort)Storage.CurrentSeqNr, new Id(playerID));
            return Storage.CurrentSeqNr;
        }

        public static int TextWindowOpen(string TargetPlayer, string Message, String ConfirmText, String CancelText)
        {
            Storage.CurrentSeqNr = CommonFunctions.SeqNrGenerator(Storage.CurrentSeqNr);
            Storage.GameAPI.Game_Request(CmdId.Request_ShowDialog_SinglePlayer, (ushort)Storage.CurrentSeqNr, new DialogBoxData()
            {
                Id = Convert.ToInt32(TargetPlayer),
                MsgText = Message,
                NegButtonText = "Close"
            });
            return Storage.CurrentSeqNr;
        }

        public static int Gents(string playfield)
        {
            Storage.CurrentSeqNr = CommonFunctions.SeqNrGenerator(Storage.CurrentSeqNr);
            Storage.GameAPI.Game_Request(CmdId.Request_GlobalStructure_Update, (ushort)Storage.CurrentSeqNr, new PString(playfield));
            return Storage.CurrentSeqNr;
        }

        public static void ConsoleCommand(String Sendable)
        {
            Storage.GameAPI.Game_Request(CmdId.Request_ConsoleCommand, (ushort)Storage.CurrentSeqNr, new PString(Sendable));
        }

    }
}
