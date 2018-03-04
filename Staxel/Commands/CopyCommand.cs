using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Commands;
using Staxel.Server;

namespace NimbusFox.WorldEdit.Staxel.Commands {
    public class CopyComamnd : ICommandBuilder {
        public string Execute(string[] bits, Blob blob, ClientServerConnection connection, ICommandsApi api,
            out object[] responseParams) {
            responseParams = new object[] { };

            var player = WorldEditManager.FoxCore.UserManager.GetPlayerEntityByUid(connection.Credentials.Uid);

            var tileCount = WorldEditManager.Copy(player);

            if (tileCount == 0) {
                return "mods.nimbusfox.worldedit.error.noregion";
            }

            responseParams = new object[] { tileCount.ToString() };
            return "mods.nimbusfox.worldedit.success.copy";
        }

        public string Kind => "/copy";
        public string Usage => "mods.nimbusfox.worldedit.command.copy.description";
        public bool Public => false;
    }
}
