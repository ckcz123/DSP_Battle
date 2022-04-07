using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace DSP_Battle
{
    class EnemyShipUIRenderer
    {
        public static void Init()
        {
            shipMesh = Resources.Load<Mesh>("test/tgs demo/enemy-base");
            var oriVerts = shipMesh.vertices;
            for (int i = 0; i < oriVerts.Length; i++)
            {
                Vector3 vert = oriVerts[i];
                vert.x *= 2;
                vert.y *= 2;
                vert.z *= 2;
                oriVerts[i] = vert;
            }
            shipMesh.vertices = oriVerts;

            shipsArr = new ShipUIRenderingData[512];
            shipsBuffer = new ComputeBuffer(shipsArr.Length, 72, ComputeBufferType.Default);
            shipCount = 0;
            argBuffer = new ComputeBuffer(25, 4, ComputeBufferType.DrawIndirect);
            argArr = new uint[25];
        }

        public static void Expand2x()
        {
            ShipUIRenderingData[] originArray = shipsArr;
            shipsArr = new ShipUIRenderingData[originArray.Length * 2];
            shipsBuffer.Release();
            shipsBuffer = new ComputeBuffer(shipsArr.Length, 72, ComputeBufferType.Default);
            Array.Copy(originArray, shipsArr, originArray.Length);
        }

        public static void Update(UIStarmap uiStarMap)
        {
            if (EnemyShips.ships.Count == 0) return;
            
            while (shipsArr.Length < EnemyShips.ships.Count)
            {
                Expand2x();
            }

            shipCount = 0;
            foreach (var ship in EnemyShips.ships.Values)
            {
                if (ship.state != EnemyShip.State.active) continue;
                shipsArr[shipCount] = ship.renderingUIData;
                shipsArr[shipCount].rpos = (shipsArr[shipCount].upos - uiStarMap.viewTargetUPos) * 0.00025;
                shipCount++;
            }

            shipsBuffer.SetData(shipsArr, 0, 0, shipCount);
        }

        public static void Draw(LogisticShipUIRenderer logisticShipUIRenderer)
        {
            if (shipCount <= 0 || logisticShipUIRenderer.uiStarmap == null
                || !logisticShipUIRenderer.uiStarmap.active
                || UIStarmap.isChangingToMilkyWay) return;

            if (shipMats == null)
            {
                shipMats = new Material[logisticShipUIRenderer.shipMats.Length];
                for (var i = 0; i < logisticShipUIRenderer.shipMats.Length; ++i)
                {
                    if (logisticShipUIRenderer.shipMats[i] != null)
                        shipMats[i] = new Material(logisticShipUIRenderer.shipMats[i]);
                }
            }

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
                Graphics.DrawMeshInstancedIndirect(shipMesh, j, shipMats[j], new Bounds(Vector3.zero, new Vector3(200000f, 200000f, 200000f)), argBuffer, j * 5 * 4, null, ShadowCastingMode.Off, false, 20);
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

        private static int shipCount;
        private static Mesh shipMesh;
        private static ShipUIRenderingData[] shipsArr;
        private static ComputeBuffer shipsBuffer;
        private static ComputeBuffer argBuffer;
        private static uint[] argArr;
        private static Material[] shipMats;
    }
}
