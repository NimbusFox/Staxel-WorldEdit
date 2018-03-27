using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NimbusFox.FoxCore.Classes;

namespace NimbusFox.WorldEdit.Classes {
    [Serializable]
    public class RedoUndo {
        //public Dictionary<Guid, List<UndoData>> Undo { get; set; }
        //public Dictionary<Guid, List<UndoData>> Redo { get; set; }
        public List<Guid> Undo { get; set; }
        public List<Guid> Redo { get; set; }
        public Dictionary<Guid, VectorCubeI> ID { get; set; }

        public RedoUndo() {
            //Undo = new Dictionary<Guid, List<UndoData>>();
            //Redo = new Dictionary<Guid, List<UndoData>>();
            Undo = new List<Guid>();
            Redo = new List<Guid>();
            ID = new Dictionary<Guid, VectorCubeI>();
        }
    }
}
