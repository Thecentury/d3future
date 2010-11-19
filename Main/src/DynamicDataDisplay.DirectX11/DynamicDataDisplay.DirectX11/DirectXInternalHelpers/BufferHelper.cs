using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.DirectX.Direct3D11;
using Microsoft.Research.DynamicDataDisplay.DirectX11.VertexStructures;
using System.Runtime.InteropServices;

namespace Microsoft.Research.DynamicDataDisplay.DirectX11.DirectXInternalHelpers
{
    internal static class BufferHelper
    {
        public static D3DBuffer CreateBuffer<T>(D3DDevice device, T[] vertices) where T : struct
        {
            int byteLength = Marshal.SizeOf(typeof(T)) * vertices.Length;
            IntPtr nativeVertex = Marshal.AllocHGlobal(byteLength);
            byte[] byteBuffer = new byte[byteLength];
            for (int i = 0; i < vertices.Length; i++)
            {
                byte[] vertexData = RawSerialize(vertices[i]);
                Buffer.BlockCopy(vertexData, 0, byteBuffer, vertexData.Length * i, vertexData.Length);
            }
            Marshal.Copy(byteBuffer, 0, nativeVertex, byteLength);

            // build vertex buffer
            BufferDescription bdv = new BufferDescription()
            {
                Usage = Usage.Default,
                ByteWidth = (uint)(Marshal.SizeOf(typeof(T)) * vertices.Length),
                BindFlags = BindFlag.VertexBuffer,
                CpuAccessFlags = 0,
                MiscFlags = 0
            };
            SubresourceData vertexInit = new SubresourceData()
            {
                SysMem = nativeVertex
            };

            return device.CreateBuffer(bdv, vertexInit); 
        }

        /// <summary>
        /// Copys an arbitrary strcuture into a byte array
        /// </summary>
        /// <param name="anything"></param>
        /// <returns></returns>
        public static byte[] RawSerialize(object anything)
        {
            int rawsize = Marshal.SizeOf(anything);
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.StructureToPtr(anything, buffer, false);
            byte[] rawdatas = new byte[rawsize];
            Marshal.Copy(buffer, rawdatas, 0, rawsize);
            Marshal.FreeHGlobal(buffer);
            return rawdatas;
        } 
    }
}
