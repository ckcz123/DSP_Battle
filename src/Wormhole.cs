using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DSP_Battle
{
    public class Wormhole
    {
        public int planetId;
        public VectorLF3 vec;

        public Wormhole() : this(0, VectorLF3.zero) { }

        public Wormhole(int planetId, VectorLF3 vec)
        {
            this.planetId = planetId;
            this.vec = vec;
        }

        public VectorLF3 uPos 
        {
            get
            {
                if (planetId == 0) return VectorLF3.zero;

                PlanetData planet = GameMain.galaxy.PlanetById(planetId);
                return planet.uPosition + vec * (planet.radius + Configs.wormholeRange);
            }
        }

        public void Export(BinaryWriter w)
        {
            w.Write(planetId);
            w.Write(vec.x);
            w.Write(vec.y);
            w.Write(vec.z);
        }

        public void Import(BinaryReader r)
        {
            planetId = r.ReadInt32();
            vec = new VectorLF3(r.ReadDouble(), r.ReadDouble(), r.ReadDouble());
        }

        public void IntoOtherSave()
        {
            planetId = 0;
            vec = VectorLF3.zero;
        }


    }
}
