﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (GameContext gc = new GameContext("CheckersDB")) {
                gc.Database.Initialize(force: true);
            } 
        }
    }
}
