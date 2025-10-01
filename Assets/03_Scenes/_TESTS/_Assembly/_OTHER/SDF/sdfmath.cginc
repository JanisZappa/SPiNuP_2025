float merge(float shape1, float shape2)
{
    return min(shape1, shape2);
}


float round_merge(float shape1, float shape2, float radius)
{
    float2 intersectionSpace = float2(shape1 - radius, shape2 - radius);
           intersectionSpace = min(intersectionSpace, 0);

    float insideDistance  = -length(intersectionSpace);
    float simpleUnion     = merge(shape1, shape2);
    float outsideDistance = max(simpleUnion, radius);

    return insideDistance + outsideDistance;
}




float2 translate(float2 samplePosition, float2 offset)
{
    return samplePosition - offset;
}


float circle(float2 samplePosition, float radius)
{
    return length(samplePosition) - radius;
}
