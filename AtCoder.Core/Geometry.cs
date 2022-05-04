using System;


class Geometry
{
    //ラジアンを度数法に変換
    double ToAngle(double radian) { return (double)(radian * 180 / Math.PI); }

    //度数法をラジアン(π表記)に変換
    double ToRadian(double angle) { return (double)(angle * Math.PI / 180); }

    //線分abとcdの交差判定
    bool IsIentersected(double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy)
    {
        var ta = (cx - dx) * (ay - cy) + (cy - dy) * (cx - ax);
        var tb = (cx - dx) * (by - cy) + (cy - dy) * (cx - bx);
        var tc = (ax - bx) * (cy - ay) + (ay - by) * (ax - cx);
        var td = (ax - bx) * (dy - ay) + (ay - by) * (ax - dx);
        return tc * td < 0 && ta * tb < 0;
        // return tc * td <= 0 && ta * tb <= 0; // 端点を含む場合
    }
}

