namespace BlackTemple.Common
{
    public delegate void EventHandler();

    public delegate void EventHandler<in T>(T data);

    public delegate void EventHandler<in T, in T2>(T data, T2 data2);

    public delegate void EventHandler<in T, in T2, in T3>(T data, T2 data2, T3 data3);

    public delegate void EventHandler<in T, in T2, in T3, in T4>(T data, T2 data2, T3 data3, T4 data4);

    public delegate void EventHandler<in T, in T2, in T3, in T4, in T5>(T data, T2 data2, T3 data3, T4 data4, T5 data5);
}