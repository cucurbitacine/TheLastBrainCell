using UnityEngine;

namespace CucuTools.Voxels
{
    public abstract class VoxelBuilder
    {
        public virtual MeshInfo BuildChunk(CucuChunk chunk)
        {
            var meshInfo = new MeshInfo();

            for (var i = 0; i < chunk.Resolution; i++)
            {
                for (var j = 0; j < chunk.Resolution; j++)
                {
                    for (var k = 0; k < chunk.Resolution; k++)
                    {
                        if (chunk[i, j, k].Value) meshInfo.Add(BuildVoxel(i, j, k, chunk));
                    }
                }
            }

            return meshInfo;
        }

        public MeshInfo BuildChunks(params CucuChunk[] chunks)
        {
            var meshInfo = new MeshInfo();
            foreach (var chunk in chunks)
                meshInfo.Add(BuildChunk(chunk));
            return meshInfo;
        }

        public abstract MeshInfo BuildVoxel(int i, int j, int k, CucuChunk chunk);
    }

    public class CubeBuilder : VoxelBuilder
    {
        public override MeshInfo BuildVoxel(int i, int j, int k, CucuChunk chunk)
        {
            var voxel = chunk[i, j, k];

            var meshInfo = new MeshInfo();

            if (!chunk[voxel.i, voxel.j, voxel.k].Value)
            {
                return meshInfo;
            }

            var verticesCount = 0;

            if (chunk.Resolution <= (voxel.i + 1) || !chunk[voxel.i + 1, voxel.j, voxel.k].Value)
            {
                verticesCount = meshInfo.vertices.Count;

                meshInfo.vertices.Add(voxel.Corner(CornerType.RightBottomBack) + chunk.Position);
                meshInfo.vertices.Add(voxel.Corner(CornerType.RightTopBack) + chunk.Position);
                meshInfo.vertices.Add(voxel.Corner(CornerType.RightTopForward) + chunk.Position);
                meshInfo.vertices.Add(voxel.Corner(CornerType.RightBottomForward) + chunk.Position);

                meshInfo.triangles.Add(verticesCount + 0);
                meshInfo.triangles.Add(verticesCount + 1);
                meshInfo.triangles.Add(verticesCount + 2);

                meshInfo.triangles.Add(verticesCount + 0);
                meshInfo.triangles.Add(verticesCount + 2);
                meshInfo.triangles.Add(verticesCount + 3);

                meshInfo.uv.Add(new Vector2(0, 0));
                meshInfo.uv.Add(new Vector2(0, 1));
                meshInfo.uv.Add(new Vector2(1, 1));
                meshInfo.uv.Add(new Vector2(1, 0));
            }

            if ((voxel.i - 1) < 0 || !chunk[voxel.i - 1, voxel.j, voxel.k].Value)
            {
                verticesCount = meshInfo.vertices.Count;

                meshInfo.vertices.Add(voxel.Corner(CornerType.LeftBottomForward) + chunk.Position);
                meshInfo.vertices.Add(voxel.Corner(CornerType.LeftTopForward) + chunk.Position);
                meshInfo.vertices.Add(voxel.Corner(CornerType.LeftTopBack) + chunk.Position);
                meshInfo.vertices.Add(voxel.Corner(CornerType.LeftBottomBack) + chunk.Position);

                meshInfo.triangles.Add(verticesCount + 0);
                meshInfo.triangles.Add(verticesCount + 1);
                meshInfo.triangles.Add(verticesCount + 2);

                meshInfo.triangles.Add(verticesCount + 0);
                meshInfo.triangles.Add(verticesCount + 2);
                meshInfo.triangles.Add(verticesCount + 3);

                meshInfo.uv.Add(new Vector2(0, 0));
                meshInfo.uv.Add(new Vector2(0, 1));
                meshInfo.uv.Add(new Vector2(1, 1));
                meshInfo.uv.Add(new Vector2(1, 0));
            }

            if (chunk.Resolution <= (voxel.j + 1) || !chunk[voxel.i, voxel.j + 1, voxel.k].Value)
            {
                verticesCount = meshInfo.vertices.Count;

                meshInfo.vertices.Add(voxel.Corner(CornerType.LeftTopBack) + chunk.Position);
                meshInfo.vertices.Add(voxel.Corner(CornerType.LeftTopForward) + chunk.Position);
                meshInfo.vertices.Add(voxel.Corner(CornerType.RightTopForward) + chunk.Position);
                meshInfo.vertices.Add(voxel.Corner(CornerType.RightTopBack) + chunk.Position);

                meshInfo.triangles.Add(verticesCount + 0);
                meshInfo.triangles.Add(verticesCount + 1);
                meshInfo.triangles.Add(verticesCount + 2);

                meshInfo.triangles.Add(verticesCount + 0);
                meshInfo.triangles.Add(verticesCount + 2);
                meshInfo.triangles.Add(verticesCount + 3);

                meshInfo.uv.Add(new Vector2(0, 0));
                meshInfo.uv.Add(new Vector2(0, 1));
                meshInfo.uv.Add(new Vector2(1, 1));
                meshInfo.uv.Add(new Vector2(1, 0));
            }

            if ((voxel.j - 1) < 0 || !chunk[voxel.i, voxel.j - 1, voxel.k].Value)
            {
                verticesCount = meshInfo.vertices.Count;

                meshInfo.vertices.Add(voxel.Corner(CornerType.LeftBottomForward) + chunk.Position);
                meshInfo.vertices.Add(voxel.Corner(CornerType.LeftBottomBack) + chunk.Position);
                meshInfo.vertices.Add(voxel.Corner(CornerType.RightBottomBack) + chunk.Position);
                meshInfo.vertices.Add(voxel.Corner(CornerType.RightBottomForward) + chunk.Position);

                meshInfo.triangles.Add(verticesCount + 0);
                meshInfo.triangles.Add(verticesCount + 1);
                meshInfo.triangles.Add(verticesCount + 2);

                meshInfo.triangles.Add(verticesCount + 0);
                meshInfo.triangles.Add(verticesCount + 2);
                meshInfo.triangles.Add(verticesCount + 3);

                meshInfo.uv.Add(new Vector2(0, 0));
                meshInfo.uv.Add(new Vector2(0, 1));
                meshInfo.uv.Add(new Vector2(1, 1));
                meshInfo.uv.Add(new Vector2(1, 0));
            }

            if (chunk.Resolution <= (voxel.k + 1) || !chunk[voxel.i, voxel.j, voxel.k + 1].Value)
            {
                verticesCount = meshInfo.vertices.Count;

                meshInfo.vertices.Add(voxel.Corner(CornerType.RightBottomForward) + chunk.Position);
                meshInfo.vertices.Add(voxel.Corner(CornerType.RightTopForward) + chunk.Position);
                meshInfo.vertices.Add(voxel.Corner(CornerType.LeftTopForward) + chunk.Position);
                meshInfo.vertices.Add(voxel.Corner(CornerType.LeftBottomForward) + chunk.Position);

                meshInfo.triangles.Add(verticesCount + 0);
                meshInfo.triangles.Add(verticesCount + 1);
                meshInfo.triangles.Add(verticesCount + 2);

                meshInfo.triangles.Add(verticesCount + 0);
                meshInfo.triangles.Add(verticesCount + 2);
                meshInfo.triangles.Add(verticesCount + 3);

                meshInfo.uv.Add(new Vector2(0, 0));
                meshInfo.uv.Add(new Vector2(0, 1));
                meshInfo.uv.Add(new Vector2(1, 1));
                meshInfo.uv.Add(new Vector2(1, 0));
            }

            if ((voxel.k - 1) < 0 || !chunk[voxel.i, voxel.j, voxel.k - 1].Value)
            {
                verticesCount = meshInfo.vertices.Count;

                meshInfo.vertices.Add(voxel.Corner(CornerType.LeftBottomBack) + chunk.Position);
                meshInfo.vertices.Add(voxel.Corner(CornerType.LeftTopBack) + chunk.Position);
                meshInfo.vertices.Add(voxel.Corner(CornerType.RightTopBack) + chunk.Position);
                meshInfo.vertices.Add(voxel.Corner(CornerType.RightBottomBack) + chunk.Position);

                meshInfo.triangles.Add(verticesCount + 0);
                meshInfo.triangles.Add(verticesCount + 1);
                meshInfo.triangles.Add(verticesCount + 2);

                meshInfo.triangles.Add(verticesCount + 0);
                meshInfo.triangles.Add(verticesCount + 2);
                meshInfo.triangles.Add(verticesCount + 3);

                meshInfo.uv.Add(new Vector2(0, 0));
                meshInfo.uv.Add(new Vector2(0, 1));
                meshInfo.uv.Add(new Vector2(1, 1));
                meshInfo.uv.Add(new Vector2(1, 0));
            }

            return meshInfo;
        }
    }
}