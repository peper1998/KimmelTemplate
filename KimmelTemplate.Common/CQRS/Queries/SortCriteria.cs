namespace KimmelTemplate.Common.CQRS.Queries
{
    public static class SortCriteria
    {
        public static string Ascending => "ASC";
        public static string Descending => "DESC";

        public static SortColumn Parse(string sortCriteria)
        {
            var sortingData = sortCriteria.Split(",");

            if (sortingData.Length != 2)
            {
                throw new ArgumentException("Invalid sorting format.");
            }

            var columnName = sortingData[0].Trim();
            var isAscending = sortingData[1].Trim().ToUpperInvariant() == Ascending;

            return new SortColumn(columnName, isAscending);
        }

        public static SortColumn Parse(string columnName, string direction)
        {
            if (direction.Trim().ToUpperInvariant() != Ascending && direction.Trim().ToUpperInvariant() != Descending)
            {
                throw new ArgumentException("Invalid direction.");
            }

            var isAscending = direction.Trim().ToUpperInvariant() == Ascending;

            return new SortColumn(columnName, isAscending);
        }
    }
}
