﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChangerMints.Business
{
    interface IRunTerminalTransactions
    {
        string RunTransaction(string queryString);
    }
}
