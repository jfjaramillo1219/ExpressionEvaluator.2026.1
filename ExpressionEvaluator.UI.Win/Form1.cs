using System;
using System.Windows.Forms;
using ExpressionEvaluator.Core; // Connecting the brain!

namespace ExpressionEvaluator.UI.Win
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Universal method for numbers and operators (0-9, +, -, *, /, ^, (, ), .)
        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            // If there is an error message, clear it before typing
            if (txtDisplay.Text == "Syntax Error")
            {
                txtDisplay.Text = "";
            }

            // Avoid typing after a result is calculated (must clear or delete first)
            if (txtDisplay.Text.Contains("="))
            {
                txtDisplay.Text = "";
            }

            txtDisplay.Text += clickedButton.Text;
        }

        // Method for the "Clear" button
        private void ButtonClear_Click(object sender, EventArgs e)
        {
            txtDisplay.Text = "";
        }

        // Method for the "Delete" button
        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (txtDisplay.Text.Length > 0)
            {
                // Remove the last character
                txtDisplay.Text = txtDisplay.Text.Substring(0, txtDisplay.Text.Length - 1);
            }
        }

        // Method for the "=" button
        private void ButtonEqual_Click(object sender, EventArgs e)
        {
            try
            {
                string expression = txtDisplay.Text;

                // If it's empty or already calculated, do nothing
                if (string.IsNullOrEmpty(expression) || expression.Contains("=")) return;

                // Call the Backend (Our Queue logic)
                double result = Evaluator.Evaluate(expression);

                // Show the output exactly as requested in the PDF (e.g. 144^(1/2)=12)
                txtDisplay.Text = $"{expression}={result}";
            }
            catch (Exception)
            {
                // Catch any mathematical or syntax error to prevent a crash
                txtDisplay.Text = "Syntax Error";
            }
        }
    }
}