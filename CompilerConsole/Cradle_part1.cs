using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CompilerConsole
{
    class Cradle_part1
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

        //--------------------------------------------------------------}
        // Main Program

        public void Main()
        {
            Init();
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