using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Commands;
using Staxel.Server;

namespace NimbusFox.WorldEdit.Staxel.Commands {
    public class CutCommand : ICommandBuilder {
        public string Execute(string[] bits, Blob blob, ClientServerConnection connection, ICommandsApi api,
            out object[] responseParams) {
            responseParams = new object[] { };

            var player = WorldEditManager.FoxCore.UserManager.GetPlayerEntityByUid(connection.Credentials.Uid);

            WorldEditManager.Copy(player);

            long tileCount;

            WorldEditManager.Set(player, "staxel.tile.Sky", out tileCount, out _);

            return "mods.nimbusfox.worldedit.success.copy";
        }

        public string Kind => "/cut";
        public string Usage => "mods.nimbusfox.worldedit.command.cut.description";
        public bool Public => false;
    }
}
