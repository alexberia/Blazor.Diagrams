using Blazor.Diagrams.Core.Events;
using System.Runtime.CompilerServices;
using MouseEventArgs = Microsoft.AspNetCore.Components.Web.MouseEventArgs;

namespace Blazor.Diagrams.Extensions;

public static class EventsExtensions
{
    public static PointerEventArgs ToCore(this Microsoft.AspNetCore.Components.Web.PointerEventArgs e)
    {
        return new PointerEventArgs(e.ClientX, e.ClientY, e.Button, e.Buttons, e.CtrlKey, e.ShiftKey, e.AltKey,
            e.PointerId, e.Width, e.Height, e.Pressure, e.TiltX, e.TiltY, e.PointerType, e.IsPrimary);
    }

    public static PointerEventArgs ToCore(this MouseEventArgs e)
    {
        return new PointerEventArgs(e.ClientX, e.ClientY, e.Button, e.Buttons, e.CtrlKey, e.ShiftKey, e.AltKey,
            0, 0, 0, 0, 0, 0, string.Empty, false);
    }

    public static KeyboardEventArgs ToCore(this Microsoft.AspNetCore.Components.Web.KeyboardEventArgs e)
    {
        return new KeyboardEventArgs(e.Key, e.Code, e.Location, e.CtrlKey, e.ShiftKey, e.AltKey);
    }

    public static WheelEventArgs ToCore(this Microsoft.AspNetCore.Components.Web.WheelEventArgs e)
    {
        return new WheelEventArgs(e.ClientX, e.ClientY, e.Button, e.Buttons, e.CtrlKey, e.ShiftKey, e.AltKey, e.DeltaX,
            e.DeltaY, e.DeltaZ, e.DeltaMode);
    }

    public static TouchEventArgs ToCore(this Microsoft.AspNetCore.Components.Web.TouchEventArgs e)
    {
        List<Blazor.Diagrams.Core.Events.TouchPoint> blazorTouchPoints = new List<TouchPoint>();
        List<Microsoft.AspNetCore.Components.Web.TouchPoint> microsoftTouchPoint = e.Touches.ToList();

        foreach( var touch in e.Touches)
        {
            Blazor.Diagrams.Core.Events.TouchPoint newTouchPoin = new TouchPoint(touch.Identifier, touch.ScreenX, touch.ScreenY, touch.ClientX, touch.ClientY, touch.PageX, touch.PageY);

            blazorTouchPoints.Add(newTouchPoin);
        }

        return new TouchEventArgs(blazorTouchPoints.ToArray(), e.CtrlKey, e.ShiftKey, e.AltKey);
    }
}