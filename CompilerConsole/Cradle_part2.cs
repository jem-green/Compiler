using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CompilerConsole
{
    class Cradle_part2
    {
        //--------------------------------------------------------------
        // Constant Declarations

        char TAB = '\t';

        //--------------------------------------------------------------
        // Variable Declarations

        char Look;                              // Lookahead Character

        //--------------------------------------------------------------
        //Read New Character From Input Stream 

        void GetChar()
        {
            ConsoleKeyInfo k;
            k = Console.ReadKey();
            Look = k.KeyChar;
        }

        //--------------------------------------------------------------
        // Report an Error 

        void Error(string s)
        {
           Console.WriteLine();
           Console.WriteLine("\n" + "Error: " + s + ".");
        }

        //--------------------------------------------------------------
        // Report Error and Halt 

        void Abort(string s)
        {
           Error(s);
           throw new System.InvalidOperationException(s);
        }


        //--------------------------------------------------------------
        // Report What Was Expected 

        void Expected(string s)
        {
           Abort(s + " Expected");
        }

        //--------------------------------------------------------------
        // Match a Specific Input Character 

        void Match(char x)
        {
            if (Look != x)
            {
                Expected("\"" + x + "\"");
            }
            else
            {
                GetChar();
            }
        }

        //--------------------------------------------------------------
        // Recognize an Alpha Character 

        Boolean IsAlpha(char c)
        {
            return(Char.IsLetter(c));
        }      

        //--------------------------------------------------------------
        // Recognize a Decimal Digit 

        Boolean IsDigit(char c)
        {
           return(Char.IsNumber(c));
        }

        //--------------------------------------------------------------
        // Recognize an Addop

        Boolean IsAddop(char c)
        {
            if ((c == '+') || (c == '-'))
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }

        //--------------------------------------------------------------
        // Get an Identifier 

        char GetName()
        {
            char c;

            if (!IsAlpha(Look))
            {
                Expected("Name");
            }
            c = Char.ToUpper(Look);
            GetChar();
            return (c);
        }

        //--------------------------------------------------------------
        // Get a Number 

        char GetNum()
        {
            char c;
            if (!IsDigit(Look))
            {
               Expected("Integer");
            }
            c =Look;
            GetChar();
            return(c);
        }

        //--------------------------------------------------------------
        // Output a String with Tab 

        void Emit(string s)
        {
           Console.Write(TAB + s);
        }

        //--------------------------------------------------------------
        // Output a String with Tab and CRLF 

        void EmitLn(string s)
        {
           Emit(s);
           Console.WriteLine();
        }

        //--------------------------------------------------------------
        // Initialize 

        void Init()
        {
            GetChar();
        }

        //---------------------------------------------------------------
        // Parse and Translate a Math Factor

        void Factor()
        {
            if (Look == '(')
            {
                Match('(');
                Expression();
                Match(')');
            }
            else
            {
                EmitLn("MOVE #" + GetNum() + ",D0");
            }
        }

        //--------------------------------------------------------------}
        // Recognize and Translate a Multiply }

        void Multiply()
        {
            Match('*');
            Factor();
            EmitLn("MULS (SP)+,D0");
        }

        //-------------------------------------------------------------}
        // Recognize and Translate a Divide }

        void Divide()
        {
            Match('/');
            Factor();
            EmitLn("MOVE (SP)+,D1");
            EmitLn("DIVS D1,D0");
        }

        //---------------------------------------------------------------}
        // Parse and Translate a Math Term

        void Term()
        {
            Factor();
            while ((Look == '*') || (Look == '/'))
            {
                EmitLn("MOVE D0,-(SP)");
                switch (Look)
                {
                    case '*':
                        {
                            Multiply();
                            break;
                        }
                    case '/':
                        {
                            Divide();
                            break;
                        }
                    default:
                        {
                            Expected("Mulop");
                            break;
                        }

                }
            }
        }

        //--------------------------------------------------------------}
        // Recognize and Translate an Add

        void Add()
        {
            Match('+');
            Term();
            EmitLn("ADD (SP)+,D0");
        }

        //-------------------------------------------------------------}
        // Recognize and Translate a Subtract

        void Subtract()
        {
            Match('-');
            Term();
            EmitLn("SUB (SP)+,D0");
            EmitLn("NEG D0");
        }

        //---------------------------------------------------------------}
        // Parse and Translate a Math Expression

        void Expression()
        {
            if (IsAddop(Look))
            {
                EmitLn("CLR D0");
            }
            else
            {
                Term();
            }
            while ((Look == '+') || (Look == '-'))
            {
                EmitLn("MOVE D0,-(SP)"); 
                switch (Look)
                {
                    case '+':
                        {
                            Add();
                            break;
                        }
                    case '-':
                        {
                            Subtract();
                            break;
                        }
                    default:
                        {
                            Expected("Addop");
                            break;
                        }
                }
            }
        }



        //--------------------------------------------------------------}
        // Main Program

        public void main()
        {
            Init();
            Expression();
        }

        //--------------------------------------------------------------}
 
        //*****************************************************************
        //*                                                               *
        //*                      COPYRIGHT NOTICE                         *
        //*                                                               *
        //*   Copyright (C) 1988 Jack W. Crenshaw. All rights reserved.   *
        //*                                                               *
        //*****************************************************************

    }
}