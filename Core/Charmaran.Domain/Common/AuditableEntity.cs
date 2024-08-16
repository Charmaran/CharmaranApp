namespace Charmaran.Domain.Common
{
    public abstract class AuditableEntity
    {
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; } = null!;
        public DateTime? LastModifiedDate { get; set; }
    }
}