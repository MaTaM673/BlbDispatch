using Dispatch.WPF.Helpers;
using Dispatch.WPF.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Point = System.Drawing.Point;

namespace Dispatch.WPF.ViewModels;

internal class MainWindowViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Unit> AllUnits { get; } = new();
    public List<Unit> AvailableUnits => AllUnits.Except(ActiveUnits).ToList();
    public ObservableCollection<Unit> ActiveUnits { get; } = new();
    public ObservableCollection<State> AvailableStates { get; } = new()
    {
        new State("10-6", Colors.Gray),
        new State("10-7", Colors.Black),
        new State("10-8", Colors.Green),
        new State("10-17", Colors.Yellow),
        new State("10-23", Colors.Red),
        new State("10-50", Colors.Red),
        new State("Down", Colors.Red),
    };
    public ObservableCollection<Postal> AllPostal { get; } = new();
    public ObservableCollection<Scene> ActiveScenes { get; } = new();

    private Scene? _selectedScene;
    public Scene? SelectedScene
    {
        get => _selectedScene;
        set
        {
            if (Equals(value, _selectedScene)) return;
            _selectedScene = value;
            OnPropertyChanged();
        }
    }

    private bool _showAddUnits;
    public bool ShowAddUnits
    {
        get => _showAddUnits;
        set
        {
            if (value == _showAddUnits) return;
            _showAddUnits = value;
            OnPropertyChanged(nameof(ShowAddUnits));
        }
    }

    private bool _eventWindowOpen;
    public bool EventWindowOpen
    {
        get => _eventWindowOpen;
        set
        {
            if (value == _eventWindowOpen) return;
            _eventWindowOpen = value;
            OnPropertyChanged(nameof(EventWindowOpen));
        }
    }

    private string? _newUnitPosition;

    public string? NewUnitPosition
    {
        get => _newUnitPosition;
        set
        {
            if (value == _newUnitPosition) return;
            _newUnitPosition = value;
            OnPropertyChanged();
        }
    }


    private ImageSource? _mapImage;
    public ImageSource? MapImage
    {
        get => _mapImage;
        set
        {
            if (Equals(value, _mapImage)) return;
            _mapImage = value;
            OnPropertyChanged();
        }
    }

    public MainWindowViewModel()
    {
        // Load roster
        using var streamReader = new StreamReader(File.OpenRead("Resources/Data/Roster.csv"));

        AllUnits.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(AvailableUnits));
        ActiveUnits.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(AvailableUnits));

        var allUnitsString = streamReader.ReadToEnd().Split("\r\n");
        foreach (var unitString in allUnitsString)
        {
            var unitStringSplit = unitString.Split(",");
            AllUnits.Add(new Unit(unitStringSplit[0], unitStringSplit[1]));
        }

        // Load postals
        using var streamReaderPostal = new StreamReader(File.OpenRead("Resources/Data/Postals.csv"));

        var allPostalString = streamReaderPostal.ReadToEnd().Split("\r\n");
        foreach (var postalString in allPostalString)
        {
            var postalStringSplit = postalString.Split(",");
            AllPostal.Add(new Postal(int.Parse(postalStringSplit[0]), new Point(int.Parse(postalStringSplit[1]), int.Parse(postalStringSplit[2]))));
        }

        var localMapImage = new BitmapImage();
        localMapImage.BeginInit();
        localMapImage.StreamSource = File.OpenRead("Resources/Images/newMap.png");
        localMapImage.DecodePixelWidth = 6144;
        localMapImage.CacheOption = BitmapCacheOption.OnLoad;
        localMapImage.EndInit();

        MapImage = localMapImage;
    }

    public ICommand ActivateUnitCommand => new Command<Unit>(param =>
    {
        if (param == null) return;
        param.CurrentPosition = AllPostal.First();
        param.CurrentState = AvailableStates.First(x => x.Name == "10-8");
        ActiveUnits.Add(param);
    });

    public ICommand ChangeStatusCommand => new Command<object>(parameter =>
    {
        if (parameter is not MenuItem menuItem)
            return;

        var unit = (Unit)menuItem.DataContext;

        switch (menuItem.Header)
        {
            case "10-17":
                unit.Destination = AllPostal.FirstOrDefault(x => x.Id.ToString() == NewUnitPosition);
                NewUnitPosition = "";
                break;
        }

        SetUnitState(unit, AvailableStates.First(x => x.Name == menuItem.Header.ToString()));
    });

    public ICommand ChangePositionCommand => new Command<object>(parameter =>
    {
        if (!(parameter is MenuItem menuItem))
            return;

        var unit = (Unit)menuItem.DataContext;

        unit.CurrentPosition = AllPostal.FirstOrDefault(x => x.Id.ToString() == NewUnitPosition);

        if (unit.CurrentPosition == unit.Destination)
            unit.Destination = null;

        NewUnitPosition = "";
    });

    public ICommand ChangeDestinationCommand => new Command<object>(parameter =>
    {
        if (parameter is not MenuItem menuItem)
            return;

        var unit = (Unit)menuItem.DataContext;

        SetUnitDestination(unit, AllPostal.FirstOrDefault(x => x.Id.ToString() == NewUnitPosition));
        NewUnitPosition = "";
    });

    public ICommand NewSceneCommand => new Command(() =>
    {
        SelectedScene = new Scene();
        EventWindowOpen = true;
    });

    public ICommand CloseEventWindow => new Command(() => EventWindowOpen = false);

    public ICommand AddActiveSceneCommand => new Command(() =>
    {
        if (SelectedScene != null)
        {
            ActiveScenes.Add(SelectedScene);
            if (SelectedScene.PrimaryUnit != null)
                SetUnitDestination(SelectedScene.PrimaryUnit, SelectedScene.Location);
            foreach (var unit in SelectedScene.AdditionalUnits)
            {
                SetUnitDestination(unit, SelectedScene.Location);
            }
        }

        EventWindowOpen = false;
        SelectedScene = null;
    });

    public ICommand ResolveSceneCommand => new Command<Scene>(scene =>
    {
        if (scene == null) return;
        scene.SceneEnd = DateTime.Now;

        var baseFolder = (Application.Current as App)?.ServiceProvider.GetRequiredService<IOptions<Helpers.Configuration>>().Value.ReportLocation ?? "";

        File.AppendAllText(Path.Combine(baseFolder, $"dispatch-{DateTime.Now:yyyyMMdd}.txt"), scene.ToString());

        if (scene.PrimaryUnit != null)
            SetUnitState(scene.PrimaryUnit, AvailableStates.First(x => x.Name == "10-8"));
        foreach (var sceneAdditionalUnit in scene.AdditionalUnits)
        {
            SetUnitState(sceneAdditionalUnit, AvailableStates.First(x => x.Name == "10-8"));
        }

        ActiveScenes.Remove(scene);
    });

    private void SetUnitDestination(Unit unit, Postal? destination)
    {
        unit.Destination = destination;

        if (unit.Destination == unit.CurrentPosition && unit.CurrentState.Name == "10-17")
            SetUnitState(unit, AvailableStates.First(x => x.Name == "10-23"));

        if (unit.Destination == unit.CurrentPosition) return;

        SetUnitState(unit, AvailableStates.First(x => x.Name == "10-17"));
    }

    private void SetUnitState(Unit unit, State state)
    {
        unit.CurrentState = state;

        switch (state.Name)
        {
            case "10-7":
                ActiveUnits.Remove(unit);
                break;
            case "10-23" when unit.Destination != null && unit.Destination != unit.CurrentPosition:
                unit.CurrentPosition = unit.Destination;
                unit.Destination = null;

                foreach (var scene in ActiveScenes.Where(x => x.PrimaryUnit == unit || x.AdditionalUnits.Contains(unit)))
                {
                    scene.AddDetails($"{unit.FullName} arrived on scene");
                }
                break;
        }

    }

    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
