using System;
using System.Collections.Generic;
using System.Text;

namespace SkillSwap.Application.DTOs
{
    public class CreateReviewDto
    {
        public int ExchangeId { get; set; }
        public int Rating { get; set; } // 1–5
        public string Comment { get; set; } = null!;
    }
}
