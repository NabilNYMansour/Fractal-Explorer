#include "rands.hlsl"
#include "comparisons.hlsl"
#define PI 3.1415926538

struct Hit {
    float dis;
    int id;
};

Hit HitInit(float dis, float id) {
    Hit h;
    h.dis = dis;
    h.id = id;
    return h;
}

Hit uHit(Hit h1, Hit h2) {
    if (h1.dis < h2.dis) return h1; else return h2;
}

Hit iHit(Hit h1, Hit h2) {
    if (h1.dis > h2.dis) return h1; else return h2;
}

Hit dHit(Hit h1, Hit h2) {

    if (h1.dis > -h2.dis) return h1; 
    else return HitInit(-h2.dis, h2.id);
}

// from http://blog.hvidtfeldts.net/index.php/2011/08/distance-estimated-3d-fractals-iii-folding-space/
float3 planeFold(float3 p, float3 n) {
    return p - 2.0 * min(0.0, dot(p, n)) * n;
}

// from https://www.shadertoy.com/view/MdyGzw
float3 sphereFold(float3 z, float radius2, float limit) {
  float r2 = dot(z.xyz, z.xyz);
  float f = clamp(radius2 / r2, 1., limit);
  return z * f;
}

float3 customMat(float4x4 m, float3 p) {
    return mul(m, float4(p,1)).xyz;
}

float3 rotateX(float theta, float3 p) {
    float c = cos(theta);
    float s = sin(theta);

    float4x4 m = float4x4(
        float4( 1, 0, 0, 0),
        float4( 0, c,-s, 0),
        float4( 0, s, c, 0),
        float4( 0, 0, 0, 1)
    );
    
    return mul(m, float4(p,1)).xyz;
}

float3 rotateY(float theta, float3 p) {
    float c = cos(theta);
    float s = sin(theta);

    float4x4 m = float4x4(
        float4( c, 0, s, 0),
        float4( 0, 1, 0, 0),
        float4(-s, 0, c, 0),
        float4( 0, 0, 0, 1)
    );
    
    return mul(m, float4(p,1)).xyz;
}

float3 rotateZ(float theta, float3 p) {
    float c = cos(theta);
    float s = sin(theta);

    float4x4 m = float4x4(
        float4( c,-s, 0, 0),
        float4( s, c, 0, 0),
        float4( 0, 0, 1, 0),
        float4( 0, 0, 0, 1)
    );
    
    return mul(m, float4(p,1)).xyz;
}

float SDFsphere(float3 p, float r) {
    return length(p)-r;
}

float SDFbox(float3 p, float3 b) {
    float3 q = abs(p) - b;
    return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
}

float MengerSponge(float3 pos, float os, int details) {
    float b = SDFbox(pos, float3(os, os, os));
    float s = os / 3.;

    float c = os/3.;
    float3 pmod;

    for (int i = 0; i < details; i++) {
        s /= 3.;
        pmod = fmod(abs(pos + c), c * 2.) - c;
        b = max(b, -SDFbox(pmod, float3(os, s, s)));
        b = max(b, -SDFbox(pmod, float3(s, os, s)));
        b = max(b, -SDFbox(pmod, float3(s, s, os)));
        c /= 3.;
    }

    s = os / 3.;
    os += 1;

    b = max(b, -SDFbox(pos, float3(os, s, s)));
    b = max(b, -SDFbox(pos, float3(s, os, s)));
    b = max(b, -SDFbox(pos, float3(s, s, os)));

    return b;
}

float OrbitTrapValue(float3 p, float s) {
    p = fmod(abs(p+s),s*2)-s;
    
    float f1 = length(p);

    return f1;
}

#define M_COL 0 // main color of the shape
// #define X_COL 1 // color along the X of the shape
// #define Y_COL 2 // color along the Y of the shape
// #define Z_COL 3 // color along the Z of the shape

