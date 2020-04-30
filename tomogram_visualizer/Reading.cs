using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace tomogram_visualizer {
    class Bin {
        public static int X, Y, Z;
        public static short[] array;
        public Bin() { }

        public void readBIN(string path) {
            if (File.Exists(path)) {
                BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open));

                X = reader.ReadInt32();
                Y = reader.ReadInt32();
                Z = reader.ReadInt32();

                int size = X * Y * Z;
                array = new short[size];
                for(int i = 0; i < size; i++) {
                    array[i] = reader.ReadInt16();
                }
            }
        }
    }
}
