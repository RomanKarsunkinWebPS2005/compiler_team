#SumNumbers — читает последовательность действительных чисел и печатает сумму чисел

input_str = input("Enter numbers in one line separated by spaces: ")
    
numbers = []
parts = input_str.split()
    
for part in parts:
    try:
        num = float(part)
        numbers.append(num)
    except ValueError:
        break
    
if numbers:
    total = sum(numbers)
    print(f"Sum of {len(numbers)} numbers: {total}")
else:

    print("No valid numbers were entered.")

