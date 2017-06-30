using System;
using System.Threading.Tasks;

public static void Run(string queueItem, out string outputSbMsg, TraceWriter log)
{
    // ******************************************************************************
    // Before enabling this function, confirm the bindings correspond to the right Queue 
    //
    log.Info($"ReprocessDLQMessages function processed message: {myQueueItem}");
    // 
    // ******************************************************************************

    outputSbMsg = queueItem;
}
