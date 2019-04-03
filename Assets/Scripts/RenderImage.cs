using UnityEngine;
[ExecuteAlways]
public class RenderImage : MonoBehaviour
{
    // 分辨率
    public int downSample = 1;
    public float grayScaleAmout = 1.0f;
    public Material curMaterial;
    // 高亮部分提取阀值
    public Color colorThreshold = Color.gray;
    // 采样率
    public int samplerScale = 1;
    // Bloom泛光颜色
    public Color bloomColor = Color.white;
    // Bloom权值
    [Range(0.0f, 1.0f)]
    public float bloomFactor = 0.5f;
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // 申请两块RT，并且分辨率按照downSample降低
        RenderTexture temp1 = RenderTexture.GetTemporary(src.width >> downSample, src.height >> downSample);
        RenderTexture temp2 = RenderTexture.GetTemporary(src.width >> downSample, src.height >> downSample);

        // 直接将场景图拷贝到低分辨率的RT上达到降分辨率的效果
        Graphics.Blit(src, temp1);
        // 根据阀值提取高亮部分，使用pass0进行高亮提取
        curMaterial.SetVector("_colorThresshold", colorThreshold);
        Graphics.Blit(temp1, temp2, curMaterial, 0);

        // 高斯模糊，两次模糊，横向纵向，使用pass1进行高斯模糊
        curMaterial.SetVector("_offsets", new Vector4(0, samplerScale, 0, 0));
        Graphics.Blit(temp2, temp1, curMaterial, 1);
        curMaterial.SetVector("_offset", new Vector4(samplerScale, 0, 0, 0));
        Graphics.Blit(temp1, temp2, curMaterial, 1);

        // Bloom，将模糊后的图作为Material的Blur图参数
        curMaterial.SetTexture("_BlurTex", temp2);
        curMaterial.SetVector("_bloomColor", bloomColor);
        curMaterial.SetFloat("_bloomFactor", bloomFactor);
        // 使用pass2进行景深效果计算，清晰场景图直接从source输入到shader的_MainTex中
        Graphics.Blit(src, dest, curMaterial, 2);

        // 释放申请的RT
        RenderTexture.ReleaseTemporary(temp1);
        RenderTexture.ReleaseTemporary(temp2);
    }
}
