using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MapPoint.Data;
using Microsoft.MapPoint.Geometry.Geometry2;
using Microsoft.MapPoint.Rendering3D;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization.ColorMap
{
    class ColorMapDataSource : SimplifiedDataSource
    {
        #region Base Methods Impl
        protected override PrimitiveCollection AddPrimitivesInternal(PrimitiveSpecification[] primitiveSpecifications, int[] bucketIds, double minScaleDenominator, double maxScaleDenominator)
        {
            throw new NotImplementedException();
        }

        protected override void DeletePrimitivesByEntityId(EntityId entityId)
        {
            throw new NotImplementedException();
        }

        protected override void DeletePrimitivesInternal(PrimitiveId[] primitiveIds)
        {
            throw new NotImplementedException();
        }

        protected override bool ContainsEntityId(EntityId entityId)
        {
            throw new NotImplementedException();
        }
        #endregion

        private Entity entity;
        Host host;
        NonUniformDataSource2D<double> field = null;
        WarpedDataSource2D<double> wfield = null;
        bool isWarped;
        double minT, maxT;
        ColorMapHelper colorMapHelper;

        public ColorMapDataSource(NonUniformDataSource2D<double> field, Host host, double minT, double maxT)
            : base(Guid.NewGuid())
        {
            OntologySpecification o = this.Ontology.Edit();
            o.PrimitiveTypes.Create("RasterPatch", "GeoEntity", typeof(RasterPatch2));
            this.UpdateOntology(o);

            EntitySpecification entitySpec = new EntitySpecification(this.Ontology.EntityTypes["GeoEntity"]); // the default entity type
            entity = this.EntityAuthorityReference.EntityAuthority.CreateEntity(true, entitySpec);

            this.host = host;

            this.minT = minT;
            this.maxT = maxT;

            this.field = field;
            //colorMapHelper = new ColorMapHelper(this.field, null);
            isWarped = false;
        }

        public ColorMapDataSource(WarpedDataSource2D<double> field, Host host, double minT, double maxT)
            : base(Guid.NewGuid())
        {
            OntologySpecification o = this.Ontology.Edit();
            o.PrimitiveTypes.Create("RasterPatch", "GeoEntity", typeof(RasterPatch2));
            this.UpdateOntology(o);

            EntitySpecification entitySpec = new EntitySpecification(this.Ontology.EntityTypes["GeoEntity"]); // the default entity type
            entity = this.EntityAuthorityReference.EntityAuthority.CreateEntity(true, entitySpec);

            this.host = host;
            this.wfield = field;

            this.minT = minT;
            this.maxT = maxT;

            //colorMapHelper = new ColorMapHelper(field, null);
            isWarped = true;
        }

        private class SingleImageResult : PrimitiveSpatialResult
        {
            SimplePrimitive prim;

            public SingleImageResult(SimplePrimitive prim)
            {
                this.prim = prim;
            }

            public override IEnumerator<Primitive> GetEnumerator()
            {
                if (prim != null)
                {
                    yield return prim;
                }
            }
        }

        protected override PrimitiveSpatialResult QueryPrimitivesInternal(PrimitiveSpatialQuery specification, bool uniqueEntities, DataSource.QueryProgress progress)
        {

            GeometryQueryRegion region = specification.SpatialFilter.Region as GeometryQueryRegion;
            if (region != null)
            {
                Box2 regionBox = region.Geometry as Box2;
                if (regionBox != null)
                {
                    RasterPatch2 rasterPatch = null;
                    if (isWarped)
                    {
                        colorMapHelper = new ColorMapHelper(wfield, regionBox, minT, maxT);
                        rasterPatch = colorMapHelper.GetWarpedTilePatch();

                        minT = colorMapHelper.MinT;
                        maxT = colorMapHelper.MaxT;
                    }
                    else
                    {
                        colorMapHelper = new ColorMapHelper(field, regionBox, minT, maxT);
                        rasterPatch = colorMapHelper.GetTilePatch();

                        minT = colorMapHelper.MinT;
                        maxT = colorMapHelper.MaxT;
                    }
                    if (rasterPatch != null)
                    {
                        PrimitiveSpecification spec = new PrimitiveSpecification(entity,
                        this.Ontology.PrimitiveTypes["RasterPatch"], rasterPatch);
                        SimplePrimitive primitive = new SimplePrimitive(this, spec);
                        return new SingleImageResult(primitive);

                    }
                }
            }
            else
            {

            }

            return new SingleImageResult(null);
        }
    }
}
