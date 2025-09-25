//SumNumbers — читает последовательность действительных чисел и печатает сумму чисел

#include <iostream>
#include <sstream>
#include <string>
using namespace std;

int main() 
{
    double sum = 0.0;
    double num;
    int count = 0;
    string input;

    cout << "Enter numbers in one line separated by spaces:" << endl;

    getline(cin, input);

    stringstream ss(input);

    while (ss >> num) 
    {
        sum += num;
        count++;
    }

    if (count == 0) 
    {
        cout << "Numbers were not entered." << endl;
    }
    else 
    {
        cout << "Sum of " << count << " numbers: " << sum << endl;
    }

    return 0;
}