float gt(float v1, float v2)
{
    return step(v2,v1);
}

float lt(float v1, float v2)
{
    return step(v1, v2);
}

float between(float val, float start, float end)
{
    return gt(val,start)*lt(val,end);
}

float eq(float v1, float v2, float e)
{
    return between(v1, v2-e, v2+e);
}

float s_gt(float v1, float v2, float e)
{
    return smoothstep(v2-e, v2+e, v1);
}

float s_lt(float v1, float v2, float e)
{
    return smoothstep(v1-e, v1+e, v2);
}

float s_between(float val, float start, float end, float epsilon)
{
    return s_gt(val,start,epsilon)*s_lt(val,end,epsilon);
}

float s_eq(float v1, float v2, float e, float s_e)
{
    return s_between(v1, v2-e, v2+e, s_e);
}