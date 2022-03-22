using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AsyncEventHandlers
{
    /// <summary>
    /// The <see cref="CancellationToken"/> is automatically set.
    /// </summary>
    public class AsyncEventArgs : EventArgs
    {
        public CancellationToken CancellationToken { get; internal set; }
    }
}
