#define MAX_STEPS 100
#define MAX_DIS 200
#define HIT_EPS 0.001
#define SLOPE_EPS 0.0000001

#include "DE.hlsl"

struct March {
    Hit hit;
    float disMarched;
    float minDis;
    int steps;
};

March RayMarch(float3 ro, float3 rd, float depth, int i, float3 playerPos) {
    March m;
    m.hit.dis = 0;
    m.hit.id = -1;
    m.disMarched = 0;
    m.minDis = MAX_DIS;
    m.steps = 0;
    
    Hit cd;
    float3 p;

    float e = HIT_EPS;

    while (m.disMarched < MAX_DIS) {
        p = ro + m.disMarched * rd;
        cd = GetHit(p, i, playerPos);
        if (abs(cd.dis) < e || m.disMarched >= depth || m.disMarched > MAX_DIS) break;

        m.disMarched += cd.dis;
        m.minDis = min(m.minDis, cd.dis);
        m.steps++;

        e *= 1.2;
    }

    m.hit.dis = cd.dis;
    m.hit.id = cd.id;

    if (m.disMarched >= depth) m.disMarched = depth;
    return m;
}

// from https://www.shadertoy.com/view/MtlXR4
float3 GetNormal(float3 pos , int i, float playerPos)
{
    const float eps = HIT_EPS;

    const float3 v1 = float3( 1.0,-1.0,-1.0);
    const float3 v2 = float3(-1.0,-1.0, 1.0);
    const float3 v3 = float3(-1.0, 1.0,-1.0);
    const float3 v4 = float3( 1.0, 1.0, 1.0);

	return normalize( v1*GetHit(pos + v1*eps, i, playerPos).dis + 
					  v2*GetHit(pos + v2*eps, i, playerPos).dis + 
					  v3*GetHit(pos + v3*eps, i, playerPos).dis + 
					  v4*GetHit(pos + v4*eps, i, playerPos).dis );
}

/*-------------UNUSED BAKED-------------*/
// float4 sampleSDFtexture(sampler3D sdf, float3 p) {
//     return tex3D(sdf, (p/SAMPLER_SIZE) + 0.5);
// }

// float3 GetNormal(float3 pos, sampler3D sdf) {
//     return sampleSDFtexture(sdf, pos).gba;
// }

// March RayMarch(float3 ro, float3 rd, float depth, sampler3D sdf) {
//     March m;
//     m.hit.dis = 0;
//     m.hit.id = -1;
//     m.disMarched = 0;
//     m.minDis = MAX_DIS;
//     m.steps = 0;
    
//     Hit cd;
//     float3 p;

//     // while (m.disMarched < MAX_DIS) {
//     for (int i = 0; i < MAX_STEPS; i++) {
//         p = ro + m.disMarched * rd;
//         cd.dis = sampleSDFtexture(sdf, p).r;// + snoise(p.xz/10)/10 + snoise(p.xy/10)/10;
//         cd.id = 0;
//         if (abs(cd.dis) < HIT_EPS || m.disMarched >= depth || m.disMarched > MAX_DIS) break;

//         m.hit = cd;
//         m.hit.id = cd.id;
//         m.disMarched += cd.dis;
//         m.minDis = min(m.minDis, cd.dis);
//         m.steps++;
//     }

//     if (m.disMarched >= depth) m.disMarched = depth;

//     return m;
// }
/*--------------------------------------*/

/*-------------UNUSED LIGHTING-------------*/
// From https://iquilezles.org/articles/rmshadows/
// float LightMarch(float3 hp, float3 n, float3 l, float3 lpos, float k, int i, float f) { // SOFT shadows
//     // inits
//     float lf = 1.; // lit factor
//     float3 initPos = hp+n*HIT_EPS*5; // counting for offset
//     float3 currpos = initPos;
//     float posTOlpos = distance(currpos, lpos);
    
//     float preDis;
//     float currDis;
//     float currDis2;
    
//     // marching
//     for (float disCovered = 0.; disCovered < MAX_DIS;) {
//         if (disCovered > posTOlpos) break;
        
//         currpos = initPos + l*disCovered;
//         currDis = GetHit(currpos, i, f).dis;
//         if (currDis < HIT_EPS) return 0.;
        
//         currDis2 = currDis*currDis;
        
//         float y = currDis2/(2.0*preDis);
//         float d = sqrt(currDis2-y*y);
        
//         // lf = min(lf, k*d/max(0.0,disCovered-y));
//         lf = min(lf, k*currDis/disCovered);
//         preDis = currDis;
//         disCovered += currDis;
//     }
//     return lf;
// }

// from https://iquilezles.org/articles/normalsSDF/
// float3 GetNormal(float3 pos, int i, float time)
// {
//     float3 n = float3(0, 0, 0);
//     for(int i=0; i<4; i++)
//     {
//         float3 e = 0.5773*(2.0*float3((((i+3)>>1)&1),((i>>1)&1),(i&1))-1.0);
//         n += e*GetHit(pos+e*SLOPE_EPS, i, time).dis;
//     }
//     return normalize(n);
// }
/*---------------------------------------*/
