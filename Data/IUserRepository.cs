using SkillConnect.Models;

namespace SkillConnect.Data
{
    public interface IUserRepository
    {
        void AddUser(User user);
        User GetUser(string email, string password);
        bool EmailExists(string email);
        void SaveQuiz(QuizResult result);
    }
}