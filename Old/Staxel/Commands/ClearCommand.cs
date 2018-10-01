using System;
using System.Collections.Generic;
using Plukit.Base;
using Staxel.Commands;
using Staxel.Server;

namespace NimbusFox.WorldEdit.Staxel.Commands {
    public class ClearCommand : ICommandBuilder {
        public string Execute(string[] bits, Blob blob, ClientServerConnection connection, ICommandsApi api,
            out object[] responseParams) {
            responseParams = new object[] { };

            try {
                var player = WorldEditManager.FoxCore.UserManager.GetPlayerEntityByUid(connection.Credentials.Uid);

                WorldEditManager.Clear(player);
            } catch(Exception ex) {
                WorldEditManager.FoxCore.ExceptionManager.HandleException(ex,
                    new Dictionary<string, object>
                        {{"input", bits}});
                responseParams = new object[3];
                responseParams[0] = "WorldEdit";
                responseParams[1] = "WorldEdit";
                responseParams[2] = "WorldEdit";
                return "mods.nimbusfox.exception.message";
            }

            return "mods.nimbusfox.worldedit.success.clearselection";
        }

        public string Kind => "/clear";
        public string Usage => "mods.nimbusfox.worldedit.command.clear.description";
        public bool Public => false;
    }
}
