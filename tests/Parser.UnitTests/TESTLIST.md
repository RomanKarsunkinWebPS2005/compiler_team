# Список тестов для Parser

## Выражения (ParseExpressionTest)

### Числа и базовые операции
- [x] `1` → `1`
- [x] `10 melomo 5 flavuk 2` → `13`
- [x] `flavuk 10 melomo 5 flavuk flavuk 5` → `-10`
- [x] `10 dibotada 5 poopaye 2` → `25`
- [x] `10 pado 2` → `0`
- [x] `10 pado 3` → `1`
- [x] `1.128 melomo 8 flavuk 7.5` → `2` (округление до int)
- [x] `2 beedo 5` → `32`
- [x] `(2 melomo 3) poopaye 10` → `0` (округление до int)
- [x] `(flavuk 2) beedo 10` → `1024`
- [x] `flavuk 2 beedo 10` → `-1024`

### Логические операции
- [x] `da` → `1`
- [x] `no` → `0`
- [x] `5 con 5` → `1`
- [x] `5 nocon 10` → `1`
- [x] `5 la 10` → `1`
- [x] `5 la con 5` → `1`
- [x] `10 looka too 5` → `1`
- [x] `10 looka too con 10` → `1`
- [x] `da tropa no` → `0`
- [x] `da bo-ca no` → `1`
- [x] `makoroni da` → `0`
- [x] `makoroni no` → `1`

### Приоритет операторов
- [x] `2 melomo 3 dibotada 4` → `14`
- [x] `2 dibotada 3 beedo 2` → `18`
- [x] `(2 melomo 3) dibotada 4` → `20`
- [x] `flavuk 5 dibotada 3` → `-15`
- [x] `da tropa no bo-ca da` → `1`

### Функции
- [x] `muak(flavuk 15)` → `15`
- [x] `miniboss(5, 4)` → `4`
- [x] `bigboss(5, 4)` → `5`
- [x] `miniboss(bigboss(1, 5), miniboss(10, 6))` → `5`

---

## Программы верхнего уровня (ParseProgramTest)

### Минимальная программа
- [x] Минимальная программа с выводом: `bello!` + `tulalilloo ti amo (1) naidu!` → `[1]`

### Константы и переменные
- [x] Константы и переменные: `trusela pi Papaya 3.14` + `poop x Papaya` + `x lumai 5` + вывод → `[5]`
- [x] Константы `belloPi` и `belloE`: объявление без вывода → `[]`

### Ввод/вывод
- [x] Ввод и вывод: `guoleila (x)` + `tulalilloo ti amo (x melomo 1)` → `[6]` (при вводе `5`)

### Условные операторы
- [x] If/else: `bi-do` + `uh-oh` с выводом → `[1]` (при `x = 5`)
- [ ] If без else с истинным условием: `bi-do (da) oca! ... stopa` → выполнение блока
- [ ] If без else с ложным условием: `bi-do (no) oca! ... stopa` → пропуск блока
- [ ] If с else, истинное условие: `bi-do (da) oca! ... stopa uh-oh oca! ... stopa` → выполнение then, пропуск else
- [ ] If с else, ложное условие: `bi-do (no) oca! ... stopa uh-oh oca! ... stopa` → пропуск then, выполнение else
- [ ] Вложенные if: `bi-do (x con 5) oca! bi-do (y con 10) oca! ... stopa stopa uh-oh oca! ... stopa` → проверка вложенности
- [ ] If с присваиванием в блоке: `bi-do (x la 10) oca! x lumai x melomo 2 naidu! stopa` → изменение переменной
- [ ] If с вводом/выводом: `bi-do (x con 0) oca! guoleila (y) naidu! tulalilloo ti amo (y) naidu! stopa` → побочные эффекты

### Циклы
- [x] While цикл: `kemari` с выводом в цикле → `[0, 1, 2]`
- [ ] While с условием, которое становится ложным: счетчик от 0 до 5 → `[0, 1, 2, 3, 4, 5]`
- [ ] While с ложным условием с начала: `kemari (no) oca! ... stopa` → блок не выполняется
- [ ] While с изменением переменной в теле: `x lumai 0 naidu! kemari (x la 3) oca! x lumai x melomo 1 naidu! stopa` → изменение условия
- [ ] While с вложенными блоками: `kemari (x la 2) oca! bi-do (y con 0) oca! ... stopa stopa` → вложенность
- [ ] While с вводом/выводом: `kemari (x con 0) oca! guoleila (x) naidu! tulalilloo ti amo (x) naidu! stopa` → побочные эффекты
- [ ] While с break через изменение условия: счетчик с условием выхода

### Функции
- [x] Определение и вызов функции: `boss add Papaya (a, b)` + вызов → `[8]`
- [ ] Функция без параметров: `boss pi Papaya () oca! tank yu 3.14 naidu! stopa` + вызов → `[3.14]`
- [ ] Функция с одним параметром: `boss square Papaya (x) oca! tank yu x dibotada x naidu! stopa` + вызов → `[25]` (при `5`)
- [ ] Функция с несколькими параметрами: `boss max Papaya (a, b, c) oca! ... stopa` → проверка всех параметров
- [ ] Вложенные вызовы функций: `boss add Papaya (a, b) ... stopa` + `add(add(1, 2), add(3, 4))` → `[10]`
- [ ] Функция с выражениями в return: `boss calc Papaya (x) oca! tank yu x melomo 2 flavuk 1 naidu! stopa` → сложное выражение
- [ ] Функция с побочными эффектами: `boss readAndDouble Papaya () oca! guoleila (x) naidu! tank yu x dibotada 2 naidu! stopa` → ввод в функции
- [ ] Функция с локальными переменными: `boss test Papaya (x) oca! poop y Papaya naidu! y lumai x melomo 2 naidu! tank yu y naidu! stopa` → область видимости
- [ ] Функция с условными операторами: `boss abs Papaya (x) oca! bi-do (x la 0) oca! tank yu flavuk x naidu! stopa uh-oh oca! tank yu x naidu! stopa stopa` → if в функции
- [ ] Функция с циклом: `boss sumToN Papaya (n) oca! poop sum Papaya naidu! poop i Papaya naidu! i lumai 1 naidu! kemari (i la con n) oca! sum lumai sum melomo i naidu! i lumai i melomo 1 naidu! stopa tank yu sum naidu! stopa` → while в функции

