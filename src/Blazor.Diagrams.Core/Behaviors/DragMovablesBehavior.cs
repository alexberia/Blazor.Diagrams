using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models.Base;
using Blazor.Diagrams.Core.Events;
using System;
using System.Collections.Generic;
using Blazor.Diagrams.Core.Models;

namespace Blazor.Diagrams.Core.Behaviors;

public class DragMovablesBehavior : Behavior
{
    private readonly Dictionary<MovableModel, Point> _initialPositions;
    private double? _lastClientX;
    private double? _lastClientY;
    private bool _moved;

    public DragMovablesBehavior(Diagram diagram) : base(diagram)
    {
        _initialPositions = new Dictionary<MovableModel, Point>();
        Diagram.PointerDown += OnPointerDown;
        Diagram.PointerMove += OnPointerMove;
        Diagram.PointerUp += OnPointerUp;
        Diagram.TouchStart += OnTouchStart;
        Diagram.TouchEnter += OnTouchEnter;
        Diagram.TouchEnd += OnTouchEnd;
        Diagram.TouchLeave += OnTouchLeave;
        Diagram.TouchMove += OnTouchMove;
        Diagram.TouchCancel += OnTouchCancel;
    }

    private void OnTouchCancel(Model? arg1, TouchEventArgs arg2)
    {

    }

    private void OnTouchEnter(Model? model, TouchEventArgs e)
    {

    }
    private void OnTouchMove(Model? model, TouchEventArgs e)
    {

        var firstTouch = e.ChangedTouches?.FirstOrDefault();

        if (_initialPositions.Count == 0 || _lastClientX == null || _lastClientY == null)
            return;

        _moved = true;
        var deltaX = (firstTouch.ClientX - _lastClientX.Value) / Diagram.Zoom;
        var deltaY = (firstTouch.ClientY - _lastClientY.Value) / Diagram.Zoom;

        foreach (var (movable, initialPosition) in _initialPositions)
        {
            var ndx = ApplyGridSize(deltaX + initialPosition.X);
            var ndy = ApplyGridSize(deltaY + initialPosition.Y);
            if (Diagram.Options.GridSnapToCenter && movable is NodeModel node)
            {
                node.SetPosition(ndx - (node.Size?.Width ?? 0) / 2, ndy - (node.Size?.Height ?? 0) / 2);
            }
            else
            {
                movable.SetPosition(ndx, ndy);
            }
        }

    }
    private void OnTouchLeave(Model? model, TouchEventArgs e)
    {

    }

    private void OnTouchEnd(Model? model, TouchEventArgs e)
    {
        if (_initialPositions.Count == 0)
            return;

        if (_moved)
        {
            foreach (var (movable, _) in _initialPositions)
            {
                movable.TriggerMoved();
            }
        }

        _initialPositions.Clear();
        _lastClientX = null;
        _lastClientY = null;
    }

    private void OnTouchStart(Model? model, TouchEventArgs e)
    {
        var firstTouch = e.ChangedTouches?.FirstOrDefault();

        if (model is not MovableModel)
            return;

        _initialPositions.Clear();
        foreach (var sm in Diagram.GetSelectedModels())
        {
            if (sm is not MovableModel movable || movable.Locked)
                continue;

            // Special case: groups without auto size on
            if (sm is NodeModel node && node.Group != null && !node.Group.AutoSize)
                continue;

            var position = movable.Position;
            if (Diagram.Options.GridSnapToCenter && movable is NodeModel n)
            {
                position = new Point(movable.Position.X + (n.Size?.Width ?? 0) / 2,
                    movable.Position.Y + (n.Size?.Height ?? 0) / 2);
            }

            _initialPositions.Add(movable, position);
        }

        _lastClientX = firstTouch.ClientX;
        _lastClientY = firstTouch.ClientY;
        _moved = false;
    }

