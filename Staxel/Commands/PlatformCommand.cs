﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Plukit.Base;
using Staxel.Commands;
using Staxel.Server;

namespace NimbusFox.WorldEdit.Staxel.Commands {
    public class PlatformCommand : ICommandBuilder {
        public string Execute(string[] bits, Blob blob, ClientServerConnection connection, ICommandsApi api,
            out object[] responseParams) {
            responseParams = new object[] { };

            var player = WorldEditManager.FoxCore.UserManager.GetPlayerEntityByUid(connection.Credentials.Uid);

            var size = 0;
            var tile = "staxel.tile.dirt.dirt";

            if (bits.Length >= 2) {
                int.TryParse(bits[1], out size);
            }

            if (bits.Length >= 3) {
                tile = bits[2];
            }

            long tileCount;

            if (!WorldEditManager.Platform(player, size, out tileCount, tile)) {
                responseParams = new object[] { tile };
                return "mods.nimbusfox.worldedit.error.invalidtile";
            }

            responseParams = new object[] {tileCount.ToString()};
            return "mods.nimbusfox.worldedit.success.set";
        }

        public string Kind => "/platform";
        public string Usage => "";
        public bool Public => false;
    }
}