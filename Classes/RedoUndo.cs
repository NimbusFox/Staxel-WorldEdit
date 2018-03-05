using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore.Classes;

namespace NimbusFox.WorldEdit.Classes {
    [Serializable]
    public class RedoUndo {
        public Dictionary<Guid, List<UndoData>> Undo { get; set; }
        public Dictionary<Guid, List<UndoData>> Redo { get; set; }
        public Dictionary<Guid, VectorCubeI> ID { get; set; }

        public RedoUndo() {
            Undo = new Dictionary<Guid, List<UndoData>>();
            Redo = new Dictionary<Guid, List<UndoData>>();
            ID = new Dictionary<Guid, VectorCubeI>();
        }
    }
}
