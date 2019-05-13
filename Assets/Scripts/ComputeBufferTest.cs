using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeBufferTest : MonoBehaviour
{
    public ComputeShader shader;

    struct Data
    {
        public float A;
        public float B;
        public float C;
    }
    // Start is called before the first frame update
    void Start()
    {
        Data[] inputData = new Data[3];
        Data[] outputData = new Data[3];

        for (int i = 0; i < inputData.Length; i++)
        {
            inputData[i].A = i * 3 + 1;
            inputData[i].B = i * 3 + 2;
            inputData[i].C = i * 3 + 3;
            Debug.LogFormat("inputData.A={0} inputData.B={1} inputData.C={2}",inputData[i].A, inputData[i].B,inputData[i].C);
        }
        ComputeBuffer inputBuffer = new ComputeBuffer(outputData.Length, 12);
        ComputeBuffer outputBuffer = new ComputeBuffer(outputData.Length, 12);

        int k = shader.FindKernel("CSMain");
        inputBuffer.SetData(inputData);
        shader.SetBuffer(k, "inputData", inputBuffer);

        shader.SetBuffer(k, "outputData", outputBuffer);
        shader.Dispatch(k, outputData.Length, 1, 1);
        outputBuffer.GetData(outputData);
        inputBuffer.Dispose();
        outputBuffer.Dispose();
        for (int i = 0; i < outputData.Length; i++)
        {
            Debug.LogFormat("outputData.A={0} outputData.B={1} outputData.C={2}", outputData[i].A, outputData[i].B, outputData[i].C);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
