Apparatus.Blazor.State
=========
Apparatus.Blazor.State is library which provides centralized state management for Blazor webassembly based implementations.

[![Build Status](https://dev.azure.com/Perpetuum-mobile/Apparatus.Blazor.State/_apis/build/status/Apparatus.Blazor.State-ASP.NET%20Core-CI?branchName=master)](https://dev.azure.com/Perpetuum-mobile/Apparatus.Blazor.State/_build/latest?definitionId=19&branchName=master)  [![Azure DevOps tests](https://img.shields.io/azure-devops/tests/Perpetuum-mobile/Apparatus.Blazor.State/19?compact_message&failed_label=failed&passed_label=passed&skipped_label=skipped)](https://dev.azure.com/Perpetuum-mobile/Apparatus.Blazor.State/_build/results?buildId=821&view=ms.vss-test-web.build-test-results-tab) [![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/Perpetuum-mobile/Apparatus.Blazor.State/19)](https://dev.azure.com/Perpetuum-mobile/Apparatus.Blazor.State/_build/results?buildId=821&view=codecoverage-tab) [![Nuget](https://img.shields.io/nuget/dt/Apparatus.Blazor.State)](https://www.nuget.org/packages/Apparatus.Blazor.State)

# Getting Started #

__1. Create Blazor webassembly project by running:__
```
dotnet new blazorwasm -o BlazorWebAssemblyApp
```
__2. Install the standard Nuget package into your ASP.NET Blazor project.__
```
    Package Manager : Install-Package Apparatus.Blazor.State 
    CLI : dotnet add package Apparatus.Blazor.State
```
__3. In `Program.cs` add below line to register the service:__
```
	builder.Services.AddStateManagement(typeof(Program).Assembly);
```
__4. Under States folder, create `CounterState.cs` class which shousl represent Counter page/component state, also all states should inherit `Apparatus.Blazor.State.Contracts.IState` interface.__
```
    public class CounterState : IState
    {
        public int CurrentCount { get; set; }
    }
```
__5. Create `IncrementCount.cs` action in "Actions" folder - all actions should inherit `Apparatus.Blazor.State.Contracts.IAction` interface.__
```
    public class IncrementCount : IAction
    {
        public int IncrementBy { get; set; }
    }
```
__6. Inherit BlazorStateComponent in `Counter.razor` page/component, reference `CounterState` in the component and implement Action Dispatcher:__
```
	@inherits Apparatus.Blazor.State.BlazorStateComponent
```	
```	
    ....
<p role="status">Current count: @State.CurrentCount</p>
<button class="btn btn-primary" @onclick="IncrementCount">Click me</button>

@code {
    CounterState State => GetState<CounterState>(); 
    [Inject] IActionDispatcher Dispatcher { get; set; }

    private void IncrementCount()
    {
        Dispatcher.Dispatch(new IncrementCount { /*IncrementBy = 2*/ });
    }
}
```	
__7. Implement Action Handler__
    - Under "Handlers" folder create `IncrementCountHandler.cs` 
```	
  public class IncrementCountHandler : IActionHandler<IncrementCount>
    {
        private IStore<CounterState> _counterStore;
        public IncrementCountHandler(IStore<CounterState> counterStore)
        {
            _counterStore = counterStore; 
        }
        
        public Task Handle(IncrementCount action)
        {
            var newState = _counterStore.State;
            newState.CurrentCount++;
            /*
             * or use action property for increment value
             * newState.CurrentCount = newState.CurrentCount + action.IncrementBy;
             */
            return _counterStore.SetState(newState); 
        }
    }

```

In this way you can keep your states separated from logic and components/pages.

# Subscribe to Action without creating Handler #

Subscribe from Blazor component.
```	
 @code {
   [Inject] IActionSubscriber ActionSubscriber { get; set; }
	ActionSubscriber.Subscribe<MyAction>((action) => { ... });
 }

```	


For more complex use cases please check SampleApp available in the [repository](https://github.com/Inaccessible1/Apparatus.Blazor.State). 





