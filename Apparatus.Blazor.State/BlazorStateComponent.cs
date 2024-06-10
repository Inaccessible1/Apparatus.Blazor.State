using Apparatus.Blazor.State.Contracts;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#nullable disable annotations
namespace Apparatus.Blazor.State
{
    public class BlazorStateComponent : ComponentBase, IDisposable, IBlazorStateComponent
    {
        public BlazorStateComponent()
        {
            string name = GetType().Name;
            string id = Guid.NewGuid().ToString();

            Id = $"{name}-{id}";
        }

        [Inject] IServiceProvider ServiceProvider { get; set; }
        [Inject] ISubscriptionService SubscriptionService { get; set; }

        public string Id { get; }

        public void ReRender() => base.InvokeAsync(StateHasChanged);

        public void Dispose()
        {
            SubscriptionService.Remove(this);
            GC.SuppressFinalize(this);
        }

        public TState GetState<TState>() where TState : IState, new()
        {
            if (ServiceProvider.GetService(typeof(TState)) is TState state)
            {
                SubscriptionService.Add(typeof(TState), this);

                return state;
            }

            return new TState(); 
        }
    }
}
