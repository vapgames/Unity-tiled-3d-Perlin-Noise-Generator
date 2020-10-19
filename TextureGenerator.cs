using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Noise3DTextureGenerator : EditorWindow
{
	static private Noise3DTextureGenerator mWindow;

	static public string path = "Assets/Test3DTexture.asset";
	static public int width = 64;
	static public int height = 64;
	static public int depth = 64;
	static public int octave = 1;
	static public Vector3 scale = new Vector3(1, 1, 1);
	static public float scaleMult = 0.05f;
	static public Vector3 shift = new Vector3(0, 0, 0);
	static public Vector3 tiling = new Vector3(0.4f, 0.4f, 0.4f);

	[MenuItem("Window/Noise3d Texture Generator")]

	private static void Init() {
		mWindow = GetWindow<Noise3DTextureGenerator>("Noise3d Texture Generator");
		mWindow.Show();
	}

	private void OnEnable() {
		path = EditorPrefs.GetString("TextureGenerator_path", "Assets/Test3DTexture.asset");
		width = EditorPrefs.GetInt("TextureGenerator_width", 64);
		height = EditorPrefs.GetInt("TextureGenerator_height", 64);
		depth = EditorPrefs.GetInt("TextureGenerator_depth", 64);
		octave = EditorPrefs.GetInt("TextureGenerator_octave", 4);
		scale.x = EditorPrefs.GetFloat("TextureGenerator_scale.x", 1);
		scale.y = EditorPrefs.GetFloat("TextureGenerator_scale.y", 1);
		scale.z = EditorPrefs.GetFloat("TextureGenerator_scale.z", 1);
		scaleMult = EditorPrefs.GetFloat("TextureGenerator_scaleMult", 0.05f);
		shift.x = EditorPrefs.GetFloat("TextureGenerator_shift.x", 0);
		shift.y = EditorPrefs.GetFloat("TextureGenerator_shift.y", 0);
		shift.z = EditorPrefs.GetFloat("TextureGenerator_shift.z", 0);
		tiling.x = EditorPrefs.GetFloat("TextureGenerator_tiling.x", 0.4f);
		tiling.y = EditorPrefs.GetFloat("TextureGenerator_tiling.y", 0.4f);
		tiling.z = EditorPrefs.GetFloat("TextureGenerator_tiling.z", 0.4f);
	}

	public void OnGUI() {
		EditorGUI.BeginChangeCheck();
		path = EditorGUILayout.TextField("path", path);
		width = EditorGUILayout.IntField("width", width, GUILayout.ExpandWidth(false));
		height = EditorGUILayout.IntField("height", height, GUILayout.ExpandWidth(false));
		depth = EditorGUILayout.IntField("depth", depth, GUILayout.ExpandWidth(false));
		octave = EditorGUILayout.IntField("octave", octave, GUILayout.ExpandWidth(false));
		tiling = EditorGUILayout.Vector3Field("tiling", tiling);
		tiling.Set(Mathf.Clamp01(tiling.x), Mathf.Clamp01(tiling.y), Mathf.Clamp01(tiling.z));

		scale = EditorGUILayout.Vector3Field("scale", scale);
		shift = EditorGUILayout.Vector3Field("shift", shift);
		scaleMult = EditorGUILayout.FloatField("scaleMult", scaleMult, GUILayout.ExpandWidth(false));

		if(EditorGUI.EndChangeCheck()) {
			EditorPrefs.SetString("TextureGenerator_path", path);
			EditorPrefs.SetInt("TextureGenerator_width", width);
			EditorPrefs.SetInt("TextureGenerator_height", height);
			EditorPrefs.SetInt("TextureGenerator_depth", depth);
			EditorPrefs.SetInt("TextureGenerator_octave", octave);
			EditorPrefs.SetFloat("TextureGenerator_scale.x", scale.x);
			EditorPrefs.SetFloat("TextureGenerator_scale.y", scale.y);
			EditorPrefs.SetFloat("TextureGenerator_scale.z", scale.z);
			EditorPrefs.SetFloat("TextureGenerator_scaleMult", scaleMult);
			EditorPrefs.SetFloat("TextureGenerator_shift.x", shift.x);
			EditorPrefs.SetFloat("TextureGenerator_shift.y", shift.y);
			EditorPrefs.SetFloat("TextureGenerator_shift.z", shift.z);
			EditorPrefs.SetFloat("TextureGenerator_tiling.x", tiling.x);
			EditorPrefs.SetFloat("TextureGenerator_tiling.y", tiling.y);
			EditorPrefs.SetFloat("TextureGenerator_tiling.z", tiling.z);
		}

		if (GUILayout.Button("CHOOCH", GUILayout.MaxWidth(200f))){
			CreateTexture3D();
		}
	}
	
	static void CreateTexture3D() {
		int tileX = (int)(width * tiling.x);
		int tileY = (int)(height * tiling.y);
		int tileZ = (int)(depth * tiling.z);
		
		int extWidth = width + tileX;
		int extHeight = height + tileY;
		int extDepth = depth + tileZ;
		Color[] colors = new Color[extWidth * extHeight * extDepth];

		for (int z = 0; z < extDepth; z++) {
			int zOffset = z * extWidth * extHeight;
			for (int y = 0; y < extHeight; y++) {
				int yOffset = y * extWidth;
				for (int x = 0; x < extWidth; x++) {
					colors[x + yOffset + zOffset] = PosToColor(new Vector3(x, y, z));
				}
			}
		}

		//TILE X
		for (int z = 0; z < extDepth; z++) {
			int zOffset = z * extWidth * extHeight;
			for (int y = 0; y < extHeight; y++) {
				int yOffset = y * extWidth;
				for (int x = 0; x < tileX; x++) {
					int offs = x + yOffset + zOffset;
					Color colorA = colors[offs];
					Color colorB = colors[x + width + y * extWidth + z * extWidth * extHeight];
					colors[offs] = Color.Lerp(colorB, colorA, (float)x / (float)tileX);
				}
			}
		}

		//TILE Y
		for (int z = 0; z < extDepth; z++) {
			int zOffset = z * extWidth * extHeight;
			for (int y = 0; y < tileY; y++) {
				int yOffset = y * extWidth;
				for (int x = 0; x < extWidth; x++) {
					int offs = x + yOffset + zOffset;
					Color colorA = colors[offs];
					Color colorB = colors[x + (y + height) * extWidth + z * extWidth * extHeight];
					colors[offs] = Color.Lerp(colorB, colorA, (float)y / (float)tileY);
				}
			}
		}

		//TILE Z
		for (int z = 0; z < tileZ; z++) {
			int zOffset = z * extWidth * extHeight;
			for (int y = 0; y < extHeight; y++) {
				int yOffset = y * extWidth;
				for (int x = 0; x < extWidth; x++) {
					int offs = x + yOffset + zOffset;
					Color colorA = colors[offs];
					Color colorB = colors[x + y * extWidth + (z + depth) * extWidth * extHeight];
					colors[offs] = Color.Lerp(colorB, colorA, (float)z / (float)tileZ);
				}
			}
		}

		//PACK ORIGINAL RES
		Color[] tiledColors = new Color[width * height * depth];
		for (int z = 0; z < depth; z++) {
			int zOffset = z * width * height;
			for (int y = 0; y < height; y++) {
				int yOffset = y * width;
				for (int x = 0; x < width; x++) {
					tiledColors[x + yOffset + zOffset] = colors[x + y * extWidth + z * extWidth * extHeight];
				}
			}
		}

		Texture3D texture;
		UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(Texture3D));
		if (obj != null) {
			//UPDATE EXISTING
			texture = obj as Texture3D;
			if(texture.width != width || texture.height != height || texture.depth != depth) {
				texture = new Texture3D(width, height, depth, TextureFormat.RGBA32, false);
				texture.wrapMode = TextureWrapMode.Repeat;
				AssetDatabase.CreateAsset(texture, path);
			}
			texture.SetPixels(tiledColors);
			texture.Apply();
			EditorUtility.SetDirty(texture);
			AssetDatabase.SaveAssets();
		}
		else {
			//CREATE NEW
			texture = new Texture3D(width, height, depth, TextureFormat.RGBA32, false);
			texture.wrapMode = TextureWrapMode.Repeat;
			texture.SetPixels(tiledColors);
			texture.Apply();
			AssetDatabase.CreateAsset(texture, path);
			AssetDatabase.SaveAssets();
		}
	}

	static private Color PosToColor(Vector3 pos) {
		pos = Vector3.Scale(pos, scale) * scaleMult + shift;
		Color color = new Color();
		color.r = Perlin.Fbm(pos.x, pos.y, pos.z, octave);
		color.g = Perlin.Fbm(pos.x + 1.23434567f, 100f - pos.y + 0.11111f, pos.z + 91.2344f, octave);
		color.b = Perlin.Fbm(100f - pos.x - 17.2793f, pos.y - 0.77777f, pos.z + 1237.23233f, octave);
		color.a = 1f;
		return color;
	}
}
