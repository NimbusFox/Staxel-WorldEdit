using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.WorldEdit.Enums;
using Plukit.Base;
using Staxel.Commands;
using Staxel.Server;

namespace NimbusFox.WorldEdit.Staxel.Commands {
    public class ReplaceCommand : ICommandBuilder {
        public string Execute(string[] bits, Blob blob, ClientServerConnection connection, ICommandsApi api,
            out object[] responseParams) {
            responseParams = new object[] { };

            bits = bits.Skip(1).ToArray();

            var newCode = "";
            var oldCode = "";

            if (bits.Length > 0) {
                newCode = bits[0];
            }

            if (bits.Length > 1) {
                oldCode = bits[1];
            }

            var player = WorldEditManager.FoxCore.UserManager.GetPlayerEntityByUid(connection.Credentials.Uid);

            long tileCount;
            ReplaceResult result;

            WorldEditManager.Replace(player, newCode, oldCode, out tileCount, out result);

            if (result == ReplaceResult.InvalidPositions) {
                return "mods.nimbusfox.worldedit.error.noregion";
            }

            if (result == ReplaceResult.InvalidTile) {
                responseParams = new object[] { newCode };
                return "mods.nimbusfox.worldedit.error.invalidtile";
            }

            responseParams = new object[] { tileCount.ToString() };

            return "mods.nimbusfox.worldedit.success.replaced";
        }

        public string Kind => "/replace";
        public string Usage => "mods.nimbusfox.worldedit.command.replace.description";
        public bool Public => false;
    }
}
