using System;
using System.ComponentModel.DataAnnotations; // 1. Necessary for the [Key] attribute

namespace SkillConnect.Models
{
    public class QuizResult
    {
        [Key] // 2. This explicitly tells EF this is your Primary Key
        public int ResultId { get; set; }

        public int UserId { get; set; }
        public int AnalyticalScore { get; set; }
        public int CreativeScore { get; set; }
        public int DataScore { get; set; }
        public int ProgrammingScore { get; set; }
        public int CommunicationScore { get; set; }
        public int PeopleScore { get; set; }

        public string RecommendedCareer { get; set; } = string.Empty;
        public DateTime TestDate { get; set; } = DateTime.Now;
    }
}