using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CompilerConsole
{
    class Cradle_part7
    {
        //--------------------------------------------------------------
        // Constant Declarations

        char TAB = '\t';
        char CR = '\r';
        char LF = '\n';

        //--------------------------------------------------------------
        // Variable Declarations

        char Look;                              // Lookahead Character
        string Token;                           // Lexicon Token
        string Symbol;                          // Lexicon Symbols
        string[] SymTab;

        //--------------------------------------------------------------
        // Definition of Keywords and Token Types

        string[] KWlist = new string[4] {"IF", "ELSE", "ENDIF", "END"};

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
            return (IsAlpha(c) || IsDigit(c));
        }

        //--------------------------------------------------------------
        // Get an Identifier 

        string GetName()
        {
            string x = "";

            if (!IsAlpha(Look))
            {
                Expected("Name");
            }
            while (IsAlNum(Look))
            {
                x = x + Char.ToUpper(Look);
                GetChar();
            }
            SkipWhite();
            return (x);
        }

        //--------------------------------------------------------------
        // Get a Number 

        string GetNum()
        {
            string x = "";
            if (!IsDigit(Look))
            {
               Expected("Integer");
            }
            while (IsDigit(Look))
            {
                x = x + Look;
                GetChar();
            }
            SkipWhite();
            return (x);
        }

        //--------------------------------------------------------------
        // Get an Operator

        string GetOp()
        {
            string x = "";
            if (!IsOp(Look))
            {        
                Expected("Operator");
            }
            while (IsOp(Look))
            {
                x = x + Look;
                GetChar();
            }
            SkipWhite();
            return (x);    
        }

        //--------------------------------------------------------------
        // Recognize White Space

        Boolean IsWhite(char c)
        {
            if ((c == ' ') || (c == '\t'))
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }

        //--------------------------------------------------------------
        // Recognize Any Operator

        Boolean IsOp(char c)
        {
            if ((c == '+') || (c == '-') || (c == '*') || (c == '/') || (c == '<') || (c == '>') || (c == ':') || (c == '='))
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
            while (IsWhite(Look))
            {
                GetChar();
            }
        }

        //--------------------------------------------------------------
        // Skip Over an End-of-Line

        void Fin()
        {
            if (Look == CR)
            {
                GetChar();
                if (Look == LF)
                {
                    GetChar();
                }
            }
        }

        // --------------------------------------------------------------
        // Table Lookup
        //
        // If the input string matches a table entry, return the entry index.  If not, return a zero.  }
                             
        int Lookup(List<string> T, string s, int n)
        {
            int i = n;
            Boolean found = false;
            while ((i > 0) && !found)
            {
                if (s == T[i])
                {
                    found = true;
                }
                else
                {
                    i = i - 1;
                }
            }
            return (i);
        }




        //--------------------------------------------------------------
        // lexical Scanner

        string Scan()
        {
            while (Look == CR)
            {
                Fin();
            }
            string x = "";
            if (IsAlpha(Look))
            {
                x = GetName();
            }
            else if (IsDigit(Look))
            {
                x = GetNum();
            }
            else if (IsOp(Look))
            {
                x = GetOp();
            }
            else
            {
                x = Char.ToString(Look);
                GetChar();
            }
            SkipWhite();
            return (x);
        }

        //--------------------------------------------------------------
        // Initialize 

        void Init()
        {
            GetChar();
        }

        //--------------------------------------------------------------}
        // Main Program

        public void Main()
        {
            Init();
            do
            {
                Token = Scan();
                Console.WriteLine(Token);
                if (Token == Char.ToString(CR))
                {
                    Fin();
                }
            }
            while (Token != ".");
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