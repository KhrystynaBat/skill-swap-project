using System;
using System.Collections.Generic;
using System.Text;

namespace SkillSwap.Application.DTOs
{
    public class AddUserSkillDto
    {
        public int SkillId { get; set; }
        public int Level { get; set; } // 1–5
    }
}
