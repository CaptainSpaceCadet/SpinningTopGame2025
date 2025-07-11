// from https://www.shadertoy.com/view/fsjcWG

// psrdnoise (c) Stefan Gustavson and Ian McEwan,
// ver. 2021-12-02, published under the MIT license:
// https://github.com/stegu/psrdnoise/

float psrdnoise(float2 x, float2 period, float alpha, out float2 gradient)
{
    float2 uv = float2(x.x + x.y * 0.5, x.y);
    float2 i0 = floor(uv);
    float2 f0 = frac(uv);

    float cmp = step(f0.y, f0.x);
    float2 o1 = float2(cmp, 1.0 - cmp);
    float2 i1 = i0 + o1;
    float2 i2 = i0 + 1.0;

    float2 v0 = float2(i0.x - i0.y * 0.5, i0.y);
    float2 v1 = float2(v0.x + o1.x - o1.y * 0.5, v0.y + o1.y);
    float2 v2 = float2(v0.x + 0.5, v0.y + 1.0);

    float2 x0 = x - v0;
    float2 x1 = x - v1;
    float2 x2 = x - v2;

    float3 iu, iv, xw, yw;
    if (any(period > 0.0))
    {
        xw = float3(v0.x, v1.x, v2.x);
        yw = float3(v0.y, v1.y, v2.y);

        if (period.x > 0.0)
            xw = fmod(xw, period.x);
        if (period.y > 0.0)
            yw = fmod(yw, period.y);

        iu = floor(xw + 0.5 * yw + 0.5);
        iv = floor(yw + 0.5);
    }
    else
    {
        iu = float3(i0.x, i1.x, i2.x);
        iv = float3(i0.y, i1.y, i2.y);
    }

    float3 hash = fmod(iu, 289.0);
    hash = fmod((hash * 51.0 + 2.0) * hash + iv, 289.0);
    hash = fmod((hash * 34.0 + 10.0) * hash, 289.0);

    float3 psi = hash * 0.07482 + alpha;
    float3 gx = cos(psi);
    float3 gy = sin(psi);

    float2 g0 = float2(gx.x, gy.x);
    float2 g1 = float2(gx.y, gy.y);
    float2 g2 = float2(gx.z, gy.z);

    float3 w = 0.8 - float3(dot(x0, x0), dot(x1, x1), dot(x2, x2));
    w = max(w, 0.0);
    float3 w2 = w * w;
    float3 w4 = w2 * w2;

    float3 gdotx = float3(dot(g0, x0), dot(g1, x1), dot(g2, x2));
    float n = dot(w4, gdotx);

    float3 w3 = w2 * w;
    float3 dw = -8.0 * w3 * gdotx;

    float2 dn0 = w4.x * g0 + dw.x * x0;
    float2 dn1 = w4.y * g1 + dw.y * x1;
    float2 dn2 = w4.z * g2 + dw.z * x2;

    gradient = 10.9 * (dn0 + dn1 + dn2);
    return 10.9 * n;
}

void hexgrid(float2 v,
    out float2 p0, out float2 i0,
    out float2 p1, out float2 i1,
    out float2 p2, out float2 i2)
{
    const float stretch = 1.0 / 0.8660;
    const float squash = 0.8660;

    v.y = v.y * stretch;

    float2 uv = float2(v.x + v.y * 0.5, v.y);
    i0 = floor(uv);
    float2 f0 = frac(uv);

    float cmp = step(f0.y, f0.x);
    float2 o1 = float2(cmp, 1.0 - cmp);

    i1 = i0 + o1;
    i2 = i0 + float2(1.0, 1.0);

    p0 = float2(i0.x - i0.y * 0.5, i0.y);
    p1 = float2(p0.x + o1.x - o1.y * 0.5, p0.y + o1.y);
    p2 = float2(p0.x + 0.5, p0.y + 1.0);

    p0.y *= squash;
    p1.y *= squash;
    p2.y *= squash;
}


float wavelet(float2 x, float2 p, float2 g, float alpha)
{
    float2 d = x - p;
    float w = 0.8 - dot(d, d);
    w = max(w, 0.0);
    float w2 = w * w;
    return w2 * sin(fmod(dot(d, g) + alpha, 1.0) * 2.0 * 6.2832);
}

float3 perm1(float3 i)
{
    float3 im = fmod(i, 289.0);
    return fmod(((im * 34.0) + 10.0) * im, 289.0);
}

float3 perm2(float3 i)
{
    float3 im = fmod(i, 361.0);
    return fmod((im * 38.0 + 8.0) * im, 361.0);
}

float3 hashphase(float3 iu, float3 iv)
{
    return perm1(perm2(iu) + iv) * (1.0 / 289.0);
}

