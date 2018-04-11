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
        //2x/2 = 4
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
            var result = Regex.Matches(equationTextBox.Text, @"\d+(?:[,.]\d+)*(?:e[-+]?\d+)?|[-^+/*()=]|\w+", RegexOptions.IgnoreCase)
             .Cast<Match>()
             .Select(p => p.Value)
             .ToList();

            for (int i = 0; i < result.Count; i++)  // normalization, x => 1x;
            {
                if (result[i] == "x" && i > 0 && !(result[i - 1].All(char.IsDigit)))
                {
                    result.Insert(i, "1");
                }

            }
            for (int i = 0; i < result.Count; i++)// normalization for * sign: 2x => 2*x; )( => )*(; etc...
            {
                if (i+1 < result.Count && 
                    (result[i] == ")" && result[i + 1].All(char.IsDigit) ||
                    result[i].All(char.IsDigit)  && result[i+1] == "(" ||
                    result[i] == ")" && result[i + 1] == "(" || 
                    result[i].All(char.IsDigit) && result[i+1] == "x"))
                {
                    result.Insert(i+1, "*"); 
                    i++;
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

               
            calculate(ref leftSide, ref listOfXFromLeftSide);
            calculate(ref rightSide, ref listOfXFromRightSide);
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
        static void calculate(ref List<string> inputList, ref List<string> listForX)
        {
            multiplyNormalization(inputList);
            bracketDivider(inputList);
            bracketMultiplyer(inputList);
            for (int i = 0; i < inputList.Count; i++)
            {
                if (inputList[i] == "x")
                {

               
                    if (i == 1)
                    {
                        listForX.Add(inputList[i - 1]);
                       
                    }
                

                        if (i > 1)
                        {
                            if (inputList[i - 2] == "-" || inputList[i - 2] == "+")
                            {
                                listForX.Add(inputList[i - 2]); 
                            }
                        listForX.Add(inputList[i - 1]);
                        }
                        



                    
                }
            }
            deleteFromInputList(inputList);

        }



        private static void multiplyNormalization(List<string> inputList)
        {
            for (int i = 0; i < inputList.Count; i++)
            {
                if (inputList[i] == "*" && i > 0)
                {
                    if (inputList[i - 1].All(char.IsDigit) && inputList[i + 1].All(char.IsDigit))
                    {
                        inputList[i - 1] = (double.Parse(inputList[i - 1]) * double.Parse(inputList[i + 1])).ToString();
                        inputList.RemoveAt(i);
                        inputList.RemoveAt(i);
                        i--;
                    }
                    if (inputList[i - 1].All(char.IsDigit) && inputList[i + 1] == "x")
                    {
                        inputList.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private static void deleteFromInputList(List<string> inputList)
        {
            for (int i = inputList.Count - 1; i >= 0; i--)
            {
                if (inputList[i] == "x")
                {             
                    if (i == 1)
                    {
                        inputList.RemoveAt(i);
                        --i;
                        inputList.RemoveAt(i);
                        continue;

                    }
                   
                        if (i > 1)
                        {
                            if (inputList[i - 2] == "-" || inputList[i - 2] == "+")
                            {
                                inputList.RemoveAt(i - 2);
                                i--;
                            }
                        }
                    inputList.RemoveAt(i);
                    i--;
                    inputList.RemoveAt(i);                     
                        
                    
                }
            }
        }

        static void checkIfListStartsWithPlus(ref List<string> inputList)
        {
            if (inputList.Any())
            {
                for (int i = 0; i < inputList.Count; i++)
                {
                    if (i > 1)
                    {
                        if (inputList[i].All(char.IsDigit) && inputList[i - 1] == "+" && !inputList[i - 2].All(char.IsDigit))
                        {
                            inputList.RemoveAt(i - 1);
                        }
                    }
                    if(i==0)
                    {
                        if(inputList[i] == "+")
                        {
                            inputList.RemoveAt(i);
                        }
                    }
                }

            }
        }

        static void bracketMultiplyer(List<string> inputList)
        {
            double placeholderRight = 1;
            double multiplyerRight = 1;

            double placeholderLeft = 1;
            double multiplyerLeft = 1;
            for (int i = 0; i < inputList.Count; i++) // looking for multiplyer value after ()
            {
                if (inputList[i] == ")")
                {

                    if (i + 1 < inputList.Count)
                    {
                        if (double.TryParse(inputList[i + 1], out placeholderRight))
                        {
                            multiplyerRight = placeholderRight;
                            inputList.RemoveAt(i + 1);
                        }
                    }
                    if (i + 2 < inputList.Count && inputList[i + 1] == "*")
                    {
                        if (double.TryParse(inputList[i + 2], out placeholderRight))
                        {
                            multiplyerRight = placeholderRight;
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
                        if (double.TryParse(inputList[i - 1], out placeholderLeft))
                        {
                            multiplyerLeft = placeholderLeft;
                            inputList.RemoveAt(i - 1);
                        }
                    }
                    if (i - 2 >= 0 && inputList[i - 1] == "*")
                    {
                        if (double.TryParse(inputList[i - 2], out placeholderLeft))
                        {
                            multiplyerLeft = placeholderLeft;
                            inputList.RemoveAt(i - 1);
                            inputList.RemoveAt(i - 2);
                        }
                    }
                }
            }
            
                multiyplyingExpressions(inputList, multiplyerLeft, multiplyerRight);
        }

        private static void multiyplyingExpressions(List<string> inputList, double multiplyerLeft, double multiplyerRight)
        {
            for (int i = 0; i < inputList.Count; i++)
            {
                if (inputList[i] == "(")
                {
                    while (inputList[i] != ")")
                    {
                        if (inputList[i].All(char.IsDigit))
                        {
                            inputList[i] = (double.Parse(inputList[i]) * multiplyerLeft * multiplyerRight).ToString();
                           
                        }
                        i++;
                    }
                }
            }
        }
        private static void bracketDivider(List<string> inputList)
        {
            //(2x+2)/2=4
            for (int i = 0; i < inputList.Count; i++)
            {
                if (inputList[i] == "x" && inputList[i + 1] == "/")
                {
                    inputList[i - 1] = (double.Parse(inputList[i - 1]) / double.Parse(inputList[i + 2])).ToString();
                    inputList.RemoveAt(i + 1);
                    inputList.RemoveAt(i + 1);
                }
            }
            double placeholderRight = 1;
            double dividerRight = 1;

            double placeholderLeft = 1;
            double dividerLeft = 1;
            for (int i = 0; i < inputList.Count; i++) // looking for multiplyer value after ()
            {
                if (inputList[i] == ")")
                {

                    if (i + 1 < inputList.Count)
                    {
                        if (double.TryParse(inputList[i + 1], out placeholderRight))
                        {
                            dividerRight = placeholderRight;
                            inputList.RemoveAt(i + 1);
                        }
                    }
                    if (i + 2 < inputList.Count && inputList[i + 1] == "/")
                    {
                        if (double.TryParse(inputList[i + 2], out placeholderRight))
                        {
                            dividerRight = placeholderRight;
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
                        if (double.TryParse(inputList[i - 1], out placeholderLeft))
                        {
                            dividerLeft = placeholderLeft;
                            inputList.RemoveAt(i - 1);
                        }
                    }
                    if (i - 2 >= 0 && inputList[i - 1] == "/")
                    {
                        if (double.TryParse(inputList[i - 2], out placeholderLeft))
                        {
                            dividerLeft = placeholderLeft;
                            inputList.RemoveAt(i - 1);
                            inputList.RemoveAt(i - 2);
                        }
                    }
                }
            }

            dividingExpressions(inputList, dividerLeft, dividerRight);
        }
        private static void dividingExpressions(List<string> inputList, double dividerLeft, double dividerRight)
        {
            for (int i = 0; i < inputList.Count; i++)
            {
                if (inputList[i] == "(")
                {
                    while (inputList[i] != ")")
                    {
                        if (inputList[i].All(char.IsDigit))
                        {
                            inputList[i] = (double.Parse(inputList[i]) / dividerLeft / dividerRight).ToString();

                        }
                        i++;
                    }
                }
            }
            
        }

    }
}
