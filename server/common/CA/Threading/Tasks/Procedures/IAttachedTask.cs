using System.Threading;

namespace CA.Threading.Tasks.Procedures
{
#pragma warning disable

    public interface IAttachedTask
    {
        CancellationToken GetToken { get; }

        void AttachToParent(CancellationToken token);
    }

#pragma warning restore
}
