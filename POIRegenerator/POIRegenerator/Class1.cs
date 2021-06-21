using System;
using System.Collections.Generic;
using System.Linq;
using Eleon.Modding;
//using ProtoBuf;
using YamlDotNet.Serialization;


namespace POIRegenerator
{
    public class MyEmpyrionMod : ModInterface
    {
        public static string ModVersion = "POIRegenerator v1.0.0";
        public static string ModPath = "Content\\Mods\\POIRegenerator\\";
        //public object ModPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); Does Not Work with multiple Mods
        internal static Dictionary<int, Storage.StorableData> SeqNrStorage = new Dictionary<int, Storage.StorableData> { };
        public int thisSeqNr = 2000;
        private Setup.Root SetupYaml = new Setup.Root { };
        public int Ticker = 0;
        public Dictionary<string, int> LoadedPlayfields = new Dictionary<string, int> { };

        //########################################################################################################################################################
        //################################################ This is where the actual Empyrion Modding API stuff Begins ############################################
        //########################################################################################################################################################
        public void Game_Start(ModGameAPI gameAPI)
        {
            Storage.GameAPI = gameAPI;
            System.IO.File.WriteAllText(ModPath + "debug.txt", ""); //Blanks the debug.txt file on server start. This is where I dump all my debuggig text using CommonFunctions.LogFile
            //SetupYaml = Setup.Retrieve(ModPath + "setup.yaml"); //reads setup.yaml
        }

        public void Game_Event(CmdId cmdId, ushort seqNr, object data)
        {
            try
            {
                switch (cmdId)
                {
                    case CmdId.Event_ChatMessage:
                        //Triggered when player says something in-game
                        ChatInfo Received_ChatInfo = (ChatInfo)data;
                        break;


                    case CmdId.Event_Player_Connected:
                        //Triggered when a player logs on
                        Id Received_PlayerConnected = (Id)data;
                        break;


                    case CmdId.Event_Player_Disconnected:
                        //Triggered when a player logs off
                        Id Received_PlayerDisconnected = (Id)data;
                        break;


                    case CmdId.Event_Player_ChangedPlayfield:
                        //Triggered when a player changes playfield
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Player_ChangePlayfield, (ushort)CurrentSeqNr, new IdPlayfieldPositionRotation( [PlayerID], [Playfield Name], [PVector3 position], [PVector3 Rotation] ));
                        IdPlayfield Received_PlayerChangedPlayfield = (IdPlayfield)data;
                        break;


                    case CmdId.Event_Playfield_Loaded:
                        //Triggered when a player goes to a playfield that isnt currently loaded in memory
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Load_Playfield, (ushort)CurrentSeqNr, new PlayfieldLoad( [float nSecs], [string nPlayfield], [int nProcessId] ));
                        PlayfieldLoad Received_PlayfieldLoaded = (PlayfieldLoad)data;
                        LoadedPlayfields[Received_PlayfieldLoaded.playfield] = Received_PlayfieldLoaded.processId;
                        CommonFunctions.LogFile("debug.txt", "PlayfieldLoad=" + Received_PlayfieldLoaded.playfield);
                        Storage.StorableData function = new Storage.StorableData { };
                        function.function = "Gents";
                        function.Match = Received_PlayfieldLoaded.playfield;
                        function.Requested = "Gents";
                        thisSeqNr = API.Gents(Received_PlayfieldLoaded.playfield);
                        /*
                        foreach (string playfield in LoadedPlayfields.Keys)
                        {
                            thisSeqNr = API.Gents(playfield);
                        }
                        */
                        SeqNrStorage[thisSeqNr] = function;

                        break;


                    case CmdId.Event_Playfield_Unloaded:
                        //Triggered when there are no players left in a playfield
                        PlayfieldLoad Received_PlayfieldUnLoaded = (PlayfieldLoad)data;
                        LoadedPlayfields.Remove(Received_PlayfieldUnLoaded.playfield);
                        //CommonFunctions.LogFile("debug.txt", "PlayfieldUnload=" + Received_PlayfieldUnLoaded.playfield);
                        break;


                    case CmdId.Event_Faction_Changed:
                        //Triggered when an Entity (player too?) changes faction
                        FactionChangeInfo Received_FactionChange = (FactionChangeInfo)data;
                        break;


                    case CmdId.Event_Statistics:
                        //Triggered on various game events like: Player Death, Entity Power on/off, Remove/Add Core
                        StatisticsParam Received_EventStatistics = (StatisticsParam)data;
                        break;


                    case CmdId.Event_Player_DisconnectedWaiting:
                        //Triggered When a player is having trouble logging into the server
                        Id Received_PlayerDisconnectedWaiting = (Id)data;
                        break;


                    case CmdId.Event_TraderNPCItemSold:
                        //Triggered when a player buys an item from a trader
                        TraderNPCItemSoldInfo Received_TraderNPCItemSold = (TraderNPCItemSoldInfo)data;
                        break;


                    case CmdId.Event_Player_List:
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Player_List, (ushort)CurrentSeqNr, null));
                        IdList Received_PlayerList = (IdList)data;
                        break;