    private void OnPointerDown(Model? model, PointerEventArgs e)
    {
        if (e.PointerType == "touch")
        {


        }
        else if (e.PointerType == "mouse")
        {
            if (model is not MovableModel)
                return;

            _initialPositions.Clear();
            foreach (var sm in Diagram.GetSelectedModels())
            {
                if (sm is not MovableModel movable || movable.Locked)
                    continue;

                // Special case: groups without auto size on
                if (sm is NodeModel node && node.Group != null && !node.Group.AutoSize)
                    continue;

                var position = movable.Position;
                if (Diagram.Options.GridSnapToCenter && movable is NodeModel n)
                {
                    position = new Point(movable.Position.X + (n.Size?.Width ?? 0) / 2,
                        movable.Position.Y + (n.Size?.Height ?? 0) / 2);
                }

                _initialPositions.Add(movable, position);
            }

            _lastClientX = e.ClientX;
            _lastClientY = e.ClientY;
            _moved = false;
        }
        else if (e.PointerType == "pen")
        {
            if (model is not MovableModel)
                return;

            _initialPositions.Clear();
            foreach (var sm in Diagram.GetSelectedModels())
            {
                if (sm is not MovableModel movable || movable.Locked)
                    continue;

                // Special case: groups without auto size on
                if (sm is NodeModel node && node.Group != null && !node.Group.AutoSize)
                    continue;

                var position = movable.Position;
                if (Diagram.Options.GridSnapToCenter && movable is NodeModel n)
                {
                    position = new Point(movable.Position.X + (n.Size?.Width ?? 0) / 2,
                        movable.Position.Y + (n.Size?.Height ?? 0) / 2);
                }

                _initialPositions.Add(movable, position);
            }

            _lastClientX = e.ClientX;
            _lastClientY = e.ClientY;
            _moved = false;
        }


    }

    private void OnPointerMove(Model? model, PointerEventArgs e)
    {

        //Console.WriteLine("Drag OnPointerMove called: model null:" + (model is null));

        if (e.PointerType == "touch")
        {
            //if (_initialPositions.Count == 0 || _lastClientX == null || _lastClientY == null)
            //    return;

            //_moved = true;
            //var deltaX = (e.ClientX - _lastClientX.Value) / Diagram.Zoom;
            //var deltaY = (e.ClientY - _lastClientY.Value) / Diagram.Zoom;

            //foreach (var (movable, initialPosition) in _initialPositions)
            //{
            //    var ndx = ApplyGridSize(deltaX + initialPosition.X);
            //    var ndy = ApplyGridSize(deltaY + initialPosition.Y);
            //    if (Diagram.Options.GridSnapToCenter && movable is NodeModel node)
            //    {
            //        node.SetPosition(ndx - (node.Size?.Width ?? 0) / 2, ndy - (node.Size?.Height ?? 0) / 2);
            //    }
            //    else
            //    {
            //        movable.SetPosition(ndx, ndy);
            //    }
            //}

        }
        else if (e.PointerType == "mouse")
        {
            if (_initialPositions.Count == 0 || _lastClientX == null || _lastClientY == null)
                return;

            _moved = true;
            var deltaX = (e.ClientX - _lastClientX.Value) / Diagram.Zoom;
            var deltaY = (e.ClientY - _lastClientY.Value) / Diagram.Zoom;

            foreach (var (movable, initialPosition) in _initialPositions)
            {
                var ndx = ApplyGridSize(deltaX + initialPosition.X);
                var ndy = ApplyGridSize(deltaY + initialPosition.Y);
                if (Diagram.Options.GridSnapToCenter && movable is NodeModel node)
                {
                    node.SetPosition(ndx - (node.Size?.Width ?? 0) / 2, ndy - (node.Size?.Height ?? 0) / 2);
                }
                else
                {
                    movable.SetPosition(ndx, ndy);
                }
            }
        }


    }

    private void OnPointerUp(Model? model, PointerEventArgs e)
    {
        if (e.PointerType == "touch")
        {

        }
        else if (e.PointerType == "mouse")
        {
            if (_initialPositions.Count == 0)
                return;

            if (_moved)
            {
                foreach (var (movable, _) in _initialPositions)
                {
                    movable.TriggerMoved();
                }
            }

            _initialPositions.Clear();
            _lastClientX = null;
            _lastClientY = null;
        }
        else if (e.PointerType == "pen")
        {
            if (_initialPositions.Count == 0)
                return;

            if (_moved)
            {
                foreach (var (movable, _) in _initialPositions)
                {
                    movable.TriggerMoved();
                }
            }

            _initialPositions.Clear();
            _lastClientX = null;
            _lastClientY = null;
        }
    }

    private double ApplyGridSize(double n)
    {
        if (Diagram.Options.GridSize == null)
            return n;

        var gridSize = Diagram.Options.GridSize.Value;
        return gridSize * Math.Floor((n + gridSize / 2.0) / gridSize);
    }

    public override void Dispose()
    {
        _initialPositions.Clear();

        Diagram.PointerDown -= OnPointerDown;
        Diagram.PointerMove -= OnPointerMove;
        Diagram.PointerUp -= OnPointerUp;

        Diagram.TouchStart -= OnTouchStart;
        Diagram.TouchEnter -= OnTouchEnter;
        Diagram.TouchEnd -= OnTouchEnd;
        Diagram.TouchLeave -= OnTouchLeave;
        Diagram.TouchMove -= OnTouchMove;
        Diagram.TouchCancel -= OnTouchCancel;
    }
}
