//// Random functions
// From https://gist.github.com/patriciogonzalezvivo/670c22f3966e662d2f83
float rand(float3 co) {
    return frac(sin(dot(co, float3(12.9898, 53.18324, 78.233))) * 439758.5453);
}

float3 permute(float3 x) { return fmod(((x*34.0)+1.0)*x, 289.0); }

float snoise(float2 v) {
  const float4 C = float4(0.211324865405187, 0.366025403784439,
           -0.577350269189626, 0.024390243902439);
  float2 i  = floor(v + dot(v, C.yy) );
  float2 x0 = v -   i + dot(i, C.xx);
  float2 i1;
  i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
  float4 x12 = x0.xyxy + C.xxzz;
  x12.xy -= i1;
  i = fmod(i, 289.0);
  float3 p = permute( permute( i.y + float3(0.0, i1.y, 1.0 ))
  + i.x + float3(0.0, i1.x, 1.0 ));
  float3 m = max(0.5 - float3(dot(x0,x0), dot(x12.xy,x12.xy),
    dot(x12.zw,x12.zw)), 0.0);
  m = m*m ;
  m = m*m ;
  float3 x = 2.0 * frac(p * C.www) - 1.0;
  float3 h = abs(x) - 0.5;
  float3 ox = floor(x + 0.5);
  float3 a0 = x - ox;
  m *= 1.79284291400159 - 0.85373472095314 * ( a0*a0 + h*h );
  float3 g;
  g.x  = a0.x  * x0.x  + h.x  * x0.y;
  g.yz = a0.yz * x12.xz + h.yz * x12.yw;
  return 130.0 * dot(m, g);
}

// From https://www.shadertoy.com/view/wsBfzK
float wnoise(float2 p, float z, float k) {
    // https://www.shadertoy.com/view/wsBfzK
    float d=0.,s=1.,m=0., a;
    for(float i=0.; i<4.; i++) {
        float2 q = p*s, g=frac(floor(q)*float2(123.34,233.53));
    	g += dot(g, g+23.234);
		a = frac(g.x*g.y)*1e3;// +z*(mod(g.x+g.y, 2.)-1.); // add vorticity
        q = mul((frac(q)-.5),float2x2(cos(a),-sin(a),sin(a),cos(a)));
        d += sin(q.x*10.+z)*smoothstep(.25, .0, dot(q,q))/s;
        p = mul(p,float2x2(.54,-.84, .84, .54)+i);
        m += 1./s;
        s *= k; 
    }
    return d/m;
}