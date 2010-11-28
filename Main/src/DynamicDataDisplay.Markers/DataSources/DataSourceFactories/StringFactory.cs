using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicDataDisplay.Markers.DataSources.DataSourceFactories;
using DynamicDataDisplay.Markers.DataSources;
using MathParser;
using System.Linq.Expressions;

namespace Microsoft.Research.DynamicDataDisplay.Markers.DataSources.DataSourceFactories
{
	public sealed class StringFactory : DataSourceFactory
	{
		public override PointDataSourceBase TryBuild(object data)
		{
			string expression = data as string;
			if (expression != null)
			{
				try
				{
					Parser parser = new Parser("x");
					var expr = parser.Parse(expression).ToExpression<Func<double, double>>();
					Func<double, double> func = expr.Compile();

					DoubleLambdaDataSource ds = new DoubleLambdaDataSource(func);
					return ds;
				}
				catch (ParserException exc1)
				{
					try
					{
						Parser parser = new Parser("x", "t");
						var expr = parser.Parse(expression).ToExpression<Func<double, double, double>>();
						Func<double, double, double> func = expr.Compile();

						AnimatedDoubleLambdaDataSource ds = new AnimatedDoubleLambdaDataSource(func);
						return ds;
					}
					catch (ParserException exc2)
					{

					}
				}
			}

			return null;
		}
	}
}
