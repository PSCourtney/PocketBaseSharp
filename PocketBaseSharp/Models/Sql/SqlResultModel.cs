namespace PocketBaseSharp.Models.Sql
{
    /// <summary>
    /// Result of a raw SQL query executed via <c>POST /api/sql</c>.
    /// </summary>
    public class SqlResultModel
    {
        /// <summary>
        /// The query execution time.
        /// </summary>
        public double ExecTime { get; set; }

        /// <summary>
        /// The number of affected rows (for drivers/statements that support it).
        /// </summary>
        public long AffectedRows { get; set; }

        /// <summary>
        /// The resulting columns metadata.
        /// </summary>
        public List<SqlColumnModel> Columns { get; set; } = new();

        /// <summary>
        /// The resulting rows, where each row value is the stringified column value (or null).
        /// </summary>
        public List<List<string?>> Rows { get; set; } = new();
    }

    /// <summary>
    /// Column metadata of a raw SQL query result.
    /// </summary>
    public class SqlColumnModel
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public bool Nullable { get; set; }
    }
}
