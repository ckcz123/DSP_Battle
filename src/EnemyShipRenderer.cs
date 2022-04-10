using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using HarmonyLib;


namespace DSP_Battle
{
    class EnemyShipRenderer
    {
        public static void Init()
        {
            PrefabDesc prefabDesc = LDB.items.Select(5002).prefabDesc;
            if (!meshAlreadySet)
            {
                shipMesh = Resources.Load<Mesh>("test/tgs demo/enemy-base");
                var oriVerts = shipMesh.vertices;
                for (int i = 0; i < oriVerts.Length; i++)
                {
                    Vector3 vert = oriVerts[i];
                    vert.x *= 2f;
                    vert.y *= 2f;
                    vert.z *= 2f;
                    if (vert.x > 5) vert.x = 5;
                    if (vert.y > 5) vert.y = 5;
                    if (vert.z > 5) vert.z = 5;
                    if (vert.x < -5) vert.x = -5;
                    if (vert.y < -5) vert.y = -5;
                    if (vert.z < -5) vert.z = -5;
                    oriVerts[i] = vert;
                }
                shipMesh.vertices = oriVerts;
                meshAlreadySet = true;

                Material[] array = prefabDesc.lodMaterials[0];
                shipMats = new Material[shipMesh.subMeshCount]; //这里有修改
                int num = 0;
                while (num < shipMesh.subMeshCount && num < array.Length)
                {
                    shipMats[num] = UnityEngine.Object.Instantiate<Material>(array[num]);
                    num++;
                }
            }
            argBuffer = new ComputeBuffer(25, 4, ComputeBufferType.DrawIndirect);
            shipCount = 0;
            shipsArr = new ShipRenderingData[512];
            shipsBuffer = new ComputeBuffer(shipsArr.Length, 64, ComputeBufferType.Default);
            argArr = new uint[25];

            
        }

        public static void Expand2x()
        {
            ShipRenderingData[] originArray = shipsArr;
            shipsArr = new ShipRenderingData[originArray.Length * 2];
            shipsBuffer.Release();
            shipsBuffer = new ComputeBuffer(shipsArr.Length, 64, ComputeBufferType.Default);
            Array.Copy(originArray, shipsArr, originArray.Length);
        }

        public static void Update()
        {
            if (EnemyShips.ships.Count == 0)
            {
                shipCount = 0;
                shipsBuffer.SetData(shipsArr, 0, 0, shipCount);
                return; 
            }

            while (shipsArr.Length < EnemyShips.ships.Count)
            {
                Expand2x();
            }

            shipCount = 0;
            foreach (var ship in EnemyShips.ships.Values)
            {
                if (ship.state != EnemyShip.State.active) continue;
                shipsArr[shipCount] = ship.renderingData;
                shipCount++;
            }

            shipsBuffer.SetData(shipsArr, 0, 0, shipCount);
        }

        public static void Draw()
        {
            if (shipCount <= 0) return;

            for (int i = 0; i < shipMats.Length; i++)
            {
                argArr[i * 5] = shipMesh.GetIndexCount(i);
                argArr[1 + i * 5] = (uint)shipCount;
                argArr[2 + i * 5] = shipMesh.GetIndexStart(i);
                argArr[3 + i * 5] = shipMesh.GetBaseVertex(i);
                argArr[4 + i * 5] = 0U;
            }
            argBuffer.SetData(argArr);
            for (int j = 0; j < shipMats.Length; j++)
            {
                shipMats[j].SetBuffer("_ShipBuffer", shipsBuffer);
                Graphics.DrawMeshInstancedIndirect(shipMesh, j, shipMats[j], new Bounds(Vector3.zero, new Vector3(200000f, 200000f, 200000f)), argBuffer, j * 5 * 4, null, (j == 0) ? ShadowCastingMode.On : ShadowCastingMode.Off, j == 0, 0);
            }
            GpuAnalysis();
        }

        public static void GpuAnalysis()
        {
            if (PerformanceMonitor.GpuProfilerOn)
            {
                int num = shipCount;
                int vertexCount = shipMesh.vertexCount;
                PerformanceMonitor.RecordGpuWork(EGpuWorkEntry.LogisticShip, num, num * vertexCount);
            }
        }

        public static void Destory()
        {
            shipsArr = null;
            shipCount = 0;
            if (shipsBuffer != null)
            {
                shipsBuffer.Release();
                shipsBuffer = null;
            }
            if (argBuffer != null)
            {
                argBuffer.Release();
                argBuffer = null;
            }
        }

        private static int shipCount;
        private static Mesh shipMesh;
        private static ShipRenderingData[] shipsArr;
        private static ComputeBuffer shipsBuffer;
        private static ComputeBuffer argBuffer;
        private static uint[] argArr;
        private static Material[] shipMats;
        private static bool meshAlreadySet = false;
    }
}
