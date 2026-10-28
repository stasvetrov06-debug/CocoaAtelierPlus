using CocoaAtelierPlus.Models;
using CocoaAtelierPlus.Services;
using Microsoft.AspNetCore.Mvc;

namespace CocoaAtelierPlus.Controllers
{
    public class SupplierController : BaseController
    {
        [HttpGet]
        public IActionResult Index()
        {
            var access = RequireLogin();
            if (!(access is OkResult))
            {
                return access;
            }

            ViewBag.CanManage = CurrentRole == "Администратор";
            return View(SupplierService.GetAll());
        }

        [HttpGet]
        public IActionResult Create()
        {
            var access = RequireRole("Администратор");
            if (!(access is OkResult))
            {
                return access;
            }

            return View("Edit", new SupplierForm());
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var access = RequireRole("Администратор");
            if (!(access is OkResult))
            {
                return access;
            }

            var supplier = SupplierService.GetById(id);
            if (supplier == null)
            {
                SetMessage("error", "Ошибка",
                    "Поставщик не найден. Обновите список и выберите запись заново.");
                return RedirectToAction("Index");
            }

            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SupplierForm model)
        {
            var access = RequireRole("Администратор");
            if (!(access is OkResult))
            {
                return access;
            }

            if (!ModelState.IsValid)
            {
                SetMessage("error", "Ошибка",
                    "Форма содержит ошибки. Исправьте отмеченные поля и сохраните данные повторно.");
                return View(model);
            }

            if (SupplierService.NameExists(model.Name, model.SupplierId))
            {
                ModelState.AddModelError(nameof(model.Name),
                    "Поставщик с таким наименованием уже существует. Укажите другое наименование.");
                SetMessage("error", "Ошибка",
                    "Не удалось сохранить поставщика: наименование должно быть уникальным.");
                return View(model);
            }

            if (SupplierService.InnExists(model.Inn, model.SupplierId))
            {
                ModelState.AddModelError(nameof(model.Inn),
                    "Поставщик с таким ИНН уже зарегистрирован. Укажите другой ИНН.");
                SetMessage("error", "Ошибка",
                    "Не удалось сохранить поставщика: ИНН должен быть уникальным.");
                return View(model);
            }

            try
            {
                if (model.SupplierId.HasValue)
                {
                    SupplierService.Update(model);
                    SetMessage("info", "Информация", "Данные поставщика обновлены.");
                }
                else
                {
                    SupplierService.Add(model);
                    SetMessage("info", "Информация", "Поставщик добавлен в список.");
                }
            }
            catch (Exception)
            {
                SetMessage("error", "Ошибка базы данных",
                    "Не удалось сохранить данные поставщика. Повторите попытку позже.");
                return View(model);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var access = RequireRole("Администратор");
            if (!(access is OkResult))
            {
                return access;
            }

            try
            {
                SupplierService.Delete(id);
                SetMessage("warning", "Предупреждение",
                    "Данные поставщика удалены. Действие нельзя отменить.");
            }
            catch (Exception)
            {
                SetMessage("error", "Ошибка",
                    "Не удалось удалить поставщика: за ним закреплены материалы. " +
                    "Удаление запрещено, чтобы не нарушить целостность данных о поставках.");
            }

            return RedirectToAction("Index");
        }
    }
}
