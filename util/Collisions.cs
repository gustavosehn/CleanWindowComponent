using CleanWindowComponent;
using System;

namespace cwc.util;

public static class Collisions
{
    public static bool IsPointInside(Rectangle rect, double px, double py)
    {
        return rect.IsPointInside(px, py);
    }

    public static bool IsPointInside(double rectX, double rectY, double rectWidth, double rectHeight, 
                                     double cornerRadius, double px, double py)
    {
        return IsPointInsideCustom(rectX, rectY, rectWidth, rectHeight, cornerRadius, cornerRadius, cornerRadius, cornerRadius, px, py);
    }

    public static bool IsPointInsideCustom(double rectX, double rectY, double rectWidth, double rectHeight, 
                                           double rTL, double rTR, double rBL, double rBR, 
                                           double px, double py)
    {
        double left = rectX;
        double right = rectX + rectWidth;
        double top = rectY;
        double bottom = rectY + rectHeight;

        if (px < left || px > right || py < top || py > bottom)
            return false;

        if (px < left + rTL && py < top + rTL)
        {
            double dx = px - (left + rTL);
            double dy = py - (top + rTL);
            return dx * dx + dy * dy <= rTL * rTL;
        }

        if (px > right - rTR && py < top + rTR)
        {
            double dx = px - (right - rTR);
            double dy = py - (top + rTR);
            return dx * dx + dy * dy <= rTR * rTR;
        }

        if (px < left + rBL && py > bottom - rBL)
        {
            double dx = px - (left + rBL);
            double dy = py - (bottom - rBL);
            return dx * dx + dy * dy <= rBL * rBL;
        }

        if (px > right - rBR && py > bottom - rBR)
        {
            double dx = px - (right - rBR);
            double dy = py - (bottom - rBR);
            return dx * dx + dy * dy <= rBR * rBR;
        }

        return true;
    }
}