using System.ComponentModel.DataAnnotations;

namespace SkillSwap.Domain.Entities
{
    public class Exchange
    {
        public int Id { get; set; }

        public int UserAId { get; set; }
        public int UserBId { get; set; }

        public DateTime? ScheduledTime { get; set; }

        public string Status { get; set; } = "requested";

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
    }

}
