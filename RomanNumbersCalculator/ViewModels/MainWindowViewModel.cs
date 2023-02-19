using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;

using RomanNumbersCalculator.Models;
using System.Security.Cryptography;

namespace RomanNumbersCalculator.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string currentOperationStringRepresentation = "";
        private string currentNumberStringRepresentation = "";
        private Stack<RomanNumberExtend> stackRomanNumbers = new Stack<RomanNumberExtend>();
        public string CurrentNumberStringRepresentation{
            get => currentNumberStringRepresentation;
            set{
                this.RaiseAndSetIfChanged(ref currentNumberStringRepresentation, value);
            }
        }
        public ReactiveCommand<string, Unit> AddNumber { get; }
        public ReactiveCommand<Unit, Unit> PlusCommand { get; }
        public ReactiveCommand<Unit, Unit> SubCommand { get; }
        public ReactiveCommand<Unit, Unit> MulCommand { get; }
        public ReactiveCommand<Unit, Unit> DivCommand { get; }
        public ReactiveCommand<Unit, Unit> CalculateCommand { get; }
        public ReactiveCommand<Unit, Unit> ResetCommand { get; }

        public MainWindowViewModel()
        {
            AddNumber = ReactiveCommand.Create<string>(str =>
            {
                if (hasRepresentationError("#ERROR")) return;
                if (currentOperationStringRepresentation == "=") reset();
                CurrentNumberStringRepresentation += str;
            });

            ResetCommand = ReactiveCommand.Create(() => reset());

            PlusCommand = ReactiveCommand.Create(() => calculateNumberByOperationType("+"));
            SubCommand = ReactiveCommand.Create(() => calculateNumberByOperationType("-"));
            MulCommand = ReactiveCommand.Create(() => calculateNumberByOperationType("*"));
            DivCommand = ReactiveCommand.Create(() => calculateNumberByOperationType("/"));

            CalculateCommand = ReactiveCommand.Create(() =>
            {
                if (stackRomanNumbers.Count != 1 || currentNumberStringRepresentation == "" || hasRepresentationError("#ERROR")) return;

                try
                {
                    RomanNumberExtend newNumber = new RomanNumberExtend(currentNumberStringRepresentation);

                    recalculateCurrentNumber(newNumber);
                    currentOperationStringRepresentation = "=";
                    CurrentNumberStringRepresentation = stackRomanNumbers.Peek().ToString();
                }
                catch (RomanNumberException exception)
                {
                    setRepresentationError(exception.Message);
                }
            });
        }

        private bool hasRepresentationError(string errorMessage) => currentNumberStringRepresentation == errorMessage;
        private void setRepresentationError(string errorMessage) => CurrentNumberStringRepresentation = errorMessage;

        private void reset()
        {
            CurrentNumberStringRepresentation = "";
            currentOperationStringRepresentation = "";
            stackRomanNumbers.Clear();
        }

        private void recalculateCurrentNumber(RomanNumberExtend number)
        {
            switch (currentOperationStringRepresentation)
            {
                case "+":
                    stackRomanNumbers.Push(stackRomanNumbers.Pop() + number);
                    break;
                case "-":
                    stackRomanNumbers.Push(stackRomanNumbers.Pop() - number);
                    break;
                case "*":
                    stackRomanNumbers.Push(stackRomanNumbers.Pop() * number);
                    break;
                case "/":
                    stackRomanNumbers.Push(stackRomanNumbers.Pop() / number);
                    break;
            }
        }

        private bool isCalculationRequired(string operationSymbol)
        {
            if (hasRepresentationError("#ERROR")) return false;

            if (currentNumberStringRepresentation == "" && currentOperationStringRepresentation != "")
            {
                currentOperationStringRepresentation = operationSymbol;
                return false;
            }

            if (currentNumberStringRepresentation == "") return false;

            if (currentOperationStringRepresentation == "=")
            {
                currentOperationStringRepresentation = operationSymbol;
                CurrentNumberStringRepresentation = "";
                return false;
            }

            return true;
        }

        private void calculateNumberByOperationType(string operationSymbol)
        {
            if (!isCalculationRequired(operationSymbol)) return;

            try
            {
                if (currentOperationStringRepresentation == "")
                {
                    currentOperationStringRepresentation = operationSymbol;
                    RomanNumberExtend newNumber = new RomanNumberExtend(currentNumberStringRepresentation);
                    stackRomanNumbers.Push(newNumber);
                    CurrentNumberStringRepresentation = "";
                }
                else
                {
                    RomanNumberExtend newNumber = new RomanNumberExtend(currentNumberStringRepresentation);
                    recalculateCurrentNumber(newNumber);
                    currentOperationStringRepresentation = operationSymbol;
                    CurrentNumberStringRepresentation = "";

                }
            }
            catch (RomanNumberException exception)
            {
                setRepresentationError(exception.Message);
            }
        }
    }
}
