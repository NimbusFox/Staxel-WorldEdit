using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Commands;
using Staxel.Server;

namespace NimbusFox.WorldEdit.Staxel.Commands {
    public class ClearCommand : ICommandBuilder {
        public string Execute(string[] bits, Blob blob, ClientServerConnection connection, ICommandsApi api,
            out object[] responseParams) {
            responseParams = new object[] { };

            var player = WorldEditManager.FoxCore.UserManager.GetPlayerEntityByUid(connection.Credentials.Uid);

            WorldEditManager.Clear(player);

            return "mods.nimbusfox.worldedit.success.clearselection";
        }

        public string Kind => "/clear";
        public string Usage => "mods.nimbusfox.worldedit.command.clear.description";
        public bool Public => false;
    }
}
