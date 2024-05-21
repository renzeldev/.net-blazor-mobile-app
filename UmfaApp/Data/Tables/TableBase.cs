using SQLite;

namespace UmfaApp.Data.Tables
{
    public class TableBase
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
