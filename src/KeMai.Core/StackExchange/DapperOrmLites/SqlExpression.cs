using System;

namespace KeMai.StackExchange.DapperOrmLites
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SelectItem
    {
        protected SelectItem(IOrmLiteDialectProvider dialectProvider, string alias)
        {
            if (dialectProvider == null)
            {
                throw new ArgumentNullException("dialectProvider");
            }
            DialectProvider = dialectProvider;
            Alias = alias;
        }

        /// <summary>
        /// Unquoted alias for the column or expression being selected.
        /// </summary>
        public string Alias { get; set; }

        protected IOrmLiteDialectProvider DialectProvider { get; set; }

        public abstract override string ToString();
    }

    public class SelectItemExpression : SelectItem
    {
        public SelectItemExpression(IOrmLiteDialectProvider dialectProvider, string selectExpression, string alias)
            : base(dialectProvider, alias)
        {
            if (string.IsNullOrEmpty(selectExpression))
                throw new ArgumentNullException("selectExpression");
            if (string.IsNullOrEmpty(alias))
                throw new ArgumentNullException("alias");

            SelectExpression = selectExpression;
            Alias = alias;
        }

        /// <summary>
        /// The SQL expression being selected, including any necessary quoting.
        /// </summary>
        public string SelectExpression { get; set; }

        public override string ToString()
        {
            var text = SelectExpression;
            if (!string.IsNullOrEmpty(Alias)) // Note that even though Alias must be non-empty in the constructor it may be set to null/empty later
                text += " AS " + DialectProvider.GetQuotedName(Alias);
            return text;
        }
    }

    public class SelectItemColumn : SelectItem
    {
        public SelectItemColumn(IOrmLiteDialectProvider dialectProvider, string columnName, string columnAlias = null, string quotedTableAlias = null)
            : base(dialectProvider, columnAlias)
        {
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentNullException("columnName");

            ColumnName = columnName;
            QuotedTableAlias = quotedTableAlias;
        }

        /// <summary>
        /// Unquoted column name being selected.
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// Table name or alias used to prefix the column name, if any. Already quoted.
        /// </summary>
        public string QuotedTableAlias { get; set; }

        public override string ToString()
        {
            var text = DialectProvider.GetQuotedColumnName(ColumnName);

            if (!string.IsNullOrEmpty(QuotedTableAlias))
                text = QuotedTableAlias + "." + text;
            if (!string.IsNullOrEmpty(Alias))
                text += " AS " + DialectProvider.GetQuotedName(Alias);

            return text;
        }
    }
}
