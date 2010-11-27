using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

namespace MathParser
{
	public sealed class Parser
	{
		public Parser() { }

		public Parser(params string[] parameters)
		{
			Contract.Assert(parameters != null);

			foreach (var parameter in parameters)
			{
				Contract.Assert(!String.IsNullOrEmpty(parameter));

				Parameters.Add(parameter);
			}
		}

		private Grammar grammar = new Grammar();
		public Grammar Grammar
		{
			get { return grammar; }
		}

		private class NameParameterEqueliatyComparer : IEqualityComparer<LexicToken>
		{
			#region IEqualityComparer<ParameterToken> Members

			public bool Equals(LexicToken x, LexicToken y)
			{
				var first = x as ParameterToken;
				var second = y as ParameterToken;
				return first.ParameterName == second.ParameterName;
			}

			public int GetHashCode(LexicToken obj)
			{
				return (obj as ParameterToken).ParameterName.GetHashCode();
			}

			#endregion
		}

		private readonly ParameterExpressionEqualityComparer comparer = new ParameterExpressionEqualityComparer();

		public ParsingResult Parse(string expression)
		{
			InputStream input = new InputStream(expression);
			var tokens = grammar.Parse(input);
			var filteredTokens = grammar.Filter(tokens);

			var tokensList = filteredTokens.ToList();

			var sortedParameters = (from token in tokensList
									where token is ParameterToken
									let i = tokensList.IndexOf(token)
									orderby i
									select token).Distinct(new NameParameterEqueliatyComparer()).ToList();

			var mixedTokens = grammar.ConvertToMixed(filteredTokens);
			var ast = grammar.CreateAST(mixedTokens);

			if (mixedTokens.Count != 1)
				throw new ParserException("Wrong expression.");

			var optimizedAst = ast.Optimize();

			var paramsDict = Parameters.ToDictionary(s => Parameters.IndexOf(s));

			var dict = new Dictionary<string, int>();

			foreach (var item in paramsDict)
			{
				dict.Add(item.Value, item.Key);
			}

			var result = new ParsingResult
			{
				Tree = optimizedAst,
				ParameterNames = sortedParameters.Select(token => ((ParameterToken)token).ParameterName),
				ParameterExpressions = grammar.ParameterExpressions.Values.OrderBy(p => dict[p.Name]).ToArray()
			};
			return result;
		}

		public double Evaluate(string expression)
		{
			return Parse(expression).Evaluate();
		}

		public Collection<string> Parameters
		{
			get { return grammar.Parameters; }
		}
	}
}
