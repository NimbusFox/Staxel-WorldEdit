using Staxel.Tiles;

namespace NimbusFox.WorldEdit.Classes {
    internal class FrameTiles {
        internal class Compass {
            internal Tile NE { get; }
            internal Tile ES { get; }
            internal Tile SW { get; }
            internal Tile WN { get; }

            internal Compass(Tile ne) {
                NE = ne.Configuration.MakeTile(ne.Configuration.BuildRotationVariant(3));
                ES = ne.Configuration.MakeTile(ne.Configuration.BuildRotationVariant(2));
                SW = ne.Configuration.MakeTile(ne.Configuration.BuildRotationVariant(1));
                WN = ne.Configuration.MakeTile(ne.Configuration.BuildRotationVariant(0));
            }
        }

        internal class PositiveNegativeAxis {
            internal Tile X { get; }
            internal Tile Z { get; }
            internal Tile NX { get; }
            internal Tile NZ { get; }

            internal PositiveNegativeAxis(Tile x) {
                X = x;
                Z = X.Configuration.MakeTile(X.Configuration.BuildRotationVariant(1));
                NX = X.Configuration.MakeTile(X.Configuration.BuildRotationVariant(2));
                NZ = X.Configuration.MakeTile(X.Configuration.BuildRotationVariant(3));
            }
        }

        internal class LineClass {
            internal Tile X { get; }
            internal Tile Y { get; }
            internal Tile Z { get; }

            internal LineClass(Tile x, Tile y) {
                X = x;
                Y = y;
                Z = X.Configuration.MakeTile(x.Configuration.BuildRotationVariant(1));
            } 
        }

        internal class LClass {
            internal Compass Side { get; }
            internal PositiveNegativeAxis Up { get; }
            internal PositiveNegativeAxis Down { get; }

            internal LClass(Tile startSide, Tile upStart, Tile downStart) {
                Side = new Compass(startSide);
                Up = new PositiveNegativeAxis(upStart);
                Down = new PositiveNegativeAxis(downStart);
            }
        }

        internal class CornerClass {
            internal Compass Up { get; }
            internal Compass Down { get; }

            internal CornerClass(Tile upStart, Tile downStart) {
                Up = new Compass(upStart);
                Down = new Compass(downStart);
            }
        }

        internal LineClass Line { get; }
        internal LClass L { get; }
        internal CornerClass Corner { get; }
        public bool Initialized { get; }

        internal FrameTiles(Tile lineX, Tile lineY, Tile lSide, Tile lUp, Tile lDown, Tile cornerUp, Tile cornerDown) {
            var def = new Tile();
            if (lineX == def || lineY == def || lSide == def || lUp == def || lDown == def || cornerUp == def ||
                cornerDown == def) {
                Initialized = false;
                return;
            }
            Line = new LineClass(lineX, lineY);
            L = new LClass(lSide, lUp, lDown);
            Corner = new CornerClass(cornerUp, cornerDown);
            Initialized = true;
        }
    }
}
