﻿using Apparatus.Blazor.State.Contracts;

namespace Apparatus.Blazor.SampleApp.Actions
{
    public class RemoveTableRow : IAction
    {
        public DateTime Date { get; set; }
    }
}
