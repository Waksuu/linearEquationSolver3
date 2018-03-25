using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace linearEquationSolver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        //(2-x)*3 = 4
        List<string> leftSide = new List<string>();
        List<string> rightSide = new List<string>();
        List<string> listOfXFromLeftSide = new List<string>();
        List<string> listOfXFromRightSide = new List<string>();
        bool equationCheckMark = false;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {




            //char[] operationArray = { '+', '-', '/', '*' };
            //string[] numberArray = Regex.Split(equationTextBox.Text, @"(?<=[x=])");
            var result = Regex.Matches(equationTextBox.Text, @"\d+(?:[,.]\d+)*(?:e[-+]?\d+)?|[-^+*/()=]|\w+", RegexOptions.IgnoreCase)
             .Cast<Match>()
             .Select(p => p.Value)
             .ToList();

            for (int i = 0; i < result.Count; i++)
            {
                if (result[i] == "x" && i > 0 && (result[i - 1] == "+" || result[i - 1] == "-"))
                {
                    result.Insert(i, "1");
                }

            }

            foreach (var number in result) // separating left side of equation from right side
            {
                if (number.Contains("="))
                {
                    equationCheckMark = true;
                }
                if (equationCheckMark)
                {


                    rightSide.Add(number);

                }
                else
                {
                    leftSide.Add(number);
                }
            }

            rightSide.RemoveAt(0);

            bracketMultiplyer(ref leftSide);
            bracketMultiplyer(ref rightSide);
            separateUnknownFromNumbers(ref leftSide, ref listOfXFromLeftSide);
            separateUnknownFromNumbers(ref rightSide, ref listOfXFromRightSide);
            checkIfListStartsWithPlus(ref leftSide);
            checkIfListStartsWithPlus(ref rightSide);
            checkIfListStartsWithPlus(ref listOfXFromLeftSide);
            checkIfListStartsWithPlus(ref listOfXFromRightSide);


            string concatLeft = String.Join(null, leftSide);
            string concatLeftX = String.Join(null, listOfXFromLeftSide);
            string concatRight = String.Join(null, rightSide);
            string concatRightX = String.Join(null, listOfXFromRightSide);
            dynamic totalLeft = 0;
            dynamic totalLeftX = 0;
            dynamic totalRight = 0;
            dynamic totalRightX = 0;

            if (leftSide.Any())
            {
                totalLeft = new NCalc.Expression(concatLeft).Evaluate();
            }
            if (listOfXFromLeftSide.Any())
            {
                totalLeftX = new NCalc.Expression(concatLeftX).Evaluate();
            }
            if (rightSide.Any())
            {
                totalRight = new NCalc.Expression(concatRight).Evaluate();
            }
            if (listOfXFromRightSide.Any())
            {
                totalRightX = new NCalc.Expression(concatRightX).Evaluate();

            }

            var sumOfX = Convert.ToDouble(totalLeftX) - Convert.ToDouble(totalRightX);
            var sumOfNumbers = Convert.ToDouble(totalRight) - Convert.ToDouble(totalLeft);

            MessageBox.Show("X = " + (sumOfNumbers / sumOfX));
            leftSide.Clear();
            rightSide.Clear();
            listOfXFromLeftSide.Clear();
            listOfXFromRightSide.Clear();
            equationCheckMark = false;



        }
        static void separateUnknownFromNumbers(ref List<string> inputList, ref List<string> listForX)
        {

            for (int i = 0; i < inputList.Count; i++)
            {

                if (inputList[i] == "x")
                {

                    if (i == 0)
                    {
                        listForX.Add("1");

                    }
                    if (i == 1)
                    {
                        //if (inputList[i - 1] == "-")
                        //{
                        //    listForX.Add("-");
                        //    listForX.Add("1");
                        //}
                        //else
                        //{
                        listForX.Add(inputList[i - 1]);
                        //}
                    }
                    if (i > 1)
                    {
                        listForX.Add(inputList[i - 2]);
                        listForX.Add(inputList[i - 1]);

                    }
                }
            }
            for (int i = inputList.Count - 1; i >= 0; i--)
            {
                if (inputList[i] == "x")
                {
                    if (i == 0)
                    {
                        inputList.RemoveAt(i);
                    }
                    if (i == 1)
                    {
                        inputList.RemoveAt(i);
                        inputList.RemoveAt(i - 1);
                        --i;

                    }
                    if (i > 1)
                    {
                        inputList.RemoveAt(i);
                        inputList.RemoveAt(i - 1);
                        inputList.RemoveAt(i - 2);
                        --i;
                        --i;
                    }
                }
            }

        }
        static void checkIfListStartsWithPlus(ref List<string> inputList)
        {
            if (inputList.Any())
            {
                if (inputList[0] == "+")
                {
                    inputList.RemoveAt(0);
                }
            }
        }

        static void bracketMultiplyer(ref List<string> inputList)
        {
            double placeholder = 1;
            double multiplyer = 1;
            for (int i = 0; i < inputList.Count; i++) // looking for multiplyer value after ()
            {
                if (inputList[i] == ")")
                {

                    if (i + 1 < inputList.Count)
                    {
                        if (double.TryParse(inputList[i + 1], out placeholder))
                        {
                            multiplyer = placeholder;
                            inputList.RemoveAt(i + 1);
                        }
                    }
                    if (i + 2 < inputList.Count && inputList[i + 1] == "*")
                    {
                        if (double.TryParse(inputList[i + 2], out placeholder))
                        {
                            multiplyer = placeholder;
                            inputList.RemoveAt(i + 1);
                            inputList.RemoveAt(i + 1);
                        }
                    }
                }

            }
            for (int i = 0; i < inputList.Count; i++) // looking for multiplyer value before ()
            {
                if (inputList[i] == "(")
                {

                    if (i - 1 >= 0)
                    {
                        if (double.TryParse(inputList[i - 1], out placeholder))
                        {
                            multiplyer = placeholder;
                            inputList.RemoveAt(i - 1);
                        }
                    }
                    if (i - 2 >= 0 && inputList[i - 1] == "*")
                    {
                        if (double.TryParse(inputList[i - 2], out placeholder))
                        {
                            multiplyer = placeholder;
                            inputList.RemoveAt(i - 1);
                            inputList.RemoveAt(i - 2);
                        }
                    }
                }
            }
            
                multiyplyingExpressions(inputList, multiplyer);
        }

        private static void multiyplyingExpressions(List<string> inputList, double multiplyer)
        {
            for (int i = 0; i < inputList.Count; i++)
            {
                if (inputList[i] == "(")
                {
                    while (inputList[i + 2] != ")")
                    {
                        if (inputList[i + 1].All(char.IsDigit))
                        {
                            inputList[i + 1] = (double.Parse(inputList[i + 1]) * multiplyer).ToString();
                        }
                        i++;
                    }
                }
            }
        }
    }
}
