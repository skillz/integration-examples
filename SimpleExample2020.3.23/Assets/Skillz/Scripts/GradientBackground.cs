using UnityEngine;
using UnityEngine.UI;

// Adapted from https://stackoverflow.com/questions/44820839/two-color-background
public sealed class GradientBackground : MonoBehaviour
{
	[SerializeField]
	private RawImage image;

	[SerializeField]
	private Color bottomColor = Color.white;

	[SerializeField]
	private Color topColor = Color.black;

	private Texture2D backgroundTexture;

	private void Awake()
	{
		backgroundTexture = new Texture2D(1, 2);
		backgroundTexture.wrapMode = TextureWrapMode.Clamp;
		backgroundTexture.filterMode = FilterMode.Bilinear;
		SetColor(bottomColor, topColor);
	}

	private void SetColor(Color color1, Color color2)
	{
		backgroundTexture.SetPixels(new Color[] { color1, color2 });
		backgroundTexture.Apply();
		image.texture = backgroundTexture;
	}
}