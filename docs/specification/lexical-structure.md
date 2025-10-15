# Лексическая структура языка Minion#

## 1. Примеры кода

### Пример 1:

**Minion#**
```minion
bello!

boss greeting
oca!
    me main
    oca!
        trusela name Spaghetti naidu!
        guoleila (name) naidu!
        tulalilloo ti amo ("Hello, " loka name loka "!") naidu!
        tank yu naidu!
    stopa
stopa
```

**C#**
```csharp
using System;

class greeting
{
    static void Main()
    {
        string name = Console.ReadLine()!;
        Console.WriteLine($"Hello, {name}!");
    }
}
```

---

### Пример 2:

**Minion#**
```minion
bello!

boss evencheck
oca!
    me main
    oca!
        poop num Banana naidu!
        guoleila (num) naidu!
        
        bi-do (con (pado num Banana 2) Banana 0) oca!
            tulalilloo ti amo ("Even") naidu!
        stopa
        uh-oh oca!
            tulalilloo ti amo ("Odd") naidu!
        stopa
        
        tank yu naidu!
    stopa
stopa
```

**C#**
```csharp
using System;

class evencheck
{
    static void Main()
    {
        int num = int.Parse(Console.ReadLine()!);
        if (num % 2 == 0)
            Console.WriteLine("Even");
        else
            Console.WriteLine("Odd");
    }
}
```

---

###  Пример 3:

**Minion#**
```minion
bello!

boss powercalc
oca!
    me main
    oca!
        tulalilloo ti amo ("Base?") naidu!
        poop base Banana naidu!
        guoleila (base) naidu!
        
        tulalilloo ti amo ("Exponent?") naidu!
        poop exp Banana naidu!
        guoleila (exp) naidu!
        
        poop result Banana naidu!
        result beedo (base Banana exp) naidu!
        tulalilloo ti amo ("Result: " loka result loka) naidu!
        
        tank yu naidu!
    stopa
stopa
```

**C#**
```csharp
using System;

class powercalc
{
    static void Main()
    {
        Console.WriteLine("Base?");
        int baseVal = int.Parse(Console.ReadLine()!);
        
        Console.WriteLine("Exponent?");
        int exp = int.Parse(Console.ReadLine()!);
        
        double result = Math.Pow(baseVal, exp);
        Console.WriteLine($"Result: {result}");
    }
}
```

---

## 2. Ключевые слова


| Слово                 | Назначение                                |
|----------------------|--------------------------------------------|
| `bello!`             | Начало программы                           |
| `boss`               | Объявление класса                          |
| `mini boss`          | Интерфейс                                  |
| `big boss`           | Абстрактный класс                          |
| `me`                 | Объявление метода                          |
| `oca!`               | Начало блока                               |
| `stopa`              | Конец блока                                |
| `bapple`             | Неизменяемая переменная (`readonly`)       |
| `poop`               | Изменяемая переменная                      |
|  `trusela`           | Константа времени компиляции (`const`)     |
| `bi-do`              | Условие `if`                               |
| `uh-oh`              | Ветвь `else`                               |
| `again`              | Цикл `for`                                 |
| `kemari`             | Цикл `while`                               |
| `aspetta`            | Оператор `continue`                        |
| `tulalilloo ti amo`  | Вывод в консоль (`print`)                  |
| `guoleila`           | Чтение из консоли (`input`)                |
| `tank yu`            | Возврат значения (`return`)                |
| `boo-ya`             | Выброс исключения (`throw`)                |
| `naidu!`             | Завершение выражения (аналог `;`)          |

---

## 3. Идентификаторы

- Начинаются с буквы (`a–z`, `A–Z`) или `_`.
- Могут содержать буквы, цифры, `_`.
- **Регистр важен**: `count ≠ Count`.
- **Нельзя использовать ключевые слова** (`trusela`, `poop`, `tim` и т.д.).
- **Нельзя использовать имена типов** как идентификаторы.

+Примеры: `score`, `_temp`, `myName`  
-Примеры: `123x`, `trusela`, `Banana`, `Gelato` - неверно

---

## 4. Типы данных (с заглавной буквы!)

| Тип         | Описание                     | C# эквивалент |
|-------------|------------------------------|---------------|
| `Banana`    | Целое число                  | `int`         |
| `Gelato`    | Логическое значение          | `bool`        |
| `Papaya`    | Число с плавающей точкой     | `double`      |
| `Spaghetti` | Строка                       | `string`      |
| `Naletuna`  | Исключение                   | `Exception`   |

---

## 5. Литералы

### 5.1. Числовые литералы
- Целые: `0`, `42`, `-7`
- Дробные: `3.14`, `-0.5`
- Разделитель разрядов: `1_000`
- Шестнадцатеричные: `0xCAFE`
- Двоичные: `0b101010`

