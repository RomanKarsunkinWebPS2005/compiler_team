//Reverse — читает строку и печатает её в перевёрнутом виде

#include <iostream>
#include <string>
using namespace std;

int main() 
{
    string input;

    cout << "Enter input string: ";
    getline(cin, input);

    string reversed = input;
    reverse(reversed.begin(), reversed.end());

    cout << "Input string: " << input << endl;
    cout << "Reversed string: " << reversed << endl;

    return 0;
}
