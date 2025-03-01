#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OMSB
{
    /// <summary>
    /// エディタのGUI関連のユーティリティ
    /// </summary>
    public class EditorGUIEx : EditorWindow
    {
        //=======================================================================================================
        //. 定数
        //=======================================================================================================
        #region -- 定数

        protected static readonly Color TITLE_COLOR         = new Color(0.25f, 0.25f, 0.25f, 1f);
        protected static readonly Color SUBTITLE_COLOR      = new Color(0.3f, 0.3f, 0.3f, 1f);
        protected static readonly Color GROUP_COLOR         = new Color(0.25f, 0.25f, 0.25f, 1f);
        protected static readonly Color GROUP_COLOR_END     = new Color(0.17f, 0.17f, 0.17f, 1f);

        #endregion

        //=======================================================================================================
        //. メンバ
        //=======================================================================================================
        #region -- フィールド

        // GUIStyleは各レイアウトタイプに応じて1つだけ定義して使い回す
        private static GUIStyle	g_TitleStyle;
        private static GUIStyle	g_SubTitleStyle;
        private static GUIStyle	g_GroupStyle;
        private static GUIStyle	g_SeparatorStyle;

        #endregion

        #region -- プロパティ

        /// <summary>
        /// タイトルのスタイル
        /// </summary>
        public static GUIStyle TitleStyle {
            get
            {
                if (g_TitleStyle == null) {
                    GetTitleStyle(ref g_TitleStyle);
                }
                return g_TitleStyle;
            }
        }

        /// <summary>
        /// サブタイトルのスタイル
        /// </summary>
        public static GUIStyle SubTitleStyle {
            get
            {
                if (g_SubTitleStyle == null) {
                    GetSubTitleStyle(ref g_SubTitleStyle);
                }
                return g_SubTitleStyle;
            }
        }

        /// <summary>
        /// グループのスタイル
        /// </summary>
        public static GUIStyle GroupStyle {
            get
            {
                if (g_GroupStyle == null) {
                    GetGroupStyle(ref g_GroupStyle);
                }
                return g_GroupStyle;
            }
        }

        /// <summary>
        /// 区切り線のスタイル
        /// </summary>
        public static GUIStyle SeparatorStyle {
            get
            {
                if (g_SeparatorStyle == null) {
                    GetSeparatorStyle(ref g_SeparatorStyle);
                }
                return g_SeparatorStyle;
            }
        }

        #endregion

        //=======================================================================================================
        //. スタイル
        //=======================================================================================================
        #region -- スタイル

        /// <summary>
        /// タイトル表示のスタイル設定
        /// </summary>
        /// <param name="style"></param>
        private static void GetTitleStyle(ref GUIStyle style) 
        {
            if (style != null) return;

            style = new GUIStyle();
            style.name = "TITLE";
            style.normal.textColor = Color.white;
            style.normal.background = Texture2D.whiteTexture;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 14;
            style.fixedHeight = 20;
            style.padding = new RectOffset(0, 0, 2, 4);
            style.margin = new RectOffset(0, 0, 2, 12);
        }

        /// <summary>
        /// サブタイトル表示のスタイル設定
        /// </summary>
        /// <param name="style"></param>
        private static void GetSubTitleStyle(ref GUIStyle style) 
        {
            if (style != null) return;

            style = new GUIStyle();
            style.name = "SUBTITLE";
            style.normal.textColor = Color.white;
            style.normal.background = Texture2D.whiteTexture;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 12;
            style.fixedHeight = 18;
            style.padding = new RectOffset(0, 0, 2, 4);
            style.margin = new RectOffset(0, 0, 2, 6);
        }

        /// <summary>
        /// グループ表示のスタイル設定
        /// </summary>
        /// <param name="style"></param>
        private static void GetGroupStyle(ref GUIStyle style) 
        {
            if (style != null) return;

            style = new GUIStyle();
            style.name = "GROUP";
            style.alignment = TextAnchor.MiddleLeft;
            style.border = new RectOffset(4, 4, 4, 4);
            style.padding = new RectOffset(4, 4, 2, 2);
            style.fontSize = 12;
            style.fontStyle = FontStyle.Normal;
            style.normal = new GUIStyleState();
            style.normal.textColor = Color.white;
            style.normal.background = Texture2D.whiteTexture;
            style.active = new GUIStyleState();
            style.active.textColor = Color.white;
            style.active.background = Texture2D.whiteTexture;
        }

        /// <summary>
        /// 区切り線表示のスタイル設定
        /// </summary>
        /// <param name="style"></param>
        private static void GetSeparatorStyle(ref GUIStyle style) 
        {
            if (style != null) return;

            style = new GUIStyle();
            style.name = "SEPARATOR";
            style.normal.textColor = Color.white;
            style.normal.background = Texture2D.whiteTexture;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 1;
            style.fixedHeight = 1;
        }

        #endregion

        //=======================================================================================================
        //. レイアウト
        //=======================================================================================================
        #region -- レイアウト

        /// <summary>
        /// タイトルのレイアウト
        /// </summary>
        /// <param name="caption">表示文字</param>
        /// <param name="color">カラー</param>
        public static void DrawTitle(string caption, Color color) 
        {
            GUI.backgroundColor = color;
            EditorGUILayout.LabelField(caption, TitleStyle);
            GUI.backgroundColor = Color.white;
        }
        public static void DrawTitle(string title) 
        {
            DrawTitle(title, TITLE_COLOR);
        }

        /// <summary>
        /// サブタイトルのレイアウト
        /// </summary>
        /// <param name="caption">表示文字</param>
        /// <param name="color">カラー</param>
        public static void DrawSubTitle(string caption, Color color) 
        {
            GUI.backgroundColor = color;
            EditorGUILayout.LabelField(caption, SubTitleStyle);
            GUI.backgroundColor = Color.white;
        }
        public static void DrawSubTitle(string title) 
        {
            DrawSubTitle(title, SUBTITLE_COLOR);
        }


        /// <summary>
        /// グループの開始
        /// </summary>
        /// <param name="key">開閉状態の保存キー</param>
        /// <param name="caption">表示文字</param>
        /// <param name="color">カラー</param>
        /// <returns>開閉状態</returns>
        public static bool BeginGroup(string key, string caption, Color color)
        {
            EditorGUIEx.DrawSeparator();

            bool state = EditorPrefs.GetBool(key, true);

            var groupCaption = state ? "\u25BC " : "\u25BA ";
            groupCaption += caption;
            GUI.backgroundColor = color;

            GUI.color = Color.white;
            GUI.changed = false;

            if (!GUILayout.Toggle(true, groupCaption, GroupStyle)) state = !state;
            if (GUI.changed) EditorPrefs.SetBool(key, state);

            EditorGUIEx.DrawSeparator(GROUP_COLOR_END);
            GUI.backgroundColor = Color.white;

            if (state) {
                EditorGUILayout.Space();
            }

            return state;
        }
        public static bool BeginGroup(string caption, Color color) 
        {
            return BeginGroup(caption, caption, color);
        }
        public static bool BeginGroup(string caption) {
            return BeginGroup(caption, caption, GROUP_COLOR);
        }

        /// <summary>
        /// グループの終了
        /// </summary>
        public static void EndGroup() 
        {
            EditorGUILayout.Space();
        }

        /// <summary>
        /// 区切り線を表示
        /// </summary>
        /// <param name="color">カラー</param>
        public static void DrawSeparator(Color color) 
        {
            GUI.backgroundColor = color;
            GUILayout.Label("", EditorGUIEx.SeparatorStyle);
            GUI.backgroundColor = Color.white;
        }
        public static void DrawSeparator() 
        {
            DrawSeparator(Color.black);
        }

        #endregion
    }
}

#endif