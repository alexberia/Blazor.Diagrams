using Blazor.Diagrams.Core.Events;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models.Base;
using System;

namespace Blazor.Diagrams.Core.Behaviors;

public class ZoomBehavior : Behavior
{

    private double _initialDistance;
    private Point _initialCenter;
    private double _initialZoom;
    private bool _isPinching = false;
    private Point _lastTouchCenter;


    public ZoomBehavior(Diagram diagram) : base(diagram)
    {
        Diagram.Wheel += Diagram_Wheel;

        Diagram.TouchStart += TouchStart;
        Diagram.TouchEnd += TouchEnd;
        Diagram.TouchEnter += TouchEnter;
        Diagram.TouchMove += TouchMove;
        Diagram.TouchLeave += TouchLeave;
    }
    private void TouchStart(Model? model, TouchEventArgs e)
    {
        //Diagram.TriggerOnDebug($"ZoomBehaviorTouchStart > Touches: {e.ChangedTouches.Length}" );

        if (e.ChangedTouches.Length == 2)
        {
            
            _isPinching = true;

            var p1 = new Point(e.ChangedTouches[0].ClientX, e.ChangedTouches[0].ClientY);
            var p2 = new Point(e.ChangedTouches[1].ClientX, e.ChangedTouches[1].ClientY);

            _initialDistance = GetDistance(p1, p2);
            _initialCenter = GetMidpoint(p1, p2);
            _lastTouchCenter = GetMidpoint(p1, p2);
            _initialZoom = Diagram.Zoom;

            Diagram.TriggerOnDebug($"ZoomBehavior -> TouchStart > Touches: {e.ChangedTouches.Length} Pinching: {_isPinching}");
        }
    }

    private double GetDistance(Point p1, Point p2)
    {
        var dx = p2.X - p1.X;
        var dy = p2.Y - p1.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private Point GetMidpoint(Point p1, Point p2)
    {
        return new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
    }

    private void TouchEnter(Model? model, TouchEventArgs e)
    {
        if (e.ChangedTouches.Length == 1)
        {
        }
    }

    private void ApplyPinchZoom(Point p1, Point p2, double initialDistance, double initialZoom)
    {
        var center = GetMidpoint(p1, p2);
        var newDistance = GetDistance(p1, p2);
        var scale = newDistance / initialDistance;
        var newZoom = Math.Clamp(initialZoom * scale, Diagram.Options.Zoom.Minimum, Diagram.Options.Zoom.Maximum);

        if (Math.Abs(newZoom - Diagram.Zoom) < 0.001)
            return;

        var container = Diagram.Container;

        // Converte o centro da pinça para coordenadas do mundo
        var logicalCenter = new Point(
            (center.X - container.Left - Diagram.Pan.X) / Diagram.Zoom,
            (center.Y - container.Top - Diagram.Pan.Y) / Diagram.Zoom
        );

        // Recalcula o pan para manter o ponto lógico fixo
        var newPanX = center.X - container.Left - logicalCenter.X * newZoom;
        var newPanY = center.Y - container.Top - logicalCenter.Y * newZoom;

        // Ajuste de pan baseado no deslocamento da pinça (frame a frame)
        var centerDelta = center - _lastTouchCenter;
        _lastTouchCenter = center;

        newPanX += centerDelta.X;
        newPanY += centerDelta.Y;

        Diagram.Batch(() =>
        {
            Diagram.SetZoom(newZoom);
            Diagram.SetPan(newPanX, newPanY);
        });

        Diagram.TriggerOnDebug($"[PinchZoom] Zoom: {newZoom:F3}, Pan: {newPanX:F3}, {newPanY:F3}, ΔCenter: {centerDelta.X:F2}, {centerDelta.Y:F2}");
    }
    private void TouchMove(Model? model, TouchEventArgs e)
    {
        if (_isPinching && e.ChangedTouches.Length == 2)
        {
            var p1 = new Point(e.ChangedTouches[0].ClientX, e.ChangedTouches[0].ClientY);
            var p2 = new Point(e.ChangedTouches[1].ClientX, e.ChangedTouches[1].ClientY);
            ApplyPinchZoom(p1, p2, _initialDistance, _initialZoom);
        }
    }
    private void TouchEnd(Model? model, TouchEventArgs e)
    {
        Diagram.TriggerOnDebug($"TouchEnd.");

        if (e.ChangedTouches.Length < 2)
            _isPinching = false;
    }
    private void TouchLeave(Model? model, TouchEventArgs e)
    {
        if (e.ChangedTouches.Length == 1)
        {
        }
    } 


    private void Diagram_Wheel(WheelEventArgs e)
    {
        if (Diagram.Container == null || e.DeltaY == 0)
            return;

        if (!Diagram.Options.Zoom.Enabled)
            return;

        var scale = Diagram.Options.Zoom.ScaleFactor;
        var oldZoom = Diagram.Zoom;
        var deltaY = Diagram.Options.Zoom.Inverse ? e.DeltaY * -1 : e.DeltaY;
        var newZoom = deltaY > 0 ? oldZoom * scale : oldZoom / scale;
        newZoom = Math.Clamp(newZoom, Diagram.Options.Zoom.Minimum, Diagram.Options.Zoom.Maximum);

        if (newZoom < 0 || newZoom == Diagram.Zoom)
            return;

        // Other algorithms (based only on the changes in the zoom) don't work for our case
        // This solution is taken as is from react-diagrams (ZoomCanvasAction)
        var clientWidth = Diagram.Container.Width;
        var clientHeight = Diagram.Container.Height;
        var widthDiff = clientWidth * newZoom - clientWidth * oldZoom;
        var heightDiff = clientHeight * newZoom - clientHeight * oldZoom;
        var clientX = e.ClientX - Diagram.Container.Left;
        var clientY = e.ClientY - Diagram.Container.Top;
        var xFactor = (clientX - Diagram.Pan.X) / oldZoom / clientWidth;
        var yFactor = (clientY - Diagram.Pan.Y) / oldZoom / clientHeight;
        var newPanX = Diagram.Pan.X - widthDiff * xFactor;
        var newPanY = Diagram.Pan.Y - heightDiff * yFactor;

        Diagram.Batch(() =>
        {
            Diagram.SetPan(newPanX, newPanY);
            Diagram.SetZoom(newZoom);
        });

        Diagram.TriggerOnDebug($"ZoomBehavior -> Wheel : newZoom: {newZoom.ToString("F3")} newPan X:{newPanX.ToString("F3")} Y: {newPanY.ToString("F3")}");
    }

    public override void Dispose()
    {
        Diagram.Wheel -= Diagram_Wheel;

        Diagram.TouchStart -= TouchStart;
        Diagram.TouchEnd -= TouchEnd;
        Diagram.TouchEnter -= TouchEnter;
        Diagram.TouchMove -= TouchMove;
        Diagram.TouchLeave -= TouchLeave;
    }
}
