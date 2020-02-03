using UnityEngine;
using UnityEditor;
using WraithavenGames.Bones3.Filter;
using WraithavenGames.Bones3.BlockProperties;
using WraithavenGames.Bones3.SceneEditing;

namespace WraithavenGames.Bones3
{
	[CustomEditor(typeof(BlockWorld))]
	public class BlockWorldEditor : Editor
	{
		private Texture[] iconTextures;
		private int controlID;
		private Texture[] voxelIconsModes;
		private Texture[] voxelIconsShapes;
		private Texture[] voxelIconsAddons;

		private void OnEnable()
		{
			BlockWorld t = target as BlockWorld;

			controlID = GUIUtility.GetControlID(FocusType.Passive);
			LoadVoxelIcons();

			// Hide wireframes in all children
			SetWireframeHidden(t.transform, true);

			// Listen for undo updates
			Undo.undoRedoPerformed += UndoPerformed;
		}

		private void UndoPerformed()
		{
			BlockWorld t = target as BlockWorld;
			if (t == null)
				return;

			t.UpdateAllChunks();
		}

		private void SetWireframeHidden(Transform t, bool hidden)
		{
			Renderer render = t.GetComponent<Renderer>();
			if (render != null)
			{
				if (hidden)
					EditorUtility.SetSelectedRenderState(render, EditorSelectedRenderState.Hidden);
				else
					EditorUtility.SetSelectedRenderState(render, EditorSelectedRenderState.Highlight);
			}

			for (int i = 0; i < t.childCount; i++)
				SetWireframeHidden(t.GetChild(i), hidden);
		}

		private void OnDisable()
		{
			BlockWorld t = target as BlockWorld;


			// Reenable wireframes in all children
			if (t != null)
				SetWireframeHidden(t.transform, false);

			// Stop listening for undo updates
			Undo.undoRedoPerformed -= UndoPerformed;
		}

		private void LoadVoxelIcons()
		{
			voxelIconsModes = new Texture[3];
			voxelIconsModes[0] = Resources.Load("Bones3Res/Single Block Editing") as Texture;
			voxelIconsModes[1] = Resources.Load("Bones3Res/Paint") as Texture;
			voxelIconsModes[2] = Resources.Load("Bones3Res/Do Nothing") as Texture;

			voxelIconsShapes = new Texture[3];
			voxelIconsShapes[0] = Resources.Load("Bones3Res/Single Block Editing") as Texture;
			voxelIconsShapes[1] = Resources.Load("Bones3Res/Wall Editing") as Texture;
			voxelIconsShapes[2] = Resources.Load("Bones3Res/Ellipse Editing") as Texture;

			voxelIconsAddons = new Texture[2];
			voxelIconsAddons[0] = Resources.Load("Bones3Res/Erase") as Texture;
			voxelIconsAddons[1] = Resources.Load("Bones3Res/Solid Floor") as Texture;
		}