### 5.2. Строковые литералы
- Обрамляются `! ... !`:
  ```minion
  banana msg ! Yam, yam! !
  ```
- Экранирование:
  - `\n` — новая строка
  - `\b` — звук «банан!»
- Многострочные:
  ```minion
  banana poem !
  Tulalilloo ti amo!
  Ba-na-na!
  !
  ```

### 5.3. Логические литералы
- `Da` → `true`
- `No` → `false`

---

## 6. Операторы


| Категория       | Minion#      | Описание               |
|----------------|--------------|------------------------|
| Присваивание   | `lumai`      | `x lumai 5`            |
| Степень        | `beedo`      | `x beedo (2 Banana 3)` |
| Умножение      | `dibotada`   | `x dibotada y`         |
| Деление        | `poopaye`    | `x poopaye y`          |
| Остаток        | `pado`       | `x pado y`             |
| Сложение       | `melomo`     | `x melomo y`           |
| Вычитание      | `flavuk`     | `x flavuk y`           |
| Равенство      | `con`        | `x con y`              |
| Больше         | `looka too`  | `x looka too t`        |
| Меньше         | `la`         | `x la y`               |
| Логическое НЕ  | `makoroni`   | `makoroni flag`        |
| Логическое И   | `tropa`      | `tropa a b`            |
| Логическое ИЛИ | `bo-ca`      | `bo-ca a b`            |



### Приоритет операций

Операторы группируются по **уровням приоритета** (от высшего к низшему).  
Операторы **внутри одного уровня** имеют **левую ассоциативность**, кроме `beedo` (правая, как в математике).

| Уровень | Операторы                     | Описание                     | Ассоциативность |
|--------|-------------------------------|------------------------------|------------------|
| 1      | `()`                          | Скобки (группировка)         | —                |
| 2      | `beedo`                       | Возведение в степень (`^`)   | **Правая**       |
| 3      | `dibotada`, `poopaye`, `pado` | Умножение, деление, остаток  | Левая            |
| 4      | `melomo`, `flavuk`            | Сложение, вычитание          | Левая            |
| 5      | `looka too`, `la`, `con`      | Сравнения (`>`, `<`, `==`)   | Левая            |
| 6      | `makoroni`                    | Логическое НЕ (`!`)          | Правая           |
| 7      | `tropa`                       | Логическое И (`&&`)          | Левая            |
| 8      | `bo-ca`                       | Логическое ИЛИ (`\|\|`)      | Левая            |

---

### Пояснение приоритета

#### Пример 1: Арифметика
```minion
x melomo (y dibotada z) naidu!   // x + (y * z) — умножение выше сложения
```

#### Пример 2: Степень (правая ассоциативность)
```minion
a beedo (b Banana c) naidu!      // a ^ (b ^ c), а не (a ^ b) ^ c
```


#### Пример 3: Смешанные операции
```minion
bi-do (makoroni con score Banana 100) oca! ... stopa
```
→ Сначала `con` (сравнение), потом `makoroni` (отрицание)

---

## 7. Прочие лексемы

| Лексема      | Назначение                                      |
|--------------|-------------------------------------------------|
| `( )`        | Группировка выражений, аргументов и приоритета |
| `"..."`      | Строковые литералы                              |
| `loka`       | Маркер интерполяции переменных в строках        |
| `naidu!`     | Завершение выражения (аналог точки с запятой `;`) |
| пробел       | Разделитель между токенами (ключевыми словами, идентификаторами, операторами) |

---

## 8. Комментарии

### Однострочные:
```minion
// Tatata bala tu = forbidden!
// Yam, yam! — this code is delicious
```

### Многострочные:
```minion
/*
 * Never say: Tatata bala tu (I hate you)
 * Always say: Tank yu!
 */
```
> Комментарии **не требуют завершения `naidu!`**, так как не являются исполняемыми выражениями.

---

## 9. Ввод и вывод


### Ввод — `guoleila`

- Считывает строку из консоли и **автоматически преобразует** её к типу **уже объявленной переменной**.  
- Поддерживаемые типы:  
  - `Banana` → целое число (`42`, `-7`)  
  - `Papaya` → дробное число (`3.14`)  
  - `Gelato` → `da`/`no` (регистронезависимо)  
  - `Spaghetti` → любая строка  
- При ошибке преобразования — исключение `Naletuna`.

### Вывод — `tulalilloo ti amo`

- Выводит **любое значение** (строку, число, логическое) — автоматически преобразуя его в текст.  
- Поддерживает **интерполяцию** через `loka`:  
  `"Score: " loka points loka "`  

### Пример:
```minion
trusela name Spaghetti naidu!
guoleila (name) naidu!
tulalilloo ti amo ("Hello, " loka name loka "!") naidu!
```