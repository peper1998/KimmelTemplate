using EnsureThat;
using KimmelTemplate.Common.CQRS.Queries;
using KimmelTemplate.Infrastructure.Factories;
using System.Text;

namespace KimmelTemplate.Infrastructure.QueryBuilder
{
    public class SqlQueryBuilder
    {
        private readonly Func<DbConnectionFactory> _dbConnectionFactory;

        private List<string> _columnsToSelect;
        private string _dataSource;
        private List<string> _whereConditions;
        private SqlQueryParameters _parameters;
        private SortColumn _sortingBy;
        private bool _sum;
        private int? _topCount;
        private bool _isRandom;
        private bool _isDistinct;

        public SqlQueryBuilder(Func<DbConnectionFactory> dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;

            Reset();
        }

        public SqlQueryBuilder Select(params string[] columns)
        {
            EnsureArg.IsNotNull(columns, nameof(columns));

            var splitColumns = columns.Select(c => c.Trim());
            _columnsToSelect.AddRange(splitColumns);

            return this;
        }

        public SqlQueryBuilder SelectDistinct(params string[] columns)
        {
            _isDistinct = true;

            return Select(columns);
        }

        public SqlQueryBuilder SelectAllProperties<T>(params string[] excludedColumns) where T : class
        {
            var properties = typeof(T).GetProperties();
            var columns = properties.Select(x => x.Name.Trim())
                .Except(excludedColumns);

            _columnsToSelect.AddRange(columns);

            return this;
        }

        public SqlQueryBuilder From(string dataSource)
        {
            Ensure.String.IsNotNullOrWhiteSpace(dataSource, nameof(dataSource));

            _dataSource = dataSource;

            return this;
        }

        public SqlQueryBuilder Where<T>(string column, T value) where T : struct
        {
            AddFilter(column, " = ", value);

            return this;
        }

        public SqlQueryBuilder Where<T>(string column, T? value) where T : struct
        {
            if (value == null)
            {
                return this;
            }

            AddFilter(column, " = ", value.Value);

            return this;
        }

        public SqlQueryBuilder Where(string column, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return this;
            }

            AddFilter(column, " = ", value);

            return this;
        }
        public SqlQueryBuilder Where(string column, object valueToFilter, SqlComparisonOperator comparisonOperator = SqlComparisonOperator.Equals)
        {
            Ensure.String.IsNotNullOrWhiteSpace(column, nameof(column));
            EnsureArg.IsNotNull(valueToFilter, nameof(valueToFilter));

            var paramName = _parameters.GetNextParameterName();

            switch (comparisonOperator)
            {
                case SqlComparisonOperator.Equals:
                    _whereConditions.Add(string.Concat(column, " = ", paramName));
                    break;
                case SqlComparisonOperator.Differs:
                    _whereConditions.Add(string.Concat("( ", column, " <> ", paramName, " OR ", column, " IS NULL) "));
                    break;
                case SqlComparisonOperator.Like:
                    _whereConditions.Add(string.Concat(column, " LIKE '%' + ", paramName, " + '%'"));
                    break;
                case SqlComparisonOperator.NotLike:
                    _whereConditions.Add(string.Concat(column, " NOT LIKE '%' + ", paramName, " + '%'"));
                    break;
                case SqlComparisonOperator.LessOrEqual:
                    _whereConditions.Add(string.Concat(column, " <= ", paramName));
                    break;
                case SqlComparisonOperator.GreaterOrEqual:
                    _whereConditions.Add(string.Concat(column, " >= ", paramName));
                    break;
                case SqlComparisonOperator.Less:
                    _whereConditions.Add(string.Concat(column, " < ", paramName));
                    break;
                case SqlComparisonOperator.Greater:
                    _whereConditions.Add(string.Concat(column, " > ", paramName));
                    break;
                default:
                    throw new ArgumentException("Implement logic for new operator.");
            }

            _parameters.Add(paramName, valueToFilter);
            return this;
        }

        public SqlQueryBuilder When(bool applyFirstMethod, Action<SqlQueryBuilder> actionToInvokeWhenTrue, Action<SqlQueryBuilder> actionToInvokeWhenFalse = null)
        {
            {
                if (applyFirstMethod)
                {
                    actionToInvokeWhenTrue.Invoke(this);
                }
                else
                {
                    actionToInvokeWhenFalse?.Invoke(this);
                }
                return this;
            }
        }

        public SqlQueryBuilder WhereIsNotNull(string column)
        {
            Ensure.String.IsNotNullOrWhiteSpace(column, nameof(column));

            _whereConditions.Add(string.Concat("( ", column, " IS NOT NULL) "));

            return this;
        }

