using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("Apparatus.Blazor.State.Test")]
namespace Apparatus.Blazor.State.Contracts
{
    public interface IActionSubscriber
    {
        void Subscribe<TAction>(Action<TAction> handler) where TAction : IAction;

        internal void Publish<TAction>(TAction action) where TAction : IAction;
    }
}
