using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.DirectX.Direct3D;
using Microsoft.WindowsAPICodePack.DirectX.Direct3D11;
using Microsoft.WindowsAPICodePack.DirectX.DXGI;

namespace Microsoft.Research.DynamicDataDisplay.DirectX11.VertexStructures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionColor
    {
        /// <summary>
        /// The vertex location
        /// </summary>
        [MarshalAs(UnmanagedType.Struct)]
        public Vector3F Vertex;

        /// <summary>
        /// The vertex color
        /// </summary>
        [MarshalAs(UnmanagedType.Struct)]
        public Vector4F Color;

        public static InputElementDescription[] InputElementsDescription
        {
            get 
            {
                return new InputElementDescription[] 
                {
                    new InputElementDescription()
                    {
                        SemanticName = "POSITION",
                        SemanticIndex = 0,
                        Format = Format.R32G32B32_FLOAT,
                        InputSlot = 0,
                        AlignedByteOffset = 0,
                        InputSlotClass = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElementDescription()
                    {
                        SemanticName = "COLOR",
                        SemanticIndex = 0,
                        Format = Format.R32G32B32A32_FLOAT,
                        InputSlot = 0,
                        AlignedByteOffset = 12,
                        InputSlotClass = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                };
            }
        }

        public static uint Size
        {
            get { return (uint)(sizeof(float) * (3 + 4)); }
        }
    };
}
