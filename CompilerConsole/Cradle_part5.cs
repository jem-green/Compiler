﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CompilerConsole
{
    class Cradle_part5
    {
        //--------------------------------------------------------------
        // Constant Declarations

        char TAB = '\t';
        char CR = '\r';

        //--------------------------------------------------------------
        // Variable Declarations

        char look;                              // Lookahead Character
        int Lcount;                             // Label Counter

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
            return (c);
        }

        //--------------------------------------------------------------
        // Get a Number 

        char GetNum()
        {
            char c;
            if (!IsDigit(look))
            {
               Expected("Integer");
            }
            c =look;
            GetChar();
            SkipWhite();
            return(c);
        }

        //--------------------------------------------------------------}
        // Generate a Unique Label }

        string NewLabel()
        {
            string S = "";
            S = Lcount.ToString();
            S = 'L' + S;
            Lcount = Lcount + 1;
            return (S);
        }

        //--------------------------------------------------------------}
        // Post a Label To Output 

        void PostLabel(string L)
        {
            Console.WriteLine(L + ':');
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

        //--------------------------------------------------------------}
        // Parse and Translate a Boolean Condition

        void Condition()
        {
            EmitLn("<condition>");
        }

        //--------------------------------------------------------------}
        // Parse and Translate an Expression }

        void Expression()
        {
            EmitLn("<expr>");
        }

        //--------------------------------------------------------------}
        // Recognize and Translate an IF Construct

        void DoIf(string L)
        {
            string L1 = "";
            string L2 = "";
            Match('i');
            Condition();
            L1 = NewLabel();
            L2 = L1;
            EmitLn("BEQ " + L1);
            Block(L);
            if (look == 'l')
            {
                Match('l');
                L2 = NewLabel();
                EmitLn("BRA " + L2);
                PostLabel(L1);
                Block(L);
            }
            Match('e');
            PostLabel(L2);
        }

        //--------------------------------------------------------------}
        // Parse and Translate a WHILE Statement 

        void DoWhile()
        {
            string L1 = "";
            string L2 = "";       
            Match('w');
            L1 = NewLabel();;
            L2 = NewLabel();;
            PostLabel(L1);
            Condition();
            EmitLn("BEQ " + L2);
            Block(L2);
            Match('e');
            EmitLn("BRA " + L1);
            PostLabel(L2);
        }

        //--------------------------------------------------------------
        // Parse and Translate a LOOP Statement

        void DoLoop()
        {
            string L1;
            string L2;
            Match('p');
            L1 = NewLabel();
            L2 = NewLabel();
            PostLabel(L1);
            Block(L2);
            Match('e');
            EmitLn("BRA " + L1);
            PostLabel(L2);
        }

        //--------------------------------------------------------------}
        // Parse and Translate a REPEAT Statement }

        void DoRepeat()
        {
            string L1;
            string L2;
            Match('r');
            L1 = NewLabel();
            L2 = NewLabel();
            PostLabel(L1);
            Block(L2);
            Match('u');
            Condition();
            EmitLn("BEQ " + L1);
            PostLabel(L2);
        }
        
        //--------------------------------------------------------------}
        // Parse and Translate a FOR Statement

        void DoFor()
        {
            string L1;
            string L2;
            char Name;
            
            Match('f');
            L1 = NewLabel();
            L2 = NewLabel();
            Name = GetName();
            Match('=');
            Expression();
            EmitLn("SUBQ #1,D0");
            EmitLn("LEA " + Name + "(PC),A0");
            EmitLn("MOVE D0,(A0)");
            Expression();
            EmitLn("MOVE D0,-(SP)");
            PostLabel(L1);
            EmitLn("LEA " + Name + "(PC),A0");
            EmitLn("MOVE (A0),D0");
            EmitLn("ADDQ #1,D0");
            EmitLn("MOVE D0,(A0)");
            EmitLn("CMP (SP),D0");
            EmitLn("BGT " + L2);
            Block(L2);
            Match('e');
            EmitLn("BRA " + L1);
            PostLabel(L2);
            EmitLn("ADDQ #2,SP");
        }

        //--------------------------------------------------------------}
        // Parse and Translate a DO Statement

        void DoDo()
        {
            string L1;
            string L2;
            Match('d');
            L1 = NewLabel();
            L2 = NewLabel();
            Expression();
            EmitLn("SUBQ #1,D0");
            PostLabel(L1);
            EmitLn("MOVE D0,-(SP)");
            Block(L2);
            EmitLn("MOVE (SP)+,D0");
            EmitLn("DBRA D0," + L1);
            EmitLn("SUBQ #2,SP");
            PostLabel(L2);
            EmitLn("ADDQ #2,SP");
        }

        //--------------------------------------------------------------}
        // Recognize and Translate a BREAK }

        void DoBreak(string L)
        {
           Match('b');
           if (L != "")
           {
              EmitLn("BRA " + L);
           }
           else
           {
               Abort("No loop to break from");
           }
        }

        //--------------------------------------------------------------}
        // Recognize and Translate an "Other"

        void Other()
        {
            EmitLn(GetName().ToString());
        }

        //--------------------------------------------------------------}
        // Recognize and Translate a Statement Block }

        void Block(string L)
        {
            while (!(look == 'e' || look == 'l' || look == 'u'))
            {
                switch (look)
                {
                    case 'b':
                        {
                            DoBreak(L);
                            break;
                        }
                    case 'd':
                        {
                            DoDo();
                            break;
                        }
                    case 'f':
                        {
                            DoFor();
                            break;
                        }
                    case 'i':
                        {
                            DoIf(L);
                            break;
                        }
                    case 'p':
                        {
                            DoLoop();
                            break;
                        }
                    case 'r':
                        {
                            DoRepeat();
                            break;
                        }
                    case 'w':
                        {
                            DoWhile();
                            break;
                        }

                    default:
                        {
                            Other();
                            break;
                        }
                }
            }
        }

        //--------------------------------------------------------------}
        // Parse and Translate a Program

        public void DoProgram()
        {
            Block("");
            if (look != 'e')
            {
                Expected("End");
            }
            EmitLn("END");
        }

        //--------------------------------------------------------------
        // Initialize 

        void Init()
        {
            Lcount = 0;
            GetChar();
            SkipWhite();
        }

        //--------------------------------------------------------------}
        // Main Program

        public void main()
        {
            Init();
            DoProgram();
        }
        //--------------------------------------------------------------}
 
        //*****************************************************************
        //*                                                               *
        //*                        COPYRIGHT NOTICE                       *
        //*                                                               *
        //*   Copyright (C) 1988 Jack W. Crenshaw. All rights reserved.   *
        //*                                                               *
        //*****************************************************************

    }
}