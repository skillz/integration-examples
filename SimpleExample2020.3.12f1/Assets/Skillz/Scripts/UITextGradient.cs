using UnityEngine;
using UnityEngine.UI;

// Pilfered from https://unitylist.com/p/1js/Unity-UI-Gradient
public sealed class UITextGradient : BaseMeshEffect
{
	public Color m_color1 = Color.white;
	public Color m_color2 = Color.white;
	[Range(-180f, 180f)]
	public float m_angle = 0f;

    public override void ModifyMesh(VertexHelper vh)
    {
		if(enabled)
		{
			Rect rect = graphic.rectTransform.rect;
			Vector2 dir = UIGradientUtils.RotationDir(m_angle);
			UIGradientUtils.Matrix2x3 localPositionMatrix = UIGradientUtils.LocalPositionMatrix(new Rect(0f, 0f, 1f, 1f), dir);

			UIVertex vertex = default;
			for (int i = 0; i < vh.currentVertCount; i++) {

				vh.PopulateUIVertex (ref vertex, i);
				Vector2 position = UIGradientUtils.VerticePositions[i % 4];
				Vector2 localPosition = localPositionMatrix * position;
				vertex.color *= Color.Lerp(m_color2, m_color1, localPosition.y);
				vh.SetUIVertex (vertex, i);
			}
		}
    }
}