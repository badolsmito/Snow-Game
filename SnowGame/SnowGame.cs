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
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
            }
            base.Dispose(disposing);
        }

        private void OnInitialize(EventArgs args)
        {
            Commands.ChatCommands.Add(new Command(Join, "join"));
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



        struct GameTeam
        {
            public TeamID color;
            public int score;
            public List<string> members;
        }


        GameTeam RedTeam = new GameTeam { color = TeamID.Red, score = 0 };
        GameTeam BlueTeam = new GameTeam { color = TeamID.Blue, score = 0 };

        public void Join(CommandArgs args)
        {
            if (RedTeam.members.Contains(args.Player.Name) || BlueTeam.members.Contains(args.Player.Name))
            {
                args.Player.SendInfoMessage("You're already in a team!");
            }
            else
            {
                // This should be RNG based but also check if there's an imbalance on
                // the amount of members in the teams.
                BlueTeam.members.Add(args.Player.Name);
                //args.TPlayer.Teleport()
            }
        }

        /*struct TeamMember
        {
            public ;
            public int score;
            public string name;
        }*/
    }
}
