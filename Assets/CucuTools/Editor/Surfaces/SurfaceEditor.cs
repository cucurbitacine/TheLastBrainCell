using System;
using System.Linq;
using CucuTools.Surfaces;
using CucuTools.Surfaces.Tools;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CucuTools.Editor.Surfaces
{
    public class SurfaceEditor : EditorWindow
    {
        [MenuItem("Tools/CucuTools/Surfaces")]
        private static void WindowShow()
        {
            var window = (SurfaceEditor)EditorWindow.GetWindow(typeof(SurfaceEditor));
            window.titleContent = new GUIContent("Surface Editor");
            window.Show();
        }

        public static void WindowShow(SurfaceBehaviour surface)
        {
            var window = (SurfaceEditor)EditorWindow.GetWindow(typeof(SurfaceEditor));
            window.titleContent = new GUIContent("Surface Editor");
            window.Show();
            window.selectedTool = 1;
            window.selectedEditTool = 0;
            window.gameObject = surface.gameObject;
        }
        
        private int selectedTool;
        private int selectedPrimitive;

        private bool selectAfterCreate = true;
        
        private static QuadEntity QuadEntity = new QuadEntity();
        private static CircleEntity CircleEntity = new CircleEntity();
        private static PipeEntity PipeEntity = new PipeEntity();
        private static SphereEntity SphereEntity = new SphereEntity();
        private static BlendSurfaceEntity BlendEntity = new BlendSurfaceEntity();

        private static GUIContent QuadContent => quadCnt ?? (quadCnt = new GUIContent("Quad", Resources.Load<Texture>("quad")));
        private static GUIContent CircleContent => circleCnt ?? (circleCnt = new GUIContent("Circle", Resources.Load<Texture>("circle")));
        private static GUIContent PipeContent => pipeCnt ?? (pipeCnt = new GUIContent("Pipe", Resources.Load<Texture>("pipe")));
        private static GUIContent SphereContent => sphereCnt ?? (sphereCnt = new GUIContent("Sphere", Resources.Load<Texture>("sphere")));
        private static GUIContent BlendContent => blendCnt ?? (blendCnt = new GUIContent("Blend", Resources.Load<Texture>("blend")));

        private static GUIContent quadCnt;
        private static GUIContent circleCnt;
        private static GUIContent pipeCnt;
        private static GUIContent sphereCnt;
        private static GUIContent blendCnt;
        
        private static readonly string[] Tools = new[] { "Create", "Edit" };
        private static string[] Primitives => primitives ?? (primitives = new[] { QuadContent.text, CircleContent.text, PipeContent.text, SphereContent.text, BlendContent.text });
        private static string[] primitives;
        
        private static bool preview;

        private Vector2 scroll;

        private void OnGUI()
        {
            selectedTool = GUILayout.Toolbar(selectedTool, Tools);

            scroll = EditorGUILayout.BeginScrollView(scroll);
            
            if (selectedTool == 0) Create();
            if (selectedTool == 1) Edit();
            
            EditorGUILayout.EndScrollView();
            
            if (GUI.changed) UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        #region Create

        private void Create()
        {
            selectedPrimitive = GUILayout.Toolbar(selectedPrimitive, Primitives);

            EditorGUILayout.BeginHorizontal();
            
            selectAfterCreate = GUILayout.Toggle(selectAfterCreate, "Select after create");
            
            if (GUILayout.Button("Create"))
            {
                var target = CreatePrimitive().transform;
                
                if (selectAfterCreate)
                {
                    if (target != null)
                    {
                        unlockedGO = true;
                        Selection.activeGameObject = target.gameObject;
                        SceneView.FrameLastActiveSceneView();
                        selectedTool = 1;
                    }
                }
            }

            EditorGUILayout.EndHorizontal();
            
            PropertiesPrimitive();
        }

        private void PropertiesPrimitive()
        {
            if (selectedPrimitive == 0)
            {
                GUIQuadEntity(ref QuadEntity);
            }
                
            if (selectedPrimitive == 1)
            {
                GUICircleEntity(ref CircleEntity);
            }
            
            if (selectedPrimitive == 2)
            {
                GUIPipeEntity(ref PipeEntity);
            }
            
            if (selectedPrimitive == 3)
            {
                GUISphereEntity(ref SphereEntity);
            }
            
            if (selectedPrimitive == 4)
            {
                GUIBlendEntity(ref BlendEntity);
            }
        }
        
        private SurfaceBehaviour CreatePrimitive()
        {
            if (selectedPrimitive == 0)
            {
                var quad = CreateSurface<QuadSurface>();
                quad.Entity = QuadEntity.CloneEntity<QuadEntity>();
                return quad;
            }
                
            if (selectedPrimitive == 1)
            {
                var circle = CreateSurface<CircleSurface>();
                circle.Entity = CircleEntity.CloneEntity<CircleEntity>();;
                return circle;
            }

            if (selectedPrimitive == 2)
            {
                var pipe = CreateSurface<PipeSurface>();
                pipe.Entity = PipeEntity.CloneEntity<PipeEntity>();
                return pipe;
            }
            
            if (selectedPrimitive == 3)
            {
                var sphere = CreateSurface<SphereSurface>();
                sphere.Entity = SphereEntity.CloneEntity<SphereEntity>();
                return sphere;
            }
            
            if (selectedPrimitive == 4)
            {
                var blend = CreateSurface<BlendSurface>();
                blend.Entity = BlendEntity.CloneEntity<BlendSurfaceEntity>();
                return blend;
            }
            
            return null;
        }

        #endregion

        #region Edit

        private bool unlockedGO = true;
        private GameObject gameObject;
        private int selectedEditTool;
        private static readonly string[] EditTools = new[] { "Surface", "Gizmos", "Mesh" };
        
        
        private AnimBool animSurface;
        private AnimBool animPreview;

        private void OnEnable()
        {
            animSurface = new AnimBool(true);
            animSurface.valueChanged.AddListener(Repaint);
            
            animPreview = new AnimBool(true);
            animPreview.valueChanged.AddListener(Repaint);
        }

        private UnityEditor.Editor previewEditor;

        private void UpdatePreview(GameObject target)
        {
            if (previewEditor != null) DestroyImmediate(previewEditor);

            if (target == null) return;

            previewEditor = UnityEditor.Editor.CreateEditor(target);
        }

        private void PreviewEditor(GameObject target = null)
        {
            if (target != null) UpdatePreview(target);
            
            if (previewEditor == null) return;
            
            previewEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(256,256), GUIStyle.none);
        }

        private SurfaceBehaviour lastSurface;
        
        private void Edit()
        {
            if (unlockedGO)
            {
                gameObject = Selection.activeGameObject;
            }

            if (gameObject == null)
            {
                unlockedGO = true;
                
                EditorGUILayout.HelpBox("Select GameObject", MessageType.Warning);
                return;
            }

            GUILayout.BeginHorizontal();

            unlockedGO = EditorGUILayout.ToggleLeft(unlockedGO ? "Unlocked" : "Locked", unlockedGO);
            
            if (GUILayout.Button("Focus"))
            {
                Selection.activeGameObject = gameObject;
                SceneView.FrameLastActiveSceneView();
            }

            GUILayout.EndHorizontal();
            
            GUILayout.Space(16);
            
            var surface = gameObject.GetComponent<SurfaceBehaviour>();

            if (surface == null)
            {
                gameObject = (GameObject)EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);
                
                EditorGUILayout.HelpBox("Select GameObject With SurfaceEntity", MessageType.Warning);
                
                return;
            }

            animSurface.target = unlockedGO;
            if (EditorGUILayout.BeginFadeGroup(animSurface.faded))
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Prev"))
                {
                    var surfaces = FindObjectsOfType<SurfaceBehaviour>().ToList();

                    var index = surfaces.IndexOf(surface);
                    if (index < 1) index = surfaces.Count - index;
                    index = (index - 1) % surfaces.Count;

                    surface = surfaces[index];
                    gameObject = surface.gameObject;
                    Selection.activeGameObject = gameObject;
                    SceneView.FrameLastActiveSceneView();
                }

                EditorGUILayout.ObjectField(surface, typeof(SurfaceBehaviour), true);

                if (GUILayout.Button("Next"))
                {
                    var surfaces = FindObjectsOfType<SurfaceBehaviour>().ToList();

                    var index = surfaces.IndexOf(surface);
                    if (index < 1) index = surfaces.Count - index;
                    index = (index + 1) % surfaces.Count;

                    surface = surfaces[index];
                    gameObject = surface.gameObject;
                    Selection.activeGameObject = gameObject;
                    SceneView.FrameLastActiveSceneView();
                }

                GUILayout.EndHorizontal();

                GUILayout.Space(16);
            }
            
            EditorGUILayout.EndFadeGroup();

            var any = surface.GetComponentsInChildren<Renderer>().Any();

            animPreview.target = any;

            if (EditorGUILayout.BeginFadeGroup(animPreview.faded))
            {
                if (lastSurface == null)
                {
                    UpdatePreview(surface.gameObject);
                    Debug.Log("First");
                }
                else
                {
                    if (lastSurface != surface)
                    {
                        UpdatePreview(surface.gameObject);
                        Debug.Log("New");
                    }
                }

                lastSurface = surface;

                PreviewEditor();

                GUILayout.Space(16);
            }
            EditorGUILayout.EndFadeGroup();
            
            selectedEditTool = GUILayout.Toolbar(selectedEditTool, EditTools);

            GUILayout.Space(16);
            
            if (selectedEditTool == 0)
            {
                EditSurface(surface);
            }
            
            if (selectedEditTool == 1)
            {
                EditGizmos(surface);
            }
            
            if (selectedEditTool == 2)
            {
                EditMesh(surface);
            }
        }

        private void EditSurface(SurfaceBehaviour surface)
        {
            if (surface is QuadSurface quad)
            {
                var prop = quad.Entity;
                GUIQuadEntity(ref prop);
                quad.Entity = prop;
            }
            
            if (surface is CircleSurface circle)
            {
                var prop = circle.Entity;
                GUICircleEntity(ref prop);
                circle.Entity = prop;
            }

            if (surface is PipeSurface pipe)
            {
                var prop = pipe.Entity;
                GUIPipeEntity(ref prop);
                pipe.Entity = prop;
            }
            
            if (surface is SphereSurface sphere)
            {
                var prop = sphere.Entity;
                GUISphereEntity(ref prop);
                sphere.Entity = prop;
            }
            
            if (surface is BlendSurface blend)
            {
                var prop = blend.Entity;
                GUIBlendEntity(ref prop);
                blend.Entity = prop;
            }
            
            BuildMesh(surface);
        }
        
        private void EditGizmos(SurfaceBehaviour surface)
        {
            EditorGUILayout.Space(16);
            
            surface.gizmos.Drawing = EditorGUILayout.Toggle("Draw", surface.gizmos.Drawing);
            surface.gizmos.OnlySelected = EditorGUILayout.Toggle("Only Selected", surface.gizmos.OnlySelected);
            
            EditorGUILayout.Space(16);
            
            surface.gizmos.SizeU = EditorGUILayout.IntSlider("Size U", surface.gizmos.SizeU, SurfaceMesh.SizeMin, SurfaceMesh.SizeMax);
            surface.gizmos.SizeV = EditorGUILayout.IntSlider("Size V", surface.gizmos.SizeV, SurfaceMesh.SizeMin, SurfaceMesh.SizeMax);
            
            EditorGUILayout.Space(16);
            
            surface.gizmos.ShowNormal  = EditorGUILayout.BeginToggleGroup("Show Normals", surface.gizmos.ShowNormal);
            surface.gizmos.NormalHeight = EditorGUILayout.Slider("Height", surface.gizmos.NormalHeight, 0, 1f);
            EditorGUILayout.EndToggleGroup();
            
            changeColor  = EditorGUILayout.BeginToggleGroup("Change Color", changeColor);

            if(changeColor)
            {
                GUILayout.BeginHorizontal();
                surface.gizmos.color01 = EditorGUILayout.ColorField(surface.gizmos.color01);
                surface.gizmos.color11 = EditorGUILayout.ColorField(surface.gizmos.color11);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                surface.gizmos.color00 = EditorGUILayout.ColorField(surface.gizmos.color00);
                surface.gizmos.color10 = EditorGUILayout.ColorField(surface.gizmos.color10);
                GUILayout.EndHorizontal();
            }
                
            EditorGUILayout.EndToggleGroup();
        }

        private bool changeColor;
        
        private SurfaceMesh meshSettings => _meshSettings ?? (_meshSettings = new SurfaceMesh());
        private SurfaceMesh _meshSettings;
        
        private void EditMesh(SurfaceBehaviour surface)
        {
            var filter = surface.GetComponent<MeshFilter>();

            if (filter == null)
            {
                if (GUILayout.Button("Add  MeshFilter"))
                {
                    filter = surface.gameObject.AddComponent<MeshFilter>();
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
            
                EditorGUILayout.ObjectField("MeshFilter", filter, typeof(MeshFilter), true);

                if (GUILayout.Button("Destroy"))
                {
                    DestroyImmediate(filter);
                }
            
                GUILayout.EndHorizontal();
            }

            var renderer = surface.GetComponent<MeshRenderer>();

            if (renderer == null)
            {
                if (GUILayout.Button("Add  MeshRenderer"))
                {
                    renderer = surface.gameObject.AddComponent<MeshRenderer>();
                    renderer.material = Resources.Load<Material>("SurfaceMaterial");
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
            
                EditorGUILayout.ObjectField("MeshRenderer", renderer, typeof(MeshRenderer), true);

                if (GUILayout.Button("Destroy"))
                {
                    DestroyImmediate(renderer);
                }
            
                GUILayout.EndHorizontal();
            }

            if (filter == null || renderer == null) return;
            
            GUILayout.Space(16);
            
            if (GUILayout.Button("Build Mesh"))
            {
                BuildMesh(surface);
            }
            
            GUILayout.Space(16);

            meshSettings.TwoSide = EditorGUILayout.Toggle("Two Side", meshSettings.TwoSide);
            meshSettings.AutoNormal = EditorGUILayout.Toggle("Auto Normal", meshSettings.AutoNormal);
            if (!meshSettings.AutoNormal)
            {
                meshSettings.InvertNormal = EditorGUILayout.Toggle("Invert Normal", meshSettings.InvertNormal);
            }
            
            meshSettings.SizeU = EditorGUILayout.IntSlider("Size U", meshSettings.SizeU, SurfaceMesh.SizeMin, SurfaceMesh.SizeMax);
            meshSettings.SizeV = EditorGUILayout.IntSlider("Size V", meshSettings.SizeV, SurfaceMesh.SizeMin, SurfaceMesh.SizeMax);
            
            surface.gizmos.SizeU = meshSettings.SizeU;
            surface.gizmos.SizeV = meshSettings.SizeV;
        }

        private void BuildMesh(SurfaceBehaviour surface, MeshFilter filter = null)
        {
            if (filter == null) filter = surface.GetComponent<MeshFilter>();
            if (filter == null) return;
            
            if (meshSettings.Build(surface, out var mesh))
            {
                filter.sharedMesh = mesh;
                    
                UpdatePreview(surface.gameObject);
            }
        }
        
        #endregion
        
        public static void GUIQuadEntity(ref QuadEntity quad)
        {
            GUILayout.Label(QuadContent.image, GUILayout.Width(64), GUILayout.Height(64));
            
            quad.Width = EditorGUILayout.FloatField("Width", quad.Width);
            quad.Height = EditorGUILayout.FloatField("Height", quad.Height);
        }

        public static void GUICircleEntity(ref CircleEntity circle)
        {
            GUILayout.Label(CircleContent.image, GUILayout.Width(64), GUILayout.Height(64));
            
            GUILayout.Box("Angles");
                
            circle.SectorAngle = EditorGUILayout.Slider("Sector", circle.SectorAngle, CircleEntity.MinSectorAngle, CircleEntity.MaxSectorAngle);

            GUILayout.Box("Radius");
                
            circle.RadiusInner = Mathf.Clamp(EditorGUILayout.FloatField("Radius Inner", circle.RadiusInner), 0, circle.RadiusOuter);
            circle.RadiusOuter = Mathf.Max(0, EditorGUILayout.FloatField("Radius Outer", circle.RadiusOuter));

            var width = Mathf.Max(0, EditorGUILayout.FloatField("Width", circle.RadiusOuter - circle.RadiusInner));
            circle.RadiusOuter = circle.RadiusInner + width;

            // Smooth
            
            circle.SectorAngle = (float) Math.Round(circle.SectorAngle, 2);
            circle.RadiusInner = (float) Math.Round(circle.RadiusInner, 2);
            circle.RadiusOuter = (float) Math.Round(circle.RadiusOuter, 2);
        }

        public static void GUIPipeEntity(ref PipeEntity pipe)
        {
            GUILayout.Label(PipeContent.image, GUILayout.Width(64), GUILayout.Height(64));
            
            pipe.Height = EditorGUILayout.FloatField("Height", pipe.Height);
            pipe.Angle = EditorGUILayout.Slider("Angle", pipe.Angle, PipeEntity.AngleMin, PipeEntity.AngleMax);
            GUILayout.Box("Radius");
            pipe.RadiusTop = EditorGUILayout.FloatField("Top", pipe.RadiusTop);
            pipe.RadiusBottom = EditorGUILayout.FloatField("Bottom", pipe.RadiusBottom);
        }

        public static void GUISphereEntity(ref SphereEntity sphere)
        {
            GUILayout.Label(SphereContent.image, GUILayout.Width(64), GUILayout.Height(64));
            
            sphere.Radius = EditorGUILayout.FloatField("Radius", sphere.Radius);
            
            GUILayout.Box("Latitude");
            sphere.Latitude = EditorGUILayout.Slider("Angle", sphere.Latitude, SphereEntity.MinLatitude, SphereEntity.MaxLatitude);

            GUILayout.Box("Longitude");
                
            sphere.MinLongitude = Mathf.Clamp(EditorGUILayout.FloatField("Min Angle",sphere.MinLongitude), SphereEntity.MinAngleLongitude, sphere.MaxLongitude);
            sphere.MaxLongitude = Mathf.Clamp(EditorGUILayout.FloatField("Max Angle", sphere.MaxLongitude), sphere.MinLongitude, SphereEntity.MaxAngleLongitude);

            var sectorLong = EditorGUILayout.Slider("Sector", sphere.MaxLongitude - sphere.MinLongitude, 0f, 180f);
            sphere.MaxLongitude = sphere.MinLongitude + sectorLong;
            
            var minLong = sphere.MinLongitude;
            var maxLong = sphere.MaxLongitude;
            EditorGUILayout.MinMaxSlider("Range", ref minLong, ref maxLong, SphereEntity.MinAngleLongitude, SphereEntity.MaxAngleLongitude);
            sphere.MinLongitude = minLong;
            sphere.MaxLongitude = maxLong;
            
            // Smooth
            
            sphere.Latitude = (float) Math.Round(sphere.Latitude, 2);
            sphere.MinLongitude = (float) Math.Round(sphere.MinLongitude, 2);
            sphere.MaxLongitude = (float) Math.Round(sphere.MaxLongitude, 2);
            sphere.Radius = (float) Math.Round(sphere.Radius, 2);
        }

        public static void GUIBlendEntity(ref BlendSurfaceEntity blend)
        {
            GUILayout.Label(BlendContent.image, GUILayout.Width(64), GUILayout.Height(64));
            
            var value = $"{blend.Blend:F2}";
            blend.Blend = EditorGUILayout.Slider(value, blend.Blend, 0f, 1f);

            blend.SurfaceA = (SurfaceBehaviour)EditorGUILayout.ObjectField(blend.SurfaceA, typeof(SurfaceBehaviour), true);
            blend.SurfaceB = (SurfaceBehaviour)EditorGUILayout.ObjectField(blend.SurfaceB, typeof(SurfaceBehaviour), true);
        }
        
        #region Static

        private static void CreateSurface<TSurf>(MenuCommand command) where TSurf : SurfaceBehaviour
        {
            Transform parent = null; 
            
            if (command != null && command.context != null && command.context is GameObject gameObject)
            {
                parent = gameObject.transform;
            }

            var surf = CreateSurface<TSurf>(parent);
            
            Selection.activeGameObject = surf.gameObject;
        }
    
        public static TSurf CreateSurface<TSurf>(Transform parent = null) where TSurf : SurfaceBehaviour
        {
            var surf = new GameObject(typeof(TSurf).Name).AddComponent<TSurf>();

            if (parent != null) surf.transform.SetParent(parent, false);

            return surf;
        }

        [MenuItem(Cucu.CreateGameObject + Cucu.SurfaceGroup + QuadSurface.ObjectName)]
        private static void CreateQuadSurface(MenuCommand command)
        {
            CreateSurface<QuadSurface>(command);
        }

        [MenuItem(Cucu.CreateGameObject + Cucu.SurfaceGroup + CircleSurface.ObjectName)]
        private static void CreateCircleSurface(MenuCommand command)
        {
            CreateSurface<CircleSurface>(command);
        }

        [MenuItem(Cucu.CreateGameObject + Cucu.SurfaceGroup + PipeSurface.ObjectName)]
        private static void CreatePipeSurface(MenuCommand command)
        {
            CreateSurface<PipeSurface>(command);
        }

        [MenuItem(Cucu.CreateGameObject + Cucu.SurfaceGroup + SphereSurface.ObjectName)]
        private static void CreateSphereSurface(MenuCommand command)
        {
            CreateSurface<SphereSurface>(command);
        }

        [MenuItem(Cucu.CreateGameObject + Cucu.SurfaceGroup + BlendSurface.ObjectName)]
        private static void CreateBlendSurface(MenuCommand command)
        {
            CreateSurface<BlendSurface>(command);
        }

        #endregion
    }
}