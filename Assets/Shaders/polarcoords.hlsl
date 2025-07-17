#ifndef POLAR_COORDS_INCLUDED
#define POLAR_COORDS_INCLUDED

float2 xy2pol(float2 xy)
{
	return float2(atan2(xy.y, xy.x), length(xy));
}

float2 pol2xy(float2 pol)
{
	return pol.y * float2(cos(pol.x), sin(pol.y));
}

#endif