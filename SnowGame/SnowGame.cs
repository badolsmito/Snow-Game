// TOOD: Have players receive equipment when they use /join
// TODO: Change the SSC file to not give the players any sort of equipment.s
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
            Commands.ChatCommands.Add(new Command(ForceGameInit, "gameinit"));
        }

        private void OnJoin(JoinEventArgs args)
        {
            WhiteTeam.members.Add(TShock.Players[args.Who].Index);
            // TODO: Remove the permission of the player to change teams and pvp mode.
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
                TShock.Utils.Broadcast("10 points go to the blue team!", Microsoft.Xna.Framework.Color.Blue); // Putting in random stuff as the second arg.
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

                    args.Player.SetPvP(true);
                    args.Player.Teleport(1078f * 16, 132f * 16);
                }
            }

        }

        public static GameTeam WhiteTeam = new GameTeam(TeamID.White, 0, new List<int>());
        private static GameTeam RedTeam = new GameTeam(TeamID.Red, 0, new List<int>());
        private static GameTeam BlueTeam = new GameTeam(TeamID.Blue, 0, new List<int>());
        
        // To be removed later. This is merely for Testing purposed.
        private void ForceGameInit(CommandArgs args)
        {
            // TODO: Add 5 minutes timer
            System.Timers.Timer gameTimer = new System.Timers.Timer(30000); // Should be 5 minutes (300000) when no longer testing
            gameTimer.Enabled = true;
            gameTimer.Elapsed += OnForceGameInit;
            // Teleport players to certain sides based on what their team is.
            // Once the game ends, print out messages indicating who won and their score.
        }

        private void OnForceGameInit(object source, System.Timers.ElapsedEventArgs args)
        {
            TShock.Utils.Broadcast($"The {(RedTeam.score > BlueTeam.score ? "red" : "blue")} team won with {(RedTeam.score > BlueTeam.score ? RedTeam.score : BlueTeam.score)} score points!", Microsoft.Xna.Framework.Color.Blue);
            foreach (int member in RedTeam.members)
            {
                TShock.Players[member].Teleport(1078f * 16, 132f * 16);
                TShock.Players[member].SetPvP(false);
                TShock.Players[member].SetTeam((int)TeamID.White);
                WhiteTeam.members.Add(member);
                RedTeam.members.Remove(member);
                // Remove equipment too.
            }
            // deactivate pvp, put everone into white team
        }
    }
}
