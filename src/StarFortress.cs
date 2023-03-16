using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSP_Battle
{
    public class StarFortress
    {

        public static void InitAll()
        {
            StarFortressSilo.InitAll();
        }

        public static bool NeedRocket(DysonSphere sphere, int rocketId)
        {

            return true;
        }

        public static void ConstructStarFortPoint(int starIndex, int rocketProtoId)
        { 
        
        }

        public static void Export(BinaryWriter w)
        {
            
        }

        public static void Import(BinaryReader r)
        {
            InitAll();
        }

        public static void IntoOtherSave()
        {
            InitAll();
        }
    }
}
