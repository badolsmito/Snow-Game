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
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
                GetDataHandlers.TogglePvp -= OnTogglePvp;
                GetDataHandlers.PlayerTeam -= OnPlayerTeamChange;

            }
            base.Dispose(disposing);
        }

        private void OnInitialize(EventArgs args)
        {
            Commands.ChatCommands.Add(new Command(WhiteTeam.Join, "join"));
        }

        private void OnJoin(JoinEventArgs args)
        {
            WhiteTeam.members.Add(TShock.Players[args.Who].Name);
            // TODO: Remove the permission of the player to change teams and pvp mode.
        }

        private void OnTogglePvp(object sender, GetDataHandlers.TogglePvpEventArgs args)
        {
            // When the server intially sets the player's pvp to true, that might be a toggle of pvp.
            args.Player.SetPvP(true);
            TShock.Players[args.PlayerId].SendInfoMessage("You cannot toggle PVP off!");

        }

        private void OnPlayerTeamChange(object sender, GetDataHandlers.PlayerTeamEventArgs args)
        {
            // This can probably be written in a better way.
            args.Player.SendInfoMessage("You cannot change your team!");
            if (RedTeam.members.Contains(args.Player.Name))
            {
                args.Player.SetTeam((int)TeamID.Red);
            }
            else if (BlueTeam.members.Contains(args.Player.Name))
            {
                args.Player.SetTeam((int)TeamID.Blue);
            }
            else if (WhiteTeam.members.Contains(args.Player.Name))
            {
                args.Player.SetTeam((int)TeamID.White);
            }
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
            public List<string> members;
            public GameTeam(TeamID color, int score, List<string> members)
            {
                this.color = color;
                this.score = score;
                this.members = members;
            }

            public void Join(CommandArgs args)
            {
                if (!WhiteTeam.members.Contains(args.Player.Name))
                {
                    args.Player.SendInfoMessage("You're already in a team!");
                }

                else
                {
                    // This should be RNG based but also check if there's an imbalance on
                    // the amount of members in the teams.
                    // if (RedTeam.members.amount or some shi < blue team members amount {add to red team} else to blue team
                    WhiteTeam.members.Remove(args.Player.Name);

                    if (RedTeam.members.Count > BlueTeam.members.Count)
                    {
                        BlueTeam.members.Add(args.Player.Name);
                        args.Player.SetTeam((int)TeamID.Blue);
                    }
                    else
                    {
                        RedTeam.members.Add(args.Player.Name);
                        args.Player.SetTeam((int)TeamID.Red);
                    }
                    

                    //args.TPlayer.team = (int)TeamID.Blue;
                    //NetMessage.SendData((int)PacketTypes.PlayerTeam, -1, -1, null, args.Player.Index, (int)TeamID.Blue);

                    args.Player.SetPvP(true);
                    //args.TPlayer.Teleport()
                }
            }

        }

        public static GameTeam WhiteTeam = new GameTeam(TeamID.White, 0, new List<string>());
        private static GameTeam RedTeam = new GameTeam(TeamID.Red, 0, new List<string>());
        private static GameTeam BlueTeam = new GameTeam(TeamID.Blue, 0, new List<string>());
    }
}
