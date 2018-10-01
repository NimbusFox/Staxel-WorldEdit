using System;
using System.Collections.Generic;
using Plukit.Base;
using Staxel.Commands;
using Staxel.Server;

namespace NimbusFox.WorldEdit.Staxel.Commands {
    public class StackCommand : ICommandBuilder {
        public string Execute(string[] bits, Blob blob, ClientServerConnection connection, ICommandsApi api,
            out object[] responseParams) {
            try {
                responseParams = new object[] { };
                var repeat = 1;
                var direction = "forwards";

                if (bits.Length >= 2) {
                    int.TryParse(bits[1], out repeat);
                }

                if (bits.Length >= 3) {
                    direction = bits[2];
                }

                WorldEditManager.Stack(WorldEditManager.FoxCore.UserManager.GetPlayerEntityByUid(connection.Credentials.Uid), repeat, direction);
            } catch (Exception ex) {
                WorldEditManager.FoxCore.ExceptionManager.HandleException(ex,
                    new Dictionary<string, object> { { "input", bits } });
                responseParams = new object[3];
                responseParams[0] = "WorldEdit";
                responseParams[1] = "WorldEdit";
                responseParams[2] = "WorldEdit";
                return "mods.nimbusfox.exception.message";
            }
            return "";
        }

        public string Kind => "/stack";
        public string Usage => "mods.nimbusfox.worldedit.command.stack.description";
        public bool Public => false;
    }
}
