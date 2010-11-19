using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MapPoint.Rendering3D.Utility;
using Microsoft.MapPoint.Data;
using Microsoft.MapPoint.Geometry.Geometry2;
using DynamicDataDisplay.VirtualEarthDisplay.Visualization.Probes;
using Microsoft.MapPoint.Rendering3D;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization.Trajectories
{
    class TrajectoriesDataSource
    : SimplifiedDataSource
    {
        #region SimplifiedDataSourceImpl
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
        PointSet pointSet;

        internal PointSet PointSet
        {
            get { return pointSet; }
        }

        Host host;
        ProbesHelper probesHelper;

        private double step;
        public double Step
        {
            get { return step; }
        }

        public TrajectoriesDataSource(Host host, PointSet data)
            : base(Guid.NewGuid())
        {
            this.pointSet = data;
            this.host = host;

            if (pointSet.Metadata.ContainsKey("ProbePicture"))
                probesHelper = new ProbesHelper(pointSet.Metadata["ProbePicture"], true);
            else
                probesHelper = new ProbesHelper("ProbeSample.png", false);

            // set up the Ontology, a description of the kind of data we contain.
            // See DataSource sample for more details.
            OntologySpecification o = this.Ontology.Edit();
            o.PrimitiveTypes.Create("RasterPatch", "GeoEntity", typeof(RasterPatch2));

            this.UpdateOntology(o);

            EntitySpecification entitySpec = new EntitySpecification(this.Ontology.EntityTypes["GeoEntity"]);
            entity = this.EntityAuthorityReference.EntityAuthority.CreateEntity(true, entitySpec);

        }

        public void SetPicture(string fileName)
        {
            probesHelper = new ProbesHelper(fileName, true);
            this.OnDataChanged(null);
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

            TileLevelOfDetail tileLevel = (TileLevelOfDetail)host.WorldEngine.ScaleRanges.ScaleToTileLevelOfDetailValue(specification.SpatialFilter.GeometryOptions.Scale);


            if (region != null)
            {
                Box2 regionBox = region.Geometry as Box2;
                if (regionBox != null)
                {
                    int levelValue = (int)tileLevel.Value > 5 ? (int)tileLevel.Value : 5;
                    double iconSize = 64.0 / Math.Pow(2, levelValue);
                    this.step = iconSize / 2.0;

                    RasterPatch2 rasterPatch = probesHelper.GetTilePatch(pointSet, regionBox, iconSize);

                    if (rasterPatch != null)
                    {
                        PrimitiveSpecification spec = new PrimitiveSpecification(entity,
                        this.Ontology.PrimitiveTypes["RasterPatch"], rasterPatch);
                        SimplePrimitive primitive = new SimplePrimitive(this, spec);
                        return new SingleImageResult(primitive);
                    }
                }
            }

            return new SingleImageResult(null);
        }


    }
}
