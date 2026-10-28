using CocoaAtelierPlus.Models;
using CocoaAtelierPlus.Services;
using Microsoft.AspNetCore.Mvc;

namespace CocoaAtelierPlus.Controllers
{
    public class MaterialController : BaseController
    {
        [HttpGet]
        public IActionResult Index()
        {
            var access = RequireLogin();
            if (!(access is OkResult))
            {
                return access;
            }

            ViewBag.Role = CurrentRole;
            ViewBag.CanEdit = CurrentRole == "Менеджер" || CurrentRole == "Администратор";
            return View(MaterialService.GetAll());
        }

        [HttpGet]
        public IActionResult Create()
        {
            var access = RequireRole("Менеджер", "Администратор");
            if (!(access is OkResult))
            {
                return access;
            }

            var form = new MaterialForm
            {
                Types = MaterialService.GetTypes()
            };
            return View("Edit", form);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var access = RequireRole("Менеджер", "Администратор");
            if (!(access is OkResult))
            {
                return access;
            }

            var material = MaterialService.GetById(id);
            if (material == null)
            {
                SetMessage("error", "Ошибка",
                    "Материал не найден. Возможно, запись была удалена другим пользователем. Обновите список материалов.");
                return RedirectToAction("Index");
            }

            var form = new MaterialForm
            {
                MaterialId = material.MaterialId,
                MaterialTypeId = material.MaterialTypeId,
                Name = material.Name,
                UnitPrice = material.UnitPrice,
                Unit = material.Unit,
                PackQty = material.PackQty,
                StockQty = material.StockQty,
                MinQty = material.MinQty,
                Types = MaterialService.GetTypes()
            };
            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MaterialForm model)
        {
            var access = RequireRole("Менеджер", "Администратор");
            if (!(access is OkResult))
            {
                return access;
            }

            if (!ModelState.IsValid)
            {
                model.Types = MaterialService.GetTypes();
                SetMessage("error", "Ошибка",
                    "Форма содержит ошибки. Исправьте отмеченные поля и сохраните данные повторно.");
                return View(model);
            }

            if (MaterialService.NameExists(model.Name, model.MaterialId))
            {
                ModelState.AddModelError(nameof(model.Name),
                    "Материал с таким наименованием уже существует. Укажите другое наименование.");
                model.Types = MaterialService.GetTypes();
                SetMessage("error", "Ошибка",
                    "Не удалось сохранить материал: наименование должно быть уникальным.");
                return View(model);
            }

            try
            {
                if (model.MaterialId.HasValue)
                {
                    MaterialService.Update(model);
                    SetMessage("info", "Информация", "Данные материала обновлены.");
                }
                else
                {
                    MaterialService.Add(model);
                    SetMessage("info", "Информация", "Материал добавлен в список.");
                }
            }
            catch (Exception)
            {
                SetMessage("error", "Ошибка базы данных",
                    "Не удалось сохранить данные материала. Повторите попытку позже.");
                model.Types = MaterialService.GetTypes();
                return View(model);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Products(int id)
        {
            var access = RequireLogin();
            if (!(access is OkResult))
            {
                return access;
            }

            var material = MaterialService.GetById(id);
            if (material == null)
            {
                SetMessage("error", "Ошибка",
                    "Материал не найден. Обновите список материалов и выберите запись заново.");
                return RedirectToAction("Index");
            }

            ViewBag.MaterialName = material.Name;
            return View(MaterialService.GetProducts(id));
        }

        [HttpGet]
        public IActionResult Suppliers(int id)
        {
            var access = RequireLogin();
            if (!(access is OkResult))
            {
                return access;
            }

            var material = MaterialService.GetById(id);
            if (material == null)
            {
                SetMessage("error", "Ошибка",
                    "Материал не найден. Обновите список материалов и выберите запись заново.");
                return RedirectToAction("Index");
            }

            ViewBag.MaterialName = material.Name;
            return View(MaterialService.GetSuppliers(id));
        }
    }
}
