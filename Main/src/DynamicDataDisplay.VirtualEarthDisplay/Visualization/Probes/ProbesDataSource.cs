using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MapPoint.Data;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.MapPoint.Rendering3D;
using System.Diagnostics;
using Microsoft.MapPoint.Geometry.Geometry2;
using Microsoft.MapPoint.CoordinateSystems;
using Microsoft.MapPoint.Rendering3D.Utility;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization.Probes
{
    class ProbesDataSource : SimplifiedDataSource
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

        IDataSource2D<double> wfield;
        PointSet pointSet;
        GeoRect gridBox;
        DSDataType dataType;
        Host host;

        private double step;
        public double Step
        {
            get
            {
                return step;
            }
        }

        public object Field
        {
            get
            {
                if (dataType == DSDataType.TwoDim)
                    return wfield;
                else
                    return pointSet;
            }
        }



        public void SetPicture(string fileName)
        {
            probesHelper = new ProbesHelper(fileName, true);
            this.OnDataChanged(null);
        }



        public ProbesDataSource(Guid guid, PointSet pointSet, Host host)
            : base(guid)
        {
            // set up the Ontology, a description of the kind of data we contain.
            // See DataSource sample for more details.
            OntologySpecification o = this.Ontology.Edit();
            o.PrimitiveTypes.Create("RasterPatch", "GeoEntity", typeof(RasterPatch2));
            this.UpdateOntology(o);

            EntitySpecification entitySpec = new EntitySpecification(this.Ontology.EntityTypes["GeoEntity"]); // the default entity type
            entity = this.EntityAuthorityReference.EntityAuthority.CreateEntity(true, entitySpec);

            this.pointSet = pointSet;

            if (pointSet.Metadata.ContainsKey("ProbePicture"))
                probesHelper = new ProbesHelper(pointSet.Metadata["ProbePicture"], true);
            else
                probesHelper = new ProbesHelper("ProbeSample.png", false);

            this.host = host;

            dataType = DSDataType.Table;
        }

        ProbesHelper probesHelper;

        public ProbesDataSource(Guid guid, IDataSource2D<double> field, Host host)
            : base(guid)
        {
            // set up the Ontology, a description of the kind of data we contain.
            // See DataSource sample for more details.
            OntologySpecification o = this.Ontology.Edit();
            o.PrimitiveTypes.Create("RasterPatch", "GeoEntity", typeof(RasterPatch2));
            this.UpdateOntology(o);

            EntitySpecification entitySpec = new EntitySpecification(this.Ontology.EntityTypes["GeoEntity"]); // the default entity type
            entity = this.EntityAuthorityReference.EntityAuthority.CreateEntity(true, entitySpec);

            this.wfield = field;

            System.Windows.Point[,] grid = wfield.Grid;
            Coordinate2D minCoordinate = new Coordinate2D(grid[0, 0].X, grid[0, 0].Y);
            Coordinate2D maxCoordinate = new Coordinate2D(grid[field.Width - 1, field.Height - 1].X, grid[field.Width - 1, field.Height - 1].Y);


            for (int j = 0; j < field.Height; j++)
            {
                for (int i = 0; i < field.Width; i++)
                {
                    if (grid[i, j].X < minCoordinate.X)
                        minCoordinate.X = grid[i, j].X;

                    if (grid[i, j].X > maxCoordinate.X)
                        maxCoordinate.X = grid[i, j].X;

                    if (grid[i, j].Y < minCoordinate.Y)
                        minCoordinate.Y = grid[i, j].Y;

                    if (grid[i, j].Y > maxCoordinate.Y)
                        maxCoordinate.Y = grid[i, j].Y;
                }
            }

            gridBox = new GeoRect(
                minCoordinate.X,
                minCoordinate.Y,
                maxCoordinate.X - minCoordinate.X,
                maxCoordinate.Y - minCoordinate.Y);

            dataType = DSDataType.TwoDim;
            this.host = host;

            probesHelper = new ProbesHelper("ProbeSample.png", false);
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
                    double iconSize = 32.0 / Math.Pow(2, levelValue);
                    step = iconSize / 2.0;

                    RasterPatch2 rasterPatch = null;
                    switch (dataType)
                    {
                        case DSDataType.TwoDim:
                            rasterPatch = probesHelper.GetTilePatch(wfield, regionBox, iconSize);
                            break;
                        case DSDataType.Table:
                            levelValue = (int)tileLevel.Value > 6 ? (int)tileLevel.Value : 6;
                            iconSize = 32.0 / Math.Pow(2, levelValue);
                            rasterPatch = probesHelper.GetTilePatch(pointSet, regionBox, iconSize);
                            break;
                        default: break;
                    }

                    if (rasterPatch != null)
                    {
                        PrimitiveSpecification spec = new PrimitiveSpecification(entity,
                        this.Ontology.PrimitiveTypes["RasterPatch"], rasterPatch);
                        SimplePrimitive primitive = new SimplePrimitive(this, spec);
                        Trace.WriteLine("Tile is ready.");

                        return new SingleImageResult(primitive);
                    }
                }
            }

            return new SingleImageResult(null);
        }

        private enum DSDataType
        {
            TwoDim,
            Table
        }


    }
}
