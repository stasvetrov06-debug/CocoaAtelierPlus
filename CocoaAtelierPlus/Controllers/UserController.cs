using CocoaAtelierPlus.Models;
using CocoaAtelierPlus.Services;
using Microsoft.AspNetCore.Mvc;

namespace CocoaAtelierPlus.Controllers
{
    public class UserController : BaseController
    {
        [HttpGet]
        public IActionResult Index()
        {
            var access = RequireRole("Администратор");
            if (!(access is OkResult))
            {
                return access;
            }

            return View(UserService.GetAll());
        }

        [HttpGet]
        public IActionResult Create()
        {
            var access = RequireRole("Администратор");
            if (!(access is OkResult))
            {
                return access;
            }

            var form = new UserForm
            {
                Roles = UserService.GetRoles()
            };
            return View("Edit", form);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var access = RequireRole("Администратор");
            if (!(access is OkResult))
            {
                return access;
            }

            var user = UserService.GetById(id);
            if (user == null)
            {
                SetMessage("error", "Ошибка",
                    "Учетная запись не найдена. Обновите список и выберите запись заново.");
                return RedirectToAction("Index");
            }

            var form = new UserForm
            {
                UserId = user.UserId,
                Login = user.Login,
                FullName = user.FullName,
                Email = user.Email,
                Password = user.Password,
                RoleId = user.RoleId,
                IsActive = user.IsActive,
                Roles = UserService.GetRoles()
            };
            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UserForm model)
        {
            var access = RequireRole("Администратор");
            if (!(access is OkResult))
            {
                return access;
            }

            if (!ModelState.IsValid)
            {
                model.Roles = UserService.GetRoles();
                SetMessage("error", "Ошибка",
                    "Форма содержит ошибки. Исправьте отмеченные поля и сохраните данные повторно.");
                return View(model);
            }

            if (UserService.LoginExists(model.Login, model.UserId))
            {
                ModelState.AddModelError(nameof(model.Login),
                    "Пользователь с таким логином уже существует. Укажите другой логин.");
                model.Roles = UserService.GetRoles();
                SetMessage("error", "Ошибка",
                    "Не удалось сохранить учетную запись: логин должен быть уникальным.");
                return View(model);
            }

            if (UserService.EmailExists(model.Email, model.UserId))
            {
                ModelState.AddModelError(nameof(model.Email),
                    "Пользователь с таким email уже существует. Укажите другой адрес.");
                model.Roles = UserService.GetRoles();
                SetMessage("error", "Ошибка",
                    "Не удалось сохранить учетную запись: email должен быть уникальным.");
                return View(model);
            }

            try
            {
                if (model.UserId.HasValue)
                {
                    UserService.Update(model);
                    SetMessage("info", "Информация", "Учетная запись обновлена.");
                }
                else
                {
                    UserService.Add(model);
                    SetMessage("info", "Информация", "Учетная запись создана.");
                }
            }
            catch (Exception)
            {
                SetMessage("error", "Ошибка базы данных",
                    "Не удалось сохранить учетную запись. Повторите попытку позже.");
                model.Roles = UserService.GetRoles();
                return View(model);
            }

            return RedirectToAction("Index");
        }
    }
}
