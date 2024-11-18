using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSP_Battle.src.NebulaCompat
{
    public class TCFVPacket
    {
        public EDataType type { get; set; }
        public int valueInt32 { get; set; }
        public int[] arrayInt32 { get; set; }
        public float valueFloat { get; set; }
        public float[] arrayFloat { get; set; }
        public double valueDouble { get; set; }
        public double[] arrayDouble { get; set; }
        public byte[] dataBinary { get; set; }

        public TCFVPacket() { }
        public TCFVPacket(EDataType type, int valueInt32, int[] arrayInt32,float valueFloat, float[] arrayFloat,  double valueDouble, double[] arrayDouble, byte[] dataBinary)
        {
            this.type = type;
            this.valueInt32 = valueInt32;
            this.arrayInt32 = arrayInt32;
            this.valueFloat = valueFloat;
            this.arrayFloat = arrayFloat;
            this.valueDouble = valueDouble;
            this.arrayDouble = arrayDouble;
            this.dataBinary = dataBinary;
        }
        public TCFVPacket(EDataType type, int valueInt32)
        {
            this.type = type;
            this.valueInt32 = valueInt32;
        }
        public TCFVPacket (EDataType type, int[] arrayInt32)
        {
            this.type = type; 
            this.arrayInt32 = arrayInt32;
        }
        public TCFVPacket(EDataType type, float valueFloat)
        {
            this.type = type;
            this.valueFloat = valueFloat;
        }
        public TCFVPacket(EDataType type, float[] arrayFloat)
        {
            this.type = type;
            this.arrayFloat = arrayFloat;
        }
        public TCFVPacket(EDataType type, double valueDouble)
        {
            this.type = type;
            this.valueDouble = valueDouble;
        }
        public TCFVPacket(EDataType type, double[] arrayDouble)
        {
            this.type = type;
            this.arrayDouble = arrayDouble;
        }

        public TCFVPacket(EDataType type, byte[] dataBinary)
        {
            this.type = type;
            this.dataBinary = dataBinary;
        }
    }
}
