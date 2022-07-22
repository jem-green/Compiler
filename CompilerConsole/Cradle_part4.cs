using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CompilerConsole
{
    class Cradle_part4
    {
        //--------------------------------------------------------------
        // Constant Declarations

        char TAB = '\t';
        char CR = '\r';
        char LF = '\n';

        //--------------------------------------------------------------
        // Variable Declarations

        string buffer;  // Console buffer
        char look;  // Lookahead Character
        Hashtable table = new Hashtable(26);    // Variable storage area

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
            return(Char.IsLetter(c));
        }      

        //--------------------------------------------------------------
        // Recognize a Decimal Digit 

        Boolean IsDigit(char c)
        {
           return(Char.IsNumber(c));
        }

        //--------------------------------------------------------------
        // Recognize an Alphanumeric }

        Boolean IsAlNum(char c)
        {
           return(IsAlpha(c) || IsDigit(c));
        }

        //--------------------------------------------------------------
        // Recognize an Addop

        Boolean IsAddop(char c)
        {
            if ((c == '+') || (c == '-'))
            {
                return(true);
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
                return(true);
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
            while(IsWhite(look))
            {
                GetChar();
            }
        }


        //--------------------------------------------------------------
        // Recognise and Skip Over a Newline

        void NewLine()
        {
            if (look == CR)
            {
                GetChar();
                if (look == LF)
                {
                    GetChar();
                }
            }
        }

        //--------------------------------------------------------------
        // Get an Identifier 

        char GetName()
        {
            char c;

            if (!IsAlpha(look))
            {
                Expected("Name");
            }
            c = Char.ToUpper(look);
            GetChar();
            SkipWhite();
            return (c);
        }

        //--------------------------------------------------------------
        // Get a Number 

        int GetNum()
        {
            int Value=0;
            if (!IsDigit(look))
            {
               Expected("Integer");
            }

            while (IsDigit(look))
            {
                Value = 10* Value + Convert.ToInt16(look) - Convert.ToInt16('0');
                GetChar();
            }
            SkipWhite();
            return(Value);
        }

        //--------------------------------------------------------------
        // Input Routine }

        void Input()
        {
            ConsoleKeyInfo k;
            char c;

            Match('?');
            c = GetName();
            k = Console.ReadKey();
            table[c] = k.KeyChar;
        }

        //--------------------------------------------------------------
        // Output Routine

        void Output()
        {
            Match('!');
            Console.WriteLine(table[GetName()]);
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
            char Name;

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

        int Factor()
        {
            int Value = 0;

            if (look == '(')
            {
                Match('(');
                Value = Expression();
                Match(')');
            }
            else if (IsAlpha(look))
            {
                Value = (int)table[GetName()];
            }
            else
            {
                Value = GetNum();
            }
            return (Value);
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

        int Term()
        {
            int Value = 0;
            Value = Factor();

            while ((look == '*')||(look== '/'))
            {
                switch (look)
                {
                    case '*':
                    {
                        Match('*');
                        Value = Value * GetNum();
                       break;
                    }
                    case '/':
                    {
                        Match('/');
                        Value = Value / GetNum();
                        break;
                    }
                    default:
                    {
                       Expected("Mulop");
                       break;
                    }
                }
            }
            return(Value);
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
        // Parse and Translate an Expression

        int Expression()
        {
            int Value = 0;

            if (IsAddop(look))
            {
                Value = 0;
            }
            else
            {
                Value = Term();
            }
            while (IsAddop(look))
            {
                switch (look)
                {
                    case '+':
                        {
                            Match('+');
                            Value = Value + GetNum();
                            break;
                        }
                    case '-':
                        {
                            Match('-');
                            Value = Value - GetNum();
                            break;
                        }
                    default:
                        {
                            Expected("Addop");
                            break;
                        }
                }
            }
            return (Value);
        }

        //--------------------------------------------------------------}
        // Parse and Translate an Assignment Statement }

        void Assignment()
        {
            char Name;            
            Name = GetName();
            Match('=');
            table[Name] = Expression();
        }

        //---------------------------------------------------------------
        // Initialize the Variable Area 

        void InitTable()
        {
            char c;

            for (c = 'A'; c < 'Z'; c++)
            {
                table[c] = 0;
            }
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
            InitTable();
            Init();
            while (look != '.')
            {
                switch (look)
                {
                    case '?':
                        {
                            Input();
                            break;
                        }
                    case '!':
                        {
                            Output();
                            break;
                        }
                    case '\r':
                        break;
                    case '\n':
                        break;
                    default:
                        {
                            Assignment();
                            break;
                        }
                }
                NewLine();
            }
        }

        //--------------------------------------------------------------}

        //*****************************************************************
        //*                                                               *
        //*                       COPYRIGHT NOTICE                        *
        //*                                                               *
        //*   Copyright (C) 1988 Jack W. Crenshaw. All rights reserved.   *
        //*                                                               *
        //*****************************************************************

    }
}