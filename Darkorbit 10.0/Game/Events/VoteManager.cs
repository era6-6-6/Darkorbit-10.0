using System;
using System.Collections.Generic;

namespace Darkorbit.Game.Events
{
    class VoteManager
    {

        private static int maxVoteSP = 10;
        private static int maxVotesIN = 15;
        private static int maxVotesJP = 20;
        private static int maxVotesHT = 40;
        private static int votosInvasion = 0;
        private static int votosSpaceball = 0;
        private static int votosJackpot = 0;
        private static int votosHitac = 0;
        public static List<int> listaVotosSP = new List<int>();
        public static List<int> listaVotosIN = new List<int>();
        public static List<int> listaVotosJP = new List<int>();
        public static List<int> listaVotosHT = new List<int>();

        public static int VotosInvasion { get => votosInvasion; set => votosInvasion = value; }
        public static int VotosSpaceball { get => votosSpaceball; set => votosSpaceball = value; }
        public static int VotosJackpot { get => votosJackpot; set => votosJackpot = value; }
        public static int VotosHitac { get => votosHitac; set => votosHitac = value; }
        public static int MaxVoteSP { get => maxVoteSP; set => maxVoteSP = value; }
        public static int MaxVotesIN { get => maxVotesIN; set => maxVotesIN = value; }
        public static int MaxVotesJP { get => maxVotesJP; set => maxVotesJP = value; }
        public static int MaxVotesHT { get => maxVotesHT; set => maxVotesHT = value; }


        public static bool checkVoteIN(int id)
        {
            if (listaVotosIN.Contains(id)) return true; else { listaVotosIN.Add(id); return false; }
        }
        public static bool checkVoteJP(int id)
        {
            if (listaVotosJP.Contains(id)) return true; else { listaVotosJP.Add(id); return false; }
        }
        public static bool checkVoteSP(int id)
        {
            if (listaVotosSP.Contains(id)) return true; else { listaVotosSP.Add(id); return false; }
        }
        public static bool checkVoteHT(int id)
        {
            if (listaVotosHT.Contains(id)) return true; else { listaVotosHT.Add(id); return false; }
        }
        public static void checkStart()
        {
            if (VotosInvasion == MaxVotesIN) { EventManager.Invasion.Start(); listaVotosIN.Clear(); VotosInvasion = 0; }
            else
            {
                GameManager.SendChatSystemMessage($"[{MaxVotesIN - VotosInvasion}]  votes left for INVASION  to start. Use /invasionvote in chat to vote. ");
                GameManager.SendPacketToAll($"0|A|STD|[{MaxVotesIN - VotosInvasion}]  votes left for INVASION . ");
            }

            if (VotosJackpot == 20) { EventManager.JackpotBattle.Start(); listaVotosJP.Clear(); VotosJackpot = 0; }
            else
            {
                GameManager.SendChatSystemMessage($"[{MaxVotesJP - VotosJackpot}]  votes left for JACKPOT to start. Use /jackpotvote in chat to vote. ");
                GameManager.SendPacketToAll($"0|A|STD|[{MaxVotesJP - VotosJackpot}]  votes left for JACKPOT. ");
            }

            if (VotosJackpot == 40) { EventManager.Hitac.Start(); listaVotosHT.Clear(); VotosHitac = 0; }
            else
            {
                GameManager.SendChatSystemMessage($"[{MaxVotesHT - VotosHitac}]  votes left for Hitac 2.0 to start. Use /hitacvote in chat to vote. ");
                GameManager.SendPacketToAll($"0|A|STD|[{MaxVotesHT - VotosHitac}]  votes left for Hitac 2.0. ");
            }

            if (VotosSpaceball == 10) { EventManager.Spaceball.Start(); listaVotosSP.Clear(); VotosSpaceball = 0; }

            else
            {
                GameManager.SendChatSystemMessage($"[{MaxVoteSP - VotosSpaceball}]  votes left for SPACEBALL to start. Use /spaceballvote in chat to vote. ");
                GameManager.SendPacketToAll($"0|A|STD|[{MaxVoteSP - VotosSpaceball}]  votes left for SPACEBALL.  ");
            }


        }
    }

}
