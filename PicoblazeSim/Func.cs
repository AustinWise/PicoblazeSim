using System;
using System.Collections.Generic;
using System.Text;

namespace Austin.PicoblazeSim
{
    //copied from System.Core in .NET 3.5
    public delegate TResult Func<TResult>();
    public delegate TResult Func<T, TResult>(T arg);
    public delegate TResult Func<T1, T2, TResult>(T1 arg1, T2 arg2);
    public delegate TResult Func<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3);
}
