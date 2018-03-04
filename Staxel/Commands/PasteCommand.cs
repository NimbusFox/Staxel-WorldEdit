﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Commands;
using Staxel.Server;

namespace NimbusFox.WorldEdit.Staxel.Commands {
    public class PasteComamnd : ICommandBuilder {
        public string Execute(string[] bits, Blob blob, ClientServerConnection connection, ICommandsApi api,
            out object[] responseParams) {
            responseParams = new object[] { };

            var player = WorldEditManager.FoxCore.UserManager.GetPlayerEntityByUid(connection.Credentials.Uid);

            var tilecount = WorldEditManager.Paste(player);

            if (tilecount == 0) {
                return "mods.nimbusfox.worldedit.error.emptyclipboard";
            }

            responseParams = new object[] { tilecount.ToString() };
            return "mods.nimbusfox.worldedit.success.paste";
        }

        public string Kind => "/paste";
        public string Usage => "mods.nimbusfox.worldedit.command.paste.description";
        public bool Public => false;
    }
}