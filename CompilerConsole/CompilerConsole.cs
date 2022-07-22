using System;
using System.Collections.Generic;
using System.Text;

namespace CompilerConsole
{
    class CompilerConsole
    {
        //-----------------------------------------------------------------
        // Main Program 

        [STAThread]
        static void Main()
        {
            Cradle_part5 i = new Cradle_part5();
            try
            {
                i.main();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        //-----------------------------------------------------------------

        //*****************************************************************
        //*                                                               *
        //*                        COPYRIGHT NOTICE                       *
        //*                                                               *
        //*   Copyright (C) 1988 Jack W. Crenshaw. All rights reserved.   *
        //*                                                               *
        //*****************************************************************

    }
}
