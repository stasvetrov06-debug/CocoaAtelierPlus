using CocoaAtelierPlus.Models;
using CocoaAtelierPlus.Services;
using Microsoft.AspNetCore.Mvc;

namespace CocoaAtelierPlus.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Login")))
            {
                return RedirectToAction("Index", "Material");
            }

            return View(new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                SetMessage("error", "Ошибка", "Заполните все поля формы входа.");
                return View(model);
            }

            var user = UserService.Authenticate(model.Login, model.Password);
            if (user == null)
            {
                SetMessage("error", "Ошибка входа",
                    "Логин или пароль указаны неверно. Проверьте раскладку клавиатуры и повторите попытку.");
                return View(model);
            }

            if (!user.IsActive)
            {
                SetMessage("warning", "Предупреждение",
                    "Учетная запись заблокирована. Обратитесь к администратору системы.");
                return View(model);
            }

            HttpContext.Session.SetString("Login", user.Login);
            HttpContext.Session.SetString("FullName", user.FullName);
            HttpContext.Session.SetString("Role", user.RoleName);

            SetMessage("info", "Информация", $"Вход выполнен. Добро пожаловать, {user.FullName}.");
            return RedirectToAction("Index", "Material");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            SetMessage("info", "Информация", "Выход из системы выполнен.");
            return RedirectToAction("Login");
        }

        private void SetMessage(string type, string title, string text)
        {
            TempData["MessageType"] = type;
            TempData["MessageTitle"] = title;
            TempData["MessageText"] = text;
        }
    }
}
