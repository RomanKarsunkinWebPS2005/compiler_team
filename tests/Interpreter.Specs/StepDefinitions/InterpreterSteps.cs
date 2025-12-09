#pragma warning disable SA1309 // Field names should not begin with underscore
using System.Text;
using Execution;
using Interpreter;
using Parser;
using Reqnroll;
using Xunit;

namespace Interpreter.Specs.StepDefinitions;

[Binding]
public class InterpreterSteps
{
    private string? _programCode;
    private string? _inputText;
    private string? _expectedOutput;
    private string? _actualOutput;
    private StringWriter? _outputWriter;
    private StringReader? _inputReader;
    private TextWriter? _originalOut;

    [Given(@"я написал программу:")]
    public void GivenIHaveWrittenProgram(string multilineText)
    {
        _programCode = multilineText.Trim();
    }

    [When(@"я ввожу в консоли:")]
    public void WhenIEnterInConsole(string multilineText)
    {
        _inputText = multilineText.Trim();

        // Настраиваем ввод
        _inputReader = new StringReader(_inputText);

        // Настраиваем вывод
        _outputWriter = new StringWriter();
        _originalOut = Console.Out;
        Console.SetOut(_outputWriter);

        // Создаем окружение с переопределенным вводом
        TestEnvironment environment = new TestEnvironment(_inputReader, _outputWriter);
        Interpreter interpreter = new Interpreter(environment);

        try
        {
            interpreter.Execute(_programCode!);
        }
        catch (Exception ex)
        {
            _outputWriter.WriteLine($"Ошибка: {ex.Message}");

            // Выводим полный стек вызовов для отладки
            if (ex.StackTrace != null)
            {
                _outputWriter.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }
        finally
        {
            // Восстанавливаем стандартный вывод
            Console.SetOut(_originalOut!);
            _actualOutput = _outputWriter.ToString().Trim();
        }
    }

    [Then(@"я увижу в консоли:")]
    public void ThenIShouldSeeInConsole(string multilineText)
    {
        _expectedOutput = multilineText.Trim();

        // Нормализуем ожидаемый и фактический вывод (убираем лишние пробелы и переводы строк)
        string normalizedExpected = NormalizeOutput(_expectedOutput);
        string normalizedActual = NormalizeOutput(_actualOutput ?? string.Empty);

        Assert.Equal(normalizedExpected, normalizedActual);
    }

    private static string NormalizeOutput(string output)
    {
        // Убираем лишние пробелы и нормализуем переводы строк
        return string.Join(" ", output
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line)));
    }

    private class TestEnvironment : IEnvironment
    {
        private readonly TextReader _input;
        private readonly TextWriter _output;

        public TestEnvironment(TextReader input, TextWriter output)
        {
            _input = input;
            _output = output;
        }

        public decimal ReadNumber()
        {
            string? input = _input.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                throw new InvalidOperationException("Input is empty");
            }

            if (!decimal.TryParse(
                input,
                System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal number))
            {
                throw new FormatException($"Invalid number format: '{input}'");
            }

            return number;
        }

        public void WriteNumber(decimal result)
        {
            _output.WriteLine(result.ToString("0.#####", System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}