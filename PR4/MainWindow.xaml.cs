using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PR4
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Блокировка ввода нецифровых символов с клавиатуры
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true;
                    break;
                }
            }
        }

        // Блокировка вставки нецифровых символов (Ctrl+V, ПКМ → Вставить)
        private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = e.DataObject.GetData(typeof(string)) as string;
                if (text == null)
                {
                    e.CancelCommand();
                    return;
                }

                // Проверка: все символы — цифры?
                bool allDigits = true;
                foreach (char c in text)
                {
                    if (!char.IsDigit(c))
                    {
                        allDigits = false;
                        break;
                    }
                }

                if (!allDigits)
                {
                    e.CancelCommand();
                    return;
                }

                // Проверка: не превысит ли вставка MaxLength?
                TextBox textBox = sender as TextBox;
                if (textBox != null)
                {
                    string currentText = textBox.Text;
                    int selectionStart = textBox.SelectionStart;
                    string newText = currentText.Insert(selectionStart, text);

                    if (newText.Length > textBox.MaxLength)
                    {
                        e.CancelCommand();
                        return;
                    }
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        // Обработчик кнопки "Рассчитать"
        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            ClearErrors();

            bool isValid = true;
            int total = 0;

            isValid &= ValidateInput(TxtMod1, ErrMod1, 0, 10, ref total);
            isValid &= ValidateInput(TxtMod2, ErrMod2, 0, 15, ref total);
            isValid &= ValidateInput(TxtMod3, ErrMod3, 0, 25, ref total);
            isValid &= ValidateInput(TxtMod4, ErrMod4, 0, 25, ref total);
            isValid &= ValidateInput(TxtMod5, ErrMod5, 0, 25, ref total);

            if (isValid)
            {
                string grade = GetGrade(total);
                ResultSummary.Text = "Общая сумма: " + total + " из 100";
                ResultGrade.Text = "Оценка: " + grade;
            }
            else
            {
                ResultSummary.Text = string.Empty;
                ResultGrade.Text = string.Empty;
            }
        }

        // Валидация одного поля
        private bool ValidateInput(TextBox textBox, TextBlock errorBlock, int min, int max, ref int total)
        {
            string input = textBox.Text.Trim();

            if (string.IsNullOrEmpty(input))
            {
                errorBlock.Text = "Поле обязательно для заполнения";
                return false;
            }

            int value;
            if (!int.TryParse(input, out value))
            {
                errorBlock.Text = "Допустимы только целые числа";
                return false;
            }

            if (value < min || value > max)
            {
                errorBlock.Text = "Значение должно быть от " + min + " до " + max;
                return false;
            }

            total += value;
            errorBlock.Text = string.Empty;
            return true;
        }

        // Очистка всех ошибок
        private void ClearErrors()
        {
            ErrMod1.Text = string.Empty;
            ErrMod2.Text = string.Empty;
            ErrMod3.Text = string.Empty;
            ErrMod4.Text = string.Empty;
            ErrMod5.Text = string.Empty;
        }

        // Определение оценки (без switch expression)
        private string GetGrade(int total)
        {
            if (total >= 90)
                return "5 (отлично)";
            else if (total >= 75)
                return "4 (хорошо)";
            else if (total >= 50)
                return "3 (удовлетворительно)";
            else
                return "2 (неудовлетворительно)";
        }
    }
}