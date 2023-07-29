// code by cliss on Github
// https://gist.github.com/cliss/f03f7268a1c9006daf88

public class DebugTimer : IDisposable
{
    private readonly string _blockName;
    private readonly System.Diagnostics.Stopwatch _watch;
    /// <summary>
    /// Creates a timer.
    /// </summary>
    /// <param name="blockName">Name of the block that's being timed</param>
    public DebugTimer( string blockName ) {
        _blockName = blockName;
        _watch = System.Diagnostics.Stopwatch.StartNew();
    }

    ~DebugTimer() {
        throw new InvalidOperationException( "Must Dispose() of all instances of " + this.GetType().FullName );
    }

    public void Dispose() {
        _watch.Stop();
        GC.SuppressFinalize( this );
        Console.WriteLine( _watch.Elapsed.TotalMilliseconds + "ms to call " + _blockName );
    }
}