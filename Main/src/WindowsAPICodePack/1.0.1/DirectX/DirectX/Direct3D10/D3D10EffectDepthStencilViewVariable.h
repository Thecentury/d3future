//Copyright (c) Microsoft Corporation.  All rights reserved.

#pragma once
#include "D3D10EffectVariable.h"

namespace  Microsoft { namespace WindowsAPICodePack { namespace DirectX { namespace Direct3D10 {

using namespace System;
using namespace System::Collections::Generic;

ref class DepthStencilView;

    /// <summary>
    /// A depth-stencil-view-variable interface accesses a depth-stencil view.
    /// <para>(Also see DirectX SDK: ID3D10EffectDepthStencilViewVariable)</para>
    /// </summary>
    public ref class EffectDepthStencilViewVariable :
        public EffectVariable
    {
    public: 
        /// <summary>
        /// Get a depth-stencil-view resource.
        /// <para>(Also see DirectX SDK: ID3D10EffectDepthStencilViewVariable::GetDepthStencil)</para>
        /// </summary>
        /// <returns>A depth-stencil-view object. See DepthStencilView Object.</returns>
        DepthStencilView^ GetDepthStencil();

        /// <summary>
        /// Get a collection of depth-stencil-view resources.
        /// <para>(Also see DirectX SDK: ID3D10EffectDepthStencilViewVariable::GetDepthStencilArray)</para>
        /// </summary>
        /// <param name="offset">The zero-based array index to get the first object.</param>
        /// <param name="count">The number of elements requested in the collection.</param>
        /// <returns>A collection of depth-stencil-view interfaces.</returns>
        ReadOnlyCollection<DepthStencilView^>^ GetDepthStencilArray(UInt32 offset, UInt32 count);

        /// <summary>
        /// Set a depth-stencil-view resource.
        /// <para>(Also see DirectX SDK: ID3D10EffectDepthStencilViewVariable::SetDepthStencil)</para>
        /// </summary>
        /// <param name="resource">A depth-stencil-view object.</param>
        void SetDepthStencil(DepthStencilView^ resource);

        /// <summary>
        /// Set a collection of depth-stencil-view resources.
        /// <para>(Also see DirectX SDK: ID3D10EffectDepthStencilViewVariable::SetDepthStencilArray)</para>
        /// </summary>
        /// <param name="resources">A collection of depth-stencil-view interfaces.</param>
        /// <param name="offset">The zero-based index to set the first object in the collection.</param>
        void SetDepthStencilArray(IEnumerable<DepthStencilView^>^ resources, UInt32 offset);

    internal:
        EffectDepthStencilViewVariable()
        {
        }
    internal:

        EffectDepthStencilViewVariable(ID3D10EffectDepthStencilViewVariable* pNativeID3D10EffectDepthStencilViewVariable) : 
            EffectVariable(pNativeID3D10EffectDepthStencilViewVariable)
        { }


        EffectDepthStencilViewVariable(ID3D10EffectDepthStencilViewVariable* pNativeID3D10EffectDepthStencilViewVariable, bool deletable): 
            EffectVariable(pNativeID3D10EffectDepthStencilViewVariable, deletable)
        { }


    };
} } } }
