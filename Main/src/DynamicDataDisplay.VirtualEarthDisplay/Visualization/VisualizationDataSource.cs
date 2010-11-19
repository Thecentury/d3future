using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicDataDisplay.VirtualEarthDisplay.Visualization
{
    public class VisualizationDataSource
    {
        private Guid guid;
        /// <summary>
        /// DataSource Guid
        /// </summary>
        public Guid Guid
        {
            get { return guid; }
        }

        private string parameterName;
        /// <summary>
        /// Name of parameter
        /// </summary>
        public string ParameterName
        {
            get { return parameterName; }
        }


        private object data;
        /// <summary>
        /// Inner Data
        /// </summary>
        public object Data
        {
            get { return data; }
            set { data = value; }
        }

        private Algorithms algorithm;
        /// <summary>
        /// Algorithm for visualization
        /// </summary>
        public Algorithms Algorithm
        {
            get { return algorithm; }
            set
            {
                algorithm = value;
            }
        }

        public bool IsDynamic
        {
            get;
            set;
        }



        public VisualizationDataSource(string parameterName, object data, Algorithms algorithm, DataType dataType, bool isDynamic)
        {
            this.guid = Guid.NewGuid();

            this.parameterName = parameterName;
            this.data = data;
            this.algorithm = algorithm;

            List<string> availibleAlgorithms = new List<string>();
            switch (dataType)
            {
                case DataType.ScalarData:
                    availibleAlgorithms.Add(GetAlgorithmName(Algorithms.ColorMap));
                    availibleAlgorithms.Add(GetAlgorithmName(Algorithms.IsolineMap));
                    availibleAlgorithms.Add(GetAlgorithmName(Algorithms.Probes));
                    break;
                case DataType.VectorData:
                    availibleAlgorithms.Add(GetAlgorithmName(Algorithms.VectorMarkers));
                    break;
                case DataType.PointSet:
                    availibleAlgorithms.Add(GetAlgorithmName(Algorithms.Probes));
                    availibleAlgorithms.Add(GetAlgorithmName(Algorithms.Trajectories));
                    break;
                default: break;
            }

            this.IsDynamic = isDynamic;
        }

        private string GetAlgorithmName(Algorithms algorithm)
        {
            switch (algorithm)
            {
                case Algorithms.ColorMap: return "Color Map";
                case Algorithms.IsolineMap: return "Isoline Map";
                case Algorithms.Probes: return "Probes";
                case Algorithms.VectorMarkers: return "Vector Markers";
                case Algorithms.Trajectories: return "Trajectories";
                default: break;
            }
            throw new Exception("Something strange has happened");
        }

        public static Algorithms GetAlgorithmByName(string algorithmName)
        {
            switch (algorithmName)
            {
                case "Color Map": return Algorithms.ColorMap;
                case "Probes": return Algorithms.Probes;
                case "Isoline Map": return Algorithms.IsolineMap;
                case "Vector Markers": return Algorithms.VectorMarkers;
                case "Trajectories": return Algorithms.Trajectories;
                default: throw new ArgumentOutOfRangeException("String must be algorithm name");
            }
        }

    }

    public enum Algorithms
    {
        ColorMap,
        IsolineMap,
        Probes,
        VectorMarkers,
        Trajectories
    }

    public enum DataType
    {
        VectorData,
        ScalarData,
        PointSet
    }
}
