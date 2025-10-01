using UnityEngine;

public static class computeExt
{
    public static ComputeBuffer SetBuffers(this ComputeShader compute, string name, int length, int stride, 
        int a = -1, int b = -1, int c = -1, int d = -1, int e = -1, int f = -1,
        ComputeBufferType type = ComputeBufferType.Default)
    {
        ComputeBuffer buffer = new ComputeBuffer(length, stride, type);
        
        if(a != -1)    compute.SetBuffer(a, name, buffer);
        if(b != -1)    compute.SetBuffer(b, name, buffer);
        if(c != -1)    compute.SetBuffer(c, name, buffer);
        if(d != -1)    compute.SetBuffer(d, name, buffer);
        if(e != -1)    compute.SetBuffer(e, name, buffer);
        if(f != -1)    compute.SetBuffer(f, name, buffer);
        
        return buffer;
    }
}
