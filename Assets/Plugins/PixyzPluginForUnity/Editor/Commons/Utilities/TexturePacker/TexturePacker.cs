using UnityEngine;
using UnityEngine.Rendering;

namespace Pixyz.Commons.Utilities
{
    public enum TexturePackerOperation
    {
        None = -1,
        OneMinus = 1,
        Multiply = 2
    }
    public static class TexturePacker
    {
        public class ChannelInfo
        {
            public int channelIndex;
            public Texture2D texture = null;
            public TexturePackerOperation operation = TexturePackerOperation.None;
            public float operationFactor = 1.0f;

            public ChannelInfo(int channelIndex, Texture2D texture, TexturePackerOperation operation = TexturePackerOperation.None, float operationFactor = 1.0f)
            {
                this.channelIndex = channelIndex;
                this.texture = texture;
                this.operation = operation;
                this.operationFactor = operationFactor;
            }
        }

        [System.Serializable]
        private struct Globals
        {
            public int channelIndex;
            public int operationType;
            public float operationFactor;

            public Globals(int channelIndex = 0, int operationType = -1, float operationFactor = 1.0f)
            {
                this.channelIndex = channelIndex;
                this.operationType = operationType;
                this.operationFactor = operationFactor;
            }

            public static int GetStride()
            {
                return sizeof(int) * 2 + sizeof(float);
            }
        }

        public static void Pack(out Texture2D outputTex, ChannelInfo redChannel = null, ChannelInfo blueChannel = null, ChannelInfo greenChannel = null, ChannelInfo alphaChannel = null)
        {
            Vector2Int size = GetTextureSize(redChannel, blueChannel, greenChannel, alphaChannel);
            outputTex = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false, false);

            UnityEngine.Material mat = new UnityEngine.Material(Shader.Find("Pixyz/TexturePacker"));

            if (redChannel != null)
            {
                mat.SetTexture("_RedTex", redChannel.texture);
                mat.SetInt("_RedTexChan", redChannel.channelIndex);
                mat.SetInt("_RedEnable", 1);
            }

            if (greenChannel != null)
            {
                mat.SetTexture("_GreenTex", greenChannel.texture);
                mat.SetInt("_GreenTexChan", greenChannel.channelIndex);
                mat.SetInt("_GreenEnable", 1);
            }

            if (blueChannel != null)
            {
                mat.SetTexture("_BlueTex", blueChannel.texture);
                mat.SetInt("_BlueTexChan", blueChannel.channelIndex);
                mat.SetInt("_BlueEnable", 1);
            }

            if (alphaChannel != null)
            {
                mat.SetTexture("_AlphaTex", alphaChannel.texture);
                mat.SetInt("_AlphaTexChan", alphaChannel.channelIndex);
                mat.SetInt("_AlphaEnable", 1);
            }

            RenderTexture tempRT = RenderTexture.GetTemporary(size.x, size.y, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            tempRT.filterMode = FilterMode.Point;

            RenderTexture oldRT = RenderTexture.active;

            RenderTexture.active = tempRT;
            Graphics.Blit(outputTex, tempRT, mat, 0);

            outputTex.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);
            outputTex.Apply();

            RenderTexture.active = oldRT;
        }

