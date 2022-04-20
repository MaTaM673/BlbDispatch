using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Dispatch.WPF.Helpers;
public class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
    {
        var handler = PropertyChanged;

        if (handler != null)
        {
            var propertyName = GetPropertyName(propertyExpression);
            if (!string.IsNullOrEmpty(propertyName))
                RaisePropertyChanged(propertyName);
        }
    }

    protected static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
    {
        if (propertyExpression == null)
        {
            throw new ArgumentNullException(nameof(propertyExpression));
        }

        if (!(propertyExpression.Body is MemberExpression body))
        {
            throw new ArgumentException("Invalid argument", nameof(propertyExpression));
        }

        var property = body.Member as PropertyInfo;

        if (property == null)
        {
            throw new ArgumentException("Argument is not a property", nameof(propertyExpression));
        }

        return property.Name;
    }

    public void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        if (string.IsNullOrWhiteSpace(propertyName)) return;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public TRet RaiseAndSetIfChanged<TRet>(ref TRet backingField, TRet newValue, [CallerMemberName] string? propertyName = null)
    {
        Contract.Requires(propertyName != null);

        if (EqualityComparer<TRet>.Default.Equals(backingField, newValue))
        {
            return newValue;
        }

        backingField = newValue;
        RaisePropertyChanged(propertyName);
        return newValue;
    }
}
