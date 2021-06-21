using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POIRegenerator
{
    class CommonFunctions
    {
        internal static void LogFile(string FileName, string FileData)
        {
            if (!System.IO.File.Exists(MyEmpyrionMod.ModPath + FileName))
            {
                System.IO.File.Create(MyEmpyrionMod.ModPath + FileName);
            }
            string FileData2 = FileData + Environment.NewLine;
            System.IO.File.AppendAllText(MyEmpyrionMod.ModPath + FileName, FileData2);
        }

        internal static int SeqNrGenerator(int LastSeqNr)
        {
            bool Fail = false;
            int CurrentSeqNr = 1000;
            do
            {
                if (LastSeqNr > 65530)
                {
                    LastSeqNr = 1000;
                }
                CurrentSeqNr = LastSeqNr + 1;
                if (MyEmpyrionMod.SeqNrStorage.ContainsKey(CurrentSeqNr)) { Fail = true; }
            } while (Fail == true);
            return CurrentSeqNr;
        }

        internal static string ArrayConcatenate(int start, string[] array)
        {
            string message = "";
            for (int i = start; i < array.Length; i++)
            {
                message = message + " ";
                message = message + array[i];
            }
            return message;
        }
        public static List<Storage.PlayerIDs> Player(string Fragment, Dictionary<int, Storage.PlayerIDs> Playerdb)
        {
            List<Storage.PlayerIDs> Matches = new List<Storage.PlayerIDs> { };
            foreach (Storage.PlayerIDs Player in Playerdb.Values)
            {
                if (Convert.ToString(Player.EmpyrionID) == Fragment)
                {
                    Matches.Add(Player);
                    return Matches;
                }
                else if (Convert.ToString(Player.PlayerName) == Fragment)
                {
                    Matches.Add(Player);
                    return Matches;
                }
                else if (Convert.ToString(Player.PlayerName).ToLower() == Fragment.ToLower())
                {
                    Matches.Add(Player);
                }
                else if (Convert.ToString(Player.PlayerName).Contains(Fragment))
                {
                    Matches.Add(Player);
                }
                else if (Convert.ToString(Player.PlayerName).ToLower().Contains(Fragment.ToLower()))
                {
                    Matches.Add(Player);
                }
            }
            return Matches;
        }

        public static void FileReader(ushort ThisSeqNr, string File)
        {
            //Checks for simple errors
            string[] Script1 = System.IO.File.ReadAllLines(File);
            for (int i = 0; i < Script1.Count(); ++i)
            {

            }
        }

        public static string ChatmessageHandler(string[] Chatmessage, string Selector)
        {
            List<string> Restring = new List<string>(Chatmessage);
            string Picked = "";
            if (Selector.Contains('*'))
            {
                if (Selector == "1*")
                {
                    Restring.Remove(Restring[0]);
                    Picked = string.Join(" ", Restring.ToArray());
                }
                else if (Selector == "2*")
                {
                    Restring.Remove(Restring[1]);
                    Restring.Remove(Restring[0]);
                    Picked = string.Join(" ", Restring.ToArray());
                }
                else if (Selector == "3*")
                {
                    Restring.Remove(Restring[2]);
                    Restring.Remove(Restring[1]);
                    Restring.Remove(Restring[0]);
                    Picked = string.Join(" ", Restring.ToArray());
                }
                else if (Selector == "4*")
                {
                    Restring.Remove(Restring[3]);
                    Restring.Remove(Restring[2]);
                    Restring.Remove(Restring[1]);
                    Restring.Remove(Restring[0]);
                    Picked = string.Join(" ", Restring.ToArray());
                }
                else if (Selector == "5*")
                {
                    Restring.Remove(Restring[4]);
                    Restring.Remove(Restring[3]);
                    Restring.Remove(Restring[2]);
                    Restring.Remove(Restring[1]);
                    Restring.Remove(Restring[0]);
                    Picked = string.Join(" ", Restring.ToArray());
                }
            }
            else
            {

            }
            return Picked;
        }

    }
}

