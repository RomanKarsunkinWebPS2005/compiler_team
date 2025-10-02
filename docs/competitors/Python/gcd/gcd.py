#GCD — читает два целых числа, печатает наибольший общий делитель (НОД, GCD) этих чисел
#использует алгоритм Евклида для поиска НОД

def gcd(a, b):
    a, b = abs(a), abs(b)
    if a < b:
        a, b = b, a
    while a % b != 0:
        a = a % b
        a, b = b, a
    return b

num1 = int(input("Enter first number: "))
num2 = int(input("Enter second number: "))
result = gcd(num1, num2)

print(f"GCD({num1}, {num2}) = {result}")

