﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Commands;
using Staxel.Server;

namespace NimbusFox.WorldEdit.Staxel.Commands {
    public class WallCommand : ICommandBuilder {
        public string Execute(string[] bits, Blob blob, ClientServerConnection connection, ICommandsApi api,
            out object[] responseParams) {
            responseParams = new object[] { };

            var player = WorldEditManager.FoxCore.UserManager.GetPlayerEntityByUid(connection.Credentials.Uid);

            var height = 0;
            var width = 0;
            var tile = "staxel.tile.dirt.dirt";

            if (bits.Length >= 2) {
                int.TryParse(bits[1], out width);
                height = width;
            }

            if (bits.Length >= 3) {
                int.TryParse(bits[2], out height);
            }

            if (bits.Length >= 4) {
                tile = bits[3];
            }

            long tileCount;

            if (!WorldEditManager.Wall(player, width, height, out tileCount, tile)) {
                responseParams = new object[] { bits[3] };
                return "mods.nimbusfox.worldedit.error.invalidtile";
            }

            responseParams = new object[] {tileCount.ToString()};

            return "mods.nimbusfox.worldedit.success.set";
        }

        public string Kind => "/wall";
        public string Usage => "mods.nimbusfox.worldedit.command.wall.description";
        public bool Public => false;
    }
}