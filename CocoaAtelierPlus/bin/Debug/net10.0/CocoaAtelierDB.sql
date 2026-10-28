-- Скрипт создания базы данных SQLite для ИС «Cocoa Atelier Plus».
-- Повторный запуск пересоздает базу: структура и данные загружаются заново.

PRAGMA foreign_keys = ON;

BEGIN TRANSACTION;

DROP TABLE IF EXISTS material_supplier;
DROP TABLE IF EXISTS recipe;
DROP TABLE IF EXISTS supplier;
DROP TABLE IF EXISTS product;
DROP TABLE IF EXISTS material;
DROP TABLE IF EXISTS product_type;
DROP TABLE IF EXISTS material_type;
DROP TABLE IF EXISTS app_user;
DROP TABLE IF EXISTS role;

CREATE TABLE role (
    role_id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL UNIQUE,
    description TEXT NOT NULL
);

CREATE TABLE app_user (
    user_id INTEGER PRIMARY KEY AUTOINCREMENT,
    login TEXT NOT NULL UNIQUE,
    full_name TEXT NOT NULL,
    email TEXT NOT NULL UNIQUE,
    password TEXT NOT NULL DEFAULT '12345',
    is_active INTEGER NOT NULL DEFAULT 1 CHECK (is_active IN (0, 1)),
    role_id INTEGER NOT NULL,
    FOREIGN KEY (role_id) REFERENCES role(role_id)
);

CREATE TABLE material_type (
    material_type_id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL UNIQUE,
    loss_percent REAL NOT NULL CHECK (loss_percent >= 0 AND loss_percent < 100)
);

CREATE TABLE material (
    material_id INTEGER PRIMARY KEY AUTOINCREMENT,
    material_type_id INTEGER NOT NULL,
    name TEXT NOT NULL UNIQUE,
    unit_price REAL NOT NULL CHECK (unit_price >= 0),
    stock_qty REAL NOT NULL CHECK (stock_qty >= 0),
    min_qty REAL NOT NULL CHECK (min_qty >= 0),
    pack_qty REAL NOT NULL CHECK (pack_qty > 0),
    unit TEXT NOT NULL,
    FOREIGN KEY (material_type_id) REFERENCES material_type(material_type_id)
);

CREATE TABLE product_type (
    product_type_id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL UNIQUE,
    coefficient REAL NOT NULL CHECK (coefficient > 0)
);

CREATE TABLE product (
    product_id INTEGER PRIMARY KEY AUTOINCREMENT,
    product_type_id INTEGER NOT NULL,
    sku TEXT NOT NULL UNIQUE,
    name TEXT NOT NULL UNIQUE,
    min_partner_price REAL NOT NULL CHECK (min_partner_price >= 0),
    FOREIGN KEY (product_type_id) REFERENCES product_type(product_type_id)
);

CREATE TABLE recipe (
    product_id INTEGER NOT NULL,
    material_id INTEGER NOT NULL,
    qty_per_unit REAL NOT NULL CHECK (qty_per_unit > 0),
    PRIMARY KEY (product_id, material_id),
    FOREIGN KEY (product_id) REFERENCES product(product_id),
    FOREIGN KEY (material_id) REFERENCES material(material_id)
);

CREATE TABLE supplier (
    supplier_id INTEGER PRIMARY KEY AUTOINCREMENT,
    supplier_type TEXT NOT NULL,
    name TEXT NOT NULL UNIQUE,
    inn TEXT NOT NULL UNIQUE,
    rating INTEGER NOT NULL CHECK (rating BETWEEN 1 AND 5),
    start_date TEXT NOT NULL
);

CREATE TABLE material_supplier (
    material_id INTEGER NOT NULL,
    supplier_id INTEGER NOT NULL,
    purchase_price REAL NOT NULL CHECK (purchase_price >= 0),
    delivery_days INTEGER NOT NULL CHECK (delivery_days >= 0),
    PRIMARY KEY (material_id, supplier_id),
    FOREIGN KEY (material_id) REFERENCES material(material_id),
    FOREIGN KEY (supplier_id) REFERENCES supplier(supplier_id)
);

INSERT INTO role (role_id, name, description) VALUES
(1, 'Пользователь', 'Просмотр списка материалов, продукции и поставщиков без изменения данных'),
(2, 'Менеджер', 'Просмотр, добавление и редактирование материалов, просмотр поставщиков и продукции'),
(3, 'Администратор', 'Полный доступ: материалы, поставщики, справочники, пользователи и роли');

