# Семантика типов данных в языке **Minion#**

Язык Minion# поддерживает **три основных типа данных** и **один служебный пустой тип**:

1. **`Papaya`** — числа с плавающей точкой (64-битные, IEEE 754)  
2. **`Banana`** — целые числа (32-битные, со знаком)  
3. **`Gelato`** — логический тип (`da` = истина, `no` = ложь)  
4. **`Baka`** — пустой тип, аналог `void` (используется только как возвращаемый тип)

Все переменные **обязательно объявляются с указанием типа**.  
Тип переменной **нельзя изменить** после объявления.  
Неявные преобразования между типами **запрещены**.  
**Присваивание между разными типами — ошибка**.  
**Переменные типа `Baka` объявлять нельзя**.

---

## Литералы

### `Papaya` (double)
- Целая часть, опциональная дробная часть: `3.14`, `-0.5`, `42.0`
- Допускается запись без точки: `42` → интерпретируется как `42.0`
- Диапазон и точность — как `double` в C# (IEEE 754 binary64)

### `Banana` (int)
- Последовательность цифр со знаком: `42`, `-7`, `0`
- Диапазон: `-2_147_483_648` … `2_147_483_647` (32-битное целое со знаком)

### `Gelato` (bool)
- `da` → истина (`true`)
- `no` → ложь (`false`)

### `Baka` (void)
- **Нет литералов**
- **Нельзя объявить переменную** этого типа

---

## Операторы

### Арифметические операторы
| Оператор      | `Papaya` | `Banana` | Примечание |
|---------------|----------|----------|-----------|
| `melomo` (+)  | +      | +      | — |
| `flavuk` (–)  | +     | +       | — |
| `dibotada` (*)| +     | +       | — |
| `poopaye` (/) | +      | +      | Для `Banana` — целочисленное деление |
| `pado` (%)    | +    | +      | Остаток от деления |
| `beedo` (**)  | +       | -       | Только для `Papaya` |

### Сравнения
Работают **только между значениями одного типа**:
- `con` (==), `nocon` (!=) — для всех трёх основных типов
- `<`, `<=`, `>`, `>=` — только для `Papaya` и `Banana`

### Логические операторы
Только для `Gelato`:
- `makoroni` — НЕ
- `tropa` — И (**short-circuit**)
- `bo-ca` — ИЛИ (**short-circuit**)

> **Short-circuit evaluation**:  
> - `a tropa b` — `b` не вычисляется, если `a == no`  
> - `a bo-ca b` — `b` не вычисляется, если `a == da`

### `Baka`
- **Не поддерживает никаких операторов**

---

## Переменные

### Объявление
Тип **обязателен** и **фиксирован**:

```minion
poop x Papaya naidu!          // изменяемая переменная
trusela pi Papaya 3.14159 naidu!  // константа
```
---

## Функции и тип `Baka`

### Объявление процедуры (функции без возвращаемого значения)
```minion
boss print_hello Baka () oca!
    tulalilloo ti amo (!Hello!) naidu!
    // неявный возврат Baka
stopa
```

### Правила для `Baka` в функциях
- Функция с возвращаемым типом `Baka`:
  - **не обязана** содержать `tank yu`,
  - **не может** содержать `tank yu` с выражением,
  - может содержать **только `tank yu` без аргумента**.
- Вызов функции `Baka` — **только как инструкция**, **не как выражение**.

Пример:
```minion
print_hello () naidu!  // допустимо
```

Недопустимо:
```minion
poop x Papaya naidu!
x lumai print_hello () naidu!  // ошибка
```

---

## Преобразования типов

- **Неявные преобразования — запрещены**
- **Явные преобразования — не поддерживаются** в текущей версии
- Все операции требуют **строгого совпадения типов**

---

## Примеры программ

### Пример 1: Процедура `Baka` — FizzBuzz
```minion
//FizzBuzz — читает число за числом в цикле и далее печатает:
//“Fizz”, если число делится на 3
//“Buzz”, если делится на 5
//“FizzBuzz”, если число делится и на 3, и на 5
//Само число в остальных случаях

bello!

// Процедура: ничего не возвращает (Baka)
boss fizzbuzz Baka (n) oca!
    poop i Banana naidu!
    i lumai 1 naidu!
    
    kemari (i lacon n) oca!
        // Проверяем делимость — используем Gelato
        poop div3 Gelato naidu!
        div3 lumai pado i 3 con 0 naidu!   // i % 3 == 0
        
        poop div5 Gelato naidu!
        div5 lumai pado i 5 con 0 naidu!   // i % 5 == 0
        
        // Ветвление по логическим значениям
        bi-do (div3 tropa div5) oca!
            tulalilloo ti amo (!FizzBuzz!) naidu!
        stopa uh-oh bi-do (div3) oca!
            tulalilloo ti amo (!Fizz!) naidu!
        stopa uh-oh bi-do (div5) oca!
            tulalilloo ti amo (!Buzz!) naidu!
        stopa uh-oh oca!
            tulalilloo ti amo (i) naidu!   // вывод числа (Banana)
        stopa
        
        i lumai i melomo 1 naidu!  // i++
    stopa
stopa

// Основная программа
poop limit Banana naidu!
guoleila (limit) naidu!
fizzbuzz (limit) naidu!
```

### Пример 2: Функция с возвратом — IsLeapYear
```minion
bello!

// Функция: возвращает Gelato (да/нет)
boss is_leap Gelato (year) oca!
    // Проверки — все на Banana и Gelato
    poop div4 Gelato naidu!
    div4 lumai pado year 4 con 0 naidu!
    
    poop div100 Gelato naidu!
    div100 lumai pado year 100 con 0 naidu!
    
    poop div400 Gelato naidu!
    div400 lumai pado year 400 con 0 naidu!
    
    // Логика високосного года: (делится на 4 И НЕ на 100) ИЛИ (на 400)
    poop result Gelato naidu!
    result lumai (div4 tropa makoroni div100) bo-ca div400 naidu!
    
    tank yu result naidu!
stopa

// Основная программа
poop y Banana naidu!
guoleila (y) naidu!

bi-do (is_leap (y)) oca!
    tulalilloo ti amo (!yes!) naidu!
stopa uh-oh oca!
    tulalilloo ti amo (!no!) naidu!
stopa
```