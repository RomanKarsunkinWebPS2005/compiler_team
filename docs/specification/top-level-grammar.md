# Грамматика верхнего уровня языка **Minion#**

## Примеры кода

### Пример 1: Приветствие
```minion
bello!

tulalilloo ti amo (!What is your name?! ) naidu!
trusela name Spaghetti naidu!
guoleila (name) naidu!
tulalilloo ti amo (!Hello, ! loka name loka !!) naidu!
```

### Пример 2: Проверка чётности
```minion
bello!

tulalilloo ti amo (!Enter number:! ) naidu!
poop num Banana naidu!
guoleila (num) naidu!
bi-do (con (pado num Banana 2) Banana 0) oca!
    tulalilloo ti amo (!Even!) naidu!
stopa
uh-oh oca!
    tulalilloo ti amo (!Odd!) naidu!
stopa
```

## Ключевые особенности языка

- **Программа начинается** с обязательной директивы `bello!`
- Программа состоит из **последовательности инструкций на верхнем уровне**
- **Переменные** объявляются явно с указанием типа:
  - `poop` — изменяемая переменная
  - `trusela` — константа времени компиляции (`const`)
- **Типы данных**: `Banana` (int), `Gelato` (bool), `Papaya` (double), `Spaghetti` (string)
- **Инструкции** разделяются лексемой `naidu!` (аналог точки с запятой)
- **Блоки кода** ограничиваются `oca!` и `stopa`
- **Ввод-вывод** осуществляется через специальные инструкции:
  - `guoleila (variable)` — чтение из консоли
  - `tulalilloo ti amo (expression)` — вывод в консоль

## Семантические правила

- **Объявление переменной обязательно** перед использованием
- **Повторное объявление переменной с тем же именем** в одной области видимости **запрещено**
- **Область видимости** переменных ограничена блоком (`oca!` ... `stopa`)
- **Инициализация**:
  - `poop` может быть объявлена без инициализации, но должна быть инициализирована до использования
  - `trusela` **обязательно инициализируется** при объявлении
- **Доступ к необъявленной переменной** — ошибка компиляции
- **Присваивание** осуществляется через оператор `lumai`
- **Все инструкции должны завершаться** `naidu!`

## Грамматика в нотации EBNF

```ebnf
(* Основная программа *)
program = "bello!" , { statement } ;

(* Инструкция *)
statement = variable-declaration
          | assignment-statement
          | input-statement
          | output-statement
          | if-statement
          | while-statement
          | for-statement
          | expression-statement
          , "naidu!" ;

(* Объявление переменной *)
variable-declaration = mutable-var | constant-var ;

mutable-var = "poop" , identifier , type-specifier ;

constant-var = "trusela" , identifier , type-specifier , const-value ;

(* Константное значение — только литерал или предопределённая константа *)
const-value = number-literal
            | boolean-literal
            | string-literal
            | constant ;

(* Присваивание *)
assignment-statement = identifier , "lumai" , expression ;

(* Ввод *)
input-statement = "guoleila" , "(" , identifier , ")" ;

(* Вывод *)
output-statement = "tulalilloo ti amo" , "(" , expression , ")" ;

(* Условный оператор *)
if-statement = "bi-do" , "(" , expression , ")" , block , [ "uh-oh" , block ] ;

(* Цикл while *)
while-statement = "kemari" , "(" , expression , ")" , block ;

(* Цикл for *)
for-statement = "again" , "(" , for-init , "con" , expression , "le" , expression , ")" , block ;
for-init = identifier , type-specifier ;

(* Выражение как инструкция *)
expression-statement = expression ;

(* Блок кода *)
block = "oca!" , { statement } , "stopa" ;

(* Тип данных *)
type-specifier = "Banana" | "Gelato" | "Papaya" | "Spaghetti";

(* Выражения *)
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

(* Унарные операторы: +, - *)
unary-expression = power-expression
                 | "melomo" , unary-expression
                 | "flavuk" , unary-expression;

(* Возведение в степень — правая ассоциативность *)
power-expression = primary-expression , [ "beedo" , power-expression ] ;

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
integer-literal = [ "-" ] , digit , { digit } ;
float-literal = [ "-" ] , digit , { digit } , "." , digit , { digit } ;

(* Логические литералы *)
boolean-literal = "da" | "no" ;

(* Строковые литералы *)
string-literal = "!" , { string-char } , "!" ;
string-char = ? любой символ кроме "!" ? | "!!" | "\" , "n" ;

(* Предопределённые константы *)
constant = "bellopi" | "bananee" ;

(* Вызов функции *)
function-call = function-name , "(" , [ argument-list ] , ")" ;
function-name = "muak" | "miniboss" | "bigboss" ;
argument-list = expression , { "," , expression } ;

(* Идентификаторы *)
identifier = letter , { letter | digit | "_" } ;
letter = "a" | "b" | "c" | "d" | "e" | "f" | "g" | "h" | "i" | "j" | "k" | "l" | "m" |
         "n" | "o" | "p" | "q" | "r" | "s" | "t" | "u" | "v" | "w" | "x" | "y" | "z" |
         "A" | "B" | "C" | "D" | "E" | "F" | "G" | "H" | "I" | "J" | "K" | "L" | "M" |
         "N" | "O" | "P" | "Q" | "R" | "S" | "T" | "U" | "V" | "W" | "X" | "Y" | "Z" ;
digit = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9" ;
```
