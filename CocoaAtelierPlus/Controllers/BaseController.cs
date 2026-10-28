using CocoaAtelierPlus.Services;
using Microsoft.AspNetCore.Mvc;

namespace CocoaAtelierPlus.Controllers
{
    public class BaseController : Controller
    {
        protected string? CurrentRole => HttpContext.Session.GetString("Role");
        protected string? CurrentLogin => HttpContext.Session.GetString("Login");

        protected bool IsAuthenticated => !string.IsNullOrEmpty(CurrentLogin);

        protected IActionResult RequireLogin()
        {
            if (!IsAuthenticated)
            {
                TempData["MessageType"] = "error";
                TempData["MessageTitle"] = "Ошибка";
                TempData["MessageText"] = "Сессия не найдена. Войдите в систему, указав логин и пароль.";
                return RedirectToAction("Login", "Account");
            }

            return Ok();
        }

        protected IActionResult RequireRole(params string[] roles)
        {
            var loginCheck = RequireLogin();
            if (!(loginCheck is OkResult))
            {
                return loginCheck;
            }

            if (CurrentRole == null || !roles.Contains(CurrentRole))
            {
                TempData["MessageType"] = "warning";
                TempData["MessageTitle"] = "Предупреждение";
                TempData["MessageText"] = "Недостаточно прав для выполнения действия. Обратитесь к администратору системы.";
                return RedirectToAction("Index", "Material");
            }

            return Ok();
        }

        protected void SetMessage(string type, string title, string text)
        {
            TempData["MessageType"] = type;
            TempData["MessageTitle"] = title;
            TempData["MessageText"] = text;
        }
    }
}
