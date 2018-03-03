using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Commands;
using Staxel.Server;

namespace NimbusFox.WorldEdit.Staxel.Commands {
    public class SaveCommand : ICommandBuilder {
        public string Execute(string[] bits, Blob blob, ClientServerConnection connection, ICommandsApi api,
            out object[] responseParams) {
            responseParams = new object[] { };

            var player = WorldEditManager.FoxCore.UserManager.GetPlayerEntityByUid(connection.Credentials.Uid);

            if (bits.Any()) {
                if (bits.Skip(1).Any()) {
                    WorldEditManager.Export(player, bits[1]);
                }
            }

            return "";
        }

        public string Kind => "/save";
        public string Usage => "";
        public bool Public => false;
    }
}
