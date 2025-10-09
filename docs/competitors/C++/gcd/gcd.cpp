//GCD — читает два целых числа, печатает наибольший общий делитель (НОД, GCD) этих чисел
//использует алгоритм Евклида для поиска НОД

#include <iostream>
using namespace std;

int gcd(int a, int b) 
{
    if (a < b) 
    {
        swap(a, b);
    }
    while (a % b != 0) 
    {
        a = a % b;
        swap(a, b);
    }
    return b;
}

int main() 
{
    int num1, num2;

    cout << "Enter first number: ";
    cin >> num1;

    cout << "Enter second number: ";
    cin >> num2;

    int result = gcd(abs(num1), abs(num2));

    cout << "GCD(" << num1 << "," << num2 << ") = " << result << endl;

    return 0;
}