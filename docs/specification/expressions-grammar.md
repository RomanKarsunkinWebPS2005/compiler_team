# Грамматика выражений языка **Minion#**

## Синтаксис выражений

Выражения в Minion# могут содержать:

- **Числовые литералы**: десятичные числа с плавающей точкой
Примеры: 42, -7, 3.14, -0.5, 0, 1.0
(запись без точки допустима и означает число с нулевой дробной частью)
- **Переменные и параметры функций**
- **Операторы**:
  - Арифметические: `melomo` (+), `flavuk` (–), `dibotada` (*), `poopaye` (/), `pado` (%), `beedo` (^)
  - Сравнения: `con` (==), `nocon` (!=), `la` (<), `lacon` (<=), `looka too` (>), `looka too con` (>=)
  - Логические: `makoroni` (НЕ), `tropa` (И), `bo-ca` (ИЛИ)
- **Скобки** для группировки: `(выражение)`
- **Встроенные функции**: `muak` (abs), `miniboss` (min), `bigboss` (max)
- **Константы**: `belloPi`, `belloE`

---

## Операторы

### Арифметические операторы

| Minion#      | Бинарная операция        | Унарная операция |
|--------------|--------------------------|------------------|
| `melomo`     | Сложение `+`             | Унарный плюс     |
| `flavuk`     | Вычитание `-`            | Унарный минус    |
| `dibotada`   | Умножение `*`            | —                |
| `poopaye`    | Деление `/`              | —                |
| `pado`       | Остаток  `%`             | —                |
| `beedo`      | Возведение в степень `**`| —                |


### Операторы сравнения

| Minion#               | Операция      |
|-----------------------|---------------|
| `con`                 | `==` (равно)  |
| `nocon`              | `!=` (не равно) |
| `la`                  | `<` (меньше)  |
| `lacon`              | `<=` (меньше или равно) |
| `looka too`           | `>` (больше)  |
| `looka too con`       | `>=` (больше или равно) |

### Логические операторы

| Minion#     | Операция        |
|-------------|-----------------|
| `makoroni`  | Логическое НЕ   |
| `tropa`     | Логическое И    |
| `bo-ca`     | Логическое ИЛИ  |

---

## Приоритет операторов 

| Уровень | Операторы                          | Ассоциативность |
|--------|------------------------------------|------------------|
| 7      | `()` (скобки, вызов функции)       | —                |
| 6      | `melomo (+)`, `flavuk (-)` (**унарные**) | Правая           |
| 5      | `beedo (**)`                            | Правая       |
| 4      | `dibotada (*)`, `poopaye(/)`, `pado(%)`      | Левая            |
| 3      | `melomo (+)`, `flavuk (-)` (**бинарные**)| Левая            |
| 2      | `looka too con (>=)`, `looka too (>)`, `la con (<=)`, `la (<)`, `nocon (!=)`, `con (==)` | Левая |
| 1      | `makoroni (НЕ)`                         | Правая           |
| 0      | `tropa (И)`                            | Левая            |
| -1     | `bo-ca (ИЛИ)`                            | Левая            |

---

## Встроенные функции и константы

Выбраны три дополнения:

1. **Возведение в степень** → оператор `beedo` (**)
2. **Встроенные функции для чисел**:
   - `abs(x)` — модуль → `muak()`
   - `min(x, y, ...)` — минимум → `miniboss()`
   - `max(x, y, ...)` — максимум → `bigboss()`
3. **Константы**:
   - `BelloPi` → 3.141592653589793
   - `BelloE` → 2.718281828459045

---

## Грамматика в нотации EBNF

```ebnf
(* Основное выражение *)
expression = conditional-expression ;

(* Тернарный условный оператор *)
conditional-expression = logical-or-expression , [ "?" , expression , "!" , expression ] ;

(* Логическое ИЛИ *)
logical-or-expression = logical-and-expression , { "bo-ca" , logical-and-expression } ;

(* Логическое И *)
logical-and-expression = logical-no-expression , { "tropa" , logical-no-expression } ;

(*Логическое НЕ*)
logical-no-expression = equality-expression , { "macoroni" , logical-no-expression } ;

(* Равенство и неравенство *)
equality-expression = relational-expression , { ("con" | "nocon") , relational-expression } ;

(* Сравнения: <, <=, >, >= *)
relational-expression = additive-expression ,
                        { ("looka too" , [ "con" ] | "la" , [ "con" ]) , additive-expression } ;

(* Сложение и вычитание *)
additive-expression = multiplicative-expression ,
                      { ("melomo" | "flavuk") , multiplicative-expression } ;

(* Умножение, деление, остаток *)
multiplicative-expression = power-expression ,
                            { ("dibotada" | "poopaye" | "pado") , power-expression } ;

(* Унарные операторы: +, - *)
unary-expression = power-expression
                 | "melomo" , unary-expression
                 | "flavuk" , unary-expression;

(* Возведение в степень — правая ассоциативность *)
power-expression = primary-expression , [ "beedo" , power-expression ] ;

(* Первичные выражения *)
primary-expression = number-literal
                   | identifier
                   | constant
                   | "(" , expression , ")"
                   | function-call ;

(* Числовые литералы *)
number-literal = [ "-" ] , digit , { digit } , "." , digit , { digit } ;

(* Идентификаторы *)
identifier = letter , { letter | digit | "_" } ;
letter = "a" | "b" | "c" | "d" | "e" | "f" | "g" | "h" | "i" | "j" | "k" | "l" | "m" |
         "n" | "o" | "p" | "q" | "r" | "s" | "t" | "u" | "v" | "w" | "x" | "y" | "z" |
         "A" | "B" | "C" | "D" | "E" | "F" | "G" | "H" | "I" | "J" | "K" | "L" | "M" |
         "N" | "O" | "P" | "Q" | "R" | "S" | "T" | "U" | "V" | "W" | "X" | "Y" | "Z" ;
digit = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9" ;

(* Константы *)
constant = "belloPi" | "belloE" ;

(* Вызов функции *)
function-call = identifier , "(" , [ argument-list ] , ")" ;
argument-list = expression , { "," , expression } ;
```