                    case CmdId.Event_Player_Info:
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Player_Info, (ushort)CurrentSeqNr, new Id( [playerID] ));
                        PlayerInfo Received_PlayerInfo = (PlayerInfo)data;
                        break;


                    case CmdId.Event_Player_Inventory:
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Player_GetInventory, (ushort)CurrentSeqNr, new Id( [playerID] ));
                        Inventory Received_PlayerInventory = (Inventory)data;
                        break;


                    case CmdId.Event_Player_ItemExchange:
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Player_ItemExchange, (ushort)CurrentSeqNr, new ItemExchangeInfo( [id], [title], [description], [buttontext], [ItemStack[]] ));
                        ItemExchangeInfo Received_ItemExhcangeInfo = (ItemExchangeInfo)data;
                        break;


                    case CmdId.Event_DialogButtonIndex:
                        //All of This is a Guess
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_ShowDialog_SinglePlayer, (ushort)CurrentSeqNr, new IdMsgPrio( [int nId], [string nMsg], [byte nPrio], [float nTime] )); //for Prio: 0=Red, 1=Yellow, 2=Blue
                        //DialogBoxData Received_DialoxButtonIndex = (DialogBoxData)data;
                        break;


                    case CmdId.Event_Player_Credits:
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Player_Credits, (ushort)CurrentSeqNr, new Id( [PlayerID] ));
                        IdCredits Received_PlayerCredits = (IdCredits)data;
                        break;


                    case CmdId.Event_Player_GetAndRemoveInventory:
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Player_GetAndRemoveInventory, (ushort)CurrentSeqNr, new Id( [playerID] ));
                        Inventory Received_PlayerGetRemoveInventory = (Inventory)data;
                        break;


                    case CmdId.Event_Playfield_List:
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Playfield_List, (ushort)CurrentSeqNr, null));
                        PlayfieldList Received_PlayfieldList = (PlayfieldList)data;
                        break;


                    case CmdId.Event_Playfield_Stats:
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Playfield_Stats, (ushort)CurrentSeqNr, new PString( [Playfield Name] ));
                        PlayfieldStats Received_PlayfieldStats = (PlayfieldStats)data;
                        break;


                    case CmdId.Event_Playfield_Entity_List:
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Playfield_Entity_List, (ushort)CurrentSeqNr, new PString( [Playfield Name] ));
                        PlayfieldEntityList Received_PlayfieldEntityList = (PlayfieldEntityList)data;
                        break;


                    case CmdId.Event_Dedi_Stats:
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Dedi_Stats, (ushort)CurrentSeqNr, null));
                        DediStats Received_DediStats = (DediStats)data;
                        break;


                    case CmdId.Event_GlobalStructure_List:
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_GlobalStructure_List, (ushort)CurrentSeqNr, null));
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_GlobalStructure_Update, (ushort)CurrentSeqNr, new PString( [Playfield Name] ));
                        GlobalStructureList Received_GlobalStructureList = (GlobalStructureList)data;
                        //foreach (GlobalStructureInfo item in Structs.globalStructures[storedInfo[seqNr].PlayerInfo.playfield])
                        if (SeqNrStorage.Keys.Contains(seqNr))
                        {
                            Storage.StorableData RequestTracker = SeqNrStorage[seqNr];
                            SeqNrStorage.Remove(seqNr);
                            if (RequestTracker.Requested == "Gents" && RequestTracker.function == "Gents" && Received_GlobalStructureList.globalStructures.Keys.Contains(RequestTracker.Match))
                            {
                                foreach (GlobalStructureInfo structure in Received_GlobalStructureList.globalStructures[RequestTracker.Match])
                                {
                                    if (structure.factionId == 0) //POIs, OPVs, PVs, etc.
                                    {
                                        int pid = LoadedPlayfields[RequestTracker.Match];
                                        string sendable = "remoteex pf=" + pid + " 'regenerate " + structure.id + "'";
                                        CommonFunctions.LogFile("debug.txt", sendable);
                                        API.ConsoleCommand(sendable);
                                    }
                                    else if (structure.type == 7) //Asteroids
                                    {
                                        int pid = LoadedPlayfields[RequestTracker.Match];
                                        string sendable = "remoteex pf=" + pid + " 'regenerate " + structure.id + "'";
                                        CommonFunctions.LogFile("debug.txt", sendable);
                                        API.ConsoleCommand(sendable);
                                    }
                                    CommonFunctions.LogFile("TestData.txt", "Type=" + structure.type + "   ID=" + structure.id + "   FactionID=" + structure.factionId + "    Name=" + structure.name + "   CoreType=" + structure.coreType + "   FactionGroup=" + structure.factionGroup);
                                }
                            }
                        }
                        /*
                        foreach (string playfield in Received_GlobalStructureList.globalStructures.Keys)
                        {
                            CommonFunctions.LogFile("TestData.txt", "Playfield=" + playfield);
                            foreach (GlobalStructureInfo structure in Received_GlobalStructureList.globalStructures[playfield])
                            {
                                if (structure.factionId == 0) //POIs, OPVs, PVs, etc.
                                {
                                    int pid = LoadedPlayfields[playfield];
                                    string sendable = "remoteex pf=" + pid + " 'regenerate " + structure.id + "'";
                                    CommonFunctions.LogFile("debug.txt", sendable);
                                    API.ConsoleCommand(sendable);
                                }
                                else if (structure.type == 7) //Asteroids
                                {
                                    int pid = LoadedPlayfields[playfield];
                                    string sendable = "remoteex pf=" + pid + " 'regenerate " + structure.id + "'";
                                    CommonFunctions.LogFile("debug.txt", sendable);
                                    API.ConsoleCommand(sendable);
                                }
                                CommonFunctions.LogFile("TestData.txt", "Type=" + structure.type + "   ID=" + structure.id + "   FactionID=" + structure.factionId + "    Name=" + structure.name + "   CoreType=" + structure.coreType + "   FactionGroup="+ structure.factionGroup);
                            }
                        }
                        */
                        break;


                    case CmdId.Event_Entity_PosAndRot:
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Entity_PosAndRot, (ushort)CurrentSeqNr, new Id( [EntityID] ));
                        IdPositionRotation Received_EntityPosRot = (IdPositionRotation)data;
                        break;


                    case CmdId.Event_Get_Factions:
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Get_Factions, (ushort)CurrentSeqNr, new Id( [int] )); //Requests all factions from a certain Id onwards. If you want all factions use Id 1.
                        FactionInfoList Received_FactionInfoList = (FactionInfoList)data;
                        break;


                    case CmdId.Event_NewEntityId:
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_NewEntityId, (ushort)CurrentSeqNr, null));
                        Id Request_NewEntityId = (Id)data;
                        break;


                    case CmdId.Event_Structure_BlockStatistics:
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Structure_BlockStatistics, (ushort)CurrentSeqNr, new Id( [EntityID] ));
                        IdStructureBlockInfo Received_StructureBlockStatistics = (IdStructureBlockInfo)data;
                        break;


                    case CmdId.Event_AlliancesAll:
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_AlliancesAll, (ushort)CurrentSeqNr, null));
                        AlliancesTable Received_AlliancesAll = (AlliancesTable)data;
                        break;


                    case CmdId.Event_AlliancesFaction:
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_AlliancesFaction, (ushort)CurrentSeqNr, new AlliancesFaction( [int nFaction1Id], [int nFaction2Id], [bool nIsAllied] ));
                        AlliancesFaction Received_AlliancesFaction = (AlliancesFaction)data;
                        break;


                    case CmdId.Event_BannedPlayers:
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_GetBannedPlayers, (ushort)CurrentSeqNr, null ));
                        BannedPlayerData Received_BannedPlayers = (BannedPlayerData)data;
                        break;


                    case CmdId.Event_GameEvent:
                        //Triggered by PDA Events
                        GameEventData Received_GameEvent = (GameEventData)data;
                        break;


                    case CmdId.Event_Ok:
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Player_SetInventory, (ushort)CurrentSeqNr, new Inventory(){ [changes to be made] });
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Player_AddItem, (ushort)CurrentSeqNr, new IdItemStack(){ [changes to be made] });
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Player_SetCredits, (ushort)CurrentSeqNr, new IdCredits( [PlayerID], [Double] ));
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Player_AddCredits, (ushort)CurrentSeqNr, new IdCredits( [PlayerID], [+/- Double] ));
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Blueprint_Finish, (ushort)CurrentSeqNr, new Id( [PlayerID] ));
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Blueprint_Resources, (ushort)CurrentSeqNr, new BlueprintResources( [PlayerID], [List<ItemStack>], [bool ReplaceExisting?] ));
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Entity_Teleport, (ushort)CurrentSeqNr, new IdPositionRotation( [EntityId OR PlayerID], [Pvector3 Position], [Pvector3 Rotation] ));
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Entity_ChangePlayfield , (ushort)CurrentSeqNr, new IdPlayfieldPositionRotation( [EntityId OR PlayerID], [Playfield],  [Pvector3 Position], [Pvector3 Rotation] ));
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Entity_Destroy, (ushort)CurrentSeqNr, new Id( [EntityID] ));
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Entity_Destroy2, (ushort)CurrentSeqNr, new IdPlayfield( [EntityID], [Playfield] ));
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Entity_SetName, (ushort)CurrentSeqNr, new Id( [EntityID] )); Wait, what? This one doesn't make sense. This is what the Wiki says though.
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Entity_Spawn, (ushort)CurrentSeqNr, new EntitySpawnInfo()); Doesn't make sense to me.
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_Structure_Touch, (ushort)CurrentSeqNr, new Id( [EntityID] ));
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_InGameMessage_SinglePlayer, (ushort)CurrentSeqNr, new IdMsgPrio( [int nId], [string nMsg], [byte nPrio], [float nTime] )); //for Prio: 0=Red, 1=Yellow, 2=Blue
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_InGameMessage_Faction, (ushort)CurrentSeqNr, new IdMsgPrio( [int nId], [string nMsg], [byte nPrio], [float nTime] )); //for Prio: 0=Red, 1=Yellow, 2=Blue
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_InGameMessage_AllPlayers, (ushort)CurrentSeqNr, new IdMsgPrio( [int nId], [string nMsg], [byte nPrio], [float nTime] )); //for Prio: 0=Red, 1=Yellow, 2=Blue
                        //Triggered by API mod request GameAPI.Game_Request(CmdId.Request_ConsoleCommand, (ushort)CurrentSeqNr, new PString( [Telnet Command] ));

                        //uh? Not Listed in Wiki... Received_ = ()data;
                        break;


                    case CmdId.Event_Error:
                        //Triggered when there is an error coming from the API
                        ErrorInfo Received_ErrorInfo = (ErrorInfo)data;
                        if (SeqNrStorage.Keys.Contains(seqNr))
                        {
                            CommonFunctions.LogFile("Debug.txt", "API Error:");
                            CommonFunctions.LogFile("Debug.txt", "ErrorType: " + Received_ErrorInfo.errorType);
                            CommonFunctions.LogFile("Debug.txt", "");
                        }
                        break;


                    case CmdId.Event_PdaStateChange:
                        //Triggered by PDA: chapter activated/deactivated/completed
                        PdaStateInfo Received_PdaStateChange = (PdaStateInfo)data;
                        break;


                    case CmdId.Event_ConsoleCommand:
                        //Triggered when a player uses a Console Command in-game
                        ConsoleCommandInfo Received_ConsoleCommandInfo = (ConsoleCommandInfo)data;
                        break;


                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.LogFile("Debug.txt", "Message: " + ex.Message);
                CommonFunctions.LogFile("Debug.txt", "Data: " + ex.Data);
                CommonFunctions.LogFile("Debug.txt", "HelpLink: " + ex.HelpLink);
                CommonFunctions.LogFile("Debug.txt", "InnerException: " + ex.InnerException);
                CommonFunctions.LogFile("Debug.txt", "Source: " + ex.Source);
                CommonFunctions.LogFile("Debug.txt", "StackTrace: " + ex.StackTrace);
                CommonFunctions.LogFile("Debug.txt", "TargetSite: " + ex.TargetSite);
            }

        }
        public void Game_Update()
        {
            //Triggered whenever Empyrion experiences "Downtime", roughly 75-100 times per second
            //Ticker = Ticker + 1;
            if (Ticker == 300)
            {
                Ticker = 0;
                CommonFunctions.LogFile("debug.txt", "Ticker=" + Ticker);
                Storage.StorableData function = new Storage.StorableData { };
                function.function = "Gents";
                function.Requested = "Gents";
                foreach (string playfield in LoadedPlayfields.Keys)
                {
                    thisSeqNr = API.Gents(playfield);
                }
                SeqNrStorage[thisSeqNr] = function;
            }
        }
        public void Game_Exit()
        {
            //Triggered when the server is Shutting down. Does NOT pause the shutdown.
        }

    }
}

