namespace Concordia.Framework
{
    public enum ColumnType
    {
        /// <summary>
        /// 32Bit Integer.
        /// </summary>
        Integer,

        /// <summary>
        /// String.
        /// </summary>
        String,

        /// <summary>
        /// Decimal.
        /// </summary>
        Decimal,

        /// <summary>
        /// DateTime.
        /// </summary>
        DateTime,

        /// <summary>
        /// Boolean
        /// </summary>
        Boolean,

        /// <summary>
        /// ForeignKey alias Integer.
        /// </summary>
        ForeignKey = Integer,
    }
}