        public SqlQueryBuilder OrderBy(string sortCriteria)
        {
            if (!string.IsNullOrWhiteSpace(sortCriteria))
            {
                _sortingBy = SortCriteria.Parse(sortCriteria);
            }

            return this;
        }

        public SqlQueryBuilder OrderBy(string columnName, string direction)
        {
            _sortingBy = SortCriteria.Parse(columnName, direction);

            return this;
        }

        public DataQuery<T> BuildQuery<T>()
        {
            var selectQuery = BuildSelectQuery();
            var query = new DataQuery<T>(_dbConnectionFactory().Connection, selectQuery, _parameters);

            Reset();
            return query;
        }
        public PagedDataQuery<T> BuildPagedQuery<T>(SearchCriteria searchCriteria)
        {
            Ensure.That(searchCriteria, nameof(searchCriteria)).IsNotNull();

            if (_sortingBy == null)
            {
                SetSortingBy();
            }

            var selectQuery = BuildSelectQuery(searchCriteria.PageNumber, searchCriteria.PageSize);
            var countQuery = BuildCountQuery();
            var query = new PagedDataQuery<T>(_dbConnectionFactory().Connection, selectQuery, countQuery, _parameters, searchCriteria);

            Reset();
            return query;
        }

        private string BuildSelectQuery()
        {
            var builder = new StringBuilder();
            builder.Append("SELECT ");

            if (_isDistinct)
            {
                builder.Append("DISTINCT ");
            }

            builder.Append(string.Join(", ", _columnsToSelect.Where(c => !string.IsNullOrEmpty(c))));

            builder.Append($" FROM {_dataSource} ");

            if (_whereConditions.Any())
            {
                builder.Append(" WHERE " + string.Join(" AND ", _whereConditions.Where(c => !string.IsNullOrEmpty(c))));
            }

            return builder.ToString();
        }

        private string BuildSelectQuery(int? pageNumber = null, int? pageSize = null)
        {
            var builder = new StringBuilder();
            builder.Append("SELECT ");

            if (_isDistinct)
            {
                builder.Append("DISTINCT ");
            }

            if (_topCount.HasValue)
            {
                builder.Append($"TOP {_topCount} ");
            }

            if (_sum)
            {
                builder.Append(string.Join(", ", _columnsToSelect.Where(c => !string.IsNullOrEmpty(c)).Select(x => "ISNULL(SUM(" + x + "), 0) AS " + x)));
            }
            else
            {
                builder.Append(string.Join(", ", _columnsToSelect.Where(c => !string.IsNullOrEmpty(c))));
            }

            builder.Append($" FROM {_dataSource} ");

            if (_whereConditions.Any())
            {
                builder.Append(" WHERE " + string.Join(" AND ", _whereConditions.Where(c => !string.IsNullOrEmpty(c))));
            }

            if (_sortingBy != null)
            {
                builder.Append($" ORDER BY {_sortingBy.Column} {_sortingBy.Direction}");
            }
            else if (_isRandom)
            {
                builder.Append(" ORDER BY NEWID()");
            }

            if (pageNumber != null && pageSize != null)
            {
                Ensure.That(pageSize.Value, nameof(pageSize)).IsGt(0);
                Ensure.That(pageNumber.Value, nameof(pageNumber)).IsGt(0);
                builder.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", (pageNumber - 1) * pageSize, pageSize);
            }

            return builder.ToString();
        }


        private void AddFilter(string column, string filterOperator, object valueToFilter)
        {
            Ensure.String.IsNotNullOrWhiteSpace(column, nameof(column));
            Ensure.String.IsNotNullOrWhiteSpace(filterOperator, nameof(filterOperator));
            EnsureArg.IsNotNull(valueToFilter, nameof(valueToFilter));

            var paramName = _parameters.GetNextParameterName();
            _parameters.Add(paramName, valueToFilter);

            _whereConditions.Add(string.Concat(column, filterOperator, paramName));
        }

        private string BuildCountQuery()
        {
            var builder = new StringBuilder();

            builder.Append("SELECT COUNT(*) FROM ");
            builder.Append(_dataSource);

            if (_whereConditions.Any())
            {
                builder.Append($" WHERE {string.Join(" AND ", _whereConditions)}");
            }

            return builder.ToString();
        }

        private void SetSortingBy()
        {
            _sortingBy = new SortColumn(_columnsToSelect.First(x => !x.Contains(" AS ")), true);
        }

        private void Reset()
        {
            _columnsToSelect = new List<string>();
            _dataSource = null;
            _whereConditions = new List<string>();

            _parameters = new SqlQueryParameters();
            _isDistinct = false;
        }
    }
}
