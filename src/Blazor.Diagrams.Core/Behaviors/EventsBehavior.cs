using Blazor.Diagrams.Core.Models.Base;
using Blazor.Diagrams.Core.Events;
using System.Diagnostics;

namespace Blazor.Diagrams.Core.Behaviors;

public class EventsBehavior : Behavior
{
    private readonly Stopwatch _mouseClickSw;
    private Model? _model;
    private bool _captureMouseMove;
    private int _mouseMovedCount;

    public EventsBehavior(Diagram diagram) : base(diagram)
    {
        _mouseClickSw = new Stopwatch();

        Diagram.PointerDown += OnPointerDown;
        Diagram.PointerMove += OnPointerMove;
        Diagram.PointerUp += OnPointerUp;
        Diagram.PointerClick += OnPointerClick;


        Diagram.TouchStart += OnTouchStart;
        Diagram.TouchMove += OnTouchMove;
        Diagram.TouchEnd += OnTouchEnd;
        Diagram.TouchEnter += OnTouchEnter;
    }

    private void OnTouchEnter(Model? model, TouchEventArgs e)
    {
        
    }

    private void OnTouchEnd(Model? model, TouchEventArgs e)
    {
        if (!_captureMouseMove) return; // Only set by OnMouseDown
        _captureMouseMove = false;
        if (_mouseMovedCount > 0) return;

        if (_model == model)
        {

            //var touches = e.ChangedTouches.First();
            //Diagram.TriggerPointerClick(model, new PointerEventArgs(touches.ClientX, touches.ClientY, e.999, e.CtrlKey, e.ShiftKey, e.AltKey, touches.Identifier));
            _model = null;
        }
    }
    private void OnTouchMove(Model? model, TouchEventArgs e)
    {
        if (!_captureMouseMove)
            return;

        _mouseMovedCount++;
    }
    private void OnTouchStart(Model? model, TouchEventArgs e)
    {
        _captureMouseMove = true;
        _mouseMovedCount = 0;
        _model = model;
    }
    private void OnPointerClick(Model? model, PointerEventArgs e)
    {
        if (_mouseClickSw.IsRunning && _mouseClickSw.ElapsedMilliseconds <= 500)
        {
            Diagram.TriggerPointerDoubleClick(model, e);
        }

        _mouseClickSw.Restart();
    }

    private void OnPointerDown(Model? model, PointerEventArgs e)
    {
        if (e.PointerType == "touch")
        {
            
        }
        else if (e.PointerType == "mouse")
        {
            _captureMouseMove = true;
            _mouseMovedCount = 0;
            _model = model;
        }
        else if (e.PointerType == "pen")
        {
            _captureMouseMove = true;
            _mouseMovedCount = 0;
            _model = model;
        }



    }

    private void OnPointerMove(Model? model, PointerEventArgs e)
    {
        if (e.PointerType == "touch")
        {

        }
        else if (e.PointerType == "mouse")
        {
            if (!_captureMouseMove)
                return;

            _mouseMovedCount++;
        }
        else if (e.PointerType == "pen")
        {
            if (!_captureMouseMove)
                return;

            _mouseMovedCount++;
        }
    }

    private void OnPointerUp(Model? model, PointerEventArgs e)
    {
        if (e.PointerType == "touch")
        {

        }
        else if (e.PointerType == "mouse")
        {
            if (!_captureMouseMove) return; // Only set by OnMouseDown
            _captureMouseMove = false;
            if (_mouseMovedCount > 0) return;

            if (_model == model)
            {
                Diagram.TriggerPointerClick(model, e);
                _model = null;
            }
        }
        else if (e.PointerType == "pen")
        {
            if (!_captureMouseMove) return; // Only set by OnMouseDown
            _captureMouseMove = false;
            if (_mouseMovedCount > 0) return;

            if (_model == model)
            {
                Diagram.TriggerPointerClick(model, e);
                _model = null;
            }
        }
    }

    public override void Dispose()
    {
        Diagram.PointerDown -= OnPointerDown;
        Diagram.PointerMove -= OnPointerMove;
        Diagram.PointerUp -= OnPointerUp;
        Diagram.PointerClick -= OnPointerClick;


        Diagram.TouchStart -= OnTouchStart;
        Diagram.TouchMove -= OnTouchMove;
        Diagram.TouchEnd -= OnTouchEnd;
        Diagram.TouchEnter -= OnTouchEnter;

        _model = null;
    }
}
