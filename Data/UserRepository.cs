using SkillConnect.Models;
using System.Linq;
using System;

namespace SkillConnect.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public void AddUser(User user)
        {
            // Glitch Fix: Clean data before saving to DB
            user.Email = user.Email.Trim().ToLower();
            user.Password = user.Password.Trim();
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public User GetUser(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) return null;

            // Glitch Fix: Convert search to lowercase and trim to ensure match works
            string cleanEmail = email.Trim().ToLower();
            string cleanPass = password.Trim();

            return _context.Users.FirstOrDefault(u =>
                u.Email.ToLower() == cleanEmail &&
                u.Password == cleanPass);
        }

        public bool EmailExists(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            string cleanEmail = email.Trim().ToLower();
            return _context.Users.Any(u => u.Email.ToLower() == cleanEmail);
        }

        public void SaveQuiz(QuizResult result)
        {
            _context.QuizResults.Add(result);
            _context.SaveChanges();
        }
    }
}