### Вложенные блоки
- [x] Вложенные блоки с областями видимости: вложенные `bi-do` → `[5]`

### Комбинации конструкций
- [ ] Функция с if и циклом: `boss factorial Papaya (n) oca! ... kemari ... bi-do ... stopa` → комбинация всех конструкций
- [ ] If с вызовом функции в условии: `bi-do (isPositive(x)) oca! ... stopa` → функция в условии
- [ ] While с вызовом функции в условии: `kemari (notFinished()) oca! ... stopa` → функция в условии цикла
- [ ] Функция, вызывающая другую функцию: `boss add Papaya (a, b) ... stopa` + `boss multiply Papaya (x, y) ... stopa` + `multiply(add(1, 2), add(3, 4))` → цепочка вызовов
- [ ] If с циклом внутри: `bi-do (x con 1) oca! kemari (i la 5) oca! ... stopa stopa` → цикл в условии
- [ ] Цикл с if внутри: `kemari (x la 10) oca! bi-do (x pado 2 con 0) oca! ... stopa stopa` → условие в цикле
- [ ] Функция с локальными переменными и вложенными блоками: проверка области видимости в сложных случаях

---

## Ввод/вывод через FakeEnvironment (ExecutesIoWithFakeEnvironment)

- [x] Ввод одного числа и вывод: ввод `5` → вывод `[6]`
- [x] Ввод двух чисел и вывод суммы: ввод `[10, 20]` → вывод `[30]`
- [x] Ввод, вычисление и вывод: ввод `5` → `x lumai x dibotada x` → вывод `[25]`

---

## Обработка ошибок (ParseErrorTest)

### Ошибки выражений
- [x] Незакрытая скобка: `(2 melomo 3`
- [x] Лишняя закрывающая скобка: `2 melomo 3)`
- [x] Неправильный порядок скобок: `)2 melomo 3(`
- [x] Отсутствие операнда: `2 melomo`
- [x] Пустые скобки: `()`
- [x] Незакрытый вызов функции: `muak(5`
- [x] Неправильный вызов функции: `muak 5)`
- [x] Пустые аргументы: `muak(,)`
- [x] Деление на ноль: `10 poopaye 0`
- [x] Остаток от деления на ноль: `10 pado 0`
- [x] Неизвестная переменная: `x`
- [x] Неизвестная переменная в выражении: `x melomo 5`
- [x] Неподдержанный строковый литерал: `!Hello!`

### Ошибки программ
- [x] Отсутствие `bello!`: программа без старта
- [x] Пропущенный `naidu!` в объявлении переменной: `poop x Papaya`
- [x] Неконстантное значение в `trusela`: `trusela pi Papaya x naidu!`
- [x] `tank yu` вне функции: `tank yu 1 naidu!`
- [x] Необъявленная переменная в функции: использование `x` без объявления
- [x] Пропущенный `naidu!` в блоке: `x lumai 1` без `naidu!`
- [x] Пропущенный `naidu!` в объявлении: `poop x Papaya` без `naidu!`
- [x] Пропущенный `naidu!` в присваивании: `x lumai 5` без `naidu!`
- [x] Пропущенный `naidu!` в выводе: `tulalilloo ti amo (x)` без `naidu!`
- [x] Пропущенный `naidu!` в цикле: `x lumai x melomo 2` без `naidu!`
- [x] Пропущенный `naidu!` в объявлении внутри функции: `poop x Papaya` без `naidu!`
- [x] Пропущенный `naidu!` в `tank yu`: `tank yu 1` без `naidu!`
- [ ] Незакрытый блок if: `bi-do (da) oca! ...` без `stopa`
- [ ] Незакрытый блок else: `bi-do (no) oca! ... stopa uh-oh oca! ...` без `stopa`
- [ ] Незакрытый блок while: `kemari (da) oca! ...` без `stopa`
- [ ] Незакрытый блок функции: `boss test Papaya () oca! ...` без `stopa`
- [ ] Отсутствие `tank yu` в функции: `boss test Papaya () oca! x lumai 1 naidu! stopa` → ошибка
- [ ] Неправильное количество аргументов при вызове функции: `add(1)` вместо `add(1, 2)`
- [ ] Вызов несуществующей функции: `unknown(5) naidu!`
- [ ] Повторное объявление функции с тем же именем: два `boss test ...`
- [ ] Использование переменной до объявления в блоке: `x lumai 1 naidu! poop x Papaya naidu!`
- [ ] Неправильный синтаксис параметров функции: `boss test Papaya (a, , b) oca! ... stopa`

---

## Примечания

- Все тесты используют `[Theory]` и `[MemberData]` для параметризации
- Тесты выражений проверяют результат через `EvaluateExpression()` (возвращает `int`)
- Тесты программ проверяют результаты через `FakeEnvironment.Results` (список `decimal`)
- Тесты ошибок проверяют выброс исключений `InvalidOperationException` или других соответствующих исключений
