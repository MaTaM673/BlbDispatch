using System;
using System.Reflection;
using System.Windows.Input;

namespace Dispatch.WPF.Helpers;
/// <remarks>Taken from the Xamarin source code</remarks>
public sealed class Command<T> : Command
{
	public Command(Action<T?> execute)
		: base(o =>
		{
			if (IsValidParameter(o))
			{
				execute((T?)o);
			}
		})
	{
		if (execute == null)
		{
			throw new ArgumentNullException(nameof(execute));
		}
	}

	public Command(Action<T?> execute, Func<T?, bool> canExecute)
		: base(o =>
		{
			if (IsValidParameter(o))
			{
				execute((T?)o);
			}
		}, o => IsValidParameter(o) && canExecute((T?)o))
	{
		if (execute == null)
			throw new ArgumentNullException(nameof(execute));
		if (canExecute == null)
			throw new ArgumentNullException(nameof(canExecute));
	}

    private static bool IsValidParameter(object? o)
	{
		if (o != null)
		{
			// The parameter isn't null, so we don't have to worry whether null is a valid option
			return o is T;
		}

		var t = typeof(T);

		// The parameter is null. Is T Nullable?
		if (Nullable.GetUnderlyingType(t) != null)
		{
			return true;
		}

		// Not a Nullable, if it's a value type then null is not valid
		return !t.GetTypeInfo().IsValueType;
	}
}

public class Command : ICommand
{
    private readonly Func<object?, bool>? _canExecute;
    private readonly Action<object?>? _execute;
    private readonly WeakEventManager _weakEventManager = new();

	public Command(Action<object?> execute)
	{
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
	}

	public Command(Action execute) : this(o => execute())
	{
		if (execute == null)
			throw new ArgumentNullException(nameof(execute));
	}

	public Command(Action<object?> execute, Func<object?, bool> canExecute) : this(execute)
	{
        _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
	}

	public Command(Action execute, Func<bool> canExecute) : this(o => execute(), o => canExecute())
	{
		if (execute == null)
			throw new ArgumentNullException(nameof(execute));
		if (canExecute == null)
			throw new ArgumentNullException(nameof(canExecute));
	}

	public bool CanExecute(object? parameter)
    {
        return _canExecute == null || _canExecute(parameter);
    }

	public event EventHandler? CanExecuteChanged
	{
		add => _weakEventManager.AddEventHandler(value);
        remove => _weakEventManager.RemoveEventHandler(value);
    }

	public void Execute(object? parameter)
    {
        _execute?.Invoke(parameter);
    }

	public void ChangeCanExecute()
	{
		_weakEventManager.HandleEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));
	}
}
