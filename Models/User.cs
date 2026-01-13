// Models/User.cs
using System.ComponentModel.DataAnnotations;

namespace SkillConnect.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        public string? EducationLevel { get; set; }
        public ICollection<QuizResult>? QuizResults { get; set; }
    }
}