        public static void UnPack(in Texture2D inputTex, ChannelInfo redChannel = null, ChannelInfo greenChannel = null, ChannelInfo blueChannel = null, ChannelInfo alphaChannel = null)
        {
            if (redChannel == null && blueChannel == null && greenChannel == null && alphaChannel == null)
            {
                Debug.LogWarning("[TexturePacker] At least one channel must not be null to unpack, skipping --");
                return;
            }

            ComputeShader cs = (ComputeShader)Resources.Load("Shaders/TexturePacker/TexturePackerCS");
            ComputeBuffer buffer = new ComputeBuffer(4, Globals.GetStride());
            RenderTexture oldRT = RenderTexture.active;
            RenderTexture[] renderTextures = new RenderTexture[4];

            int kernelIndex = cs.FindKernel("Unpack");

            RenderTextureDescriptor desc = new RenderTextureDescriptor();
            desc.autoGenerateMips = false;
            desc.enableRandomWrite = true;
            desc.width = inputTex.width;
            desc.height = inputTex.height;
            desc.colorFormat = RenderTextureFormat.ARGB32;
            desc.depthBufferBits = 16;
            desc.volumeDepth = 1;
            desc.msaaSamples = 1;
            desc.dimension = TextureDimension.Tex2D;

            cs.SetTexture(kernelIndex, "MainTex", inputTex);

            for (int i = 0; i < 4; ++i)
            {
                renderTextures[i] = RenderTexture.GetTemporary(desc);
                renderTextures[i].Create();
                cs.SetTexture(kernelIndex, "Tex" + i, renderTextures[i]);
            }

            buffer.SetData(new Globals[] { SetGlobals(redChannel), SetGlobals(greenChannel), SetGlobals(blueChannel), SetGlobals(alphaChannel) });
            cs.SetBuffer(kernelIndex, "Params", buffer);

            cs.Dispatch(kernelIndex, inputTex.width, inputTex.height, 1);

            if (redChannel != null)
            {
                RenderTexture.active = renderTextures[0];
                redChannel.texture = new Texture2D(inputTex.width, inputTex.height, TextureFormat.RGBA32, false, false);
                redChannel.texture.ReadPixels(new Rect(0, 0, inputTex.width, inputTex.height), 0, 0);
                redChannel.texture.Apply();
            }

            if (greenChannel != null)
            {
                RenderTexture.active = renderTextures[1];
                greenChannel.texture = new Texture2D(inputTex.width, inputTex.height, TextureFormat.RGBA32, false, false);
                greenChannel.texture.ReadPixels(new Rect(0, 0, inputTex.width, inputTex.height), 0, 0);
                greenChannel.texture.Apply();
            }

            if (blueChannel != null)
            {
                RenderTexture.active = renderTextures[2];
                blueChannel.texture = new Texture2D(inputTex.width, inputTex.height, TextureFormat.RGBA32, false, false);
                blueChannel.texture.ReadPixels(new Rect(0, 0, inputTex.width, inputTex.height), 0, 0);
                blueChannel.texture.Apply();
            }

            if (alphaChannel != null)
            {
                RenderTexture.active = renderTextures[3];
                alphaChannel.texture = new Texture2D(inputTex.width, inputTex.height, TextureFormat.RGBA32, false, false);
                alphaChannel.texture.ReadPixels(new Rect(0, 0, inputTex.width, inputTex.height), 0, 0);
                alphaChannel.texture.Apply();
            }

            RenderTexture.active = oldRT;

            foreach (RenderTexture rt in renderTextures)
            {
                rt.Release();
            }
            buffer.Dispose();
        }

        private static Globals SetGlobals(ChannelInfo channel)
        {
            Globals global = new Globals();

            if (channel == null)
                return global;

            global.channelIndex = channel != null ? channel.channelIndex : -1;
            global.operationType = (int)channel.operation;
            global.operationFactor = channel.operationFactor;

            return global;
        }

        private static Vector2Int GetTextureSize(ChannelInfo redChannel = null, ChannelInfo blueChannel = null, ChannelInfo greenChannel = null, ChannelInfo alphaChannel = null)
        {
            Vector2Int size = new Vector2Int();
            bool allEmpty = true;

            if (redChannel != null)
            {
                if (redChannel.texture == null)
                    throw new System.Exception("[TexturePacker] Missing texture for redChannel");
                size = new Vector2Int(redChannel.texture.width, redChannel.texture.height);
                allEmpty = false;
            }

            if (blueChannel != null)
            {
                if (blueChannel.texture == null)
                    throw new System.Exception("[TexturePacker] Missing texture for blueChannel");
                if (size == Vector2Int.zero)
                {
                    size = new Vector2Int(blueChannel.texture.width, blueChannel.texture.height);
                }
                else
                {
                    size.x = Mathf.Max(size.x, blueChannel.texture.width);
                    size.y = Mathf.Max(size.y, blueChannel.texture.height);
                }
                allEmpty = false;
            }
            if (greenChannel != null)
            {
                if (greenChannel.texture == null)
                    throw new System.Exception("[TexturePacker] Missing texture for greenChannel");
                if (size == Vector2Int.zero)
                {
                    size = new Vector2Int(greenChannel.texture.width, greenChannel.texture.height);
                }
                else
                {
                    size.x = Mathf.Max(size.x, greenChannel.texture.width);
                    size.y = Mathf.Max(size.y, greenChannel.texture.height);
                }
                allEmpty = false;
            }
            if (alphaChannel != null)
            {
                if (alphaChannel.texture == null)
                    throw new System.Exception("[TexturePacker] Missing texture for alphaChannel");
                if (size == Vector2Int.zero)
                {
                    size = new Vector2Int(alphaChannel.texture.width, alphaChannel.texture.height);
                }
                else
                {
                    size.x = Mathf.Max(size.x, alphaChannel.texture.width);
                    size.y = Mathf.Max(size.y, alphaChannel.texture.height);
                }
                allEmpty = false;
            }

            if (allEmpty)
                throw new System.Exception("[TexturePacker] No channel set to be packed");

            return size;
        }
    }
}