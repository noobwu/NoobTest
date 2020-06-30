using Dapper;
using KeMai.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace KeMai.StackExchange.Dapper
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlSever2012Template : SqlSelectBuilder.SelectTemplate
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="sqlSelect">eg:SELECT /**select**/</param>
        /// <param name="sqlBody">eg:FROM  Person /**leftjoin**/ /**where**/</param>
        /// <param name="sqlOrderBy">eg:/**orderby**/</param>
        /// <param name="parameters"></param>
        public SqlSever2012Template(SqlSelectBuilder builder, string sqlSelect, string sqlBody, string sqlOrderBy, object parameters)
            : base(builder, sqlSelect, sqlBody, sqlOrderBy, parameters)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public override string SqlLimit(int? offset = null, int? rows = null)
        {
            return rows == null && offset == null
? ""
: rows != null
? "OFFSET " + offset.GetValueOrDefault() + " ROWS FETCH NEXT " + rows + " ROWS ONLY"
: "OFFSET " + offset.GetValueOrDefault(int.MaxValue) + " ROWS";
        }

    }
    /// <summary>
    /// Sql Template
    /// </summary>
    public class SqlSeverTemplate : SqlSelectBuilder.SelectTemplate
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="sqlSelect">eg:SELECT /**select**/</param>
        /// <param name="sqlBody">eg:FROM  Person /**leftjoin**/ /**where**/</param>
        /// <param name="sqlOrderBy">eg:/**orderby**/</param>
        /// <param name="parameters"></param>
        public SqlSeverTemplate(SqlSelectBuilder builder, string sqlSelect, string sqlBody, string sqlOrderBy, object parameters)
            : base(builder, sqlSelect, sqlBody, sqlOrderBy, parameters)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectExpression"></param>
        /// <param name="bodyExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="offset"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public override string GetRowSql(string selectExpression, string bodyExpression, string orderByExpression = null, int? offset = null, int? rows = null)
        {
            var sb = StringBuilderCache.Allocate()
                .Append(selectExpression)
                .Append(bodyExpression);

            if (!offset.HasValue && !rows.HasValue)
                return StringBuilderCache.ReturnAndFree(sb) + orderByExpression;

            if (offset.HasValue && offset.Value < 0)
                throw new ArgumentException("Skip value:'" + offset.Value + "' must be>=0");

            if (rows.HasValue && rows.Value < 0)
                throw new ArgumentException("Rows value:'" + rows.Value + "' must be>=0");

            var skip = offset ?? 0;
            var take = rows ?? int.MaxValue;

            var selectType = selectExpression.StartsWithIgnoreCase("SELECT DISTINCT") ? "SELECT DISTINCT" : "SELECT";

            //Temporary hack till we come up with a more robust paging sln for SqlServer
            if (skip == 0)
            {
                var sql = StringBuilderCache.ReturnAndFree(sb) + orderByExpression;

                if (take == int.MaxValue)
                {
                    return sql;
                }

                if (sql.Length < "SELECT".Length)
                {
                    return sql;
                }

                return string.Format("{0} TOP {1}", selectType, take + sql.Substring(selectType.Length));
            }

            // Required because ordering is done by Windowing function
            if (string.IsNullOrEmpty(orderByExpression))
            {
                orderByExpression = "ORDER BY 1";
            }

            var row = take == int.MaxValue ? take : skip + take;

            var ret = string.Format("SELECT * FROM (SELECT {0}, ROW_NUMBER() OVER ({1}) As RowNum {2}) AS RowConstrainedResult WHERE RowNum > {3} AND RowNum <= {4}", selectExpression.Substring(selectType.Length), orderByExpression, bodyExpression, skip, row);

            return ret;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class SqlSelectBuilder
    {
        private readonly Dictionary<string, Clauses> _bodyData = new Dictionary<string, Clauses>();
        private readonly Dictionary<string, Clauses> _selectData = new Dictionary<string, Clauses>();
        private readonly Dictionary<string, Clauses> _orderByData = new Dictionary<string, Clauses>();
        private int _seq;
        /// <summary>
        /// 
        /// </summary>
        private class Clause
        {
            public string Sql { get; set; }
            public object Parameters { get; set; }
            public bool IsInclusive { get; set; }
        }
        /// <summary>
        /// 
        /// </summary>
        private class Clauses : List<Clause>
        {
            private readonly string _joiner, _prefix, _postfix;

            public Clauses(string joiner, string prefix = "", string postfix = "")
            {
                _joiner = joiner;
                _prefix = prefix;
                _postfix = postfix;
            }

            public string ResolveClauses(DynamicParameters p)
            {
                foreach (var item in this)
                {
                    p.AddDynamicParams(item.Parameters);
                }
                return this.Any(a => a.IsInclusive)
                    ? _prefix +
                      string.Join(_joiner,
                          this.Where(a => !a.IsInclusive)
                              .Select(c => c.Sql)
                              .Union(new[]
                              {
                                  " ( " +
                                  string.Join(" OR ", this.Where(a => a.IsInclusive).Select(c => c.Sql).ToArray()) +
                                  " ) "
                              }).ToArray()) + _postfix
                    : _prefix + string.Join(_joiner, this.Select(c => c.Sql).ToArray()) + _postfix;
            }
        }

        /// <summary>
        /// Sql Template
        /// </summary>
        public class SelectTemplate
        {
            private readonly string _sqlBody;
            private readonly string _sqlSelect;
            private readonly string _sqlOrderBy;
            private readonly SqlSelectBuilder _builder;
            private readonly object _initParams;
            private int _dataSeq = -1; // Unresolved

            /// <summary>
            /// 
            /// </summary>
            /// <param name="builder"></param>
            /// <param name="sqlSelect">eg:SELECT /**select**/</param>
            /// <param name="sqlBody">eg:FROM  Person /**leftjoin**/ /**where**/</param>
            /// <param name="sqlOrderBy">eg:/**orderby**/</param>
            /// <param name="parameters"></param>
            public SelectTemplate(SqlSelectBuilder builder, string sqlSelect, string sqlBody, string sqlOrderBy, object parameters)
            {
                _initParams = parameters;
                _sqlSelect = sqlSelect;
                _sqlBody = sqlBody;
                _sqlOrderBy = sqlOrderBy;
                _builder = builder;
            }
            /// <summary>
            /// 
            /// </summary>
            private static readonly Regex _regex = new Regex(@"\/\*\*.+?\*\*\/", RegexOptions.Compiled | RegexOptions.Multiline);
            /// <summary>
            /// 
            /// </summary>
            private void ResolveSql()
            {
                if (_dataSeq != _builder._seq)
                {
                    var p = new DynamicParameters(_initParams);

                    string rawSqlSelect = _sqlSelect;
                    foreach (var pair in _builder._selectData)
                    {
                        rawSqlSelect = rawSqlSelect.Replace("/**" + pair.Key + "**/", pair.Value.ResolveClauses(p));
                    }

                    string rawSqlBody = _sqlBody;
                    foreach (var pair in _builder._bodyData)
                    {
                        rawSqlBody = rawSqlBody.Replace("/**" + pair.Key + "**/", pair.Value.ResolveClauses(p));
                    }

                    string rawSqlOrderBy = _sqlOrderBy;
                    foreach (var pair in _builder._orderByData)
                    {
                        rawSqlOrderBy = rawSqlOrderBy.Replace("/**" + pair.Key + "**/", pair.Value.ResolveClauses(p));
                    }
                    parameters = p;

                    // replace all that is left with empty
                    rawSqlSelect = _regex.Replace(rawSqlSelect, string.Empty);
                    rawSqlBody = _regex.Replace(rawSqlBody, string.Empty);
                    rawSqlOrderBy = _regex.Replace(rawSqlOrderBy, string.Empty);
                    rawSql = GetRowSql(rawSqlSelect, rawSqlBody, rawSqlOrderBy, _builder.Offset, _builder.Rows);
                    _dataSeq = _builder._seq;
                }
            }
            /// <summary>
            /// resolveType(0:selectSql and count sql,1:selectSql,2:countSql)
            /// </summary>
            /// <param name="resolveType">0:selectSql and count sql,1:selectSql,2:countSql</param>
            /// <returns></returns>
            public Tuple<string, string> ResolveRawSql(byte resolveType)
            {
                string selectSql=string.Empty, countSql=string.Empty;
                if (_dataSeq != _builder._seq)
                {
                    var p = new DynamicParameters(_initParams);
                   
                    string rawSqlBody = _sqlBody;
                    foreach (var pair in _builder._bodyData)
                    {
                        rawSqlBody = rawSqlBody.Replace("/**" + pair.Key + "**/", pair.Value.ResolveClauses(p));
                    }
                    string rawSqlSelect = _sqlSelect;
                    string rawSqlOrderBy = _sqlOrderBy;
                    
                    if (resolveType == 0 || resolveType == 1)
                    {
                        rawSqlSelect = _sqlSelect;
                        foreach (var pair in _builder._selectData)
                        {
                            rawSqlSelect = rawSqlSelect.Replace("/**" + pair.Key + "**/", pair.Value.ResolveClauses(p));
                        }
                        foreach (var pair in _builder._orderByData)
                        {
                            rawSqlOrderBy = rawSqlOrderBy.Replace("/**" + pair.Key + "**/", pair.Value.ResolveClauses(p));
                        }
                    }
                    parameters = p;
                    // replace all that is left with empty
                    rawSqlSelect = _regex.Replace(rawSqlSelect, string.Empty);
                    rawSqlBody = _regex.Replace(rawSqlBody, string.Empty);
                    rawSqlOrderBy = _regex.Replace(rawSqlOrderBy, string.Empty);
                    if (resolveType == 0 || resolveType == 1)
                    {
                        rawSql = GetRowSql(rawSqlSelect, rawSqlBody, rawSqlOrderBy, _builder.Offset, _builder.Rows);
                    }
                    selectSql = rawSql;
                    countSql = "SELECT COUNT(0)  FROM ("+rawSqlSelect+rawSqlBody+") AS tmpCountTable" ;
                    _dataSeq = _builder._seq;
                }
                return Tuple.Create(selectSql, countSql);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="selectExpression"></param>
            /// <param name="bodyExpression"></param>
            /// <param name="orderByExpression"></param>
            /// <param name="offset"></param>
            /// <param name="rows"></param>
            /// <returns></returns>
            public virtual string GetRowSql(string selectExpression, string bodyExpression, string orderByExpression = null, int? offset = null, int? rows = null)
            {
                var sb = StringBuilderCache.Allocate();
                sb.Append(selectExpression);
                sb.Append(bodyExpression);
                if (!string.IsNullOrEmpty(orderByExpression))
                {
                    sb.Append(orderByExpression);
                }
                if (offset != null || rows != null)
                {
                    if (offset != null && string.IsNullOrEmpty(orderByExpression))
                    {
                        sb.Append(" ORDER BY 1");
                    }
                    sb.Append(SqlLimit(offset, rows));
                }

                return StringBuilderCache.ReturnAndFree(sb);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="offset"></param>
            /// <param name="rows"></param>
            /// <returns></returns>
            public virtual string SqlLimit(int? offset = null, int? rows = null)
            {
                return rows == null && offset == null ? "" : offset == null
                    ? "LIMIT " + rows
                    : "LIMIT " + rows.GetValueOrDefault(int.MaxValue) + " OFFSET " + offset;
            }

            private string rawSql;
            private object parameters;

            public string RawSql
            {
                get { ResolveSql(); return rawSql; }
            }

            public object Parameters
            {
                get { ResolveSql(); return parameters; }
            }
        }

        /// <summary>
        /// Add Sql Template
        /// </summary>
        /// <param name="sqlSelect">eg:SELECT /**select**/</param>
        /// <param name="sqlBody">eg:FROM  Person /**leftjoin**/ /**where**/</param>
        /// <param name="sqlOrderBy">eg:/**orderby**/</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SelectTemplate AddTemplate(string sqlSelect, string sqlBody, string sqlOrderBy, dynamic parameters = null)
        {
            return new SelectTemplate(this, sqlSelect, sqlBody, sqlOrderBy, parameters);
        }
        /// <summary>
        /// Add Sql Template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public SelectTemplate AddTemplate(SelectTemplate template)
        {
            return template;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="joiner"></param>
        /// <param name="prefix"></param>
        /// <param name="postfix"></param>
        /// <param name="isInclusive"></param>
        /// <returns></returns>
        protected SqlSelectBuilder AddBodyClause(string name, string sql, object parameters, string joiner, string prefix = "", string postfix = "", bool isInclusive = false)
        {
            Clauses clauses;
            if (!_bodyData.TryGetValue(name, out clauses))
            {
                clauses = new Clauses(joiner, prefix, postfix);
                _bodyData[name] = clauses;
            }
            clauses.Add(new Clause { Sql = sql, Parameters = parameters, IsInclusive = isInclusive });
            _seq++;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="joiner"></param>
        /// <param name="prefix"></param>
        /// <param name="postfix"></param>
        /// <param name="isInclusive"></param>
        /// <returns></returns>
        protected SqlSelectBuilder AddSelectClause(string name, string sql, object parameters, string joiner, string prefix = "", string postfix = "", bool isInclusive = false)
        {
            Clauses clauses;
            if (!_selectData.TryGetValue(name, out clauses))
            {
                clauses = new Clauses(joiner, prefix, postfix);
                _selectData[name] = clauses;
            }
            clauses.Add(new Clause { Sql = sql, Parameters = parameters, IsInclusive = isInclusive });
            _seq++;
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="joiner"></param>
        /// <param name="prefix"></param>
        /// <param name="postfix"></param>
        /// <param name="isInclusive"></param>
        /// <returns></returns>
        protected SqlSelectBuilder AddOrderByClause(string name, string sql, object parameters, string joiner, string prefix = "", string postfix = "", bool isInclusive = false)
        {
            Clauses clauses;
            if (!_orderByData.TryGetValue(name, out clauses))
            {
                clauses = new Clauses(joiner, prefix, postfix);
                _orderByData[name] = clauses;
            }
            clauses.Add(new Clause { Sql = sql, Parameters = parameters, IsInclusive = isInclusive });
            _seq++;
            return this;
        }
        public SqlSelectBuilder Intersect(string sql, dynamic parameters = null)
        {
            AddBodyClause("intersect", sql, parameters, "\nINTERSECT\n ", "\n ", "\n", false);
            return this;
        }

        public SqlSelectBuilder InnerJoin(string sql, dynamic parameters = null)
        {
            AddBodyClause("innerjoin", sql, parameters, "\nINNER JOIN ", "\nINNER JOIN ", "\n", false);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlSelectBuilder LeftJoin(string sql, dynamic parameters = null)
        {
            AddBodyClause("leftjoin", sql, parameters, "\nLEFT JOIN ", "\nLEFT JOIN ", "\n", false);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlSelectBuilder RightJoin(string sql, dynamic parameters = null)
        {
            AddBodyClause("rightjoin", sql, parameters, "\nRIGHT JOIN ", "\nRIGHT JOIN ", "\n", false);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlSelectBuilder Where(string sql, dynamic parameters = null)
        {
            AddBodyClause("where", sql, parameters, " AND ", "WHERE ", "\n", false);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlSelectBuilder OrWhere(string sql, dynamic parameters = null)
        {
            AddBodyClause("where", sql, parameters, " OR ", "WHERE ", "\n", true);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlSelectBuilder OrderBy(string sql, dynamic parameters = null)
        {
            AddOrderByClause("orderby", sql, parameters, " , ", "ORDER BY ", "\n", false);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlSelectBuilder Select(string sql, dynamic parameters = null)
        {
            AddSelectClause("select", sql, parameters, " , ", "", "\n", false);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlSelectBuilder AddParameters(dynamic parameters)
        {
            AddBodyClause("--parameters", "", parameters, "", "", "", false);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlSelectBuilder Join(string sql, dynamic parameters = null)
        {
            AddBodyClause("join", sql, parameters, "\nJOIN ", "\nJOIN ", "\n", false);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlSelectBuilder GroupBy(string sql, dynamic parameters = null)
        {
            AddBodyClause("groupby", sql, parameters, " , ", "\nGROUP BY ", "\n", false);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlSelectBuilder Having(string sql, dynamic parameters = null)
        {
            AddBodyClause("having", sql, parameters, "\nAND ", "HAVING ", "\n", false);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public int? Rows { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? Offset { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public virtual SqlSelectBuilder Limit(int? offset = null, int? rows = null)
        {
            Offset = offset;
            Rows = rows;
            return this;
        }
    }

}
