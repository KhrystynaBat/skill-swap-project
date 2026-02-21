using System;
using System.Collections.Generic;
using System.Text;

namespace SkillSwap.Application.DTOs
{
    public class UpdateProfileDto
    {
        public string? Name { get; set; }
        public string? Bio { get; set; }
        public string? City { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
