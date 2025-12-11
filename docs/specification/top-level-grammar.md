# Грамматика верхнего уровня языка **Minion#**

## Примеры кода

### Пример 1: Приветствие
```minion
bello!

tulalilloo ti amo (!What is your name?! ) naidu!
trusela name Spaghetti naidu!
guoleila (name) naidu!
tulalilloo ti amo (!Hello, ! loka name loka !!) naidu! 
// Вывод: Hello, имя!
```

### Пример 2: Проверка чётности
```minion
bello!

tulalilloo ti amo (!Enter number:! ) naidu!
poop num Banana naidu!
guoleila (num) naidu!
bi-do ((num pado 2) con 0) oca!
    tulalilloo ti amo (!Even!) naidu!
stopa
uh-oh oca!
    tulalilloo ti amo (!Odd!) naidu!
stopa
```

## Ключевые особенности языка

- **Программа начинается** с `bello!`
- Программа состоит из **последовательности инструкций на верхнем уровне**
- **Объявления**:
  - `poop` — изменяемая переменная
  - `trusela` — константа (`const`)
  - `boss` — функция 
- **Типы данных**: (`Banana` (int), `Gelato` (bool), `Papaya` (double), `Spaghetti` (string)) На данный момент поддерживается один тип данных — `Papaya` (double)
- **Логические условия**: 0 = ложь, ≠0 = истина
- **Инструкции** разделяются лексемой `naidu!` (аналог точки с запятой)
- **Блоки кода** ограничиваются `oca!` и `stopa`
- **Ввод-вывод** осуществляется через специальные инструкции:
  - `guoleila (variable)` — чтение из консоли
  - `tulalilloo ti amo (expression)` — вывод в консоль

## Семантические правила

- **Все переменные и параметры** — типа **Papaya**
- **Объявление переменной обязательно** перед использованием
- **Повторное объявление переменной с тем же именем** в одной области видимости **запрещено**
- **Область видимости** переменных ограничена блоком (`oca!` ... `stopa`)
- **Инициализация**:
  - `poop` может быть объявлена без инициализации, но должна быть инициализирована до использования
  - `trusela` инициализируется константным выражением (литерал, belloPi, belloE), не может содержать переменные, вызовы функций или операторы
  - `boss` всегда возвращает Papaya, тело — блок с обязательным tank yu (return)
- **Доступ к необъявленной переменной** — ошибка компиляции
- **Присваивание** осуществляется через оператор `lumai`
- **Все инструкции должны завершаться** `naidu!`
- **Проблема висячего else** решается на уровне грамматики: каждый uh-oh связывается с ближайшим предшествующим bi-do, у которого ещё нет uh-oh.  Это достигается структурой грамматики, где uh-oh является необязательной частью правила if-statement, а не отдельной инструкцией.


## Параметры и аргументы функции

### Объявление параметров

- В языке **допускаются функции без параметров**.  
  Пример:  
  ```minion
  boss pi Papaya () oca!
      tank yu belloPi naidu!
  stopa
  ```

- **Каждый параметр** в объявлении функции **не содержит указания типа**.  
  **Все параметры неявно имеют тип `Papaya`** (единственный числовой тип в языке).  

- **Грамматика списка параметров**:
  ```ebnf
  parameter-list = identifier , { "," , identifier } ;
  ```
  Если параметров нет, список пропускается (пустые скобки `()`).

- Пример объявления с параметрами:
  ```minion
  boss add Papaya (x, y) oca!
      tank yu x melomo y naidu!
  stopa
  ```

### Вызов функции и аргументы

- **Список аргументов** при вызове функции — это **список выражений**, разделённых запятыми:
  ```ebnf
  argument-list = expression , { "," , expression } ;
  ```

- Аргументы **могут быть любыми выражениями**: литералами, переменными, вызовами других функций и т.д.

- Пример вызова:
  ```minion
  poop result Papaya naidu!
  result lumai add (5.0, square (3.0)) naidu!
  ```

### Порядок вычисления аргументов

- **Аргументы вычисляются строго слева направо**.  
  Это означает, что если в аргументах есть побочные эффекты (например, ввод числа), они происходят в порядке записи.

- Пример:
  ```minion
  boss sum Papaya (a, b) oca!
      tank yu a melomo b naidu!
  stopa

  tulalilloo ti amo (sum (guoleila (), guoleila ())) naidu!
  ```
  → Сначала будет выполнен **первый** `guoleila()`, затем **второй**.


## Грамматика в нотации EBNF

(* Основная программа *)
program = "bello!" , { top-level-item } ;

top-level-item = const-declaration
               | function-definition
               | statement ;

(* Объявление константы *)
const-declaration = "trusela" , identifier , "Papaya" , const-value , "naidu!" ;

(* Константное значение — только литерал или встроенная константа *)
const-value = number-literal
            | constant ;

(* Объявление функции *)
function-definition = "boss" , identifier , "Papaya" , "(" , [ parameter-list ] , ")" , block ;

parameter-list = identifier , { "," , identifier } ;

(* Инструкция *)
statement = variable-declaration
          | assignment-statement
          | input-statement
          | output-statement
          | if-statement
          | while-statement
          | for-statement
          | return-statement
          | expression-statement
          , "naidu!" ;

(* Объявление переменной *)
variable-declaration = "poop" , identifier , "Papaya" ;

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
for-statement = "again" , "(" , identifier , "=" , expression , "to" , expression , ")" , block ;

(* Возврат из функции *)
return-statement = "tank yu" , expression ;

(* Выражение как инструкция *)
expression-statement = expression ;

(* Блок кода *)
block = "oca!" , { statement } , "stopa" ;
