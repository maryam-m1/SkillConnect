using Microsoft.AspNetCore.Mvc;
using SkillConnect.Data;
using SkillConnect.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks; // <--  namespace for async 

namespace SkillConnect.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository _repo;

        public HomeController(IUserRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Register() => View();

        [HttpGet]
        public IActionResult Login() => View();

        // Modified: Analysis --> async banaya
        [HttpGet]
        public async Task<IActionResult> Analysis()
        {
            await Task.Yield(); // Background process ki efficiency ke liye
            return View();
        }

        // Modified: Matching ko async banaya (SignalR ke liye zaroori hai)
        [HttpGet]
        public async Task<IActionResult> Matching()
        {
            await Task.Yield();
            return View();
        }

        [HttpGet]
        public IActionResult Quiz()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (_repo.EmailExists(user.Email))
            {
                ViewBag.Error = "User already exists!";
                return View(user);
            }
            _repo.AddUser(user);

            var newUser = _repo.GetUser(user.Email, user.Password);
            if (newUser != null)
            {
                HttpContext.Session.SetInt32("UserId", newUser.UserId);
                HttpContext.Session.SetString("FullName", newUser.FullName);
                return RedirectToAction("Quiz");
            }
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _repo.GetUser(email?.Trim(), password?.Trim());

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("FullName", user.FullName);
                return RedirectToAction("Quiz");
            }
            ViewBag.Error = "Invalid credentials. Please try again.";
            return View();
        }

        // Modified: SubmitQuiz ko async banaya taake DB operations smooth hon
        [HttpPost]
        public async Task<IActionResult> SubmitQuiz(IFormCollection form)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            // Scores calculation (Same as before)
            var scores = new Dictionary<string, int>
            {
                { "Analytical", (int)new[] { 1, 2, 3 }.Average(id => int.Parse(form[$"q{id}"].ToString() ?? "3")) },
                { "Creative", (int)new[] { 4, 5, 6 }.Average(id => int.Parse(form[$"q{id}"].ToString() ?? "3")) },
                { "Data", (int)new[] { 7, 8, 9 }.Average(id => int.Parse(form[$"q{id}"].ToString() ?? "3")) },
                { "Programming", (int)new[] { 10, 11, 12 }.Average(id => int.Parse(form[$"q{id}"].ToString() ?? "3")) },
                { "Communication", (int)new[] { 13, 14, 15 }.Average(id => int.Parse(form[$"q{id}"].ToString() ?? "3")) },
                { "People", (int)new[] { 16, 17, 18 }.Average(id => int.Parse(form[$"q{id}"].ToString() ?? "3")) }
            };

            var highestScore = scores.OrderByDescending(x => x.Value).First();
            var result = new QuizResult
            {
                UserId = userId.Value,
                AnalyticalScore = scores["Analytical"],
                CreativeScore = scores["Creative"],
                DataScore = scores["Data"],
                ProgrammingScore = scores["Programming"],
                CommunicationScore = scores["Communication"],
                PeopleScore = scores["People"],
                RecommendedCareer = highestScore.Key,
                TestDate = DateTime.Now
            };

            // Database saving logic
            await Task.Run(() => _repo.SaveQuiz(result));

            ViewBag.Career = result.RecommendedCareer;
            ViewBag.Analytical = result.AnalyticalScore;
            ViewBag.Creative = result.CreativeScore;
            ViewBag.Data = result.DataScore;
            ViewBag.Programming = result.ProgrammingScore;
            ViewBag.Communication = result.CommunicationScore;
            ViewBag.People = result.PeopleScore;

            return View("Results", result);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}