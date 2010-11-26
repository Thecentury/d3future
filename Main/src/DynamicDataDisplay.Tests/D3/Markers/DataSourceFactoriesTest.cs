using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using DynamicDataDisplay.Markers.DataSources.DataSourceFactories;
using DynamicDataDisplay.Markers.DataSources;
using System.Collections;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using System.Xml;
using Microsoft.Research.DynamicDataDisplay.Markers.DataSources;

namespace DynamicDataDisplay.Tests.D3.Markers
{
	[TestClass]
	public class DataSourceFactoriesTest
	{
		[TestMethod]
		public void CreateDataSourceFromFuncDoubleDouble()
		{
			var func = new Func<double, double>(i => i);
			var store = DataSourceFactoryStore.Current;

			var ds = store.BuildDataSource(func);

			Assert.IsInstanceOfType(ds, typeof(DoubleLambdaDataSource));
		}

		[TestMethod]
		public void CreateDataSourceFromDataSource()
		{
			EnumerablePointDataSource ds = new EnumerablePointDataSource(Enumerable.Range(0, 0).Select(i => new Point()));
			var store = DataSourceFactoryStore.Current;

			var newDs = store.BuildDataSource(ds);

			Assert.AreEqual(ds, newDs);
		}

		[TestMethod]
		public void CreateIEnumerablePointDataSource()
		{
			var seq = Enumerable.Range(0, 0).Select(i => new Point());
			var store = DataSourceFactoryStore.Current;

			var ds = store.BuildDataSource(seq);

			Assert.IsInstanceOfType(ds, typeof(EnumerablePointDataSource));
		}

		[TestMethod]
		public void CreateDataSourceFromPointArray()
		{
			Point[] pts = new Point[] { new Point(0.1, 0.2) };
			var store = DataSourceFactoryStore.Current;
			var ds = store.BuildDataSource(pts);

			Assert.IsTrue(ds is PointArrayDataSource);
		}

		[TestMethod]
		public void UseDataSourceAsAnItemSource()
		{
			Point[] points = new Point[] { new Point(0, 0), new Point(1, 1) };
			PointArrayDataSource ds = new PointArrayDataSource(points);

			var store = DataSourceFactoryStore.Current;
			var newDs = store.BuildDataSource(ds);

			Assert.AreEqual(ds, newDs);
		}

		[TestMethod]
		public void CreateDataSourceFromGenericIList()
		{
			var data = new GenericIList();
			var store = DataSourceFactoryStore.Current;

			var ds = store.BuildDataSource(data);

			Assert.IsInstanceOfType(ds, typeof(GenericIListDataSource<int>));
		}

		[TestMethod]
		public void CreateDataSourceFromGenericIEnumerable()
		{
			var data = Enumerable.Range(0, 10);
			var store = DataSourceFactoryStore.Current;

			var ds = store.BuildDataSource(data);

			Assert.IsInstanceOfType(ds, typeof(GenericIEnumerableDataSource<int>));
		}

		[TestMethod]
		public void CreateDataSourceFromIDataSource2d()
		{
			var data = new EmptyDataSource2D<int>();
			var store = DataSourceFactoryStore.Current;

			var ds = store.BuildDataSource(data);

			Assert.IsInstanceOfType(ds, typeof(GenericDataSource2D<int>));
		}

		[TestMethod]
		public void CreateDataSourceFromXmlElement()
		{
			var data = new XmlDocument().CreateElement("name");
			var store = DataSourceFactoryStore.Current;

			var ds = store.BuildDataSource(data);

			Assert.IsInstanceOfType(ds, typeof(XmlElementDataSource));
		}

		[TestMethod]
		public void CreateDataSourceFromIEnumerable()
		{
			var data = new EnumerableClass();
			var store = DataSourceFactoryStore.Current;

			var ds = store.BuildDataSource(data);

			Assert.IsInstanceOfType(ds, typeof(EnumerableDataSource));
		}

		// todo create dataSourceFactory for XElement.

		/// <summary>
		/// Class that implements IEnumerable. Used for testing the creation of GenericIEnumerableDataSource.
		/// </summary>
		private sealed class EnumerableClass : IEnumerable
		{
			#region IEnumerable Members

			public IEnumerator GetEnumerator()
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		/// <summary>
		/// Class that implements IList<>. Used to test creation of GenericIListDataSource.
		/// </summary>
		private sealed class GenericIList : IList<int>
		{
			#region IList<int> Members

			public int IndexOf(int item)
			{
				throw new NotImplementedException();
			}

			public void Insert(int index, int item)
			{
				throw new NotImplementedException();
			}

			public void RemoveAt(int index)
			{
				throw new NotImplementedException();
			}

			public int this[int index]
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			#endregion

			#region ICollection<int> Members

			public void Add(int item)
			{
				throw new NotImplementedException();
			}

			public void Clear()
			{
				throw new NotImplementedException();
			}

			public bool Contains(int item)
			{
				throw new NotImplementedException();
			}

			public void CopyTo(int[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}

			public int Count
			{
				get { throw new NotImplementedException(); }
			}

			public bool IsReadOnly
			{
				get { throw new NotImplementedException(); }
			}

			public bool Remove(int item)
			{
				throw new NotImplementedException();
			}

			#endregion

			#region IEnumerable<int> Members

			public IEnumerator<int> GetEnumerator()
			{
				throw new NotImplementedException();
			}

			#endregion

			#region IEnumerable Members

			IEnumerator IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			#endregion
		}
	}
}
