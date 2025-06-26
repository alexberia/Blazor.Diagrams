using System;
using Blazor.Diagrams.Core.Events;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models.Base;
using Microsoft.AspNetCore.Components;

namespace Blazor.Diagrams.Components.Widgets;

public partial class SelectionBoxWidget : IDisposable
{
    private Point? _initialClientPoint;
    private Size? _selectionBoxSize; // Todo: remove unneeded instantiations
    private Point? _selectionBoxTopLeft; // Todo: remove unneeded instantiations

    [CascadingParameter] public BlazorDiagram BlazorDiagram { get; set; } = null!;

    [Parameter] public string Background { get; set; } = "rgb(110 159 212 / 25%)";

    public void Dispose()
    {
        //BlazorDiagram.PointerDoubleClick -= OnPointerDoubleClick;
        BlazorDiagram.PointerDown -= OnPointerDown;
        BlazorDiagram.PointerMove -= OnPointerMove;
        BlazorDiagram.PointerUp -= OnPointerUp;
    }

    protected override void OnInitialized()
    {
        BlazorDiagram.PointerDown += OnPointerDown;
        BlazorDiagram.PointerMove += OnPointerMove;
        BlazorDiagram.PointerUp += OnPointerUp;
        //BlazorDiagram.PointerDoubleClick += OnPointerDoubleClick;
    }

    //private void OnPointerDoubleClick(Model? model, MouseEventArgs e)
    //{
    //    if (model != null || e.Button != 0)
    //        return;
    //    _initialClientPoint = new Point(e.ClientX, e.ClientY);

    //}

    private string GenerateStyle()
    {
        return FormattableString.Invariant(
            $"position: absolute; background: {Background}; top: {_selectionBoxTopLeft!.Y}px; left: {_selectionBoxTopLeft.X}px; width: {_selectionBoxSize!.Width}px; height: {_selectionBoxSize.Height}px;");
    }

    private void OnPointerDown(Model? model, PointerEventArgs e)
    {
        if (e.PointerType == "touch")
        {
            if (model != null || e.Button != 0)
                return;

            BlazorDiagram.UnselectAll();
            _initialClientPoint = new Point(e.ClientX, e.ClientY);

            InvokeAsync(StateHasChanged);
        }
        else if (e.PointerType == "mouse")
        {
            if (model != null || e.Button != 0)
                return;

            BlazorDiagram.UnselectAll();
            _initialClientPoint = new Point(e.ClientX, e.ClientY);

            InvokeAsync(StateHasChanged);
        }

    }

    private void OnPointerMove(Model? model, PointerEventArgs e)
    {
        if (e.PointerType == "touch")
        {

        }
        else if (e.PointerType == "mouse")
        {
            if (_initialClientPoint == null)
                return;

            SetSelectionBoxInformation(e);

            var start = BlazorDiagram.GetRelativeMousePoint(_initialClientPoint.X, _initialClientPoint.Y);
            var end = BlazorDiagram.GetRelativeMousePoint(e.ClientX, e.ClientY);
            var (sX, sY) = (Math.Min(start.X, end.X), Math.Min(start.Y, end.Y));
            var (eX, eY) = (Math.Max(start.X, end.X), Math.Max(start.Y, end.Y));
            var bounds = new Rectangle(sX, sY, eX, eY);

            foreach (var node in BlazorDiagram.Nodes)
            {
                var nodeBounds = node.GetBounds();
                if (nodeBounds == null)
                    continue;

                if (bounds.Overlap(nodeBounds))
                    BlazorDiagram.SelectModel(node, false);
                else if (node.Selected)
                    BlazorDiagram.UnselectModel(node);
            }

            InvokeAsync(StateHasChanged);
        }


        
    }

    private void SetSelectionBoxInformation(PointerEventArgs e)
    {
        var start = BlazorDiagram.GetRelativePoint(_initialClientPoint!.X, _initialClientPoint.Y);
        var end = BlazorDiagram.GetRelativePoint(e.ClientX, e.ClientY);
        var (sX, sY) = (Math.Min(start.X, end.X), Math.Min(start.Y, end.Y));
        var (eX, eY) = (Math.Max(start.X, end.X), Math.Max(start.Y, end.Y));
        _selectionBoxTopLeft = new Point(sX, sY);
        _selectionBoxSize = new Size(eX - sX, eY - sY);
    }

    private void OnPointerUp(Model? model, PointerEventArgs e)
    {
        if (e.PointerType == "touch")
        {

        }
        else if (e.PointerType == "mouse")
        {

            _initialClientPoint = null;
            _selectionBoxTopLeft = null;
            _selectionBoxSize = null;

            InvokeAsync(StateHasChanged);
        }
    }
}