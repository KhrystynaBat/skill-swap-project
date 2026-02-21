using System;
using System.Collections.Generic;
using System.Text;

namespace SkillSwap.Application.DTOs
{
    public class CreateExchangeDto
    {
        public int MatchId { get; set; }
        public DateTime ScheduledTime { get; set; }
    }
}
