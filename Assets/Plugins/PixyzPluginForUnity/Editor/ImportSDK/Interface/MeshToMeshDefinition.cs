using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixyz.Plugin4Unity;

namespace Pixyz.ImportSDK {

    /// <summary>
    /// Sync conversion from Mesh to MeshDefinition
    /// </summary>
    public sealed class MeshToMeshDefinition
    {
        public Polygonal.Native.MeshDefinition meshDefinition;

        public IEnumerator Convert(Mesh mesh, Material.Native.MaterialDefinition[] materials, bool isLeftHanded, bool isZup, float scaleFactor)
        {
            Geom.Native.Point3List ivertices;
            Core.Native.ColorAlphaList icolors;
            Geom.Native.Vector3List inormals;
            Geom.Native.Vector4List itangents;
            List<Polygonal.Native.StylizedLine> lines;
            List<Geom.Native.Point3> lineVertices;
            List<Polygonal.Native.DressedPoly> dressedPolys;
            List<int> triangles;

            // If destination is right handed (unlike Unity), we should reverse the X axis
            float flipX = (isLeftHanded ? 1 : -1);

            meshDefinition = new Polygonal.Native.MeshDefinition();
            meshDefinition.id = mesh.GetInstanceID().ToUInt32();

            if (mesh.subMeshCount != 1 && mesh.subMeshCount != materials.Length)
            {
                throw new Exception("This mesh submesh count isn't coherent with given material array");
            }

            // Bruteforce : vertices for topologies different than mesh are also copied this way, but MeshDefinition has its own buffers for such vertices.
            // Unused vertices are deleted upon interface conversion
            // Same goes for all vertex attributes : normals, tangents, colors, uvs, ...

            // Convert Vertices
            var uvertices = mesh.vertices;
            ivertices = new Geom.Native.Point3List(uvertices.Length);
            if (isZup)
                for (int i = 0; i < uvertices.Length; i++)
                    ivertices[i] = new Geom.Native.Point3 { x = uvertices[i].x * scaleFactor * flipX, y = uvertices[i].z * scaleFactor, z = uvertices[i].y * scaleFactor };
            else
                for (int i = 0; i < uvertices.Length; i++)
                    ivertices[i] = new Geom.Native.Point3 { x = uvertices[i].x * scaleFactor * flipX, y = uvertices[i].y * scaleFactor, z = uvertices[i].z * scaleFactor };

            // Convert Vertex Colors
            var uvertexColors = mesh.colors;
            icolors = new Core.Native.ColorAlphaList(uvertexColors.Length);
            for (int i = 0; i < uvertexColors.Length; i++)
                icolors[i] = new Core.Native.ColorAlpha { r = uvertexColors[i].r, g = uvertexColors[i].g, b = uvertexColors[i].b, a = 1 };

            // Convert Normals
            var unormals = mesh.normals;
            inormals = new Geom.Native.Vector3List(unormals.Length);
            if (isZup) {
                for (int i = 0; i < unormals.Length; i++) {
                    unormals[i].Normalize();
                    inormals[i] = new Geom.Native.Point3 { x = unormals[i].x * flipX, y = unormals[i].z, z = unormals[i].y };
                }
            } else {
                for (int i = 0; i < unormals.Length; i++) {
                    unormals[i].Normalize();
                    inormals[i] = new Geom.Native.Point3 { x = unormals[i].x * flipX, y = unormals[i].y, z = unormals[i].z };
                }
            }

            // Convert Tangents
            var utangents = mesh.tangents;
            itangents = new Geom.Native.Vector4List(utangents.Length);
            if (isZup) {
                for (int i = 0; i < utangents.Length; i++) {
                    utangents[i].Normalize();
                    itangents[i] = new Geom.Native.Point4 { x = utangents[i].x * flipX, y = utangents[i].z, z = utangents[i].y, w = utangents[i].w };
                }
            } else {
                for (int i = 0; i < utangents.Length; i++) {
                    utangents[i].Normalize();
                    itangents[i] = new Geom.Native.Point4 { x = utangents[i].x * flipX, y = utangents[i].y, z = utangents[i].z, w = utangents[i].w };
                }
            }

            // Using uvChannels to map. Puts all 8 unity uvs. Can be improved.
            meshDefinition.uvs = new Geom.Native.Point2ListList(8);
            List<int> channels = new List<int>();
            for (int j = 0; j < 8; j++) {
                List<Vector2> uvs = new List<Vector2>();
                mesh.GetUVs(j, uvs);
                meshDefinition.uvs[j] = new Geom.Native.Point2List(uvs.Count);
                if (uvs.Count > 0) {
                    channels.Add(j);
                    for (int i = 0; i < uvs.Count; i++) {
                        meshDefinition.uvs[j].list[i] = new Geom.Native.Point2 { x = uvs[i].x, y = uvs[i].y };
                    }
                }
            }
            meshDefinition.uvChannels = new Core.Native.IntList(channels.ToArray());

            // Prepare Triangles
            int lineVerticesCount = 0;
            int lineSubmeshCount = 0;
            triangles = new List<int>();
            dressedPolys = new List<Polygonal.Native.DressedPoly>();
            lineVertices = new List<Geom.Native.Point3>();
            lines = new List<Polygonal.Native.StylizedLine>();
            var freeVerticesColors = new Geom.Native.Vector3List(0);
            var freeVertices = new Geom.Native.Point3List(0);
            for (int s = 0; s < mesh.subMeshCount; s++) {
                switch (mesh.GetTopology(s)) {
                    case MeshTopology.Triangles:
                        var utriangles = mesh.GetTriangles(s);
                        int firstTriInt = triangles.Count;
                        triangles.AddRange(utriangles);
                        var dressedPoly = new Polygonal.Native.DressedPoly();
                        dressedPoly.firstTri = firstTriInt / 3;
                        dressedPoly.firstQuad = -1;
                        int triCountInt = (int)mesh.GetIndexCount(s);
                        dressedPoly.triCount = triCountInt / 3;
                        dressedPoly.quadCount = 0;
                        dressedPoly.externalId = (uint)dressedPolys.Count;
                        dressedPoly.material = (materials[s] == null) ? 0 : materials[s].id; // Material to IMaterial to do. Handle in MeshConverter or externally ?
                        dressedPolys.Add(dressedPoly);
                        if (isLeftHanded ^ !isZup) {
                            for (int i = 0; i < triCountInt; i += 3) {
                                triangles[firstTriInt + i + 0] = utriangles[i + 1];
                                triangles[firstTriInt + i + 1] = utriangles[i + 0];
                                triangles[firstTriInt + i + 2] = utriangles[i + 2];
                            }
                        } else {
                            for (int i = 0; i < triCountInt; i += 3) {
                                triangles[firstTriInt + i + 0] = utriangles[i + 0];
                                triangles[firstTriInt + i + 1] = utriangles[i + 1];
                                triangles[firstTriInt + i + 2] = utriangles[i + 2];
                            }
                        }
                        break;
                    case MeshTopology.Lines:
                        lineSubmeshCount++;
                        var linesSubmesh = mesh.GetSubmesh(s);
                        var line = new Polygonal.Native.StylizedLine();
                        line.color = new Core.Native.ColorAlpha();
                        line.color.r = materials[s].albedo.color.r;
                        line.color.g = materials[s].albedo.color.g;
                        line.color.b = materials[s].albedo.color.b;
                        line.color.a = 1;
                        line.lines = new Core.Native.IntList(linesSubmesh.indices);
                        var vCount = linesSubmesh.verticesData.Length;
                        lineVerticesCount += vCount;
                        if (isZup) {
                            for (int ind = 0; ind < vCount; ind++) {
                                lineVertices.Add(new Geom.Native.Point3 { x = linesSubmesh.verticesData.vertices[ind].x * scaleFactor * flipX, y = linesSubmesh.verticesData.vertices[ind].z * scaleFactor, z = linesSubmesh.verticesData.vertices[ind].y * scaleFactor });
                            }
                        } else {
                            for (int ind = 0; ind < vCount; ind++) {
                                lineVertices.Add(new Geom.Native.Point3 { x = linesSubmesh.verticesData.vertices[ind].x * scaleFactor * flipX, y = linesSubmesh.verticesData.vertices[ind].y * scaleFactor, z = linesSubmesh.verticesData.vertices[ind].z * scaleFactor });
                            }
                        }
                        lines.Add(line);
                        break;
                    case MeshTopology.Points:
                        var pointsSubmesh = mesh.GetSubmesh(s);
                        int offset = freeVertices.length;
                        Array.Resize(ref freeVertices.list, offset + pointsSubmesh.verticesData.Length);
                        if (isZup) {
                            for (int i = 0; i < pointsSubmesh.verticesData.Length; i++) {
                                freeVertices[i + offset] = new Geom.Native.Point3 { x = pointsSubmesh.verticesData.vertices[i].x * scaleFactor * flipX, y = pointsSubmesh.verticesData.vertices[i].z * scaleFactor, z = pointsSubmesh.verticesData.vertices[i].y * scaleFactor };
                            }
                        } else {
                            for (int i = 0; i < pointsSubmesh.verticesData.Length; i++) {
                                freeVertices[i + offset] = new Geom.Native.Point3 { x = pointsSubmesh.verticesData.vertices[i].x * scaleFactor * flipX, y = pointsSubmesh.verticesData.vertices[i].y * scaleFactor, z = pointsSubmesh.verticesData.vertices[i].z * scaleFactor };
                            }
                        }
                        Array.Resize(ref freeVerticesColors.list, offset + pointsSubmesh.verticesData.Length);
                        for (int i = 0; i < pointsSubmesh.verticesData.Length; i++) {
                            freeVerticesColors[i + offset] = new Geom.Native.Point3 { x = 0.00392156862f * pointsSubmesh.verticesData.colors[i].r, y = 0.00392156862f * pointsSubmesh.verticesData.colors[i].g, z = 0.00392156862f * pointsSubmesh.verticesData.colors[i].b };
                        }
                        break;
                }
            }

            meshDefinition.vertices = ivertices;
            meshDefinition.vertexColors = icolors;
            meshDefinition.linesVertices = new Geom.Native.Point3List(lineVertices.ToArray());
            meshDefinition.lines = new Polygonal.Native.StylizedLineList(lines.ToArray());
            meshDefinition.triangles = new Core.Native.IntList(triangles.ToArray());
            meshDefinition.dressedPolys = new Polygonal.Native.DressedPolyList(dressedPolys.ToArray());
            meshDefinition.normals = inormals;
            meshDefinition.tangents = itangents;
            meshDefinition.points = freeVertices;
            meshDefinition.pointsColors = freeVerticesColors;

            yield break;
        }
    }
}