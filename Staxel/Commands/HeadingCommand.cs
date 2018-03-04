using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore;
using Plukit.Base;
using Staxel.Commands;
using Staxel.Server;

namespace NimbusFox.WorldEdit.Staxel.Commands {
    public class HeadingCommand : ICommandBuilder {
        public string Execute(string[] bits, Blob blob, ClientServerConnection connection, ICommandsApi api,
            out object[] responseParams) {
            responseParams = new object[] { };

            var player = WorldEditManager.FoxCore.UserManager.GetPlayerEntityByUid(connection.Credentials.Uid);

            responseParams = new object[] {
                player.PlayerEntityLogic.Heading().X,
                player.PlayerEntityLogic.Heading().Y,
                player.PlayerEntityLogic.Heading().GetDirection().ToString()
            };

            return "mods.nimbusfox.worldedit.success.heading";
        }

        public string Kind => "/heading";
        public string Usage => "mods.nimbusfox.worldedit.command.heading.description";
        public bool Public => false;
    }
}
