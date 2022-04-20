using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Dispatch.WPF.Models;
using Dispatch.WPF.ViewModels;

namespace Dispatch.WPF.Controls;
/// <summary>
/// Interaction logic for ActiveScene.xaml
/// </summary>
public partial class ActiveScene : UserControl
{
    public ActiveScene()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty CurrentSceneProperty = DependencyProperty.Register(
        "CurrentScene", typeof(Scene), typeof(ActiveScene), new PropertyMetadata(default(Scene), CurrentSceneChanged));

    public Scene CurrentScene
    {
        get => (Scene)GetValue(CurrentSceneProperty);
        set => SetValue(CurrentSceneProperty, value);
    }

    private static void CurrentSceneChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        if(o is not ActiveScene { DataContext: ActiveSceneViewModel viewModel })
            return;

        viewModel.CurrentScene = e.NewValue as Scene ?? new Scene();
    }

    public static readonly DependencyProperty AvailableUnitsProperty = DependencyProperty.Register(
        "AvailableUnits", typeof(ObservableCollection<Unit>), typeof(ActiveScene), new PropertyMetadata(default(ObservableCollection<Unit>), AvailableUnitsChanged));

    public ObservableCollection<Unit> AvailableUnits
    {
        get => (ObservableCollection<Unit>)GetValue(AvailableUnitsProperty);
        set => SetValue(AvailableUnitsProperty, value);
    }

    private static void AvailableUnitsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        if (o is not ActiveScene { DataContext: ActiveSceneViewModel viewModel })
            return;

        viewModel.AvailableUnits = e.NewValue as ObservableCollection<Unit> ?? new ObservableCollection<Unit>();
    }

    public static readonly DependencyProperty PostalsProperty = DependencyProperty.Register(
        "Postals", typeof(ObservableCollection<Postal>), typeof(ActiveScene), new PropertyMetadata(default(ObservableCollection<Postal>), PostalsChanged));

    public ObservableCollection<Postal> Postals
    {
        get => (ObservableCollection<Postal>)GetValue(PostalsProperty);
        set => SetValue(PostalsProperty, value);
    }

    private static void PostalsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        if (o is not ActiveScene { DataContext: ActiveSceneViewModel viewModel })
            return;

        viewModel.PostalList = (e.NewValue as ObservableCollection<Postal>) ?? new ObservableCollection<Postal>();
    }
}