		public override void OnInspectorGUI()
		{
			// Get the target block world
			BlockWorld t = target as BlockWorld;

			int materialCount = t.BlockTypes.GetVisibleLength();

			// Material selection area
			{

				// Update texture cache size, if needed
				if (iconTextures == null || iconTextures.Length != materialCount)
				{
					iconTextures = new Texture[materialCount];

					// Cap the selected material index to the length of this list
					// Selected index returns -1 if the material list is empty
					if (t.selectedMaterialIndex >= iconTextures.Length || t.selectedMaterialIndex < 0)
						t.selectedMaterialIndex = iconTextures.Length - 1;
				}

				// Render material list
				{
					for (int i = 0; i < iconTextures.Length; i++)
					{
						MaterialBlock props = t.BlockTypes.GetVisibleAt(i);
						iconTextures[i] = AssetPreview.GetAssetPreview(props.Material);

						EditorGUILayout.BeginHorizontal();

						bool selected = GUILayout.Toggle(t.selectedMaterialIndex == i, iconTextures[i], EditorStyles.miniButton,
							GUILayout.Width(64), GUILayout.Height(64));
						if (selected)
							t.selectedMaterialIndex = i;

						{
							// Material Properties
							EditorGUI.BeginChangeCheck();

							EditorGUILayout.BeginVertical();
							props.Transparent = GUILayout.Toggle(props.Transparent, "Transparent");
							props.GroupBlocks = GUILayout.Toggle(props.GroupBlocks, "Group Blocks");
							props.ViewInsides = GUILayout.Toggle(props.ViewInsides, "View Insides");
							EditorGUILayout.EndVertical();

							EditorGUILayout.BeginVertical();
							props.DepthSort = GUILayout.Toggle(props.DepthSort, "Depth Sort");
							EditorGUILayout.EndVertical();

							if(EditorGUI.EndChangeCheck())
							{
								EditorUtility.SetDirty(t);
								t.UpdateAllBlockStates(props.Id, props.BlockState);
							}
						}

						if (GUILayout.Button("X", GUILayout.Width(30), GUILayout.Height(30)))
						{
							// Record undo point and remove material
							Undo.RecordObject(t, "Removed Material from BlockWorld");
							props.HiddenInInspector = true;
							EditorUtility.SetDirty(t);
						}

						EditorGUILayout.EndHorizontal();

					}

					GUILayout.Space(20);
				}

				// If we are still loading asset previews, queue the inspector for another repaint
				if (AssetPreview.IsLoadingAssetPreviews())
					EditorUtility.SetDirty(t);
			}

			// Add/Remove material area
			{
				// Show an object field for quickly adding new materials to the list
				Material newMaterial = EditorGUILayout.ObjectField("Add Material", null, typeof(Material), false) as Material;
				if (newMaterial != null)
				{
					MaterialBlock matProps = t.BlockTypes.GetMaterialProperties(newMaterial);
					if (matProps.HiddenInInspector)
					{
						// Record undo point and add material
						Undo.RecordObject(t, "Added Material to BlockWorld");
						matProps.HiddenInInspector = false;
						EditorUtility.SetDirty(t);
					}
				}

				GUILayout.Space(30);
			}

			// Regen and Clear buttons
			{
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Regen") && materialCount > 0)
				{
					t.Clear();
					t.SetBlock(0, 0, 0, t.BlockTypes.GetVisibleAt(t.selectedMaterialIndex).Material);
				}
				if (GUILayout.Button("Clear"))
					t.Clear();
				if (GUILayout.Button("Remesh"))
					t.RemeshAllChunks();
				EditorGUILayout.EndHorizontal();
			}
		}

