using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CompilerConsole
{
    class Cradle_part3
    {
        //--------------------------------------------------------------
        // Constant Declarations

        char TAB = '\t';
        char CR = '\r';

        //--------------------------------------------------------------
        // Variable Declarations

        char look;                              // Lookahead Character

        //--------------------------------------------------------------
        //Read New Character From Input Stream 

        void GetChar()
        {
            ConsoleKeyInfo k;
            k = Console.ReadKey();
            look = k.KeyChar;
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
            if (look != x)
            {
                Expected("\"" + x + "\"");
            }
            else
            {
                GetChar();
                SkipWhite();
            }
        }
 
        //--------------------------------------------------------------
        // Recognize an Alpha Character 

        Boolean IsAlpha(char c)
        {
            return (Char.IsLetter(c));
        }

        //--------------------------------------------------------------
        // Recognize a Decimal Digit 

        Boolean IsDigit(char c)
        {
            return (Char.IsNumber(c));
        }

        //--------------------------------------------------------------
        // Recognize an Alphanumeric }

        Boolean IsAlNum(char c)
        {
            return (IsAlpha(c) || IsDigit(c));
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
        // Recognize White Space

        Boolean IsWhite(char c)
        {
            if ((c == ' ') || (c == '\n'))
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }

        //--------------------------------------------------------------
        // Skip Over Leading White Space

        void SkipWhite()
        {
            while (IsWhite(look))
            {
                GetChar();
            }
        }
        //--------------------------------------------------------------
        // Get an Identifier 

        string GetName()
        {
            string Token = "";

            if (!IsAlpha(look))
            {
                Expected("Name");
            }
            while (IsAlNum(look))
            {
                Token = Token + Char.ToUpper(look);
                GetChar();
            }
            SkipWhite();
            return (Token);
        }

        //--------------------------------------------------------------
        // Get a Number 

        string GetNum()
        {
            string value = "";
            if (!IsDigit(look))
            {
                Expected("Integer");
            }
            while (IsDigit(look))
            {
                value = value + look;
                GetChar();
            }
            SkipWhite();
            return (value);
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

        //---------------------------------------------------------------
        // Parse and Translate an Identifier

        void Ident()
        {
            string Name = "" ;

            Name = GetName();
            if (look == '(')
            {
                Match('(');
                Match(')');
                EmitLn("BSR " + Name);
            }
            else
            {
                EmitLn("MOVE " + Name + "(PC),D0");
            }
        }

        //---------------------------------------------------------------
        // Parse and Translate a Math Factor

        void Factor()
        {
            if (look == '(')
            {
                Match('(');
                Expression();
                Match(')');
            }
            else if (IsAlpha(look))
            {
                Ident();
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
            EmitLn("EXS.L D0");
            EmitLn("DIVS D1,D0");
        }

        //---------------------------------------------------------------}
        // Parse and Translate a Math Term

        void Term()
        {
            Factor();
            while ((look == '*') || (look == '/'))
            {
                EmitLn("MOVE D0,-(SP)");
                switch (look)
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
            if (IsAddop(look))
            {
                EmitLn("CLR D0");
            }
            else
            {
                Term();
            }
            while (IsAddop(look))
            {
                EmitLn("MOVE D0,-(SP)");
                switch (look)
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
        // Parse and Translate an Assignment Statement }

        void Assignment()
        {
            string Name="";
            Name = GetName();
            Match('=');
            Expression();
            EmitLn("LEA " + Name + "(PC),A0");
            EmitLn("MOVE D0,(A0)");
        }

        //--------------------------------------------------------------
        // Initialize 

        void Init()
        {
            GetChar();
            SkipWhite();
        }

        //--------------------------------------------------------------}
        // Main Program

        public void main()
        {
            Init();
            Expression();
            if (look != CR)
            {
                Expected("NewLine");
            }
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