INSERT INTO app_user (login, full_name, email, role_id, is_active) VALUES
('user01', 'Иванова Мария Сергеевна', 'm.ivanova@cocoa-plus.ru', 1, 1),
('user02', 'Петров Алексей Викторович', 'a.petrov@cocoa-plus.ru', 1, 1),
('manager01', 'Соколова Анна Игоревна', 'a.sokolova@cocoa-plus.ru', 2, 1),
('manager02', 'Ким Дмитрий Олегович', 'd.kim@cocoa-plus.ru', 2, 1),
('admin01', 'Кузнецова Елена Павловна', 'e.kuznetsova@cocoa-plus.ru', 3, 1);

INSERT INTO material_type (name, loss_percent) VALUES
('Какао-масса', 0.18),
('Какао-масло', 0.14),
('Сахар', 0.05),
('Молочные ингредиенты', 0.1),
('Орехи', 0.22),
('Фруктовые добавки', 0.2),
('Ароматизаторы и специи', 0.08),
('Декор', 0.06),
('Упаковка', 0.03),
('Маркировка', 0.02);

INSERT INTO material (name, material_type_id, unit_price, stock_qty, min_qty, pack_qty, unit) VALUES
('Какао-масса Ecuador 70%', 1, 2150, 240, 80, 25, 'кг'),
('Какао-масса Peru 65%', 1, 1980, 180, 60, 25, 'кг'),
('Какао-масло deodorized', 2, 2680, 110, 35, 10, 'кг'),
('Какао-масло premium', 2, 2890, 90, 30, 10, 'кг'),
('Сахар тростниковый мелкий', 3, 126, 520, 150, 50, 'кг'),
('Сахарная пудра экстра', 3, 142, 340, 100, 25, 'кг'),
('Молоко сухое цельное', 4, 420, 160, 50, 20, 'кг'),
('Сливки сухие premium', 4, 510, 120, 40, 20, 'кг'),
('Фундук жареный дробленый', 5, 980, 78, 25, 5, 'кг'),
('Миндаль лепестки', 5, 1140, 64, 20, 5, 'кг'),
('Фисташка очищенная', 5, 1680, 42, 15, 5, 'кг'),
('Малина сублимированная', 6, 2150, 26, 8, 1, 'кг'),
('Апельсин сублимированный', 6, 1870, 22, 7, 1, 'кг'),
('Лимон сублимированный', 6, 1790, 18, 6, 1, 'кг'),
('Ваниль натуральная Bourbon', 7, 6900, 6.5, 2, 0.25, 'кг'),
('Морская соль хлопья', 7, 320, 35, 10, 1, 'кг'),
('Корица молотая цейлонская', 7, 860, 14, 4, 0.5, 'кг'),
('Какао-пудра dark', 8, 520, 38, 12, 2, 'кг'),
('Коробка крафт малая', 9, 16, 1800, 600, 100, 'шт'),
('Коробка крафт премиум', 9, 28, 1200, 350, 50, 'шт'),
('Фольга золотая', 9, 4.2, 4200, 1200, 500, 'шт'),
('Лента атласная бордо', 9, 9.5, 680, 200, 50, 'м'),
('Этикетка фирменная', 10, 2.4, 5200, 1500, 500, 'шт'),
('Стикер состав/аллергены', 10, 1.8, 4800, 1200, 500, 'шт');

INSERT INTO product_type (name, coefficient) VALUES
('Плитка шоколада', 1),
('Премиальная плитка', 1.15),
('Ассорти конфет', 1.3),
('Подарочный набор', 1.45),
('Сезонная коллекция', 1.25),
('Мини-дегустация', 0.85);

INSERT INTO product (product_type_id, sku, name, min_partner_price) VALUES
(1, 'CA-001', 'Шоколад 70% Ecuador', 390),
(1, 'CA-002', 'Молочный шоколад классический', 360),
(2, 'CA-003', 'Шоколад с фундуком и морской солью', 460),
(2, 'CA-004', 'Белый шоколад с малиной', 480),
(2, 'CA-005', 'Темный шоколад с апельсином', 470),
(3, 'CA-006', 'Трюфели классические 9 шт', 890),
(3, 'CA-007', 'Трюфели премиум 16 шт', 1490),
(4, 'CA-008', 'Набор «Classic Cocoa»', 1790),
(4, 'CA-009', 'Набор «Berry Luxury»', 2190),
(5, 'CA-010', 'Коллекция «Winter Spices»', 1290),
(5, 'CA-011', 'Коллекция «Citrus Mood»', 1240),
(6, 'CA-012', 'Набор мини-плиток 6 вкусов', 990);

