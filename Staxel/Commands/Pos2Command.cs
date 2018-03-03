using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Commands;
using Staxel.Server;

namespace NimbusFox.WorldEdit.Staxel.Commands {
    public class Pos2Command : ICommandBuilder {
        public string Execute(string[] bits, Blob blob, ClientServerConnection connection, ICommandsApi api,
            out object[] responseParams) {
            responseParams = new object[] { };

            var player = WorldEditManager.FoxCore.UserManager.GetPlayerEntityByUid(connection.Credentials.Uid);

            WorldEditManager.AddPos2(player);

            responseParams = new object[] {
                player.Physics.BottomPosition().X,
                player.Physics.BottomPosition().Y,
                player.Physics.BottomPosition().Z
            };
            return "mods.nimbusfox.worldedit.success.pos2";
        }

        public string Kind => "/pos2";
        public string Usage => "mods.nimbusfox.worldedit.command.pos2.description";
        public bool Public => false;
    }
}
