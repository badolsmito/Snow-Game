// TODO: Add the OnServerLeave thing to check if a player was in a match when he left and if he was the last one of his team. this would end the game
// and grant the win to the opposing team.
// TODO: Disable NPC spawn, turn on Day-only and no events.
// TODO: Find a better way to guarantee PVP statuses.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;

namespace SnowGame
{
    [ApiVersion(2, 1)]
    public class SnowGame : TerrariaPlugin
    {

        public override string Author => "badols mito";


        public override string Description => "A plugin for a winter-themed minigame.";


        public override string Name => "Snow Game";


        public override Version Version => new Version(1, 0, 0, 0);


        public SnowGame(Main game) : base(game)
        {

        }


        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
            GetDataHandlers.TogglePvp += OnTogglePvp;
            GetDataHandlers.PlayerTeam += OnPlayerTeamChange;
            GetDataHandlers.KillMe += OnKillMe;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
                GetDataHandlers.TogglePvp -= OnTogglePvp;
                GetDataHandlers.PlayerTeam -= OnPlayerTeamChange;
                GetDataHandlers.KillMe -= OnKillMe;

            }
            base.Dispose(disposing);
        }

        private void OnInitialize(EventArgs args)
        {
            Commands.ChatCommands.Add(new Command(WhiteTeam.Join, "join"));
            //Commands.ChatCommands.Add(new Command(ForceGameInit, "gameinit"));
        }

        private void OnJoin(JoinEventArgs args)
        {
            WhiteTeam.members.Add(TShock.Players[args.Who].Index);
        }

        private void OnTogglePvp(object sender, GetDataHandlers.TogglePvpEventArgs args)
        {

            args.Player.SetPvP(true);
            TShock.Players[args.PlayerId].SendInfoMessage("You cannot toggle PVP off!");

        }

        private void OnPlayerTeamChange(object sender, GetDataHandlers.PlayerTeamEventArgs args)
        {
            // This can probably be written in a better way.
            args.Player.SendInfoMessage("You cannot change your team!");
            if (RedTeam.members.Contains(args.Player.Index))
            {
                args.Player.SetTeam((int)TeamID.Red);
            }
            else if (BlueTeam.members.Contains(args.Player.Index))
            {
                args.Player.SetTeam((int)TeamID.Blue);
            }
            else if (WhiteTeam.members.Contains(args.Player.Index))
            {
                args.Player.SetTeam((int)TeamID.White);
            }
        }

        private void OnKillMe(object sender, GetDataHandlers.KillMeEventArgs args)
        {
            TShock.Utils.Broadcast($"{TShock.Players[args.PlayerId].Name} just got killed!", Microsoft.Xna.Framework.Color.Aqua);
            if (args.Player.Team == (int)TeamID.Red)
            {
                TShock.Utils.Broadcast("10 points go to the blue team!", Microsoft.Xna.Framework.Color.Blue);
                BlueTeam.score += 10;
            }
            else
            {
                TShock.Utils.Broadcast("10 points go to the red team!", Microsoft.Xna.Framework.Color.Red);
                RedTeam.score += 10;
            }
            TShock.Utils.Broadcast($"The red team has currently {RedTeam.score} points and the blue team has {BlueTeam.score} points!", Microsoft.Xna.Framework.Color.AliceBlue);
        }
        public enum TeamID
        {
            White,
            Red,
            Green,
            Blue,
            Yellow,
            Pink
        }



        public class GameTeam
        {
            public TeamID color;
            public int score;
            public List<int> members;
            public GameTeam(TeamID color, int score, List<int> members)
            {
                this.color = color;
                this.score = score;
                this.members = members;
            }

            public void Join(CommandArgs args)
            {
                if (!WhiteTeam.members.Contains(args.Player.Index))
                {
                    args.Player.SendInfoMessage("You're already in a team!");
                }

                else
                {
                    WhiteTeam.members.Remove(args.Player.Index);

                    if (RedTeam.members.Count > BlueTeam.members.Count)
                    {
                        BlueTeam.members.Add(args.Player.Index);
                        args.Player.SetTeam((int)TeamID.Blue);
                    }
                    else
                    {
                        RedTeam.members.Add(args.Player.Index);
                        args.Player.SetTeam((int)TeamID.Red);
                    }

                    args.Player.SendInfoMessage("Giving you equipment!");

                    args.TPlayer.inventory[0] = TShock.Utils.GetItemById(1319); // Puts the Snowball Cannon into the first inventory slot.
                    NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, args.Player.Index, 0); // Sends the packet that does just that.

                    args.TPlayer.inventory[54] = TShock.Utils.GetItemById(949); // Puts Snowballs into the first Ammo slot.
                    args.TPlayer.inventory[54].stack = 999;
                    NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, args.Player.Index, 54, 0, 999);

                    if (args.TPlayer.Male)
                    {
                        args.TPlayer.armor[0] = TShock.Utils.GetItemById(803); // Puts the Snow Hood into the head armor slot.
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, args.Player.Index, 59);

                        args.TPlayer.armor[1] = TShock.Utils.GetItemById(804); // Snow Coat
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, args.Player.Index, 60);

                        args.TPlayer.armor[2] = TShock.Utils.GetItemById(805); // Snow Pants
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, args.Player.Index, 61);

                    }
                    else
                    {
                        args.TPlayer.armor[0] = TShock.Utils.GetItemById(978); // Pink Snow Hood
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, args.Player.Index, 59);

                        args.TPlayer.armor[1] = TShock.Utils.GetItemById(979); // Pink Snow Coat
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, args.Player.Index, 60);

                        args.TPlayer.armor[2] = TShock.Utils.GetItemById(980); // Pink Snow Pants
                        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, args.Player.Index, 61);
                    }

                    args.Player.SetPvP(true);
                    args.Player.Teleport(1380f * 16, 255f * 16);
                    if (RedTeam.members.Count > 0 && BlueTeam.members.Count > 0) { GameInit(); }
                }
            }

        }

        public static GameTeam WhiteTeam = new GameTeam(TeamID.White, 0, new List<int>());
        private static GameTeam RedTeam = new GameTeam(TeamID.Red, 0, new List<int>());
        private static GameTeam BlueTeam = new GameTeam(TeamID.Blue, 0, new List<int>());

        static private void GameInit()
        {
            System.Timers.Timer gameTimer = new System.Timers.Timer(300000);
            TShock.Utils.Broadcast("The game has begun!", Microsoft.Xna.Framework.Color.Green);
            gameTimer.Enabled = true;
            gameTimer.Elapsed += OnGameElapse;
        }

        static private void OnGameElapse(object source, System.Timers.ElapsedEventArgs args)
        {
            TShock.Utils.Broadcast($"The {(RedTeam.score > BlueTeam.score ? "red" : "blue")} team won with {(RedTeam.score > BlueTeam.score ? RedTeam.score : BlueTeam.score)} score points!", Microsoft.Xna.Framework.Color.Blue);
            foreach (int member in RedTeam.members)
            {
                TShock.Players[member].Teleport(1380f * 16, 255f * 16);
                TShock.Players[member].SetPvP(false);
                TShock.Players[member].SetTeam((int)TeamID.White);

                for (int i = 0; i < 59; i++)
                {
                    TShock.Players[member].TPlayer.inventory[i] = TShock.Utils.GetItemById(0);
                    NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, member, i);
                }

                for (int i = 0; i < 19; i++)
                {
                    TShock.Players[member].TPlayer.armor[i] = TShock.Utils.GetItemById(0);
                    NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, member, i + 58);
                }

                WhiteTeam.members.Add(member);
                RedTeam.members.Remove(member);
            }

            foreach (int member in BlueTeam.members)
            {
                TShock.Players[member].Teleport(1380f * 16, 255f * 16);
                TShock.Players[member].SetPvP(false);
                TShock.Players[member].SetTeam((int)TeamID.White);

                for (int i = 0; i < 59; i++)
                {
                    TShock.Players[member].TPlayer.inventory[i] = TShock.Utils.GetItemById(0);
                    NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, member, i);
                }

                for (int i = 0; i < 19; i++)
                {
                    TShock.Players[member].TPlayer.armor[i] = TShock.Utils.GetItemById(0);
                    NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, member, i + 58);
                }

                WhiteTeam.members.Add(member);
                BlueTeam.members.Remove(member);
            }
        }
    }
}
