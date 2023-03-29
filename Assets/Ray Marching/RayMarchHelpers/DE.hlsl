#include "sdf.hlsl"

Hit GetHit(float3 pos, int i, float3 playerPos) {
    // p = fmod(abs(p+size), size*2)-size;
    // p.xy = fmod(abs(p-size), size*2)-size;

    // float time = size;
    // p.x += gt(fmod(abs(p.y)+size, size*4), size*2)*lt(fmod(time, size*4), size*2)*time;
    // p.y += gt(fmod(abs(p.y)+size, size*4), size*2)*gt(fmod(time, size*4), size*2)*time;
    // p.xy = fmod(abs(p.xy)+size, size*2.)-size;

    return MengerSpongeFolded(pos, 100, i, playerPos);

    // Hit h;
    // h.dis = SDFbox(p, float3(16,16,16));
    // h.dis = SDFsphere(p, 16);
    // h.id = 1;
    // return h;
}