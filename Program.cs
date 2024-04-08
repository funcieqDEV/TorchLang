using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

class Variable
{
    public string Name { get; set; }
    public string Value { get; set; }
    public int intvalue { get; set; }

    public Variable(string name, string value)
    {
        Name = name;
        Value = value;
        int intValue;
        if (int.TryParse(value, out intValue))
        {
            intvalue = intValue;
        }
    }
}

class Program
{
    static void Main(string[] args)
    {


      
        int ivalue = 0;
        bool condition = true;
        bool inif = true;
        List<Variable> variables = new List<Variable>();
        string path = @"script.tr";
        string[] lines = File.ReadAllLines(path);
        bool can = true;

        Dictionary<string, int> labels = new Dictionary<string, int>();

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (line.StartsWith(":") && inif == true)
            {
                string label = line.Substring(1);
                labels[label] = i;
            }
        }

        int currentLine = 0;

        while (currentLine < lines.Length)
        {
            string line = lines[currentLine];

            if (line.StartsWith("print ") && inif == true)
            {
                string modifiedLine = line.Substring(6);
                Print(modifiedLine, variables);
            }
            else if (line.StartsWith("var ") && inif == true)
            {
                string modifiedLine = line.Substring(4);
                string[] parts = modifiedLine.Split(' ');

                foreach (var old in variables)
                {
                    can = true;
                    if (parts[0] == old.Name)
                    {
                        old.Value = parts[2];
                        can = false;
                    }
                }

                if (can)
                {
                    if (parts[0] == "int")
                    {
                        variables.Add(new Variable(parts[1], parts[3]));
                    }
                    else
                    {
                        variables.Add(new Variable(parts[0], parts[2]));
                    }
                }

            }
            else if (line.StartsWith("goto ") && inif == true)
            {
                string label = line.Substring(5);
                if (labels.ContainsKey(label))
                {
                    currentLine = labels[label];
                }
                else
                {
                    Console.WriteLine($"Label '{label}' not found.");
                }
            }
            else if (line.StartsWith("sleep ") && inif == true)
            {
                string modifiedLine = line.Substring(6);
                Thread.Sleep(Convert.ToInt32(modifiedLine));
            }
            else if (line.StartsWith("input ") && inif == true)
            {
                string modifiedLine = line.Substring(6);
                foreach (var old in variables)
                {  
                    if (old.Name == modifiedLine)
                    {
                        if (old.intvalue != null)
                        {
                            old.intvalue = Convert.ToInt32(Console.ReadLine());
                        }
                        else
                        {
                            old.Value = Console.ReadLine();
                        }
                    }
                }
            }
            else if (line.StartsWith("if ") && inif == true)
            {
                string startvalue = "";
                int intstartvalue = 0;
                int intendvalue = 0;

                string endvalue = "";
                bool started = false;
                string modifiedLine = line.Substring(3);
                string[] parts = modifiedLine.Split(' ');

                for (int j = 0; j < variables.Count; j++)
                {
                    var old = variables[j];
                    if (old.intvalue != null)
                    {
                        if ("%" + old.Name + "%" == parts[0] && !started)
                        {
                            intstartvalue = old.intvalue;
                            started = true;
                        }
                        else if (started && "%" + old.Name + "%" == parts[2])
                        {
                            intendvalue = old.intvalue;
                            break;
                        }
                    }
                    else
                    {
                        if ("%" + old.Name + "%" == parts[0] && !started)
                        {
                            startvalue = old.Value;
                            started = true;
                        }
                        else if (started && "%" + old.Name + "%" == parts[2])
                        {
                            endvalue = old.Value;
                            break;
                        }
                    }
                }

                if (intstartvalue != 0 && intendvalue != 0)
                {
                    if (parts[1] == "==")
                    {
                        condition = (intstartvalue == intendvalue);
                    }
                    else if (parts[1] == "!=")
                    {
                        condition = (intstartvalue != intendvalue);
                    }
                    else if (parts[1] == ">=")
                    {
                        condition = (intstartvalue >= intendvalue);
                    }
                    else if (parts[1] == "<=")
                    {
                        condition = (intstartvalue <= intendvalue);
                    }
                    else if (parts[1] == ">")
                    {
                        condition = (intstartvalue > intendvalue);
                    }
                    else if (parts[1] == "<")
                    {
                        condition = (intstartvalue < intendvalue);
                    }
                }
                else if (startvalue != "" && endvalue != "")
                {
                    if (parts[1] == "==")
                    {
                        condition = (startvalue == endvalue);
                    }
                    else if (parts[1] == "!=")
                    {
                        condition = (startvalue != endvalue);
                    }
                    /*////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// else if (parts[1] == ">")
                     {
                         condition = (String.Compare(startvalue, endvalue) > 0);
                     }
                     else if (parts[1] == "<")
                     {
                         condition = (String.Compare(startvalue, endvalue) < 0);
                     }
                    */
                }

                inif = condition;
            }
            else if (line.StartsWith("end"))
            {
                inif = true;
            }
            else if (line.StartsWith("clear") && inif == true)
            {
                Console.Clear();
            }
            else if (line.StartsWith("set ") && inif == true)
            {
                Random rnd = new Random();
                string modifiedLine = line.Substring(4);
                string[] parts = modifiedLine.Split(' ');

                string variableName = parts[0];
                string operation = parts[1];
                int result = 0;

                Variable sourceVariable = variables.Find(v => v.Name == parts[2]);

                Variable targetVariable = variables.Find(v => v.Name == variableName);

                if (targetVariable != null)
                {
                    if (sourceVariable != null)
                    {
                        // Jeśli wartość jest inną zmienną, dodaj jej wartość do wartości zmiennej docelowej
                        switch (operation)
                        {
                            case "+=":
                                targetVariable.intvalue += sourceVariable.intvalue;
                                break;
                            case "-=":
                                targetVariable.intvalue -= sourceVariable.intvalue;
                                break;
                            case "*=":
                                targetVariable.intvalue *= sourceVariable.intvalue;
                                break;
                            case "/=":
                                if (sourceVariable.intvalue != 0)
                                    targetVariable.intvalue /= sourceVariable.intvalue;
                                else
                                    Console.WriteLine("Error: Division by zero.");
                                break;
                            default:
                                Console.WriteLine("Error: Invalid operation.");
                                break;
                        }
                    }
                    else
                    {

                        int operand;
                        if (int.TryParse(parts[2], out operand))
                        {
                            switch (operation)
                            {
                                case "=":
                                    targetVariable.intvalue = operand;
                                    break;
                                case "+=":
                                    targetVariable.intvalue += operand;
                                    break;
                                case "-=":
                                    targetVariable.intvalue -= operand;
                                    break;
                                case "*=":
                                    targetVariable.intvalue *= operand;
                                    break;
                                case "/=":
                                    if (operand != 0)
                                        targetVariable.intvalue /= operand;
                                    else
                                        Console.WriteLine("Error: Division by zero.");
                                    break;
                                case "random":
                                    if (parts.Length == 4)
                                    {
                                        int minValue, maxValue;
                                        if (int.TryParse(parts[2], out minValue) && int.TryParse(parts[3], out maxValue))
                                        {
                                            if (maxValue >= minValue)
                                            {
                                                targetVariable.intvalue = rnd.Next(minValue, maxValue + 1);
                                            }
                                            else
                                            {
                                                Console.WriteLine("Error: Min value cannot be greater than max Value.");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Error: Invalid arguments for random operation.");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error: Invalid number of arguments for random operation.");
                                    }
                                    break;
                                default:
                                    Console.WriteLine("Error: Invalid operation.");
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error: Invalid operand.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Error: Variable '{variableName}' not found.");
                }
            }

            else if (line.StartsWith("exit") && inif == true)
            {
                System.Environment.Exit(0);
            }

            currentLine++;
        }
    }

    public static void Print(string text, List<Variable> variables)
    {
        foreach (var var in variables)
        {
            if (var.intvalue != null)
            {
                text = text.Replace("%" + var.Name + "%", Convert.ToString(var.intvalue));
            }
            else
            {
                text = text.Replace("%" + var.Name + "%", var.Value);
            }
        }
        Console.WriteLine(text);
    }
}