Hit MengerSpongeFolded(float3 pos, float os, int details, float3 playerPos) {
    // Hit b = HitInit(0, M_COL);
    Hit b = HitInit(SDFbox(pos, float3(os*10, os*10, os*10)), M_COL);

    // Player
    // b = dHit(b, HitInit(SDFbox(pos-playerPos,float3(5,5,5)), M_COL));

    float s = os / 3.;

    float c = os / 3.;
    float3 pmod;

    // int i1 = int(float(details)/2);
    int i1 = 0;

    float v;
    int id = 0;
    float3 n1;

    for (int i = 0; i < details; i++) {
        i1++;

        pos = rotateX(PI/(i1*2), pos);
        pos = rotateY(PI/(i1*2), pos);
        pos = rotateZ(PI/(i1*2), pos);

        n1 = normalize(float3(1,-1,1));
        pos = planeFold(pos, n1);

        s /= 3.;
        pmod = fmod(abs(pos + c), c * 2.) - c;

        v = OrbitTrapValue(pmod, c) + sin(c);
        id = int(v + tan(v));
    
        b = dHit(b, HitInit(SDFbox(pmod, float3(os, s, s)), id));
        b = dHit(b, HitInit(SDFbox(pmod, float3(s, os, s)), id));
        b = dHit(b, HitInit(SDFbox(pmod, float3(s, s, os)), id));
        c /= 3.;
    }

    s = os / 3.;

    b = dHit(b, HitInit(SDFbox(pos, float3(os, s, s)), id));
    b = dHit(b, HitInit(SDFbox(pos, float3(s, os, s)), id));
    b = dHit(b, HitInit(SDFbox(pos, float3(s, s, os)), id));

    b.dis -= 0.01; // fix for rotation flicker
    return b;
}

/*------------Made by Vlad Andreuta------------*/
#define BOX_FOLD 1.
#define FIXED_RADIUS 1.
#define MIN_RADIUS .5
#define LINEAR_SCALE 1.
#define SCALE 2.

float MandelBox(float3 pos, int details) {
float4 z = float4(pos, 1.0);
float4 c = z;
    for (int i = 0; i < details; i++) {
        z.xyz = clamp(z.xyz, -BOX_FOLD, BOX_FOLD) * 2.0 - z.xyz;

        float zDot = dot(z.xyz, z.xyz);

        if (zDot < MIN_RADIUS) z *= FIXED_RADIUS / MIN_RADIUS;
        else if (zDot < FIXED_RADIUS) z *= FIXED_RADIUS / zDot;

        z = SCALE * z + c;
    }
    return length(z.xyz) / abs(z.w);
}
/*---------------------------------------------*/


// Hit MengerSpongeFolded(float3 pos, float os, int details, float3 actualPos, float foldParam) {
//     Hit b = HitInit(SDFbox(pos, float3(os, os, os))-os/50, M_COL);
//     float s = os / 3.;

//     float c = os/3.;
//     float3 pmod;

//     int i1 = int(float(details)/2);

//     // foldParam = 0;

//     for (int i = 0; i < details; i++) {
//         i1++;

//         // pos = rotateX((PI+foldParam*1)/(i1*2), pos);
//         // pos = rotateY((PI+foldParam*2)/(i1*2), pos);
//         // pos = rotateZ((PI+foldParam*3)/(i1*2), pos);

//         s /= 3.;
//         pmod = fmod(abs(pos + c), c * 2.) - c;
//         b = dHit(b, HitInit(SDFbox(pmod, float3(os, s, s)), X_COL));
//         b = dHit(b, HitInit(SDFbox(pmod, float3(s, os, s)), Y_COL));
//         b = dHit(b, HitInit(SDFbox(pmod, float3(s, s, os)), Z_COL));
//         c /= 3.;
//     }

//     s = os / 3.;
//     os += 1;

//     b = dHit(b, HitInit(SDFbox(pos, float3(os, s, s)), X_COL));
//     b = dHit(b, HitInit(SDFbox(pos, float3(s, os, s)), Y_COL));
//     b = dHit(b, HitInit(SDFbox(pos, float3(s, s, os)), Z_COL));

//     // b.dis += os/100;

//     return b;
// }