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

        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

            }
            base.Dispose(disposing);
        }
    }
}
