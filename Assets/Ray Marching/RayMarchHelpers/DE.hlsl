#include "sdf.hlsl"

Hit GetHit(float3 pos, int i) {
    return MengerSpongeFolded(pos, 100, i);
}