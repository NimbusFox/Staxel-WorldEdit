using System;
using System.Collections.Generic;
using Plukit.Base;
using Staxel.Commands;
using Staxel.Server;

namespace NimbusFox.WorldEdit.Staxel.Commands {
    public class CutCommand : ICommandBuilder {
        public string Execute(string[] bits, Blob blob, ClientServerConnection connection, ICommandsApi api,
            out object[] responseParams) {
            responseParams = new object[] { };
            try {

                var player = WorldEditManager.FoxCore.UserManager.GetPlayerEntityByUid(connection.Credentials.Uid);

                WorldEditManager.Copy(player);

                long tileCount;

                WorldEditManager.Set(player, "staxel.tile.Sky", out tileCount, out _);
            } catch (Exception ex) {
                WorldEditManager.FoxCore.ExceptionManager.HandleException(ex,
                    new Dictionary<string, object>
                    {{"input", bits
                    }
                    });
                responseParams = new object[3];
                responseParams[0] = "WorldEdit";
                responseParams[1] = "WorldEdit";
                responseParams[2] = "WorldEdit";
                return "mods.nimbusfox.exception.message";
            }

            return "mods.nimbusfox.worldedit.success.copy";
        }

        public string Kind => "/cut";
        public string Usage => "mods.nimbusfox.worldedit.command.cut.description";
        public bool Public => false;
    }
}
