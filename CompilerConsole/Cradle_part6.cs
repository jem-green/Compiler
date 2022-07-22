﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CompilerConsole
{
    class Cradle_part6
    {
        //--------------------------------------------------------------
        // Constant Declarations

        char TAB = '\t';
        char CR = '\r';
        char LF = '\n';

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

        //--------------------------------------------------------------}
        // Recognize a Mulop 

        Boolean IsMulop(char c)
        {
            if ((c == '*') || (c == '/'))
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
        // Recognise a Boolean Literal

        Boolean IsBoolean(char c)
        {
            if ((c == 'T') || (c == 't') || (c == 'F') || (c == 'f'))
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }

        //--------------------------------------------------------------
        // Get an Boolean Literal 

        Boolean GetBoolean()
        {
            char c;
            Boolean b;

            if (!IsBoolean(look))
            {
                Expected("Boolean Literal");
            }
            c = Char.ToUpper(look);
            if ((c == 'T') || (c == 't'))
            {
                b = true;
            }
            else
            {
                b = false;
            }
            GetChar();
            return (b);
        }

        //--------------------------------------------------------------
        // Parse and Translate a Boolean Expression

        void BooleanExpression()
        {
            BoolTerm();
            while (IsOrOpp(look))
            {
                EmitLn("MOVE DO,-(SP)");
                switch (look)
                {
                    case '|':
                        {
                            BoolOr();
                            break;
                        }
                    case '~':
                        {
                            BoolXor();
                            break;
                        }
                }
            }
        }

        //--------------------------------------------------------------
        // Recognise and Translate a Boolean OR

        void BoolOr()
        {
            Match('|');
            BoolTerm();
            EmitLn("OR (SP)+,D0");
        }

        //--------------------------------------------------------------
        // Recognise and Translate a Boolean XOR

        void BoolXor()
        {
            Match('~');
            BoolTerm();
            EmitLn("XOR (SP)+,D0");
        }

        //--------------------------------------------------------------
        // Recognise and Translate a Boolean Expression

        void BoolExpression()
        {
            BoolTerm();
            while (IsOrOpp(look))
            {
                EmitLn("MOVE D0,-(SP)");
                switch (look)
                {
                    case '|':
                        {
                            BoolOr();
                            break;
                        }
                    case '~':
                        {
                            BoolXor();
                            break;
                        }
                }
            }
        }

        //--------------------------------------------------------------
        // Recognise and Translate a Boolean Expression

        void BoolTerm()
        {
            NotFactor();
            while (look == '&')
            {
                EmitLn("MOVE D0,-(SP)");
                Match('&');
                NotFactor();
                EmitLn("ADD (SP)+,D0");
            }
        }

        //--------------------------------------------------------------
        // Parse and Translate a Boolean Factor with NOT

        void NotFactor()
        {
            if (look == '!')
            {
                Match('!');
                BoolFactor();
                EmitLn("EOR #-1,D0");
            }
            else
            {
                BoolFactor();
            }
        }

        //--------------------------------------------------------------
        // Parse and Translate a Boolean Factor

        void BoolFactor()
        {
            if (IsBoolean(look))
            {
                if (GetBoolean())
                {
                    EmitLn("MOVE #-1,D0");
                }
                else
                {
                    EmitLn("CLR D0");
                }
            }
            else
            {
                Relation();
            }
        }

        //--------------------------------------------------------------
        // Recognise a Boolean Orop

        Boolean IsOrOpp(char c)
        {
            if ((c == '|') || (c == '~'))
            {
                return(true);
            }
            else
            {
                return (false);
            }
        }

        //--------------------------------------------------------------
        // Recognise a Relop

        Boolean IsRelop(char c)
        {
            if ((c == '=') || (c == '#') || (c == '<') || (c == '>'))
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }

        //--------------------------------------------------------------
        // Recognize and Translate a Relational 'Equals'

        void Equals()
        {
            Match('=');
            Expression();
            EmitLn("CMP (SP)+,D0");
            EmitLn("SEQ D0");
        }

        //--------------------------------------------------------------
        // Recognize and Translate a Relational 'Not Equals'

        void NotEquals()
        {
            Match('#');
            Expression();
            EmitLn("CMP (SP)+,D0");
            EmitLn("SNE D0");
        }

        //--------------------------------------------------------------
        // Recognize and Translate a Relational 'Less Than'

        void Less()
        {
            Match('<');
            Expression();
            EmitLn("CMP (SP)+,D0");
            EmitLn("SGT D0");
        }

        //--------------------------------------------------------------
        // Recognize and Translate a Relational 'Greater Than'

        void Greater()
        {
            Match('>');
            Expression();
            EmitLn("CMP (SP)+,D0");
            EmitLn("SLE D0");
        }

        //--------------------------------------------------------------
        // Pasrse and Translate a Relation

        void Relation()
        {
            Expression();
            if (IsRelop(look))
            {
                EmitLn("MOVE D0,-(SP)");
                switch (look)
                {
                    case '=':
                        {
                            Equals();
                            break;
                        }
                    case '#':
                        {
                            NotEquals();
                            break;
                        }
                    case '<':
                        {
                            Less();
                            break;
                        }
                    case '>':
                        {
                            Greater();
                            break;
                        }
                }
                EmitLn("TST D0");
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
            while (IsMulop(look))
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
        // Parse and Translate an Expression

        public void Expression()
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
        // Recognize and Translate an IF Construct

        void DoIf(string L)
        {
            string L1 = "";
            string L2 = "";
            Match('i');
            BooleanExpression();
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
            BooleanExpression();
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
            BooleanExpression();
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

        //--------------------------------------------------------------
        // Skip Over an End-of-Line

        void Fin()
        {
            if (look == CR)
            {
                GetChar();
            }
            if (look == LF)
            {
                GetChar();
            }
        }

        //--------------------------------------------------------------}
        // Parse and Translate an Assignment Statement }

        void Assignment()
        {
            char Name;
            Name = GetName();
            BooleanExpression() ;
            EmitLn("LEA " + Name + "(PC),A0");
            EmitLn("MOVE D0,(A0)");
        }

        //--------------------------------------------------------------}
        // Recognize and Translate a Statement Block }

        void Block(string L)
        {
            while (!(look == 'e' || look == 'l' || look == 'u'))
            {
                Fin();
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
                            Assignment();
                            break;
                        }
                }
                Fin();
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

        public void Main()
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