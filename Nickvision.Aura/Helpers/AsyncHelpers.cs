﻿using System.Threading.Tasks;

namespace Nickvision.Aura.Helpers;

/// <summary>
/// Helper methods for working with System.Threading.Tasks.Task
/// </summary>
public static class AsyncHelpers
{
    /// <summary>
    /// Fires an async method and forgets about it's return
    /// </summary>
    /// <param name="task">The async Task</param>
    public static async void FireAndForget(this Task task)
    {
        try
        {
            await task;
        }
        catch { }
    }
}
