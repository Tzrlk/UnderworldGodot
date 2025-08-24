using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace Underworld.Utility;

public static class Async
{

    /// <summary>
    /// Uses the Godot DirAccess api to generate a stream of paths from the
    /// given directory path.
    /// </summary>
    /// <param name="path">The path to a directory.</param>
    /// <returns>The names of each child in the directory.</returns>
    /// <exception cref="GodotException">If something goes wrong during the process.</exception>
    public static IAsyncEnumerable<string> DirChildList(string path)
        => DirChildList(path, CancellationToken.None);

    /// <inheritdoc cref="DirChildList(string)"/>
    /// <param name="token">A cancellation token in case the stream can end early.</param>
    public static async IAsyncEnumerable<string> DirChildList(string path,
            [EnumeratorCancellation] CancellationToken token)
    {
        // Open the path as a directory.
        using var dir = DirAccess.Open(path);
        GodotException.HandleError(
            DirAccess.GetOpenError(),
            $"Failed to open {path}.");

        // Start a stream of files in the directory.
        GodotException.HandleError(
            dir.ListDirBegin(),
            $"Failed to list children of {path}.");

        string file;
        while ((file = dir.GetNext()) != "")
        {
            // If a cancellation has been requested, finish immediately without
            // incident.
            if (token.IsCancellationRequested)
                yield break;

            // Provide the next value and yield control to _actually_ make this
            // function asynchronous.
            yield return file;
            await Task.Yield();
        }
    }

    /// <summary>
    /// This function attempts to find one of the Underworld executables in
    /// the directory of the provided file, or one of its parents.
    /// </summary>
    /// <param name="path">The file/dir to determine which game it belongs to.</param>
    /// <param name="height">The maximum number of parent directories to search, or the exact parent to search if negative.</param>
    /// <param name="fallback">Which game to assume if an appropriate exe can't be found.</param>
    /// <returns></returns>
    public static async Task<byte> WhichGame(
        string path,
        int height = 1,
        byte fallback = UWClass.GAME_UW1)
    {

        // If we've been given a negative height, _immediately_ recurse up
        // without checking the path.
        if (height < 0)
            return await WhichGame(Path.GetDirectoryName(path), height + 1, fallback);

        // If passed a file, immediately move on to its directory. Don't
        // reduce the height yet, as we only do that when going up dirs.
        // NOTE: Couldn't figure out how to check this in pure godot.
        if (!File.GetAttributes(path).HasFlag(FileAttributes.Directory))
            return await WhichGame(Path.GetDirectoryName(path), height, fallback);

        // Iterate through all the files in the directory, looking for one
        // of the underworld executables.
        var cancellation = new CancellationTokenSource();
        await foreach (var item in Async.DirChildList(path, cancellation.Token))
        {
            switch (item.ToUpper())
            {
                case "UW.EXE":
                    cancellation.Cancel();
                    return UWClass.GAME_UW1;
                case "UW2.EXE":
                    cancellation.Cancel();
                    return UWClass.GAME_UW2;
                default:
                    Debug.WriteLine("Within '{0}', found '{1}'.", path, item);
                    continue;
            }
        }

        // If we haven't found anything and there aren't any levels left to
        // recurse into, we can't do anything else but return the fallback.
        if (height == 0)
            return fallback;

        // Otherwise, we can search one level up, reducing the height
        // appropriately.
        return await WhichGame(Path.GetDirectoryName(path), height - 1, fallback);

    }
        
}