INSERT INTO recipe (product_id, material_id, qty_per_unit) VALUES
(1, 1, 0.085),
(1, 3, 0.012),
(1, 5, 0.018),
(1, 21, 1),
(1, 23, 1),
(1, 24, 1),
(2, 2, 0.06),
(2, 3, 0.01),
(2, 5, 0.02),
(2, 7, 0.018),
(2, 21, 1),
(2, 23, 1),
(2, 24, 1),
(3, 1, 0.075),
(3, 4, 0.014),
(3, 5, 0.018),
(3, 9, 0.02),
(3, 16, 0.001),
(3, 21, 1),
(3, 23, 1),
(3, 24, 1),
(4, 4, 0.022),
(4, 6, 0.03),
(4, 7, 0.024),
(4, 12, 0.009),
(4, 15, 0.001),
(4, 21, 1),
(4, 23, 1),
(4, 24, 1),
(5, 1, 0.078),
(5, 3, 0.013),
(5, 5, 0.017),
(5, 13, 0.01),
(5, 21, 1),
(5, 23, 1),
(5, 24, 1),
(6, 2, 0.11),
(6, 4, 0.018),
(6, 8, 0.032),
(6, 18, 0.006),
(6, 19, 1),
(6, 24, 1),
(6, 23, 1),
(7, 1, 0.165),
(7, 4, 0.024),
(7, 8, 0.045),
(7, 18, 0.01),
(7, 20, 1),
(7, 22, 0.8),
(7, 24, 1),
(7, 23, 1),
(8, 1, 0.16),
(8, 3, 0.02),
(8, 5, 0.028),
(8, 20, 1),
(8, 22, 1),
(8, 23, 1),
(8, 24, 1),
(9, 4, 0.032),
(9, 6, 0.04),
(9, 7, 0.028),
(9, 12, 0.014),
(9, 20, 1),
(9, 22, 1.2),
(9, 23, 1),
(9, 24, 1),
(10, 2, 0.09),
(10, 3, 0.014),
(10, 5, 0.02),
(10, 17, 0.002),
(10, 19, 1),
(10, 23, 1),
(10, 24, 1),
(11, 1, 0.088),
(11, 3, 0.013),
(11, 5, 0.019),
(11, 13, 0.008),
(11, 14, 0.004),
(11, 19, 1),
(11, 23, 1),
(11, 24, 1),
(12, 1, 0.18),
(12, 3, 0.024),
(12, 5, 0.042),
(12, 21, 6),
(12, 20, 1),
(12, 23, 1),
(12, 24, 1);

INSERT INTO supplier (supplier_type, name, inn, rating, start_date) VALUES
('ООО', 'КакаоТрейд', '7701012345', 5, '14.02.2021'),
('ООО', 'Global Cacao Import', '7701012377', 4, '02.06.2021'),
('ООО', 'Sweet Base', '7701012401', 4, '19.09.2021'),
('ИП', 'Nut Story', '7701012448', 5, '08.11.2021'),
('ООО', 'BerryDry', '7701012500', 5, '17.01.2022'),
('ООО', 'Aroma Ingredient', '7701012528', 4, '25.03.2022'),
('ООО', 'PackLine Pro', '7701012604', 5, '12.05.2022'),
('ООО', 'Label Factory', '7701012686', 4, '03.08.2022');

INSERT INTO material_supplier (material_id, supplier_id, purchase_price, delivery_days) VALUES
(1, 1, 1980, 12),
(1, 2, 2015, 14),
(2, 1, 1840, 11),
(2, 2, 1895, 13),
(3, 1, 2460, 10),
(3, 2, 2490, 12),
(4, 2, 2680, 13),
(5, 3, 108, 4),
(6, 3, 122, 4),
(7, 3, 380, 6),
(8, 3, 462, 6),
(9, 4, 890, 5),
(10, 4, 1030, 5),
(11, 4, 1540, 7),
(12, 5, 1980, 6),
(13, 5, 1710, 6),
(14, 5, 1630, 6),
(15, 6, 6420, 9),
(16, 6, 270, 5),
(17, 6, 790, 5),
(18, 3, 470, 4),
(19, 7, 13.5, 4),
(20, 7, 24.5, 5),
(21, 7, 3.4, 3),
(22, 7, 7.9, 4),
(23, 8, 1.95, 3),
(24, 8, 1.4, 3);

CREATE INDEX idx_material_type_id ON material (material_type_id);
CREATE INDEX idx_product_type_id ON product (product_type_id);
CREATE INDEX idx_recipe_material_id ON recipe (material_id);
CREATE INDEX idx_recipe_product_id ON recipe (product_id);
CREATE INDEX idx_app_user_role_id ON app_user (role_id);
CREATE INDEX idx_material_supplier_supplier_id ON material_supplier (supplier_id);

COMMIT;