using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Dispatch.WPF.Helpers;

namespace Dispatch.WPF.Models;
public class Scene : ObservableObject
{
    public DateTime SceneStart = DateTime.Now;
    public DateTime SceneEnd { get; set; }

    private string? _title;
    public string? Title
    {
        get => _title;
        set => RaiseAndSetIfChanged(ref _title, value);
    }

    private Unit? _primaryUnit;
    public Unit? PrimaryUnit
    {
        get => _primaryUnit;
        set => RaiseAndSetIfChanged(ref _primaryUnit, value);
    }

    public ObservableCollection<Unit> AdditionalUnits { get; set; } = new();

    private readonly ObservableCollection<TimedDetail> _details = new ();

    public ReadOnlyObservableCollection<TimedDetail> Details { get; }

    private Postal? _location;
    public Postal? Location
    {
        get => _location;
        set
        {
            if (_location == value) return;

            if (_location != null)
                LocationHistory.Add((_location, DateTime.Now));

            RaiseAndSetIfChanged(ref _location, value);
        }
    }

    private List<(Postal postal, DateTime time)> LocationHistory { get; } = new();

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"{Title}");
        stringBuilder.AppendLine($"Start: {SceneStart}, End: {SceneEnd}");
        stringBuilder.AppendLine($"Primary unit: {PrimaryUnit?.FullName}");
        stringBuilder.AppendLine($"Secondary units: {string.Join(", ", AdditionalUnits.Select(x => x.FullName))}");

        foreach (var eventHistoryElement in LocationHistory.Select(x => new EventHistoryElement(x.time, x.postal)).Union(Details.Select(x => new EventHistoryElement(x.Time, x.Detail))).OrderBy(x => x.EventTime))
        {
            switch (eventHistoryElement.Event)
            {
                case Postal postal:
                    stringBuilder.AppendLine($"{eventHistoryElement.EventTime} - New location: {postal.Id}");
                    break;
                case string:
                    stringBuilder.AppendLine($"{eventHistoryElement.EventTime} - {eventHistoryElement.Event}");
                    break;
            }
        }
        stringBuilder.AppendLine("=========================================================================================");
        return stringBuilder.ToString();
    }

    private class EventHistoryElement
    {
        public DateTime EventTime { get; set; }
        public object Event { get; set; }

        public EventHistoryElement(DateTime eventTime, object @event)
        {
            EventTime = eventTime;
            Event = @event;
        }
    }

    public Scene()
    {
        Details = new ReadOnlyObservableCollection<TimedDetail>(_details);
    }

    public void AddDetails(string detail)
    {
        _details.Add(new TimedDetail(detail));
    }
}