		private void OnSceneGUI()
		{
			// Get targeted script
			BlockWorld t = target as BlockWorld;
			if (t == null) return;

			// Process GUI
			{
				Handles.BeginGUI();

				ToolBelt.Mode = GUILayout.SelectionGrid(ToolBelt.Mode, voxelIconsModes, 1, GUILayout.Width(48),
					GUILayout.Height(48 * voxelIconsModes.Length));

				GUILayout.Space(20);

				ToolBelt.Shape = GUILayout.SelectionGrid(ToolBelt.Shape, voxelIconsShapes, 1, GUILayout.Width(48),
					GUILayout.Height(48 * voxelIconsShapes.Length));

				GUILayout.Space(20);

				ToolBelt.EraserEnabled = GUILayout.Toggle(ToolBelt.EraserEnabled, voxelIconsAddons[0], EditorStyles.miniButton,
					GUILayout.Width(40), GUILayout.Height(40));
				ToolBelt.FloorEnabled = GUILayout.Toggle(ToolBelt.FloorEnabled, voxelIconsAddons[1], EditorStyles.miniButton,
					GUILayout.Width(40), GUILayout.Height(40));

				Handles.EndGUI();
			}

			// Process Controls
			{
				SelectedBlock block;
				Event e = Event.current;

				bool solidFloor = ToolBelt.FloorEnabled;
				if (e.control) solidFloor = !solidFloor;

				// Prevent selecting objects (Namely chunks)
				HandleUtility.AddDefaultControl(controlID);

				// Get selection block, if any
				block = GetMousedOverBlock(t.transform, solidFloor);
				{
					if (block != t.selectedBlock)
					{
						t.selectedBlock = block;
						SceneView.RepaintAll();
					}

					if (!block.hasSelectedBlock)
						return;
				}

				// In do nothing mode, editing is disabled.
				if (ToolBelt.Mode == ToolBelt.DO_NOTHING_MODE)
					return;

				// Do nothing if we have no blocks in our inventory
				if (t.BlockTypes.GetVisibleLength() == 0)
					return;

				// Handle events
				{
					ToolBelt.ClearSelectionMode = ToolBelt.EraserEnabled;
					if (e.shift) ToolBelt.ClearSelectionMode = !ToolBelt.ClearSelectionMode;

					// If the left mouse button is pressed, start area selection
					if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
						t.dragStart = block;

					// If the mouse moves with no keys held, end area selection
					if (e.type == EventType.MouseMove || e.alt)
						t.dragStart.hasSelectedBlock = false;

					// If the left mouse is released, end area selection and apply
					if (e.type == EventType.MouseUp && e.button == 0 && t.dragStart.hasSelectedBlock)
					{
						t.dragStart.hasSelectedBlock = false;

						Undo.IncrementCurrentGroup();
						Undo.SetCurrentGroupName("Edited voxel world.");

						switch (ToolBelt.Mode)
						{
							case ToolBelt.NORMAL_MODE:
							{
								IFilter filter = ToolBelt.ActiveFilter;

								if (ToolBelt.ClearSelectionMode)
									t.SetArea(t.dragStart.xOn, t.dragStart.yOn, t.dragStart.zOn,
										t.selectedBlock.xOn, t.selectedBlock.yOn, t.selectedBlock.zOn, null, filter);
								else
									t.SetArea(t.dragStart.xInside, t.dragStart.yInside, t.dragStart.zInside,
										t.selectedBlock.xInside, t.selectedBlock.yInside, t.selectedBlock.zInside,
										t.BlockTypes.GetVisibleAt(t.selectedMaterialIndex).Material, filter);
								break;
							}

							case ToolBelt.PAINT_MODE:
							{
								IFilter filter = ToolBelt.ActiveFilter;

								t.SetArea(t.dragStart.xOn, t.dragStart.yOn, t.dragStart.zOn,
									t.selectedBlock.xOn, t.selectedBlock.yOn, t.selectedBlock.zOn,
									t.BlockTypes.GetVisibleAt(t.selectedMaterialIndex).Material, filter, true);
								break;
							}
						}
					}
				}
			}
		}

		private SelectedBlock GetMousedOverBlock(Transform t, bool groundLock)
		{
			// Raycast block world
			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			RaycastHit hit;
			float groundDistance = float.PositiveInfinity;
			Plane ground = new Plane(Vector3.up, Vector3.zero);
			if (Physics.Raycast(ray, out hit) || (groundLock && ground.Raycast(ray, out groundDistance)))
			{
				Vector3 hitPoint;
				if (hit.distance != 0 && hit.distance < groundDistance)
					hitPoint = hit.point;
				else
					hitPoint = ray.direction * groundDistance + ray.origin;

				// Created selected block data
				SelectedBlock block = new SelectedBlock();
				block.hasSelectedBlock = true;

				// Shift hit location to avoid floating point errors
				Vector3 inside = hitPoint - ray.direction * .0001f;
				Vector3 over = hitPoint + ray.direction * .0001f;

				// Translate hit location to blockworld coords
				Vector3 subWorldPos = t.InverseTransformPoint(inside);
				block.xInside = Mathf.FloorToInt(subWorldPos.x);
				block.yInside = Mathf.FloorToInt(subWorldPos.y);
				block.zInside = Mathf.FloorToInt(subWorldPos.z);

				subWorldPos = t.InverseTransformPoint(over);
				block.xOn = Mathf.FloorToInt(subWorldPos.x);
				block.yOn = Mathf.FloorToInt(subWorldPos.y);
				block.zOn = Mathf.FloorToInt(subWorldPos.z);

				return block;
			}

			return new SelectedBlock();
		}
	}
}
