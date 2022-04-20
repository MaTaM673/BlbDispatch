using System;
using Dispatch.WPF.Helpers;
using Dispatch.WPF.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Dispatch.WPF.ViewModels;
internal class ActiveSceneViewModel : ObservableObject
{
    private Scene _currentScene = new();
    public Scene CurrentScene
    {
        get => _currentScene;
        set
        {
            RaiseAndSetIfChanged(ref _currentScene, value);
            RaisePropertyChanged(()=> LocationString);
        }
    }

    private ObservableCollection<Unit> _availableUnits = new();
    public ObservableCollection<Unit> AvailableUnits
    {
        get => _availableUnits;
        set => RaiseAndSetIfChanged(ref _availableUnits, value);
    }

    private ObservableCollection<Postal> _postalList = new();

    public ObservableCollection<Postal> PostalList
    {
        get => _postalList;
        set => RaiseAndSetIfChanged(ref _postalList, value);
    }

    public Unit? AdditionalUnit
    {
        get => null;
        set
        {
            if (value != null && !CurrentScene.AdditionalUnits.Contains(value))
                CurrentScene.AdditionalUnits.Add(value);

            RaisePropertyChanged();
        }
    }

    private string? _newDetail;
    public string? NewDetail
    {
        get => _newDetail;
        set => RaiseAndSetIfChanged(ref _newDetail, value);
    }

    public ICommand RemoveAdditionalUnitCommand => new Command<Unit>(param =>
    {
        if(param == null) return;
        CurrentScene.AdditionalUnits.Remove(param);
    });

    public ICommand AddDetailCommand => new Command(() =>
    {
        if(NewDetail == null)
            return;
        CurrentScene.AddDetails(NewDetail);
        NewDetail = null;
    });
    
    public string? LocationString
    {
        get => CurrentScene.Location?.Id.ToString();
        set
        {
            if (int.TryParse(value, out var locationNumber))
                CurrentScene.Location = PostalList.FirstOrDefault(x => x.Id == locationNumber);
            RaisePropertyChanged();
        }
    }
}
