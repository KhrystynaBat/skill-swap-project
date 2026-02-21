using System;
using System.Collections.Generic;
using System.Text;

namespace SkillSwap.Application.DTOs
{
    public class AddUserInterestDto
    {
        public int SkillId { get; set; }
        public int Priority { get; set; } // 1–3
    }
}
