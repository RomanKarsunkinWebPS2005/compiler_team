# Грамматика выражений языка **Minion#**

## Синтаксис выражений

Выражения в Minion# могут содержать:

- **Числовые литералы**: целые (`42`, `-7`) и дробные (`3.14`, `-0.5`)
- **Логические литералы**: `da` (истина), `no` (ложь)
- **Строковые литералы**: `"Hello!"`, с интерполяцией через `loka`
- **Переменные**: идентификаторы (пример: `score`, `name`)
- **Операторы**:
  - Арифметические: `melomo` (+), `flavuk` (–), `dibotada` (*), `poopaye` (/), `pado` (%), `beedo` (^)
  - Сравнения: `con` (==), `nocon` (!=), `la` (<), `lacon` (<=), `looka too` (>), `looka too con` (>=)
  - Логические: `makoroni` (НЕ), `tropa` (И), `bo-ca` (ИЛИ)
- **Скобки** для группировки: `(выражение)`
- **Встроенные функции**: `abs`, `min`, `max`
- **Константы**: `pi`, `euler`

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
expression = logical-or-expression ;

(* Логическое ИЛИ *)
logical-or-expression = logical-and-expression , { "bo-ca" , logical-and-expression } ;

(* Логическое И *)
logical-and-expression = equality-expression , { "tropa" , equality-expression } ;

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

(* Возведение в степень — правая ассоциативность *)
power-expression = unary-expression , [ "beedo" , power-expression ] ;

(* Унарные операторы: +, -, ! *)
unary-expression = primary-expression
                 | "melomo" , unary-expression
                 | "flavuk" , unary-expression
                 | "makoroni" , unary-expression ;

(* Первичные выражения *)
primary-expression = number-literal
                   | boolean-literal
                   | string-literal
                   | identifier
                   | constant
                   | "(" , expression , ")"
                   | function-call ;

(* Числовые литералы *)
number-literal = integer-literal | float-literal ;
integer-literal = digit , { digit | "_" , digit } ;
float-literal = digit , { digit | "_" , digit } , "." , digit , { digit | "_" , digit } ;

(* Логические литералы *)
boolean-literal = "da" | "no" ;

(* Строковые литералы *)
string-literal = '"' , { string-char } , '"' ;
string-char = ? любой символ кроме " и \ ? | escape-sequence ;
escape-sequence = "\" , ( "n" | """ ) ;

(* Строковые литералы *)
string-literal = "!" , { string-char } , "!" ;
string-char = letter | digit | space-char | punctuation-char | "!!" | escape-sequence ;
escape-sequence = "\" , "n" ;

(* Идентификаторы *)
identifier = letter , { letter | digit | "_" } ;
letter = "a" | "b" | "c" | "d" | "e" | "f" | "g" | "h" | "i" | "j" | "k" | "l" | "m" |
         "n" | "o" | "p" | "q" | "r" | "s" | "t" | "u" | "v" | "w" | "x" | "y" | "z" |
         "A" | "B" | "C" | "D" | "E" | "F" | "G" | "H" | "I" | "J" | "K" | "L" | "M" |
         "N" | "O" | "P" | "Q" | "R" | "S" | "T" | "U" | "V" | "W" | "X" | "Y" | "Z" ;
digit = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9" ;

space-char = " " ;
punctuation-char = "." | "," | "?" | ":" | ";" | "-" | "_" | "'" | "(" | ")" |
                   "[" | "]" | "{" | "}" | "/" | "\\" | "@" | "#" | "$" | "%" |
                    "^" | "&" | "*" | "+" |"=" | "<" | ">" | "|" | "~" | "`" ;

(* Константы *)
constant = "belloPi" | "belloE" ;

(* Вызов функции *)
function-call = function-name , "(" , [ argument-list ] , ")" ;
function-name = "muak" | "miniboss" | "bigboss" ;
argument-list = expression , { "," , expression } ;
```
