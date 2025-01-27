using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace BlackTemple.Common
{
    public static class Extensions
    {
        // ---------- Transform extensions
        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        public static string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    CopyTo(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        public static void ClearChildObjects(this Transform transform)
        {
            foreach (Transform child in transform)
                Object.Destroy(child.gameObject);
        }

        public static void ClearChildObjects(this Transform transform, int fromIndex)
        {
            for(var i = fromIndex; i < transform.childCount; i ++)
                Object.Destroy(transform.GetChild(i).gameObject);
        }

        // ---------- Color extensions

        public static void SetColor(this Graphic graphic, string hexColor)
        {
            ColorUtility.TryParseHtmlString(hexColor, out var color);
            graphic.color = color;
        }

        public static string CovertToHex(this Color col)
        {
            return ColorUtility.ToHtmlStringRGB(col);
        }

        public static Color GetHexColor(string hexColor)
        {
            ColorUtility.TryParseHtmlString(hexColor, out var color);
            return  color;
        }

        //  ---------- Json extensions

        public static T FromJson<T>(this string str)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto};
            return  JsonConvert.DeserializeObject<T>(str, settings);
        }

        public static string ToJson<T>(this T obj, Formatting formatting = Formatting.Indented)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto};
            return JsonConvert.SerializeObject(obj, formatting, settings);
        }

    public static T RandomElement<T>(this IList<T> array)
        {
            return array[Random.Range(0, array.Count)];
        }

        public static T DefaultRandomElement<T>(this IList<T> array, T val)
        {
            if (array == null || array.Count == 0)
                return val;

            return array[Random.Range(0, array.Count)];
        }

        // ---------- Encode/Decode

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }


        //  ---------- Data extraction

        public static TU ConvertValue<T,TU>(string val)
        {
            var keyConverter = TypeDescriptor.GetConverter(typeof(T));
            var key = (TU)keyConverter.ConvertFromString(val);
            return key;
        }

        public static List<T> SplitToList<T>(string str, string splitter)
        {
            var newList = new List<T>();

            if (string.IsNullOrEmpty(str))
                return newList;

            var array = str.Split(splitter.ToCharArray());

            var valueConverter = TypeDescriptor.GetConverter(typeof(T));

            newList = !string.IsNullOrEmpty(str)
                ? new List<T>(Array.ConvertAll(array, x => (T)valueConverter.ConvertFromString(x)))
                : new List<T>();

            return newList;
        }

        public static Dictionary<T, TU> GetDictionaryBySplitKeyValuePair<T, TU>(string str, char splitter = ',')
        {
            var newDic = new Dictionary<T, TU>();

            if (string.IsNullOrEmpty(str))
                return newDic; 

            var array = str.Split(splitter);
            foreach (var pairStr in array)
            {
                var pair = GetStringSplitKeyPair<T, TU>(pairStr);

                if (newDic.ContainsKey(pair.Key))
                {
                    Debug.LogError("error " + pair.Key + ":" + pair.Value);
                    continue;
                }
                newDic.Add(pair.Key, pair.Value);
            }

            return newDic;
        }

        public static KeyValuePair<T,TU> GetStringSplitKeyPair<T,TU>(string str, char splitter = ':') 
        {
            if(string.IsNullOrEmpty(str))
                return new KeyValuePair<T, TU>();

            var array = str.Split(splitter);

            var keyConverter = TypeDescriptor.GetConverter(typeof(T));
            var valueConverter = TypeDescriptor.GetConverter(typeof(TU));


            var key = (T)keyConverter.ConvertFromString(array[0]);
            var value = (TU)valueConverter.ConvertFromString(array[1]);

            return new KeyValuePair<T, TU>(key, value);
        }
        

        public static void SetActiveRecursive(this Transform transform, bool isActive)
        {
            transform.gameObject.SetActive(isActive);
            foreach (Transform child in transform)
                child.SetActiveRecursive(isActive);
        }

        public static void ChangeLayerRecursive(this Transform transform, int layer)
        {
            transform.gameObject.layer = layer;
            foreach (Transform child in transform)
                child.ChangeLayerRecursive(layer);
        }


        // ---------- Render extensions

        private static int CountCornersVisibleFrom(this RectTransform rectTransform, Camera camera)
        {
            var screenBounds = new Rect(0f, 0f, Screen.width, Screen.height);
            var objectCorners = new Vector3[4];
            rectTransform.GetWorldCorners(objectCorners);

            var visibleCorners = 0;
            foreach (var corner in objectCorners)
            {
                var tempScreenSpaceCorner = camera.WorldToScreenPoint(corner);
                if (screenBounds.Contains(tempScreenSpaceCorner))
                    visibleCorners++;
            }

            return visibleCorners;
        }

        public static bool IsFullyVisibleFrom(this RectTransform rectTransform, Camera camera)
        {
            return CountCornersVisibleFrom(rectTransform, camera) == 4;
        }

        public static bool IsVisibleFrom(this RectTransform rectTransform, Camera camera)
        {
            return CountCornersVisibleFrom(rectTransform, camera) > 0;
        }


        // ---------- Mesh extensions


        public static Mesh CombineMeshes(GameObject aGo, bool clearChild = true)
        {
            var meshRenders = aGo.GetComponentsInChildren<MeshRenderer>(false);
            var totalVertexCount = 0;
            var totalMeshCount = 0;

            if (meshRenders != null && meshRenders.Length > 0)
            {
                foreach (MeshRenderer meshRenderer in meshRenders)
                {
                    var filter = meshRenderer.gameObject.GetComponent<MeshFilter>();
                    if (filter != null && filter.sharedMesh != null)
                    {
                        totalVertexCount += filter.sharedMesh.vertexCount;
                        totalMeshCount++;
                    }
                }
            }

            if (totalMeshCount == 0)
            {
                Debug.Log("No meshes found in children. There's nothing to combine.");
                return null;
            }
            if (totalMeshCount == 1)
            {
                Debug.Log("Only 1 mesh found in children. There's nothing to combine.");
                return null;
            }
            if (totalVertexCount > 65535)
            {
                Debug.Log("There are too many vertices to combine into 1 mesh (" + totalVertexCount + "). The max. limit is 65535");
                return null;
            }

            var mesh = new Mesh();
            var myTransform = aGo.transform.worldToLocalMatrix;
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var uv1s = new List<Vector2>();
            var uv2s = new List<Vector2>();
            var subMeshes = new Dictionary<Material, List<int>>();

            if (meshRenders != null && meshRenders.Length > 0)
            {
                foreach (MeshRenderer meshRenderer in meshRenders)
                {
                    var filter = meshRenderer.gameObject.GetComponent<MeshFilter>();
                    if (filter != null && filter.sharedMesh != null)
                    {
                        MergeMeshInto(filter.sharedMesh, meshRenderer.sharedMaterials, myTransform * filter.transform.localToWorldMatrix,
                            vertices, normals, uv1s, uv2s, subMeshes);
                        if (filter.gameObject != aGo)
                        {
                            filter.gameObject.SetActive(false);
                        }
                    }
                }
            }

            mesh.vertices = vertices.ToArray();
            if (normals.Count > 0) mesh.normals = normals.ToArray();
            if (uv1s.Count > 0) mesh.uv = uv1s.ToArray();
            if (uv2s.Count > 0) mesh.uv2 = uv2s.ToArray();
            mesh.subMeshCount = subMeshes.Keys.Count;
            var materials = new Material[subMeshes.Keys.Count];
            var mIdx = 0;
            foreach (var m in subMeshes.Keys)
            {
                materials[mIdx] = m;
                mesh.SetTriangles(subMeshes[m].ToArray(), mIdx++);
            }

            if (meshRenders != null && meshRenders.Length > 0)
            {
                var meshRend = aGo.GetComponent<MeshRenderer>();
                if (meshRend == null) meshRend = aGo.AddComponent<MeshRenderer>();
                meshRend.sharedMaterials = materials;

                var meshFilter = aGo.GetComponent<MeshFilter>();
                if (meshFilter == null) meshFilter = aGo.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = mesh;
            }

            if (clearChild)
            {
                aGo.transform.ClearChildObjects();
            }
            return mesh;
        }

        private static void MergeMeshInto(Mesh meshToMerge, Material[] ms, Matrix4x4 transformMatrix, List<Vector3> vertices,
            List<Vector3> normals, List<Vector2> uv1s, List<Vector2> uv2s, Dictionary<Material, List<int>> subMeshes)
        {
            if (meshToMerge == null) return;
            var vertexOffset = vertices.Count;
            var vs = meshToMerge.vertices;

            for (int i = 0; i < vs.Length; i++)
            {
                vs[i] = transformMatrix.MultiplyPoint3x4(vs[i]);
            }
            vertices.AddRange(vs);

            var rotation = Quaternion.LookRotation(transformMatrix.GetColumn(2), transformMatrix.GetColumn(1));
            var ns = meshToMerge.normals;
            if (ns != null && ns.Length > 0)
            {
                for (var i = 0; i < ns.Length; i++) ns[i] = rotation * ns[i];
                normals.AddRange(ns);
            }

            var uvs = meshToMerge.uv;
            if (uvs != null && uvs.Length > 0) uv1s.AddRange(uvs);
            uvs = meshToMerge.uv2;
            if (uvs != null && uvs.Length > 0) uv2s.AddRange(uvs);

            for (var i = 0; i < ms.Length; i++)
            {
                if (i < meshToMerge.subMeshCount)
                {
                    var ts = meshToMerge.GetTriangles(i);
                    if (ts.Length > 0)
                    {
                        if (ms[i] != null && !subMeshes.ContainsKey(ms[i]))
                        {
                            subMeshes.Add(ms[i], new List<int>());
                        }
                        var subMesh = subMeshes[ms[i]];
                        for (var t = 0; t < ts.Length; t++)
                        {
                            ts[t] += vertexOffset;
                        }
                        subMesh.AddRange(ts);
                    }
                }
            }
        }

    }
}